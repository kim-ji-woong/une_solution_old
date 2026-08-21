namespace FireManagement
{
    partial class DockingEquipHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockingEquipHistory));
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridFA = new System.Windows.Forms.DataGridView();
            this.colFAHistoryID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFACheckTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFAStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridHD = new System.Windows.Forms.DataGridView();
            this.colHDHistoryID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDCheckTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridFE = new System.Windows.Forms.DataGridView();
            this.colHistoryID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheckTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBoxCircle03 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle02 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle01 = new UnE.GUI.TextPictureBox();
            this.btnFireAlarm = new UnE.GUI.RibbonButton();
            this.btnFirePlug = new UnE.GUI.RibbonButton();
            this.btnFireExtingusher = new UnE.GUI.RibbonButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "설비이력";
            // 
            // dataGridFA
            // 
            this.dataGridFA.AllowUserToAddRows = false;
            this.dataGridFA.AllowUserToDeleteRows = false;
            this.dataGridFA.AllowUserToResizeColumns = false;
            this.dataGridFA.AllowUserToResizeRows = false;
            this.dataGridFA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFAHistoryID,
            this.colFACheckTime,
            this.colFAStatus});
            this.dataGridFA.Location = new System.Drawing.Point(12, 144);
            this.dataGridFA.Name = "dataGridFA";
            this.dataGridFA.ReadOnly = true;
            this.dataGridFA.RowHeadersVisible = false;
            this.dataGridFA.RowTemplate.Height = 23;
            this.dataGridFA.Size = new System.Drawing.Size(408, 411);
            this.dataGridFA.TabIndex = 19;
            this.dataGridFA.Visible = false;
            this.dataGridFA.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFA_CellClick_1);
            this.dataGridFA.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFA_CellDoubleClick);
            // 
            // colFAHistoryID
            // 
            this.colFAHistoryID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFAHistoryID.FillWeight = 23F;
            this.colFAHistoryID.HeaderText = "관리번호";
            this.colFAHistoryID.Name = "colFAHistoryID";
            this.colFAHistoryID.ReadOnly = true;
            // 
            // colFACheckTime
            // 
            this.colFACheckTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFACheckTime.FillWeight = 60F;
            this.colFACheckTime.HeaderText = "최근 점검시간";
            this.colFACheckTime.Name = "colFACheckTime";
            this.colFACheckTime.ReadOnly = true;
            // 
            // colFAStatus
            // 
            this.colFAStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFAStatus.FillWeight = 20F;
            this.colFAStatus.HeaderText = "상태";
            this.colFAStatus.Name = "colFAStatus";
            this.colFAStatus.ReadOnly = true;
            // 
            // dataGridHD
            // 
            this.dataGridHD.AllowUserToAddRows = false;
            this.dataGridHD.AllowUserToResizeColumns = false;
            this.dataGridHD.AllowUserToResizeRows = false;
            this.dataGridHD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridHD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHDHistoryID,
            this.colHDCheckTime,
            this.colHDStatus});
            this.dataGridHD.Location = new System.Drawing.Point(12, 144);
            this.dataGridHD.Name = "dataGridHD";
            this.dataGridHD.ReadOnly = true;
            this.dataGridHD.RowHeadersVisible = false;
            this.dataGridHD.RowTemplate.Height = 23;
            this.dataGridHD.Size = new System.Drawing.Size(408, 411);
            this.dataGridHD.TabIndex = 18;
            this.dataGridHD.Visible = false;
            this.dataGridHD.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFA_CellClick_1);
            this.dataGridHD.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFA_CellDoubleClick);
            // 
            // colHDHistoryID
            // 
            this.colHDHistoryID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDHistoryID.FillWeight = 23F;
            this.colHDHistoryID.HeaderText = "관리번호";
            this.colHDHistoryID.Name = "colHDHistoryID";
            this.colHDHistoryID.ReadOnly = true;
            // 
            // colHDCheckTime
            // 
            this.colHDCheckTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDCheckTime.FillWeight = 60F;
            this.colHDCheckTime.HeaderText = "최근 점검시간";
            this.colHDCheckTime.Name = "colHDCheckTime";
            this.colHDCheckTime.ReadOnly = true;
            // 
            // colHDStatus
            // 
            this.colHDStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDStatus.FillWeight = 20F;
            this.colHDStatus.HeaderText = "상태";
            this.colHDStatus.Name = "colHDStatus";
            this.colHDStatus.ReadOnly = true;
            // 
            // dataGridFE
            // 
            this.dataGridFE.AllowUserToAddRows = false;
            this.dataGridFE.AllowUserToResizeColumns = false;
            this.dataGridFE.AllowUserToResizeRows = false;
            this.dataGridFE.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridFE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFE.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHistoryID,
            this.colCheckTime,
            this.colStatus});
            this.dataGridFE.Location = new System.Drawing.Point(12, 144);
            this.dataGridFE.Name = "dataGridFE";
            this.dataGridFE.ReadOnly = true;
            this.dataGridFE.RowHeadersVisible = false;
            this.dataGridFE.RowTemplate.Height = 23;
            this.dataGridFE.Size = new System.Drawing.Size(408, 411);
            this.dataGridFE.TabIndex = 17;
            this.dataGridFE.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFA_CellClick_1);
            this.dataGridFE.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridFA_CellDoubleClick);
            // 
            // colHistoryID
            // 
            this.colHistoryID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHistoryID.FillWeight = 23F;
            this.colHistoryID.HeaderText = "관리번호";
            this.colHistoryID.Name = "colHistoryID";
            this.colHistoryID.ReadOnly = true;
            // 
            // colCheckTime
            // 
            this.colCheckTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCheckTime.FillWeight = 60F;
            this.colCheckTime.HeaderText = "최근 점검시간";
            this.colCheckTime.Name = "colCheckTime";
            this.colCheckTime.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colStatus.FillWeight = 20F;
            this.colStatus.HeaderText = "상태";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // pictureBoxCircle03
            // 
            this.pictureBoxCircle03.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle03.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCircle03.BackgroundImage")));
            this.pictureBoxCircle03.Location = new System.Drawing.Point(291, 561);
            this.pictureBoxCircle03.Name = "pictureBoxCircle03";
            this.pictureBoxCircle03.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle03.TabIndex = 23;
            this.pictureBoxCircle03.TabStop = false;
            this.pictureBoxCircle03.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle02
            // 
            this.pictureBoxCircle02.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle02.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCircle02.BackgroundImage")));
            this.pictureBoxCircle02.Location = new System.Drawing.Point(197, 561);
            this.pictureBoxCircle02.Name = "pictureBoxCircle02";
            this.pictureBoxCircle02.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle02.TabIndex = 22;
            this.pictureBoxCircle02.TabStop = false;
            this.pictureBoxCircle02.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle01
            // 
            this.pictureBoxCircle01.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle01.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCircle01.BackgroundImage")));
            this.pictureBoxCircle01.Location = new System.Drawing.Point(102, 561);
            this.pictureBoxCircle01.Name = "pictureBoxCircle01";
            this.pictureBoxCircle01.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle01.TabIndex = 21;
            this.pictureBoxCircle01.TabStop = false;
            this.pictureBoxCircle01.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // btnFireAlarm
            // 
            this.btnFireAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireAlarm.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.CheckedBkgndImage")));
            this.btnFireAlarm.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.CheckedImage")));
            this.btnFireAlarm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireAlarm.DisabledBkgndImage = null;
            this.btnFireAlarm.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.DisabledImage")));
            this.btnFireAlarm.ID = -1;
            this.btnFireAlarm.InitButtonWidth = 70;
            this.btnFireAlarm.IsChecked = false;
            this.btnFireAlarm.Location = new System.Drawing.Point(186, 63);
            this.btnFireAlarm.MouseOverBkgndImage = null;
            this.btnFireAlarm.Name = "btnFireAlarm";
            this.btnFireAlarm.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.NormalImage")));
            this.btnFireAlarm.Owner = null;
            this.btnFireAlarm.Size = new System.Drawing.Size(70, 76);
            this.btnFireAlarm.TabIndex = 16;
            this.btnFireAlarm.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireAlarm.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireAlarm.UseCustomImageRect = false;
            this.btnFireAlarm.UseTextLocation = false;
            this.btnFireAlarm.UseVisualStyleBackColor = true;
            this.btnFireAlarm.Click += new System.EventHandler(this.btnFireAlarm_Click);
            // 
            // btnFirePlug
            // 
            this.btnFirePlug.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirePlug.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.CheckedBkgndImage")));
            this.btnFirePlug.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.CheckedImage")));
            this.btnFirePlug.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFirePlug.DisabledBkgndImage = null;
            this.btnFirePlug.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.DisabledImage")));
            this.btnFirePlug.ID = -1;
            this.btnFirePlug.InitButtonWidth = 70;
            this.btnFirePlug.IsChecked = false;
            this.btnFirePlug.Location = new System.Drawing.Point(98, 64);
            this.btnFirePlug.MouseOverBkgndImage = null;
            this.btnFirePlug.Name = "btnFirePlug";
            this.btnFirePlug.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.NormalImage")));
            this.btnFirePlug.Owner = null;
            this.btnFirePlug.Size = new System.Drawing.Size(70, 76);
            this.btnFirePlug.TabIndex = 15;
            this.btnFirePlug.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFirePlug.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFirePlug.UseCustomImageRect = false;
            this.btnFirePlug.UseTextLocation = false;
            this.btnFirePlug.UseVisualStyleBackColor = true;
            this.btnFirePlug.Click += new System.EventHandler(this.btnFirePlug_Click);
            // 
            // btnFireExtingusher
            // 
            this.btnFireExtingusher.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireExtingusher.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.CheckedBkgndImage")));
            this.btnFireExtingusher.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.CheckedImage")));
            this.btnFireExtingusher.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireExtingusher.DisabledBkgndImage = null;
            this.btnFireExtingusher.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.DisabledImage")));
            this.btnFireExtingusher.ID = -1;
            this.btnFireExtingusher.InitButtonWidth = 70;
            this.btnFireExtingusher.IsChecked = true;
            this.btnFireExtingusher.Location = new System.Drawing.Point(12, 63);
            this.btnFireExtingusher.MouseOverBkgndImage = null;
            this.btnFireExtingusher.Name = "btnFireExtingusher";
            this.btnFireExtingusher.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.NormalImage")));
            this.btnFireExtingusher.Owner = null;
            this.btnFireExtingusher.Size = new System.Drawing.Size(70, 76);
            this.btnFireExtingusher.TabIndex = 14;
            this.btnFireExtingusher.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireExtingusher.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireExtingusher.UseCustomImageRect = false;
            this.btnFireExtingusher.UseTextLocation = false;
            this.btnFireExtingusher.UseVisualStyleBackColor = true;
            this.btnFireExtingusher.Click += new System.EventHandler(this.btnFireExtingusher_Click);
            // 
            // DockingEquipHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.ClientSize = new System.Drawing.Size(447, 600);
            this.Controls.Add(this.pictureBoxCircle03);
            this.Controls.Add(this.pictureBoxCircle02);
            this.Controls.Add(this.pictureBoxCircle01);
            this.Controls.Add(this.dataGridFA);
            this.Controls.Add(this.dataGridHD);
            this.Controls.Add(this.dataGridFE);
            this.Controls.Add(this.btnFireAlarm);
            this.Controls.Add(this.btnFirePlug);
            this.Controls.Add(this.btnFireExtingusher);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingEquipHistory";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DockingEquipHistory";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private UnE.GUI.RibbonButton btnFireAlarm;
        private UnE.GUI.RibbonButton btnFirePlug;
        private UnE.GUI.RibbonButton btnFireExtingusher;
        private System.Windows.Forms.DataGridView dataGridFA;
        private System.Windows.Forms.DataGridView dataGridHD;
        private System.Windows.Forms.DataGridView dataGridFE;
        private UnE.GUI.TextPictureBox pictureBoxCircle03;
        private UnE.GUI.TextPictureBox pictureBoxCircle02;
        private UnE.GUI.TextPictureBox pictureBoxCircle01;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAHistoryID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFACheckTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDHistoryID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDCheckTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistoryID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCheckTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
    }
}