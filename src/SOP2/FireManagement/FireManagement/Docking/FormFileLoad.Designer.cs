namespace FireManagement.Docking
{
    partial class FormFileLoad
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFileLoad));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridVersion = new System.Windows.Forms.DataGridView();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.btnFloor = new UnE.GUI.RibbonButton();
            this.pictureBoxFloor = new FireManagement.TextPictureBoxEx();
            this.btnBuilding = new UnE.GUI.RibbonButton();
            this.pictureBoxBuilding = new FireManagement.TextPictureBoxEx();
            this.btnBuildingGroup = new UnE.GUI.RibbonButton();
            this.pictureBoxGroup = new FireManagement.TextPictureBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFloor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBuilding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.label1.Location = new System.Drawing.Point(61, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "점검 관리 대상";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.label2.Location = new System.Drawing.Point(63, 230);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(159, 45);
            this.label2.TabIndex = 1;
            this.label2.Text = "문서 버전";
            // 
            // dataGridVersion
            // 
            this.dataGridVersion.AllowUserToAddRows = false;
            this.dataGridVersion.AllowUserToDeleteRows = false;
            this.dataGridVersion.AllowUserToResizeColumns = false;
            this.dataGridVersion.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridVersion.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridVersion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridVersion.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridVersion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridVersion.ColumnHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridVersion.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridVersion.Location = new System.Drawing.Point(71, 293);
            this.dataGridVersion.MultiSelect = false;
            this.dataGridVersion.Name = "dataGridVersion";
            this.dataGridVersion.ReadOnly = true;
            this.dataGridVersion.RowHeadersVisible = false;
            this.dataGridVersion.RowTemplate.Height = 23;
            this.dataGridVersion.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridVersion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridVersion.ShowEditingIcon = false;
            this.dataGridVersion.Size = new System.Drawing.Size(941, 200);
            this.dataGridVersion.TabIndex = 9;
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.BackgroundImage = global::FireManagement.Properties.Resources.ButtonArea;
            this.btnLoadFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLoadFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnLoadFile.FlatAppearance.BorderSize = 0;
            this.btnLoadFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadFile.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoadFile.Location = new System.Drawing.Point(831, 508);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(181, 55);
            this.btnLoadFile.TabIndex = 11;
            this.btnLoadFile.Text = "도면열기";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(354, 97);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(7, 45);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(771, 97);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(7, 45);
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox3.BackgroundImage")));
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(77, 97);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(7, 45);
            this.pictureBox3.TabIndex = 12;
            this.pictureBox3.TabStop = false;
            // 
            // btnFloor
            // 
            this.btnFloor.BackgroundImage = global::FireManagement.Properties.Resources.Building_ComboBox_Button;
            this.btnFloor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFloor.CheckedBkgndImage = null;
            this.btnFloor.CheckedImage = null;
            this.btnFloor.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFloor.DisabledBkgndImage = null;
            this.btnFloor.DisabledImage = null;
            this.btnFloor.ID = -1;
            this.btnFloor.InitButtonWidth = 43;
            this.btnFloor.IsChecked = false;
            this.btnFloor.Location = new System.Drawing.Point(970, 101);
            this.btnFloor.MouseOverBkgndImage = null;
            this.btnFloor.Name = "btnFloor";
            this.btnFloor.NormalImage = null;
            this.btnFloor.Owner = null;
            this.btnFloor.Size = new System.Drawing.Size(43, 38);
            this.btnFloor.TabIndex = 8;
            this.btnFloor.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFloor.UseCustomImageRect = false;
            this.btnFloor.UseVisualStyleBackColor = true;
            this.btnFloor.Click += new System.EventHandler(this.btnFloor_Click);
            // 
            // pictureBoxFloor
            // 
            this.pictureBoxFloor.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxFloor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxFloor.BackgroundImage")));
            this.pictureBoxFloor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxFloor.Location = new System.Drawing.Point(778, 97);
            this.pictureBoxFloor.Name = "pictureBoxFloor";
            this.pictureBoxFloor.Size = new System.Drawing.Size(192, 45);
            this.pictureBoxFloor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxFloor.TabIndex = 7;
            this.pictureBoxFloor.TabStop = false;
            this.pictureBoxFloor.TextColor = System.Drawing.Color.Black;
            // 
            // btnBuilding
            // 
            this.btnBuilding.BackgroundImage = global::FireManagement.Properties.Resources.Building_ComboBox_Button;
            this.btnBuilding.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBuilding.CheckedBkgndImage = null;
            this.btnBuilding.CheckedImage = null;
            this.btnBuilding.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnBuilding.DisabledBkgndImage = null;
            this.btnBuilding.DisabledImage = null;
            this.btnBuilding.ID = -1;
            this.btnBuilding.InitButtonWidth = 43;
            this.btnBuilding.IsChecked = false;
            this.btnBuilding.Location = new System.Drawing.Point(677, 101);
            this.btnBuilding.MouseOverBkgndImage = null;
            this.btnBuilding.Name = "btnBuilding";
            this.btnBuilding.NormalImage = null;
            this.btnBuilding.Owner = null;
            this.btnBuilding.Size = new System.Drawing.Size(43, 38);
            this.btnBuilding.TabIndex = 6;
            this.btnBuilding.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnBuilding.UseCustomImageRect = false;
            this.btnBuilding.UseVisualStyleBackColor = true;
            this.btnBuilding.Click += new System.EventHandler(this.btnBuilding_Click);
            // 
            // pictureBoxBuilding
            // 
            this.pictureBoxBuilding.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxBuilding.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxBuilding.BackgroundImage")));
            this.pictureBoxBuilding.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxBuilding.Location = new System.Drawing.Point(361, 97);
            this.pictureBoxBuilding.Name = "pictureBoxBuilding";
            this.pictureBoxBuilding.Size = new System.Drawing.Size(316, 45);
            this.pictureBoxBuilding.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBuilding.TabIndex = 5;
            this.pictureBoxBuilding.TabStop = false;
            this.pictureBoxBuilding.TextColor = System.Drawing.Color.Black;
            // 
            // btnBuildingGroup
            // 
            this.btnBuildingGroup.BackgroundImage = global::FireManagement.Properties.Resources.Building_ComboBox_Button;
            this.btnBuildingGroup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBuildingGroup.CheckedBkgndImage = null;
            this.btnBuildingGroup.CheckedImage = null;
            this.btnBuildingGroup.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnBuildingGroup.DisabledBkgndImage = null;
            this.btnBuildingGroup.DisabledImage = null;
            this.btnBuildingGroup.ID = -1;
            this.btnBuildingGroup.InitButtonWidth = 43;
            this.btnBuildingGroup.IsChecked = false;
            this.btnBuildingGroup.Location = new System.Drawing.Point(272, 101);
            this.btnBuildingGroup.MouseOverBkgndImage = null;
            this.btnBuildingGroup.Name = "btnBuildingGroup";
            this.btnBuildingGroup.NormalImage = null;
            this.btnBuildingGroup.Owner = null;
            this.btnBuildingGroup.Size = new System.Drawing.Size(43, 38);
            this.btnBuildingGroup.TabIndex = 4;
            this.btnBuildingGroup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnBuildingGroup.UseCustomImageRect = false;
            this.btnBuildingGroup.UseVisualStyleBackColor = true;
            this.btnBuildingGroup.Click += new System.EventHandler(this.btnBuildingGroup_Click);
            // 
            // pictureBoxGroup
            // 
            this.pictureBoxGroup.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxGroup.BackgroundImage = global::FireManagement.Properties.Resources.Building_ComboBox_body;
            this.pictureBoxGroup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxGroup.Location = new System.Drawing.Point(84, 97);
            this.pictureBoxGroup.Name = "pictureBoxGroup";
            this.pictureBoxGroup.Size = new System.Drawing.Size(188, 45);
            this.pictureBoxGroup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxGroup.TabIndex = 3;
            this.pictureBoxGroup.TabStop = false;
            this.pictureBoxGroup.TextColor = System.Drawing.Color.Black;
            // 
            // FormFileLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1073, 618);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnLoadFile);
            this.Controls.Add(this.dataGridVersion);
            this.Controls.Add(this.btnFloor);
            this.Controls.Add(this.pictureBoxFloor);
            this.Controls.Add(this.btnBuilding);
            this.Controls.Add(this.pictureBoxBuilding);
            this.Controls.Add(this.btnBuildingGroup);
            this.Controls.Add(this.pictureBoxGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormFileLoad";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormFileLoad";
            this.Load += new System.EventHandler(this.FormFileLoad_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFloor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBuilding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGroup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private UnE.GUI.RibbonButton btnBuildingGroup;
        private UnE.GUI.RibbonButton btnBuilding;
        private UnE.GUI.RibbonButton btnFloor;
        private System.Windows.Forms.DataGridView dataGridVersion;
        private TextPictureBoxEx pictureBoxGroup;
        private TextPictureBoxEx pictureBoxBuilding;
        private TextPictureBoxEx pictureBoxFloor;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}