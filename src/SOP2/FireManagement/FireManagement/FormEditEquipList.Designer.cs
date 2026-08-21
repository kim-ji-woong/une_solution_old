namespace FireManagement
{
    partial class FormEditEquipList
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
            this.dataGridFA = new System.Windows.Forms.DataGridView();
            this.colFAEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFARFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFAStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridHD = new System.Windows.Forms.DataGridView();
            this.colHDEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDRFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHDStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridFE = new System.Windows.Forms.DataGridView();
            this.colEquipID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRFIDTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFireAlarm = new UnE.GUI.RibbonButton();
            this.btnFirePlug = new UnE.GUI.RibbonButton();
            this.btnFireExtingusher = new UnE.GUI.RibbonButton();
            this.pictureBoxCircle03 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle02 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle01 = new UnE.GUI.TextPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridHD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridFA
            // 
            this.dataGridFA.AllowUserToAddRows = false;
            this.dataGridFA.AllowUserToDeleteRows = false;
            this.dataGridFA.AllowUserToResizeColumns = false;
            this.dataGridFA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFAEquipID,
            this.colFARFID,
            this.colFAStatus});
            this.dataGridFA.Location = new System.Drawing.Point(33, 147);
            this.dataGridFA.Name = "dataGridFA";
            this.dataGridFA.ReadOnly = true;
            this.dataGridFA.RowHeadersVisible = false;
            this.dataGridFA.RowTemplate.Height = 23;
            this.dataGridFA.Size = new System.Drawing.Size(349, 375);
            this.dataGridFA.TabIndex = 26;
            this.dataGridFA.Visible = false;
            // 
            // colFAEquipID
            // 
            this.colFAEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFAEquipID.HeaderText = "설비관리번호";
            this.colFAEquipID.Name = "colFAEquipID";
            this.colFAEquipID.ReadOnly = true;
            // 
            // colFARFID
            // 
            this.colFARFID.HeaderText = "RFID Tag";
            this.colFARFID.Name = "colFARFID";
            this.colFARFID.ReadOnly = true;
            // 
            // colFAStatus
            // 
            this.colFAStatus.HeaderText = "상태";
            this.colFAStatus.Name = "colFAStatus";
            this.colFAStatus.ReadOnly = true;
            // 
            // dataGridHD
            // 
            this.dataGridHD.AllowUserToAddRows = false;
            this.dataGridHD.AllowUserToResizeColumns = false;
            this.dataGridHD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridHD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHDEquipID,
            this.colHDRFID,
            this.colHDStatus});
            this.dataGridHD.Location = new System.Drawing.Point(33, 147);
            this.dataGridHD.Name = "dataGridHD";
            this.dataGridHD.RowHeadersVisible = false;
            this.dataGridHD.RowTemplate.Height = 23;
            this.dataGridHD.Size = new System.Drawing.Size(349, 375);
            this.dataGridHD.TabIndex = 25;
            this.dataGridHD.Visible = false;
            // 
            // colHDEquipID
            // 
            this.colHDEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHDEquipID.HeaderText = "설비관리번호";
            this.colHDEquipID.Name = "colHDEquipID";
            // 
            // colHDRFID
            // 
            this.colHDRFID.HeaderText = "RFID Tag";
            this.colHDRFID.Name = "colHDRFID";
            // 
            // colHDStatus
            // 
            this.colHDStatus.HeaderText = "상태";
            this.colHDStatus.Name = "colHDStatus";
            // 
            // dataGridFE
            // 
            this.dataGridFE.AllowUserToAddRows = false;
            this.dataGridFE.AllowUserToResizeColumns = false;
            this.dataGridFE.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridFE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFE.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEquipID,
            this.colRFIDTag,
            this.colStatus});
            this.dataGridFE.Location = new System.Drawing.Point(33, 146);
            this.dataGridFE.Name = "dataGridFE";
            this.dataGridFE.RowHeadersVisible = false;
            this.dataGridFE.RowTemplate.Height = 23;
            this.dataGridFE.Size = new System.Drawing.Size(349, 376);
            this.dataGridFE.TabIndex = 24;
            // 
            // colEquipID
            // 
            this.colEquipID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEquipID.HeaderText = "설비관리번호";
            this.colEquipID.Name = "colEquipID";
            // 
            // colRFIDTag
            // 
            this.colRFIDTag.HeaderText = "RFID Tag";
            this.colRFIDTag.Name = "colRFIDTag";
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "상태";
            this.colStatus.Name = "colStatus";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(2)))), ((int)(((byte)(2)))));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 40);
            this.label1.TabIndex = 17;
            this.label1.Text = "설비리스트 편집";
            // 
            // btnFireAlarm
            // 
            this.btnFireAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireAlarm.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFireAlarm.CheckedImage = global::FireManagement.Properties.Resources.ListFireAlarm_Red;
            this.btnFireAlarm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireAlarm.DisabledBkgndImage = null;
            this.btnFireAlarm.DisabledImage = global::FireManagement.Properties.Resources.ListFireAlarm_disposaled;
            this.btnFireAlarm.ID = -1;
            this.btnFireAlarm.InitButtonWidth = 70;
            this.btnFireAlarm.IsChecked = false;
            this.btnFireAlarm.Location = new System.Drawing.Point(207, 64);
            this.btnFireAlarm.MouseOverBkgndImage = null;
            this.btnFireAlarm.Name = "btnFireAlarm";
            this.btnFireAlarm.NormalImage = global::FireManagement.Properties.Resources.ListFireAlarm__normal;
            this.btnFireAlarm.Owner = null;
            this.btnFireAlarm.Size = new System.Drawing.Size(70, 76);
            this.btnFireAlarm.TabIndex = 23;
            this.btnFireAlarm.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireAlarm.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireAlarm.UseCustomImageRect = false;
            this.btnFireAlarm.UseTextLocation = false;
            this.btnFireAlarm.UseVisualStyleBackColor = true;
            // 
            // btnFirePlug
            // 
            this.btnFirePlug.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirePlug.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFirePlug.CheckedImage = global::FireManagement.Properties.Resources.ListFirePlug_Red;
            this.btnFirePlug.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFirePlug.DisabledBkgndImage = null;
            this.btnFirePlug.DisabledImage = global::FireManagement.Properties.Resources.ListFirePlug_disposaled;
            this.btnFirePlug.ID = -1;
            this.btnFirePlug.InitButtonWidth = 70;
            this.btnFirePlug.IsChecked = false;
            this.btnFirePlug.Location = new System.Drawing.Point(119, 65);
            this.btnFirePlug.MouseOverBkgndImage = null;
            this.btnFirePlug.Name = "btnFirePlug";
            this.btnFirePlug.NormalImage = global::FireManagement.Properties.Resources.ListFirePlug_normal;
            this.btnFirePlug.Owner = null;
            this.btnFirePlug.Size = new System.Drawing.Size(70, 76);
            this.btnFirePlug.TabIndex = 22;
            this.btnFirePlug.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFirePlug.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFirePlug.UseCustomImageRect = false;
            this.btnFirePlug.UseTextLocation = false;
            this.btnFirePlug.UseVisualStyleBackColor = true;
            // 
            // btnFireExtingusher
            // 
            this.btnFireExtingusher.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireExtingusher.CheckedBkgndImage = global::FireManagement.Properties.Resources.ListBtnClick_BG;
            this.btnFireExtingusher.CheckedImage = global::FireManagement.Properties.Resources.ListFireEx_Red;
            this.btnFireExtingusher.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireExtingusher.DisabledBkgndImage = null;
            this.btnFireExtingusher.DisabledImage = global::FireManagement.Properties.Resources.ListFireExting_normal;
            this.btnFireExtingusher.ID = -1;
            this.btnFireExtingusher.InitButtonWidth = 70;
            this.btnFireExtingusher.IsChecked = true;
            this.btnFireExtingusher.Location = new System.Drawing.Point(33, 64);
            this.btnFireExtingusher.MouseOverBkgndImage = null;
            this.btnFireExtingusher.Name = "btnFireExtingusher";
            this.btnFireExtingusher.NormalImage = global::FireManagement.Properties.Resources.ListFireExting_normal;
            this.btnFireExtingusher.Owner = null;
            this.btnFireExtingusher.Size = new System.Drawing.Size(70, 76);
            this.btnFireExtingusher.TabIndex = 21;
            this.btnFireExtingusher.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireExtingusher.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireExtingusher.UseCustomImageRect = false;
            this.btnFireExtingusher.UseTextLocation = false;
            this.btnFireExtingusher.UseVisualStyleBackColor = true;
            // 
            // pictureBoxCircle03
            // 
            this.pictureBoxCircle03.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            this.pictureBoxCircle03.Location = new System.Drawing.Point(291, 538);
            this.pictureBoxCircle03.Name = "pictureBoxCircle03";
            this.pictureBoxCircle03.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle03.TabIndex = 20;
            this.pictureBoxCircle03.TabStop = false;
            this.pictureBoxCircle03.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle02
            // 
            this.pictureBoxCircle02.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            this.pictureBoxCircle02.Location = new System.Drawing.Point(197, 538);
            this.pictureBoxCircle02.Name = "pictureBoxCircle02";
            this.pictureBoxCircle02.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle02.TabIndex = 19;
            this.pictureBoxCircle02.TabStop = false;
            this.pictureBoxCircle02.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle01
            // 
            this.pictureBoxCircle01.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
            this.pictureBoxCircle01.Location = new System.Drawing.Point(102, 538);
            this.pictureBoxCircle01.Name = "pictureBoxCircle01";
            this.pictureBoxCircle01.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle01.TabIndex = 18;
            this.pictureBoxCircle01.TabStop = false;
            this.pictureBoxCircle01.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // FormEditEquipList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 592);
            this.Controls.Add(this.dataGridFA);
            this.Controls.Add(this.dataGridHD);
            this.Controls.Add(this.dataGridFE);
            this.Controls.Add(this.btnFireAlarm);
            this.Controls.Add(this.btnFirePlug);
            this.Controls.Add(this.btnFireExtingusher);
            this.Controls.Add(this.pictureBoxCircle03);
            this.Controls.Add(this.pictureBoxCircle02);
            this.Controls.Add(this.pictureBoxCircle01);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEditEquipList";
            this.Text = "FormEditEquipList";
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

        private System.Windows.Forms.DataGridView dataGridFA;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFARFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFAStatus;
        private System.Windows.Forms.DataGridView dataGridHD;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDRFID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHDStatus;
        private System.Windows.Forms.DataGridView dataGridFE;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRFIDTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private UnE.GUI.RibbonButton btnFireAlarm;
        private UnE.GUI.RibbonButton btnFirePlug;
        private UnE.GUI.RibbonButton btnFireExtingusher;
        private UnE.GUI.TextPictureBox pictureBoxCircle03;
        private UnE.GUI.TextPictureBox pictureBoxCircle02;
        private UnE.GUI.TextPictureBox pictureBoxCircle01;
        private System.Windows.Forms.Label label1;
    }
}