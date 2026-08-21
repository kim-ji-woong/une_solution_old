namespace RtspUrlEditor
{
    partial class FormEquipZoneCCTV
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.cboZone = new System.Windows.Forms.ComboBox();
            this.gridEquipZoneCCTV = new System.Windows.Forms.DataGridView();
            this.colEquipZone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCCTVList = new System.Windows.Forms.Button();
            this.pictureBoxScreen = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelCCTV2 = new RtspUrlEditor.CCTVPanel();
            this.panelCCTV4 = new RtspUrlEditor.CCTVPanel();
            this.panelCCTV3 = new RtspUrlEditor.CCTVPanel();
            this.panelCCTV1 = new RtspUrlEditor.CCTVPanel();
            ((System.ComponentModel.ISupportInitialize)(this.gridEquipZoneCCTV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(12, 12);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(212, 20);
            this.cboBuildingGroup.TabIndex = 0;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(12, 42);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(212, 20);
            this.cboBuilding.TabIndex = 0;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // cboZone
            // 
            this.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZone.FormattingEnabled = true;
            this.cboZone.Location = new System.Drawing.Point(12, 73);
            this.cboZone.Name = "cboZone";
            this.cboZone.Size = new System.Drawing.Size(212, 20);
            this.cboZone.TabIndex = 0;
            this.cboZone.SelectedIndexChanged += new System.EventHandler(this.cboZone_SelectedIndexChanged);
            // 
            // gridEquipZoneCCTV
            // 
            this.gridEquipZoneCCTV.AllowUserToAddRows = false;
            this.gridEquipZoneCCTV.AllowUserToDeleteRows = false;
            this.gridEquipZoneCCTV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridEquipZoneCCTV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridEquipZoneCCTV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEquipZoneCCTV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEquipZone,
            this.colCCTV1,
            this.colCCTV2,
            this.colCCTV3,
            this.colCCTV4,
            this.colCCTV5,
            this.colCCTV6});
            this.gridEquipZoneCCTV.Location = new System.Drawing.Point(12, 99);
            this.gridEquipZoneCCTV.Name = "gridEquipZoneCCTV";
            this.gridEquipZoneCCTV.RowHeadersVisible = false;
            this.gridEquipZoneCCTV.RowTemplate.Height = 23;
            this.gridEquipZoneCCTV.Size = new System.Drawing.Size(641, 592);
            this.gridEquipZoneCCTV.TabIndex = 2;
            this.gridEquipZoneCCTV.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridEquipZoneCCTV_KeyDown);
            // 
            // colEquipZone
            // 
            this.colEquipZone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colEquipZone.DefaultCellStyle = dataGridViewCellStyle2;
            this.colEquipZone.HeaderText = "설비영역";
            this.colEquipZone.Name = "colEquipZone";
            this.colEquipZone.ReadOnly = true;
            this.colEquipZone.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCCTV1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV1.DefaultCellStyle = dataGridViewCellStyle3;
            this.colCCTV1.HeaderText = "CCTV1";
            this.colCCTV1.Name = "colCCTV1";
            this.colCCTV1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV1.Width = 80;
            // 
            // colCCTV2
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV2.DefaultCellStyle = dataGridViewCellStyle4;
            this.colCCTV2.HeaderText = "CCTV2";
            this.colCCTV2.Name = "colCCTV2";
            this.colCCTV2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV2.Width = 80;
            // 
            // colCCTV3
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV3.DefaultCellStyle = dataGridViewCellStyle5;
            this.colCCTV3.HeaderText = "CCTV3";
            this.colCCTV3.Name = "colCCTV3";
            this.colCCTV3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV3.Width = 80;
            // 
            // colCCTV4
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV4.DefaultCellStyle = dataGridViewCellStyle6;
            this.colCCTV4.HeaderText = "CCTV4";
            this.colCCTV4.Name = "colCCTV4";
            this.colCCTV4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV4.Width = 80;
            // 
            // colCCTV5
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV5.DefaultCellStyle = dataGridViewCellStyle7;
            this.colCCTV5.HeaderText = "CCTV5";
            this.colCCTV5.Name = "colCCTV5";
            this.colCCTV5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV5.Width = 80;
            // 
            // colCCTV6
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCCTV6.DefaultCellStyle = dataGridViewCellStyle8;
            this.colCCTV6.HeaderText = "CCTV6";
            this.colCCTV6.Name = "colCCTV6";
            this.colCCTV6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCCTV6.Width = 80;
            // 
            // btnCCTVList
            // 
            this.btnCCTVList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCCTVList.Location = new System.Drawing.Point(677, 12);
            this.btnCCTVList.Name = "btnCCTVList";
            this.btnCCTVList.Size = new System.Drawing.Size(131, 23);
            this.btnCCTVList.TabIndex = 4;
            this.btnCCTVList.Text = "전체 CCTV List 보기";
            this.btnCCTVList.UseVisualStyleBackColor = true;
            this.btnCCTVList.Click += new System.EventHandler(this.btnCCTVList_Click);
            // 
            // pictureBoxScreen
            // 
            this.pictureBoxScreen.Image = global::RtspUrlEditor.Properties.Resources.cctv_4;
            this.pictureBoxScreen.Location = new System.Drawing.Point(241, 12);
            this.pictureBoxScreen.Name = "pictureBoxScreen";
            this.pictureBoxScreen.Size = new System.Drawing.Size(130, 81);
            this.pictureBoxScreen.TabIndex = 1;
            this.pictureBoxScreen.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(814, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(131, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "저장하기";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelCCTV2
            // 
            this.panelCCTV2.AllowDrop = true;
            this.panelCCTV2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCCTV2.BackColor = System.Drawing.Color.Black;
            this.panelCCTV2.Location = new System.Drawing.Point(1020, 99);
            this.panelCCTV2.Name = "panelCCTV2";
            this.panelCCTV2.Size = new System.Drawing.Size(337, 293);
            this.panelCCTV2.TabIndex = 3;
            // 
            // panelCCTV4
            // 
            this.panelCCTV4.AllowDrop = true;
            this.panelCCTV4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCCTV4.BackColor = System.Drawing.Color.Black;
            this.panelCCTV4.Location = new System.Drawing.Point(1020, 398);
            this.panelCCTV4.Name = "panelCCTV4";
            this.panelCCTV4.Size = new System.Drawing.Size(337, 293);
            this.panelCCTV4.TabIndex = 3;
            // 
            // panelCCTV3
            // 
            this.panelCCTV3.AllowDrop = true;
            this.panelCCTV3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCCTV3.BackColor = System.Drawing.Color.Black;
            this.panelCCTV3.Location = new System.Drawing.Point(677, 398);
            this.panelCCTV3.Name = "panelCCTV3";
            this.panelCCTV3.Size = new System.Drawing.Size(337, 293);
            this.panelCCTV3.TabIndex = 3;
            // 
            // panelCCTV1
            // 
            this.panelCCTV1.AllowDrop = true;
            this.panelCCTV1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCCTV1.BackColor = System.Drawing.Color.Black;
            this.panelCCTV1.Location = new System.Drawing.Point(677, 99);
            this.panelCCTV1.Name = "panelCCTV1";
            this.panelCCTV1.Size = new System.Drawing.Size(337, 293);
            this.panelCCTV1.TabIndex = 3;
            this.panelCCTV1.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelCCTV_DragDrop);
            this.panelCCTV1.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelCCTV_DragEnter);
            this.panelCCTV1.DragOver += new System.Windows.Forms.DragEventHandler(this.panelCCTV_DragOver);
            // 
            // FormEquipZoneCCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1367, 704);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCCTVList);
            this.Controls.Add(this.panelCCTV2);
            this.Controls.Add(this.panelCCTV4);
            this.Controls.Add(this.panelCCTV3);
            this.Controls.Add(this.panelCCTV1);
            this.Controls.Add(this.gridEquipZoneCCTV);
            this.Controls.Add(this.pictureBoxScreen);
            this.Controls.Add(this.cboZone);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.cboBuildingGroup);
            this.Name = "FormEquipZoneCCTV";
            this.Text = "설비영역별 CCTV 설정";
            this.Load += new System.EventHandler(this.FormEquipZoneCCTV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridEquipZoneCCTV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.ComboBox cboZone;
        private System.Windows.Forms.PictureBox pictureBoxScreen;
        private System.Windows.Forms.DataGridView gridEquipZoneCCTV;
        private CCTVPanel panelCCTV1;
        private CCTVPanel panelCCTV2;
        private CCTVPanel panelCCTV3;
        private CCTVPanel panelCCTV4;
        private System.Windows.Forms.Button btnCCTVList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEquipZone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV5;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCCTV6;
        private System.Windows.Forms.Button btnSave;
    }
}