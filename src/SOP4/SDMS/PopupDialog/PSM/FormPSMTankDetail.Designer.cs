namespace SDMS.PopupDialog
{
    partial class FormPSMTankDetail
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
            this.pnlTankInfo = new System.Windows.Forms.Panel();
            this.btnMSDS = new System.Windows.Forms.Button();
            this.textBoxRemains = new System.Windows.Forms.TextBox();
            this.labelRemains = new System.Windows.Forms.LabelEx();
            this.cmbSelectUsed = new System.Windows.Forms.ComboBox();
            this.btnSelectUsed = new System.Windows.Forms.Button();
            this.btnPSMMaterial = new System.Windows.Forms.Button();
            this.lblValueOutAmount = new System.Windows.Forms.Label();
            this.lblValueInAmount = new System.Windows.Forms.Label();
            this.lblValueCapacity = new System.Windows.Forms.Label();
            this.lblValueRemains = new System.Windows.Forms.Label();
            this.lblValueTankLocation = new System.Windows.Forms.Label();
            this.lblValueMaterialName = new System.Windows.Forms.Label();
            this.lblColOutAmount = new System.Windows.Forms.Label();
            this.lblColInAmount = new System.Windows.Forms.Label();
            this.lblColCapacity = new System.Windows.Forms.Label();
            this.lblColRemains = new System.Windows.Forms.Label();
            this.lblColTankLocation = new System.Windows.Forms.Label();
            this.lblColMaterialName = new System.Windows.Forms.Label();
            this.lblTitleOutAmount = new System.Windows.Forms.Label();
            this.lblTitleInAmount = new System.Windows.Forms.Label();
            this.lblCapacity = new System.Windows.Forms.Label();
            this.lblTitleRemains = new System.Windows.Forms.Label();
            this.lblTitleTankLocation = new System.Windows.Forms.Label();
            this.lblTitleMaterialName = new System.Windows.Forms.Label();
            this.imgPSMUsual = new UnE.Controls.GifPictureBox();
            this.chkMonitoring = new System.Windows.Forms.CheckBox();
            this.chkCCTV = new System.Windows.Forms.CheckBox();
            this.pnlMonitor = new System.Windows.Forms.Panel();
            this.cctvCtrl1 = new UnE.Control.CCTVCtrl();
            this.pnlTankInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPSMUsual)).BeginInit();
            this.pnlMonitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTankInfo
            // 
            this.pnlTankInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTankInfo.BackColor = System.Drawing.Color.White;
            this.pnlTankInfo.Controls.Add(this.btnMSDS);
            this.pnlTankInfo.Controls.Add(this.textBoxRemains);
            this.pnlTankInfo.Controls.Add(this.labelRemains);
            this.pnlTankInfo.Controls.Add(this.cmbSelectUsed);
            this.pnlTankInfo.Controls.Add(this.btnSelectUsed);
            this.pnlTankInfo.Controls.Add(this.btnPSMMaterial);
            this.pnlTankInfo.Controls.Add(this.lblValueOutAmount);
            this.pnlTankInfo.Controls.Add(this.lblValueInAmount);
            this.pnlTankInfo.Controls.Add(this.lblValueCapacity);
            this.pnlTankInfo.Controls.Add(this.lblValueRemains);
            this.pnlTankInfo.Controls.Add(this.lblValueTankLocation);
            this.pnlTankInfo.Controls.Add(this.lblValueMaterialName);
            this.pnlTankInfo.Controls.Add(this.lblColOutAmount);
            this.pnlTankInfo.Controls.Add(this.lblColInAmount);
            this.pnlTankInfo.Controls.Add(this.lblColCapacity);
            this.pnlTankInfo.Controls.Add(this.lblColRemains);
            this.pnlTankInfo.Controls.Add(this.lblColTankLocation);
            this.pnlTankInfo.Controls.Add(this.lblColMaterialName);
            this.pnlTankInfo.Controls.Add(this.lblTitleOutAmount);
            this.pnlTankInfo.Controls.Add(this.lblTitleInAmount);
            this.pnlTankInfo.Controls.Add(this.lblCapacity);
            this.pnlTankInfo.Controls.Add(this.lblTitleRemains);
            this.pnlTankInfo.Controls.Add(this.lblTitleTankLocation);
            this.pnlTankInfo.Controls.Add(this.lblTitleMaterialName);
            this.pnlTankInfo.Controls.Add(this.imgPSMUsual);
            this.pnlTankInfo.Location = new System.Drawing.Point(12, 12);
            this.pnlTankInfo.Name = "pnlTankInfo";
            this.pnlTankInfo.Size = new System.Drawing.Size(550, 196);
            this.pnlTankInfo.TabIndex = 6;
            // 
            // btnMSDS
            // 
            this.btnMSDS.Location = new System.Drawing.Point(441, 8);
            this.btnMSDS.Name = "btnMSDS";
            this.btnMSDS.Size = new System.Drawing.Size(50, 23);
            this.btnMSDS.TabIndex = 25;
            this.btnMSDS.Text = "MSDS";
            this.btnMSDS.UseVisualStyleBackColor = true;
            // 
            // textBoxRemains
            // 
            this.textBoxRemains.Location = new System.Drawing.Point(254, 72);
            this.textBoxRemains.Name = "textBoxRemains";
            this.textBoxRemains.Size = new System.Drawing.Size(50, 21);
            this.textBoxRemains.TabIndex = 24;
            this.textBoxRemains.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelRemains
            // 
            this.labelRemains.AutoSize = true;
            this.labelRemains.BackColor = System.Drawing.Color.Transparent;
            this.labelRemains.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelRemains.ForeColor = System.Drawing.Color.Black;
            this.labelRemains.Location = new System.Drawing.Point(59, 87);
            this.labelRemains.Name = "labelRemains";
            this.labelRemains.Size = new System.Drawing.Size(36, 13);
            this.labelRemains.TabIndex = 23;
            this.labelRemains.Text = "40%";
            this.labelRemains.EnabledChanged += new System.EventHandler(this.labelRemains_EnabledChanged);
            // 
            // cmbSelectUsed
            // 
            this.cmbSelectUsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSelectUsed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectUsed.Enabled = false;
            this.cmbSelectUsed.FormattingEnabled = true;
            this.cmbSelectUsed.Location = new System.Drawing.Point(381, 103);
            this.cmbSelectUsed.Name = "cmbSelectUsed";
            this.cmbSelectUsed.Size = new System.Drawing.Size(110, 20);
            this.cmbSelectUsed.TabIndex = 22;
            this.cmbSelectUsed.Visible = false;
            // 
            // btnSelectUsed
            // 
            this.btnSelectUsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectUsed.Enabled = false;
            this.btnSelectUsed.Location = new System.Drawing.Point(497, 101);
            this.btnSelectUsed.Name = "btnSelectUsed";
            this.btnSelectUsed.Size = new System.Drawing.Size(50, 23);
            this.btnSelectUsed.TabIndex = 21;
            this.btnSelectUsed.Text = "선택";
            this.btnSelectUsed.UseVisualStyleBackColor = true;
            this.btnSelectUsed.Visible = false;
            // 
            // btnPSMMaterial
            // 
            this.btnPSMMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPSMMaterial.Location = new System.Drawing.Point(497, 8);
            this.btnPSMMaterial.Name = "btnPSMMaterial";
            this.btnPSMMaterial.Size = new System.Drawing.Size(50, 23);
            this.btnPSMMaterial.TabIndex = 20;
            this.btnPSMMaterial.Text = "특성";
            this.btnPSMMaterial.UseVisualStyleBackColor = true;
            // 
            // lblValueOutAmount
            // 
            this.lblValueOutAmount.AutoSize = true;
            this.lblValueOutAmount.Location = new System.Drawing.Point(254, 168);
            this.lblValueOutAmount.Name = "lblValueOutAmount";
            this.lblValueOutAmount.Size = new System.Drawing.Size(9, 12);
            this.lblValueOutAmount.TabIndex = 19;
            this.lblValueOutAmount.Text = " ";
            // 
            // lblValueInAmount
            // 
            this.lblValueInAmount.AutoSize = true;
            this.lblValueInAmount.Location = new System.Drawing.Point(254, 137);
            this.lblValueInAmount.Name = "lblValueInAmount";
            this.lblValueInAmount.Size = new System.Drawing.Size(9, 12);
            this.lblValueInAmount.TabIndex = 18;
            this.lblValueInAmount.Text = " ";
            // 
            // lblValueCapacity
            // 
            this.lblValueCapacity.AutoSize = true;
            this.lblValueCapacity.Location = new System.Drawing.Point(254, 106);
            this.lblValueCapacity.Name = "lblValueCapacity";
            this.lblValueCapacity.Size = new System.Drawing.Size(9, 12);
            this.lblValueCapacity.TabIndex = 17;
            this.lblValueCapacity.Text = " ";
            // 
            // lblValueRemains
            // 
            this.lblValueRemains.AutoSize = true;
            this.lblValueRemains.Location = new System.Drawing.Point(254, 75);
            this.lblValueRemains.Name = "lblValueRemains";
            this.lblValueRemains.Size = new System.Drawing.Size(9, 12);
            this.lblValueRemains.TabIndex = 16;
            this.lblValueRemains.Text = " ";
            // 
            // lblValueTankLocation
            // 
            this.lblValueTankLocation.AutoSize = true;
            this.lblValueTankLocation.Location = new System.Drawing.Point(254, 44);
            this.lblValueTankLocation.Name = "lblValueTankLocation";
            this.lblValueTankLocation.Size = new System.Drawing.Size(9, 12);
            this.lblValueTankLocation.TabIndex = 15;
            this.lblValueTankLocation.Text = " ";
            // 
            // lblValueMaterialName
            // 
            this.lblValueMaterialName.AutoSize = true;
            this.lblValueMaterialName.Location = new System.Drawing.Point(254, 13);
            this.lblValueMaterialName.Name = "lblValueMaterialName";
            this.lblValueMaterialName.Size = new System.Drawing.Size(9, 12);
            this.lblValueMaterialName.TabIndex = 14;
            this.lblValueMaterialName.Text = " ";
            // 
            // lblColOutAmount
            // 
            this.lblColOutAmount.AutoSize = true;
            this.lblColOutAmount.Location = new System.Drawing.Point(239, 168);
            this.lblColOutAmount.Name = "lblColOutAmount";
            this.lblColOutAmount.Size = new System.Drawing.Size(9, 12);
            this.lblColOutAmount.TabIndex = 13;
            this.lblColOutAmount.Text = ":";
            this.lblColOutAmount.Visible = false;
            // 
            // lblColInAmount
            // 
            this.lblColInAmount.AutoSize = true;
            this.lblColInAmount.Location = new System.Drawing.Point(239, 137);
            this.lblColInAmount.Name = "lblColInAmount";
            this.lblColInAmount.Size = new System.Drawing.Size(9, 12);
            this.lblColInAmount.TabIndex = 12;
            this.lblColInAmount.Text = ":";
            this.lblColInAmount.Visible = false;
            // 
            // lblColCapacity
            // 
            this.lblColCapacity.AutoSize = true;
            this.lblColCapacity.Location = new System.Drawing.Point(239, 106);
            this.lblColCapacity.Name = "lblColCapacity";
            this.lblColCapacity.Size = new System.Drawing.Size(9, 12);
            this.lblColCapacity.TabIndex = 11;
            this.lblColCapacity.Text = ":";
            // 
            // lblColRemains
            // 
            this.lblColRemains.AutoSize = true;
            this.lblColRemains.Location = new System.Drawing.Point(239, 75);
            this.lblColRemains.Name = "lblColRemains";
            this.lblColRemains.Size = new System.Drawing.Size(9, 12);
            this.lblColRemains.TabIndex = 9;
            this.lblColRemains.Text = ":";
            // 
            // lblColTankLocation
            // 
            this.lblColTankLocation.AutoSize = true;
            this.lblColTankLocation.Location = new System.Drawing.Point(239, 44);
            this.lblColTankLocation.Name = "lblColTankLocation";
            this.lblColTankLocation.Size = new System.Drawing.Size(9, 12);
            this.lblColTankLocation.TabIndex = 8;
            this.lblColTankLocation.Text = ":";
            // 
            // lblColMaterialName
            // 
            this.lblColMaterialName.AutoSize = true;
            this.lblColMaterialName.Location = new System.Drawing.Point(239, 13);
            this.lblColMaterialName.Name = "lblColMaterialName";
            this.lblColMaterialName.Size = new System.Drawing.Size(9, 12);
            this.lblColMaterialName.TabIndex = 7;
            this.lblColMaterialName.Text = ":";
            // 
            // lblTitleOutAmount
            // 
            this.lblTitleOutAmount.AutoSize = true;
            this.lblTitleOutAmount.Location = new System.Drawing.Point(173, 168);
            this.lblTitleOutAmount.Name = "lblTitleOutAmount";
            this.lblTitleOutAmount.Size = new System.Drawing.Size(41, 12);
            this.lblTitleOutAmount.TabIndex = 6;
            this.lblTitleOutAmount.Text = "출고량";
            this.lblTitleOutAmount.Visible = false;
            // 
            // lblTitleInAmount
            // 
            this.lblTitleInAmount.AutoSize = true;
            this.lblTitleInAmount.Location = new System.Drawing.Point(173, 137);
            this.lblTitleInAmount.Name = "lblTitleInAmount";
            this.lblTitleInAmount.Size = new System.Drawing.Size(41, 12);
            this.lblTitleInAmount.TabIndex = 5;
            this.lblTitleInAmount.Text = "입고량";
            this.lblTitleInAmount.Visible = false;
            // 
            // lblCapacity
            // 
            this.lblCapacity.AutoSize = true;
            this.lblCapacity.Location = new System.Drawing.Point(173, 106);
            this.lblCapacity.Name = "lblCapacity";
            this.lblCapacity.Size = new System.Drawing.Size(29, 12);
            this.lblCapacity.TabIndex = 4;
            this.lblCapacity.Text = "용량";
            // 
            // lblTitleRemains
            // 
            this.lblTitleRemains.AutoSize = true;
            this.lblTitleRemains.Location = new System.Drawing.Point(173, 75);
            this.lblTitleRemains.Name = "lblTitleRemains";
            this.lblTitleRemains.Size = new System.Drawing.Size(29, 12);
            this.lblTitleRemains.TabIndex = 3;
            this.lblTitleRemains.Text = "잔량";
            // 
            // lblTitleTankLocation
            // 
            this.lblTitleTankLocation.AutoSize = true;
            this.lblTitleTankLocation.Location = new System.Drawing.Point(173, 44);
            this.lblTitleTankLocation.Name = "lblTitleTankLocation";
            this.lblTitleTankLocation.Size = new System.Drawing.Size(53, 12);
            this.lblTitleTankLocation.TabIndex = 2;
            this.lblTitleTankLocation.Text = "탱크위치";
            // 
            // lblTitleMaterialName
            // 
            this.lblTitleMaterialName.AutoSize = true;
            this.lblTitleMaterialName.Location = new System.Drawing.Point(173, 13);
            this.lblTitleMaterialName.Name = "lblTitleMaterialName";
            this.lblTitleMaterialName.Size = new System.Drawing.Size(41, 12);
            this.lblTitleMaterialName.TabIndex = 1;
            this.lblTitleMaterialName.Text = "물질명";
            // 
            // imgPSMUsual
            // 
            this.imgPSMUsual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.imgPSMUsual.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.imgPSMUsual.Image = global::SDMS.Properties.Resources._0;
            this.imgPSMUsual.Location = new System.Drawing.Point(3, 3);
            this.imgPSMUsual.Name = "imgPSMUsual";
            this.imgPSMUsual.OnlyLastImage = false;
            this.imgPSMUsual.Size = new System.Drawing.Size(155, 190);
            this.imgPSMUsual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgPSMUsual.TabIndex = 0;
            this.imgPSMUsual.TabStop = false;
            this.imgPSMUsual.UseSingleLoop = true;
            // 
            // chkMonitoring
            // 
            this.chkMonitoring.AutoSize = true;
            this.chkMonitoring.Location = new System.Drawing.Point(15, 214);
            this.chkMonitoring.Name = "chkMonitoring";
            this.chkMonitoring.Size = new System.Drawing.Size(72, 16);
            this.chkMonitoring.TabIndex = 7;
            this.chkMonitoring.Text = "모니터링";
            this.chkMonitoring.UseVisualStyleBackColor = true;
            this.chkMonitoring.Visible = false;
            // 
            // chkCCTV
            // 
            this.chkCCTV.AutoSize = true;
            this.chkCCTV.Location = new System.Drawing.Point(107, 214);
            this.chkCCTV.Name = "chkCCTV";
            this.chkCCTV.Size = new System.Drawing.Size(114, 16);
            this.chkCCTV.TabIndex = 8;
            this.chkCCTV.Text = "주변 CCTV 보기";
            this.chkCCTV.UseVisualStyleBackColor = true;
            this.chkCCTV.Visible = false;
            // 
            // pnlMonitor
            // 
            this.pnlMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMonitor.BackColor = System.Drawing.Color.White;
            this.pnlMonitor.Controls.Add(this.cctvCtrl1);
            this.pnlMonitor.Location = new System.Drawing.Point(12, 236);
            this.pnlMonitor.Name = "pnlMonitor";
            this.pnlMonitor.Size = new System.Drawing.Size(550, 338);
            this.pnlMonitor.TabIndex = 9;
            this.pnlMonitor.Visible = false;
            // 
            // cctvCtrl1
            // 
            this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cctvCtrl1.BackColor = System.Drawing.Color.Black;
            this.cctvCtrl1.CCTVOwner = null;
            this.cctvCtrl1.Location = new System.Drawing.Point(3, 3);
            this.cctvCtrl1.Name = "cctvCtrl1";
            this.cctvCtrl1.Size = new System.Drawing.Size(544, 332);
            this.cctvCtrl1.TabIndex = 9;
            // 
            // FormPSMTankDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(574, 236);
            this.Controls.Add(this.pnlMonitor);
            this.Controls.Add(this.chkCCTV);
            this.Controls.Add(this.chkMonitoring);
            this.Controls.Add(this.pnlTankInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPSMTankDetail";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PSM Tank - Detail Information";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPSMTankDetail_FormClosing);
            this.pnlTankInfo.ResumeLayout(false);
            this.pnlTankInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPSMUsual)).EndInit();
            this.pnlMonitor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTankInfo;
        private System.Windows.Forms.Label lblColOutAmount;
        private System.Windows.Forms.Label lblColInAmount;
        private System.Windows.Forms.Label lblColCapacity;
        private System.Windows.Forms.Label lblColRemains;
        private System.Windows.Forms.Label lblColTankLocation;
        private System.Windows.Forms.Label lblTitleOutAmount;
        private System.Windows.Forms.Label lblTitleInAmount;
        private System.Windows.Forms.Label lblCapacity;
        private System.Windows.Forms.Label lblTitleRemains;
        private System.Windows.Forms.Label lblTitleTankLocation;
        private System.Windows.Forms.Label lblTitleMaterialName;
        private UnE.Controls.GifPictureBox imgPSMUsual;
        private System.Windows.Forms.ComboBox cmbSelectUsed;
        private System.Windows.Forms.Button btnSelectUsed;
        private System.Windows.Forms.Button btnPSMMaterial;
        private System.Windows.Forms.Label lblValueOutAmount;
        private System.Windows.Forms.Label lblValueInAmount;
        private System.Windows.Forms.Label lblValueCapacity;
        private System.Windows.Forms.Label lblValueRemains;
        private System.Windows.Forms.Label lblValueTankLocation;
        private System.Windows.Forms.Label lblValueMaterialName;
        private System.Windows.Forms.CheckBox chkMonitoring;
        private System.Windows.Forms.CheckBox chkCCTV;
        private System.Windows.Forms.Panel pnlMonitor;
        private UnE.Control.CCTVCtrl cctvCtrl1;
        private System.Windows.Forms.Label lblColMaterialName;
        private System.Windows.Forms.TextBox textBoxRemains;
        private System.Windows.Forms.Button btnMSDS;
        private System.Windows.Forms.LabelEx labelRemains;
    }
}