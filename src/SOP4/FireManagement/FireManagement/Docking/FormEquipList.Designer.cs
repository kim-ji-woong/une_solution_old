namespace FireManagement
{
    partial class FormEquipList
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
            this.dataGridFR = new System.Windows.Forms.DataGridView();
            this.colFRRFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFREquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFRStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEquipList = new System.Windows.Forms.Label();
            this.dataGridHD = new System.Windows.Forms.DataGridView();
            this.colHDRFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridFA = new System.Windows.Forms.DataGridView();
            this.colFARFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFAEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFAStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridFE = new System.Windows.Forms.DataGridView();
            this.colRFIDTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnFireAlarm = new UnE.GUI.RibbonButton();
            this.btnFirePlug = new UnE.GUI.RibbonButton();
            this.btnFireExtingusher = new UnE.GUI.RibbonButton();
            this.pictureBoxCircle03 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle02 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle01 = new UnE.GUI.TextPictureBox();
            this.btnFireReciver = new UnE.GUI.RibbonButton();
            this.textPictureBox1 = new UnE.GUI.TextPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridFR
            // 
            this.dataGridFR.AllowUserToAddRows = false;
            this.dataGridFR.AllowUserToDeleteRows = false;
            this.dataGridFR.AllowUserToResizeColumns = false;
            this.dataGridFR.AllowUserToResizeRows = false;
            this.dataGridFR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFR.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFRRFID,
            this.colFREquipID,
            this.colFRStatus});
            this.dataGridFR.Location = new System.Drawing.Point(33, 146);
            this.dataGridFR.MultiSelect = false;
            this.dataGridFR.Name = "dataGridFR";
            this.dataGridFR.ReadOnly = true;
            this.dataGridFR.RowHeadersVisible = false;
            this.dataGridFR.RowTemplate.Height = 23;
            this.dataGridFR.Size = new System.Drawing.Size(349, 375);
            this.dataGridFR.TabIndex = 15;
            this.dataGridFR.Visible = false;
            this.dataGridFR.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridFR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridFR_MouseDown);
            this.dataGridFR.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridFR_MouseMove);
            this.dataGridFR.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridFR_MouseUp);
            // 
            // colFRRFID
            // 
            this.colFRRFID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFRRFID.FillWeight = 45F;
            this.colFRRFID.HeaderText = "RFID Tag";
            this.colFRRFID.Name = "colFRRFID";
            this.colFRRFID.ReadOnly = true;
            // 
            // colFREquipID
            // 
            this.colFREquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFREquipID.FillWeight = 31F;
            this.colFREquipID.HeaderText = "관리번호";
            this.colFREquipID.Name = "colFREquipID";
            this.colFREquipID.ReadOnly = true;
            // 
            // colFRStatus
            // 
            this.colFRStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFRStatus.FillWeight = 24F;
            this.colFRStatus.HeaderText = "상태";
            this.colFRStatus.Name = "colFRStatus";
            this.colFRStatus.ReadOnly = true;
            // 
            // lblEquipList
            // 
            this.lblEquipList.AutoSize = true;
            this.lblEquipList.BackColor = System.Drawing.Color.Transparent;
            this.lblEquipList.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblEquipList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(2)))), ((int)(((byte)(2)))));
            this.lblEquipList.Location = new System.Drawing.Point(12, 9);
            this.lblEquipList.Name = "lblEquipList";
            this.lblEquipList.Size = new System.Drawing.Size(162, 40);
            this.lblEquipList.TabIndex = 0;
            this.lblEquipList.Text = "설비리스트";
            // 
            // dataGridHD
            // 
            this.dataGridHD.AllowUserToAddRows = false;
            this.dataGridHD.AllowUserToDeleteRows = false;
            this.dataGridHD.AllowUserToResizeColumns = false;
            this.dataGridHD.AllowUserToResizeRows = false;
            this.dataGridHD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridHD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHDRFID,
            this.colHDEquipID,
            this.colHDStatus});
            this.dataGridHD.Location = new System.Drawing.Point(33, 146);
            this.dataGridHD.MultiSelect = false;
            this.dataGridHD.Name = "dataGridHD";
            this.dataGridHD.ReadOnly = true;
            this.dataGridHD.RowHeadersVisible = false;
            this.dataGridHD.RowTemplate.Height = 23;
            this.dataGridHD.Size = new System.Drawing.Size(349, 375);
            this.dataGridHD.TabIndex = 15;
            this.dataGridHD.Visible = false;
            this.dataGridHD.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridHD.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseDown);
            this.dataGridHD.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseMove);
            this.dataGridHD.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseUp);
            // 
            // colHDRFID
            // 
            this.colHDRFID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDRFID.FillWeight = 45F;
            this.colHDRFID.HeaderText = "RFID Tag";
            this.colHDRFID.Name = "colHDRFID";
            this.colHDRFID.ReadOnly = true;
            // 
            // colHDEquipID
            // 
            this.colHDEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDEquipID.FillWeight = 31F;
            this.colHDEquipID.HeaderText = "관리번호";
            this.colHDEquipID.Name = "colHDEquipID";
            this.colHDEquipID.ReadOnly = true;
            // 
            // colHDStatus
            // 
            this.colHDStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDStatus.FillWeight = 24F;
            this.colHDStatus.HeaderText = "상태";
            this.colHDStatus.Name = "colHDStatus";
            this.colHDStatus.ReadOnly = true;
            // 
            // dataGridFA
            // 
            this.dataGridFA.AllowUserToAddRows = false;
            this.dataGridFA.AllowUserToDeleteRows = false;
            this.dataGridFA.AllowUserToResizeColumns = false;
            this.dataGridFA.AllowUserToResizeRows = false;
            this.dataGridFA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFARFID,
            this.colFAEquipID,
            this.colFAStatus});
            this.dataGridFA.Location = new System.Drawing.Point(33, 146);
            this.dataGridFA.MultiSelect = false;
            this.dataGridFA.Name = "dataGridFA";
            this.dataGridFA.ReadOnly = true;
            this.dataGridFA.RowHeadersVisible = false;
            this.dataGridFA.RowTemplate.Height = 23;
            this.dataGridFA.Size = new System.Drawing.Size(349, 375);
            this.dataGridFA.TabIndex = 16;
            this.dataGridFA.Visible = false;
            this.dataGridFA.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridFA.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseDown);
            this.dataGridFA.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseMove);
            this.dataGridFA.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseUp);
            // 
            // colFARFID
            // 
            this.colFARFID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFARFID.FillWeight = 45F;
            this.colFARFID.HeaderText = "RFID Tag";
            this.colFARFID.Name = "colFARFID";
            this.colFARFID.ReadOnly = true;
            // 
            // colFAEquipID
            // 
            this.colFAEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFAEquipID.FillWeight = 31F;
            this.colFAEquipID.HeaderText = "관리번호";
            this.colFAEquipID.Name = "colFAEquipID";
            this.colFAEquipID.ReadOnly = true;
            // 
            // colFAStatus
            // 
            this.colFAStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFAStatus.FillWeight = 24F;
            this.colFAStatus.HeaderText = "상태";
            this.colFAStatus.Name = "colFAStatus";
            this.colFAStatus.ReadOnly = true;
            // 
            // dataGridFE
            // 
            this.dataGridFE.AllowUserToAddRows = false;
            this.dataGridFE.AllowUserToDeleteRows = false;
            this.dataGridFE.AllowUserToResizeColumns = false;
            this.dataGridFE.AllowUserToResizeRows = false;
            this.dataGridFE.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridFE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFE.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRFIDTag,
            this.colEquipID,
            this.colStatus});
            this.dataGridFE.Location = new System.Drawing.Point(33, 146);
            this.dataGridFE.MultiSelect = false;
            this.dataGridFE.Name = "dataGridFE";
            this.dataGridFE.ReadOnly = true;
            this.dataGridFE.RowHeadersVisible = false;
            this.dataGridFE.RowTemplate.Height = 23;
            this.dataGridFE.Size = new System.Drawing.Size(349, 375);
            this.dataGridFE.TabIndex = 14;
            this.dataGridFE.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellClick);
            this.dataGridFE.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseDown);
            this.dataGridFE.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseMove);
            this.dataGridFE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridFA_MouseUp);
            // 
            // colRFIDTag
            // 
            this.colRFIDTag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRFIDTag.FillWeight = 45F;
            this.colRFIDTag.HeaderText = "RFID Tag";
            this.colRFIDTag.Name = "colRFIDTag";
            this.colRFIDTag.ReadOnly = true;
            // 
            // colEquipID
            // 
            this.colEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEquipID.FillWeight = 31F;
            this.colEquipID.HeaderText = "관리번호";
            this.colEquipID.Name = "colEquipID";
            this.colEquipID.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colStatus.FillWeight = 24F;
            this.colStatus.HeaderText = "상태";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // btnFireReciver
            // 
            this.btnFireReciver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireReciver.CheckButton = false;
            this.btnFireReciver.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFireReciver.CheckedImage = global::FireManagement.Properties.Resources.ListFireReciver_Red;
            this.btnFireReciver.ClickedBackgroundImage = null;
            this.btnFireReciver.ClickedImage = null;
            this.btnFireReciver.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 66);
            this.btnFireReciver.DisabledBkgndImage = null;
            this.btnFireReciver.DisabledImage = global::FireManagement.Properties.Resources.ListFireReciver_disposaled;
            this.btnFireReciver.ID = -1;
            this.btnFireReciver.InitButtonWidth = 70;
            this.btnFireReciver.IsChecked = false;
            this.btnFireReciver.Location = new System.Drawing.Point(291, 64);
            this.btnFireReciver.MouseOverBkgndImage = null;
            this.btnFireReciver.MouseOverImage = null;
            this.btnFireReciver.Name = "btnFireReciver";
            this.btnFireReciver.NormalImage = global::FireManagement.Properties.Resources.ListFireReciver__normal;
            this.btnFireReciver.Owner = null;
            this.btnFireReciver.Size = new System.Drawing.Size(70, 76);
            this.btnFireReciver.TabIndex = 13;
            this.btnFireReciver.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireReciver.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireReciver.ToolTipText = "수신반";
            this.btnFireReciver.UseCustomImageRect = false;
            this.btnFireReciver.UseTextLocation = false;
            this.btnFireReciver.UseVisualStyleBackColor = true;
            this.btnFireReciver.Click += new System.EventHandler(this.btnFireReciver_Click);
    
            // 
            // btnFireAlarm
            // 
            this.btnFireAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireAlarm.CheckButton = false;
            this.btnFireAlarm.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFireAlarm.CheckedImage = global::FireManagement.Properties.Resources.ListFireAlarm_Red;
            this.btnFireAlarm.ClickedBackgroundImage = null;
            this.btnFireAlarm.ClickedImage = null;
            this.btnFireAlarm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 66);
            this.btnFireAlarm.DisabledBkgndImage = null;
            this.btnFireAlarm.DisabledImage = global::FireManagement.Properties.Resources.ListFireAlarm_disposaled;
            this.btnFireAlarm.ID = -1;
            this.btnFireAlarm.InitButtonWidth = 70;
            this.btnFireAlarm.IsChecked = false;
            this.btnFireAlarm.Location = new System.Drawing.Point(205, 64);
            this.btnFireAlarm.MouseOverBkgndImage = null;
            this.btnFireAlarm.MouseOverImage = null;
            this.btnFireAlarm.Name = "btnFireAlarm";
            this.btnFireAlarm.NormalImage = global::FireManagement.Properties.Resources.ListFireAlarm__normal;
            this.btnFireAlarm.Owner = null;
            this.btnFireAlarm.Size = new System.Drawing.Size(70, 76);
            this.btnFireAlarm.TabIndex = 13;
            this.btnFireAlarm.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireAlarm.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireAlarm.ToolTipText = "발신기";
            this.btnFireAlarm.UseCustomImageRect = false;
            this.btnFireAlarm.UseTextLocation = false;
            this.btnFireAlarm.UseVisualStyleBackColor = true;
            this.btnFireAlarm.Click += new System.EventHandler(this.btnFireAlarm_Click);
            // 
            // btnFirePlug
            // 
            this.btnFirePlug.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirePlug.CheckButton = false;
            this.btnFirePlug.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFirePlug.CheckedImage = global::FireManagement.Properties.Resources.ListFirePlug_Red;
            this.btnFirePlug.ClickedBackgroundImage = null;
            this.btnFirePlug.ClickedImage = null;
            this.btnFirePlug.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 66);
            this.btnFirePlug.DisabledBkgndImage = null;
            this.btnFirePlug.DisabledImage = global::FireManagement.Properties.Resources.ListFirePlug_disposaled;
            this.btnFirePlug.ID = -1;
            this.btnFirePlug.InitButtonWidth = 70;
            this.btnFirePlug.IsChecked = false;
            this.btnFirePlug.Location = new System.Drawing.Point(119, 64);
            this.btnFirePlug.MouseOverBkgndImage = null;
            this.btnFirePlug.MouseOverImage = null;
            this.btnFirePlug.Name = "btnFirePlug";
            this.btnFirePlug.NormalImage = global::FireManagement.Properties.Resources.ListFirePlug_normal;
            this.btnFirePlug.Owner = null;
            this.btnFirePlug.Size = new System.Drawing.Size(70, 76);
            this.btnFirePlug.TabIndex = 12;
            this.btnFirePlug.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFirePlug.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFirePlug.ToolTipText = "소화전";
            this.btnFirePlug.UseCustomImageRect = false;
            this.btnFirePlug.UseTextLocation = false;
            this.btnFirePlug.UseVisualStyleBackColor = true;
            this.btnFirePlug.Click += new System.EventHandler(this.btnFirePlug_Click);
            // 
            // btnFireExtingusher
            // 
            this.btnFireExtingusher.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireExtingusher.CheckButton = false;
            this.btnFireExtingusher.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFireExtingusher.CheckedImage = global::FireManagement.Properties.Resources.ListFireEx_Red;
            this.btnFireExtingusher.ClickedBackgroundImage = null;
            this.btnFireExtingusher.ClickedImage = null;
            this.btnFireExtingusher.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 66);
            this.btnFireExtingusher.DisabledBkgndImage = null;
            this.btnFireExtingusher.DisabledImage = global::FireManagement.Properties.Resources.ListFireExting_normal;
            this.btnFireExtingusher.ID = -1;
            this.btnFireExtingusher.InitButtonWidth = 70;
            this.btnFireExtingusher.IsChecked = true;
            this.btnFireExtingusher.Location = new System.Drawing.Point(33, 64);
            this.btnFireExtingusher.MouseOverBkgndImage = null;
            this.btnFireExtingusher.MouseOverImage = null;
            this.btnFireExtingusher.Name = "btnFireExtingusher";
            this.btnFireExtingusher.NormalImage = global::FireManagement.Properties.Resources.ListFireExting_normal;
            this.btnFireExtingusher.Owner = null;
            this.btnFireExtingusher.Size = new System.Drawing.Size(70, 76);
            this.btnFireExtingusher.TabIndex = 11;
            this.btnFireExtingusher.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireExtingusher.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireExtingusher.ToolTipText = "소화기";
            this.btnFireExtingusher.UseCustomImageRect = false;
            this.btnFireExtingusher.UseTextLocation = false;
            this.btnFireExtingusher.UseVisualStyleBackColor = true;
            this.btnFireExtingusher.Click += new System.EventHandler(this.btnFireExtingusher_Click);
            // 
            // pictureBoxCircle03
            // 
            this.pictureBoxCircle03.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            this.pictureBoxCircle03.Location = new System.Drawing.Point(243, 538);
            this.pictureBoxCircle03.Name = "pictureBoxCircle03";
            this.pictureBoxCircle03.Owner = null;
            this.pictureBoxCircle03.PictureBoxText = "";
            this.pictureBoxCircle03.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle03.TabIndex = 10;
            this.pictureBoxCircle03.TabStop = false;
            this.pictureBoxCircle03.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle02
            // 
            this.pictureBoxCircle02.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            this.pictureBoxCircle02.Location = new System.Drawing.Point(167, 538);
            this.pictureBoxCircle02.Name = "pictureBoxCircle02";
            this.pictureBoxCircle02.Owner = null;
            this.pictureBoxCircle02.PictureBoxText = "";
            this.pictureBoxCircle02.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle02.TabIndex = 9;
            this.pictureBoxCircle02.TabStop = false;
            this.pictureBoxCircle02.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle01
            // 
            this.pictureBoxCircle01.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
            this.pictureBoxCircle01.Location = new System.Drawing.Point(100, 538);
            this.pictureBoxCircle01.Name = "pictureBoxCircle01";
            this.pictureBoxCircle01.Owner = null;
            this.pictureBoxCircle01.PictureBoxText = "";
            this.pictureBoxCircle01.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle01.TabIndex = 8;
            this.pictureBoxCircle01.TabStop = false;
            this.pictureBoxCircle01.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        // 
            // textPictureBox1
            // 
            this.textPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.textPictureBox1.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            this.textPictureBox1.Location = new System.Drawing.Point(307, 538);
            this.textPictureBox1.Name = "textPictureBox1";
            this.textPictureBox1.Owner = null;
            this.textPictureBox1.PictureBoxText = "";
            this.textPictureBox1.Size = new System.Drawing.Size(18, 18);
            this.textPictureBox1.TabIndex = 17;
            this.textPictureBox1.TabStop = false;
            this.textPictureBox1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // FormEquipList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(424, 568);
            this.Controls.Add(this.textPictureBox1);
            this.Controls.Add(this.dataGridFA);
            this.Controls.Add(this.dataGridHD);
            this.Controls.Add(this.dataGridFE);
            this.Controls.Add(this.dataGridFR);
            this.Controls.Add(this.btnFireReciver);
            this.Controls.Add(this.btnFireAlarm);
            this.Controls.Add(this.btnFirePlug);
            this.Controls.Add(this.btnFireExtingusher);
            this.Controls.Add(this.pictureBoxCircle03);
            this.Controls.Add(this.pictureBoxCircle02);
            this.Controls.Add(this.pictureBoxCircle01);
            this.Controls.Add(this.lblEquipList);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEquipList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormEquipList";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormEquipList_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormEquipList_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormEquipList_MouseUp);
            this.Resize += new System.EventHandler(this.FormEquipList_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textPictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEquipList;
        private UnE.GUI.TextPictureBox pictureBoxCircle01;
        private UnE.GUI.TextPictureBox pictureBoxCircle02;
        private UnE.GUI.TextPictureBox pictureBoxCircle03;
        private System.Windows.Forms.DataGridView dataGridHD;
        private System.Windows.Forms.DataGridView dataGridFA;
        private System.Windows.Forms.DataGridView dataGridFE;
        private System.Windows.Forms.DataGridView dataGridFR;
        private UnE.GUI.RibbonButton btnFireExtingusher;
        private UnE.GUI.RibbonButton btnFirePlug;
        private UnE.GUI.RibbonButton btnFireAlarm;
        private UnE.GUI.RibbonButton btnFireReciver;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFRRFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFREquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFRStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDRFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFARFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRFIDTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private UnE.GUI.TextPictureBox textPictureBox1;
        
    }
}