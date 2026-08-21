namespace SOPMonitoringSystem
{
    partial class PageBackStageMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackStageMessage));
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.btnEditDisaster = new System.Windows.Forms.Button();
            this.pictureBoxComboButton = new System.Windows.Forms.PictureBox();
            this.pictureBoxComboHeader = new System.Windows.Forms.PictureBox();
            this.pictureBoxComboBody = new UnE.GUI.TextPictureBox();
            this.labelDisaster = new System.Windows.Forms.Label();
            this.dataGridViewDisaster = new System.Windows.Forms.DataGridView();
            this.colDisaster = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnEarthquake = new UnE.GUI.RibbonButton();
            this.btnTyphoon = new UnE.GUI.RibbonButton();
            this.btnSubmergence = new UnE.GUI.RibbonButton();
            this.btnGeneralDisaster = new UnE.GUI.RibbonButton();
            this.btnHeavySnow = new UnE.GUI.RibbonButton();
            this.btnTerror = new UnE.GUI.RibbonButton();
            this.btnPollution = new UnE.GUI.RibbonButton();
            this.btnFire = new UnE.GUI.RibbonButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComboButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComboHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComboBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDisaster)).BeginInit();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMiddle
            // 
            this.panelMiddle.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.middle_graybar;
            this.panelMiddle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMiddle.Controls.Add(this.btnSave);
            this.panelMiddle.Controls.Add(this.btnEditDisaster);
            this.panelMiddle.Controls.Add(this.pictureBoxComboButton);
            this.panelMiddle.Controls.Add(this.pictureBoxComboHeader);
            this.panelMiddle.Controls.Add(this.pictureBoxComboBody);
            this.panelMiddle.Controls.Add(this.labelDisaster);
            this.panelMiddle.Location = new System.Drawing.Point(230, 25);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(818, 79);
            this.panelMiddle.TabIndex = 0;
            this.panelMiddle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMiddle_MouseDown);
            // 
            // btnEditDisaster
            // 
            this.btnEditDisaster.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnEditDisaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditDisaster.Location = new System.Drawing.Point(598, 25);
            this.btnEditDisaster.Name = "btnEditDisaster";
            this.btnEditDisaster.Size = new System.Drawing.Size(87, 30);
            this.btnEditDisaster.TabIndex = 5;
            this.btnEditDisaster.Text = "편집";
            this.btnEditDisaster.UseVisualStyleBackColor = false;
            this.btnEditDisaster.Click += new System.EventHandler(this.btnEditDisaster_Click);
            // 
            // pictureBoxComboButton
            // 
            this.pictureBoxComboButton.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxComboButton.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.ComboBox_Button;
            this.pictureBoxComboButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxComboButton.Location = new System.Drawing.Point(552, 25);
            this.pictureBoxComboButton.Name = "pictureBoxComboButton";
            this.pictureBoxComboButton.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxComboButton.TabIndex = 4;
            this.pictureBoxComboButton.TabStop = false;
            this.pictureBoxComboButton.Click += new System.EventHandler(this.pictureBoxComboButton_Click);
            // 
            // pictureBoxComboHeader
            // 
            this.pictureBoxComboHeader.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxComboHeader.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.ComboBox_Bar_Header;
            this.pictureBoxComboHeader.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxComboHeader.Location = new System.Drawing.Point(148, 25);
            this.pictureBoxComboHeader.Name = "pictureBoxComboHeader";
            this.pictureBoxComboHeader.Size = new System.Drawing.Size(4, 30);
            this.pictureBoxComboHeader.TabIndex = 3;
            this.pictureBoxComboHeader.TabStop = false;
            // 
            // pictureBoxComboBody
            // 
            this.pictureBoxComboBody.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxComboBody.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.ComboBox_Bar_Body;
            this.pictureBoxComboBody.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxComboBody.Location = new System.Drawing.Point(152, 25);
            this.pictureBoxComboBody.Name = "pictureBoxComboBody";
            this.pictureBoxComboBody.Size = new System.Drawing.Size(400, 30);
            this.pictureBoxComboBody.TabIndex = 1;
            this.pictureBoxComboBody.TabStop = false;
            this.pictureBoxComboBody.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // labelDisaster
            // 
            this.labelDisaster.AutoSize = true;
            this.labelDisaster.BackColor = System.Drawing.Color.Transparent;
            this.labelDisaster.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDisaster.Location = new System.Drawing.Point(33, 22);
            this.labelDisaster.Name = "labelDisaster";
            this.labelDisaster.Size = new System.Drawing.Size(97, 30);
            this.labelDisaster.TabIndex = 0;
            this.labelDisaster.Text = "재난상황";
            this.labelDisaster.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelDisaster_MouseDown);
            // 
            // dataGridViewDisaster
            // 
            this.dataGridViewDisaster.AllowUserToAddRows = false;
            this.dataGridViewDisaster.AllowUserToDeleteRows = false;
            this.dataGridViewDisaster.AllowUserToResizeRows = false;
            this.dataGridViewDisaster.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewDisaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDisaster.ColumnHeadersVisible = false;
            this.dataGridViewDisaster.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDisaster});
            this.dataGridViewDisaster.Location = new System.Drawing.Point(378, 121);
            this.dataGridViewDisaster.MultiSelect = false;
            this.dataGridViewDisaster.Name = "dataGridViewDisaster";
            this.dataGridViewDisaster.ReadOnly = true;
            this.dataGridViewDisaster.RowHeadersVisible = false;
            this.dataGridViewDisaster.RowTemplate.Height = 23;
            this.dataGridViewDisaster.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewDisaster.Size = new System.Drawing.Size(404, 150);
            this.dataGridViewDisaster.TabIndex = 6;
            this.dataGridViewDisaster.Visible = false;
            this.dataGridViewDisaster.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewDisaster_CellBeginEdit);
            this.dataGridViewDisaster.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDisaster_CellDoubleClick);
            this.dataGridViewDisaster.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDisaster_CellEndEdit);
            this.dataGridViewDisaster.CellStateChanged += new System.Windows.Forms.DataGridViewCellStateChangedEventHandler(this.dataGridViewDisaster_CellStateChanged);
            this.dataGridViewDisaster.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewDisaster_UserAddedRow);
            this.dataGridViewDisaster.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewDisaster_UserDeletedRow);
            this.dataGridViewDisaster.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewDisaster_KeyDown);
            this.dataGridViewDisaster.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewDisaster_KeyUp);
            this.dataGridViewDisaster.Leave += new System.EventHandler(this.dataGridViewDisaster_Leave);
            // 
            // colDisaster
            // 
            this.colDisaster.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDisaster.HeaderText = "재난";
            this.colDisaster.Name = "colDisaster";
            this.colDisaster.ReadOnly = true;
            // 
            // panelLeft
            // 
            this.panelLeft.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Left_BG;
            this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLeft.Controls.Add(this.btnEarthquake);
            this.panelLeft.Controls.Add(this.btnTyphoon);
            this.panelLeft.Controls.Add(this.btnSubmergence);
            this.panelLeft.Controls.Add(this.btnGeneralDisaster);
            this.panelLeft.Controls.Add(this.btnHeavySnow);
            this.panelLeft.Controls.Add(this.btnTerror);
            this.panelLeft.Controls.Add(this.btnPollution);
            this.panelLeft.Controls.Add(this.btnFire);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 25);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(230, 499);
            this.panelLeft.TabIndex = 2;
            this.panelLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelLeft_MouseDown);
            // 
            // btnEarthquake
            // 
            this.btnEarthquake.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnEarthquake.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnEarthquake.CheckedImage = null;
            this.btnEarthquake.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnEarthquake.DisabledBkgndImage = null;
            this.btnEarthquake.DisabledImage = null;
            this.btnEarthquake.ID = -1;
            this.btnEarthquake.InitButtonWidth = 230;
            this.btnEarthquake.IsChecked = false;
            this.btnEarthquake.Location = new System.Drawing.Point(0, 357);
            this.btnEarthquake.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnEarthquake.Name = "btnEarthquake";
            this.btnEarthquake.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Earthquake_Normal;
            this.btnEarthquake.Owner = null;
            this.btnEarthquake.Size = new System.Drawing.Size(230, 51);
            this.btnEarthquake.TabIndex = 0;
            this.btnEarthquake.Text = "지진";
            this.btnEarthquake.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnEarthquake.UseCustomImageRect = true;
            this.btnEarthquake.UseVisualStyleBackColor = false;
            // 
            // btnTyphoon
            // 
            this.btnTyphoon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnTyphoon.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnTyphoon.CheckedImage = null;
            this.btnTyphoon.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnTyphoon.DisabledBkgndImage = null;
            this.btnTyphoon.DisabledImage = null;
            this.btnTyphoon.ID = -1;
            this.btnTyphoon.InitButtonWidth = 230;
            this.btnTyphoon.IsChecked = false;
            this.btnTyphoon.Location = new System.Drawing.Point(0, 306);
            this.btnTyphoon.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnTyphoon.Name = "btnTyphoon";
            this.btnTyphoon.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Typhoon_Normal;
            this.btnTyphoon.Owner = null;
            this.btnTyphoon.Size = new System.Drawing.Size(230, 51);
            this.btnTyphoon.TabIndex = 0;
            this.btnTyphoon.Text = "태풍";
            this.btnTyphoon.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnTyphoon.UseCustomImageRect = true;
            this.btnTyphoon.UseVisualStyleBackColor = false;
            // 
            // btnSubmergence
            // 
            this.btnSubmergence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnSubmergence.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnSubmergence.CheckedImage = null;
            this.btnSubmergence.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnSubmergence.DisabledBkgndImage = null;
            this.btnSubmergence.DisabledImage = null;
            this.btnSubmergence.ID = -1;
            this.btnSubmergence.InitButtonWidth = 230;
            this.btnSubmergence.IsChecked = false;
            this.btnSubmergence.Location = new System.Drawing.Point(0, 255);
            this.btnSubmergence.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnSubmergence.Name = "btnSubmergence";
            this.btnSubmergence.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Submergence_Normal;
            this.btnSubmergence.Owner = null;
            this.btnSubmergence.Size = new System.Drawing.Size(230, 51);
            this.btnSubmergence.TabIndex = 0;
            this.btnSubmergence.Text = "침수";
            this.btnSubmergence.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnSubmergence.UseCustomImageRect = true;
            this.btnSubmergence.UseVisualStyleBackColor = false;
            // 
            // btnGeneralDisaster
            // 
            this.btnGeneralDisaster.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnGeneralDisaster.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnGeneralDisaster.CheckedImage = null;
            this.btnGeneralDisaster.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnGeneralDisaster.DisabledBkgndImage = null;
            this.btnGeneralDisaster.DisabledImage = null;
            this.btnGeneralDisaster.ID = -1;
            this.btnGeneralDisaster.InitButtonWidth = 230;
            this.btnGeneralDisaster.IsChecked = false;
            this.btnGeneralDisaster.Location = new System.Drawing.Point(0, 204);
            this.btnGeneralDisaster.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnGeneralDisaster.Name = "btnGeneralDisaster";
            this.btnGeneralDisaster.NormalImage = global::SOPMonitoringSystem.Properties.Resources.General_Disaster_Normal;
            this.btnGeneralDisaster.Owner = null;
            this.btnGeneralDisaster.Size = new System.Drawing.Size(230, 51);
            this.btnGeneralDisaster.TabIndex = 0;
            this.btnGeneralDisaster.Text = "일반재해";
            this.btnGeneralDisaster.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnGeneralDisaster.UseCustomImageRect = true;
            this.btnGeneralDisaster.UseVisualStyleBackColor = false;
            // 
            // btnHeavySnow
            // 
            this.btnHeavySnow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnHeavySnow.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnHeavySnow.CheckedImage = null;
            this.btnHeavySnow.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnHeavySnow.DisabledBkgndImage = null;
            this.btnHeavySnow.DisabledImage = null;
            this.btnHeavySnow.ID = -1;
            this.btnHeavySnow.InitButtonWidth = 230;
            this.btnHeavySnow.IsChecked = false;
            this.btnHeavySnow.Location = new System.Drawing.Point(0, 153);
            this.btnHeavySnow.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnHeavySnow.Name = "btnHeavySnow";
            this.btnHeavySnow.NormalImage = global::SOPMonitoringSystem.Properties.Resources.HeavySnow_Normal;
            this.btnHeavySnow.Owner = null;
            this.btnHeavySnow.Size = new System.Drawing.Size(230, 51);
            this.btnHeavySnow.TabIndex = 0;
            this.btnHeavySnow.Text = "폭설";
            this.btnHeavySnow.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnHeavySnow.UseCustomImageRect = true;
            this.btnHeavySnow.UseVisualStyleBackColor = false;
            // 
            // btnTerror
            // 
            this.btnTerror.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnTerror.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnTerror.CheckedImage = null;
            this.btnTerror.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnTerror.DisabledBkgndImage = null;
            this.btnTerror.DisabledImage = null;
            this.btnTerror.ID = -1;
            this.btnTerror.InitButtonWidth = 230;
            this.btnTerror.IsChecked = false;
            this.btnTerror.Location = new System.Drawing.Point(0, 102);
            this.btnTerror.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnTerror.Name = "btnTerror";
            this.btnTerror.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Terror_Normal;
            this.btnTerror.Owner = null;
            this.btnTerror.Size = new System.Drawing.Size(230, 51);
            this.btnTerror.TabIndex = 0;
            this.btnTerror.Text = "테러";
            this.btnTerror.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnTerror.UseCustomImageRect = true;
            this.btnTerror.UseVisualStyleBackColor = false;
            // 
            // btnPollution
            // 
            this.btnPollution.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnPollution.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnPollution.CheckedImage = null;
            this.btnPollution.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnPollution.DisabledBkgndImage = null;
            this.btnPollution.DisabledImage = null;
            this.btnPollution.ID = -1;
            this.btnPollution.InitButtonWidth = 230;
            this.btnPollution.IsChecked = false;
            this.btnPollution.Location = new System.Drawing.Point(0, 51);
            this.btnPollution.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnPollution.Name = "btnPollution";
            this.btnPollution.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Pollution_Normal;
            this.btnPollution.Owner = null;
            this.btnPollution.Size = new System.Drawing.Size(230, 51);
            this.btnPollution.TabIndex = 0;
            this.btnPollution.Text = "오염";
            this.btnPollution.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnPollution.UseCustomImageRect = true;
            this.btnPollution.UseVisualStyleBackColor = false;
            // 
            // btnFire
            // 
            this.btnFire.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnFire.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.leftbar_mouseover_bkgnd;
            this.btnFire.CheckedImage = null;
            this.btnFire.CustomImageRect = new System.Drawing.Rectangle(9, 9, 32, 32);
            this.btnFire.DisabledBkgndImage = null;
            this.btnFire.DisabledImage = null;
            this.btnFire.ID = -1;
            this.btnFire.InitButtonWidth = 230;
            this.btnFire.IsChecked = true;
            this.btnFire.Location = new System.Drawing.Point(0, 0);
            this.btnFire.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.select_skyblue;
            this.btnFire.Name = "btnFire";
            this.btnFire.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Fire_Normal;
            this.btnFire.Owner = null;
            this.btnFire.Size = new System.Drawing.Size(230, 51);
            this.btnFire.TabIndex = 0;
            this.btnFire.Text = "화재";
            this.btnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnFire.UseCustomImageRect = true;
            this.btnFire.UseVisualStyleBackColor = false;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelTop.BackgroundImage")));
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1079, 25);
            this.panelTop.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(694, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 30);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PageBackStageMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1079, 524);
            this.Controls.Add(this.dataGridViewDisaster);
            this.Controls.Add(this.panelMiddle);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackStageMessage";
            this.Text = "PageBackStageMessage";
            this.Load += new System.EventHandler(this.PageBackStageMessage_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PageBackStageMessage_MouseDown);
            this.Resize += new System.EventHandler(this.PageBackStageMessage_Resize);
            this.panelMiddle.ResumeLayout(false);
            this.panelMiddle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComboButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComboHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComboBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDisaster)).EndInit();
            this.panelLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelMiddle;
        private UnE.GUI.TextPictureBox pictureBoxComboBody;
        private System.Windows.Forms.Label labelDisaster;
        private System.Windows.Forms.PictureBox pictureBoxComboHeader;
        private System.Windows.Forms.PictureBox pictureBoxComboButton;
        private System.Windows.Forms.DataGridView dataGridViewDisaster;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisaster;
        private System.Windows.Forms.Button btnEditDisaster;
        private UnE.GUI.RibbonButton btnFire;
        private UnE.GUI.RibbonButton btnEarthquake;
        private UnE.GUI.RibbonButton btnTyphoon;
        private UnE.GUI.RibbonButton btnSubmergence;
        private UnE.GUI.RibbonButton btnGeneralDisaster;
        private UnE.GUI.RibbonButton btnHeavySnow;
        private UnE.GUI.RibbonButton btnTerror;
        private UnE.GUI.RibbonButton btnPollution;
        private System.Windows.Forms.Button btnSave;
    }
}