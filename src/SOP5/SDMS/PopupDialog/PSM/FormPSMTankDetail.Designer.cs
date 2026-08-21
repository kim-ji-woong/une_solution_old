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
            this.textBoxRemains = new System.Windows.Forms.TextBox();
            this.labelRemains = new System.Windows.Forms.LabelEx();
            this.cmbSelectUsed = new System.Windows.Forms.ComboBox();
            this.btnSelectUsed = new System.Windows.Forms.Button();
            this.lblValueOutAmount = new System.Windows.Forms.Label();
            this.lblValueInAmount = new System.Windows.Forms.Label();
            this.lblValueCapacity = new System.Windows.Forms.Label();
            this.lblValueRemains = new System.Windows.Forms.Label();
            this.lblValueTankLocation = new System.Windows.Forms.Label();
            this.lblValueMaterialName = new System.Windows.Forms.Label();
            this.lblTitleOutAmount = new System.Windows.Forms.Label();
            this.lblTitleInAmount = new System.Windows.Forms.Label();
            this.imgPSMUsual = new UnE.Controls.GifPictureBox();
            this.chkMonitoring = new System.Windows.Forms.CheckBox();
            this.chkCCTV = new System.Windows.Forms.CheckBox();
            this.pnlMonitor = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new UnE.GUI.ImageButton();
            this.btnPSMMaterial = new UnE.GUI.ImageButton();
            this.btnMSDS = new UnE.GUI.ImageButton();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.picMaterialName = new System.Windows.Forms.PictureBox();
            this.picTankLocation = new System.Windows.Forms.PictureBox();
            this.picRemains = new System.Windows.Forms.PictureBox();
            this.picCapacity = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgPSMUsual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPSMMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMSDS)).BeginInit();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMaterialName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTankLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRemains)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCapacity)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxRemains
            // 
            this.textBoxRemains.Font = new System.Drawing.Font("굴림", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxRemains.Location = new System.Drawing.Point(49, 197);
            this.textBoxRemains.Name = "textBoxRemains";
            this.textBoxRemains.Size = new System.Drawing.Size(86, 25);
            this.textBoxRemains.TabIndex = 24;
            this.textBoxRemains.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelRemains
            // 
            this.labelRemains.AutoSize = true;
            this.labelRemains.BackColor = System.Drawing.Color.Transparent;
            this.labelRemains.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelRemains.ForeColor = System.Drawing.Color.Black;
            this.labelRemains.Location = new System.Drawing.Point(129, 9);
            this.labelRemains.Name = "labelRemains";
            this.labelRemains.Size = new System.Drawing.Size(51, 20);
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
            this.cmbSelectUsed.Location = new System.Drawing.Point(-263, 6);
            this.cmbSelectUsed.Name = "cmbSelectUsed";
            this.cmbSelectUsed.Size = new System.Drawing.Size(110, 20);
            this.cmbSelectUsed.TabIndex = 22;
            this.cmbSelectUsed.Visible = false;
            // 
            // btnSelectUsed
            // 
            this.btnSelectUsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectUsed.Enabled = false;
            this.btnSelectUsed.Location = new System.Drawing.Point(-147, 6);
            this.btnSelectUsed.Name = "btnSelectUsed";
            this.btnSelectUsed.Size = new System.Drawing.Size(50, 23);
            this.btnSelectUsed.TabIndex = 21;
            this.btnSelectUsed.Text = "선택";
            this.btnSelectUsed.UseVisualStyleBackColor = true;
            this.btnSelectUsed.Visible = false;
            // 
            // lblValueOutAmount
            // 
            this.lblValueOutAmount.AutoSize = true;
            this.lblValueOutAmount.Location = new System.Drawing.Point(53, 35);
            this.lblValueOutAmount.Name = "lblValueOutAmount";
            this.lblValueOutAmount.Size = new System.Drawing.Size(9, 12);
            this.lblValueOutAmount.TabIndex = 19;
            this.lblValueOutAmount.Text = " ";
            this.lblValueOutAmount.Visible = false;
            // 
            // lblValueInAmount
            // 
            this.lblValueInAmount.AutoSize = true;
            this.lblValueInAmount.Location = new System.Drawing.Point(53, 9);
            this.lblValueInAmount.Name = "lblValueInAmount";
            this.lblValueInAmount.Size = new System.Drawing.Size(9, 12);
            this.lblValueInAmount.TabIndex = 18;
            this.lblValueInAmount.Text = " ";
            this.lblValueInAmount.Visible = false;
            // 
            // lblValueCapacity
            // 
            this.lblValueCapacity.AutoSize = true;
            this.lblValueCapacity.BackColor = System.Drawing.Color.Transparent;
            this.lblValueCapacity.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblValueCapacity.Location = new System.Drawing.Point(45, 256);
            this.lblValueCapacity.Name = "lblValueCapacity";
            this.lblValueCapacity.Size = new System.Drawing.Size(44, 18);
            this.lblValueCapacity.TabIndex = 17;
            this.lblValueCapacity.Text = "용량";
            // 
            // lblValueRemains
            // 
            this.lblValueRemains.AutoSize = true;
            this.lblValueRemains.BackColor = System.Drawing.Color.Transparent;
            this.lblValueRemains.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblValueRemains.Location = new System.Drawing.Point(141, 199);
            this.lblValueRemains.Name = "lblValueRemains";
            this.lblValueRemains.Size = new System.Drawing.Size(44, 18);
            this.lblValueRemains.TabIndex = 16;
            this.lblValueRemains.Text = "잔량";
            // 
            // lblValueTankLocation
            // 
            this.lblValueTankLocation.AutoSize = true;
            this.lblValueTankLocation.BackColor = System.Drawing.Color.Transparent;
            this.lblValueTankLocation.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblValueTankLocation.Location = new System.Drawing.Point(45, 145);
            this.lblValueTankLocation.Name = "lblValueTankLocation";
            this.lblValueTankLocation.Size = new System.Drawing.Size(86, 18);
            this.lblValueTankLocation.TabIndex = 15;
            this.lblValueTankLocation.Text = " 탱크위치";
            // 
            // lblValueMaterialName
            // 
            this.lblValueMaterialName.AutoSize = true;
            this.lblValueMaterialName.BackColor = System.Drawing.Color.Transparent;
            this.lblValueMaterialName.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblValueMaterialName.Location = new System.Drawing.Point(45, 88);
            this.lblValueMaterialName.Name = "lblValueMaterialName";
            this.lblValueMaterialName.Size = new System.Drawing.Size(62, 18);
            this.lblValueMaterialName.TabIndex = 14;
            this.lblValueMaterialName.Text = "물질명";
            // 
            // lblTitleOutAmount
            // 
            this.lblTitleOutAmount.AutoSize = true;
            this.lblTitleOutAmount.Location = new System.Drawing.Point(6, 36);
            this.lblTitleOutAmount.Name = "lblTitleOutAmount";
            this.lblTitleOutAmount.Size = new System.Drawing.Size(41, 12);
            this.lblTitleOutAmount.TabIndex = 6;
            this.lblTitleOutAmount.Text = "출고량";
            this.lblTitleOutAmount.Visible = false;
            // 
            // lblTitleInAmount
            // 
            this.lblTitleInAmount.AutoSize = true;
            this.lblTitleInAmount.Location = new System.Drawing.Point(6, 9);
            this.lblTitleInAmount.Name = "lblTitleInAmount";
            this.lblTitleInAmount.Size = new System.Drawing.Size(41, 12);
            this.lblTitleInAmount.TabIndex = 5;
            this.lblTitleInAmount.Text = "입고량";
            this.lblTitleInAmount.Visible = false;
            // 
            // imgPSMUsual
            // 
            this.imgPSMUsual.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.imgPSMUsual.BackColor = System.Drawing.Color.Transparent;
            this.imgPSMUsual.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.imgPSMUsual.Image = null;
            this.imgPSMUsual.Location = new System.Drawing.Point(0, 0);
            this.imgPSMUsual.Name = "imgPSMUsual";
            this.imgPSMUsual.OnlyLastImage = false;
            this.imgPSMUsual.Size = new System.Drawing.Size(190, 292);
            this.imgPSMUsual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgPSMUsual.TabIndex = 0;
            this.imgPSMUsual.TabStop = false;
            this.imgPSMUsual.UseSingleLoop = true;
            // 
            // chkMonitoring
            // 
            this.chkMonitoring.AutoSize = true;
            this.chkMonitoring.Location = new System.Drawing.Point(68, 10);
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
            this.chkCCTV.Location = new System.Drawing.Point(68, 31);
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
            this.pnlMonitor.Location = new System.Drawing.Point(421, 679);
            this.pnlMonitor.Name = "pnlMonitor";
            this.pnlMonitor.Size = new System.Drawing.Size(435, 338);
            this.pnlMonitor.TabIndex = 9;
            this.pnlMonitor.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "물질명";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ImageClicked = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.btnClose.ImageNormal = global::SDMS.Properties.Resources.Close_40_40_Default;
            this.btnClose.Location = new System.Drawing.Point(420, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 11;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPSMMaterial
            // 
            this.btnPSMMaterial.BackColor = System.Drawing.Color.Transparent;
            this.btnPSMMaterial.ButtonText = "";
            this.btnPSMMaterial.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPSMMaterial.ImageClicked = global::SDMS.Properties.Resources.PSMTankDetail_Material_Click;
            this.btnPSMMaterial.ImageDisabled = null;
            this.btnPSMMaterial.ImageMouseOver = global::SDMS.Properties.Resources.PSMTankDetail_Material_Click;
            this.btnPSMMaterial.ImageNormal = global::SDMS.Properties.Resources.PSMTankDetail_Material_Default;
            this.btnPSMMaterial.Location = new System.Drawing.Point(200, 9);
            this.btnPSMMaterial.Name = "btnPSMMaterial";
            this.btnPSMMaterial.Owner = null;
            this.btnPSMMaterial.Size = new System.Drawing.Size(50, 28);
            this.btnPSMMaterial.TabIndex = 12;
            this.btnPSMMaterial.TabStop = false;
            this.btnPSMMaterial.TextColor = System.Drawing.Color.Black;
            this.btnPSMMaterial.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPSMMaterial.ToolTipText = "";
            this.btnPSMMaterial.UseToolTip = false;
            this.btnPSMMaterial.WindowRateWidth = 1F;
            this.btnPSMMaterial.Click += new System.EventHandler(this.btnPSMMaterial_Click);
            // 
            // btnMSDS
            // 
            this.btnMSDS.BackColor = System.Drawing.Color.Transparent;
            this.btnMSDS.ButtonText = "";
            this.btnMSDS.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMSDS.ImageClicked = global::SDMS.Properties.Resources.PSMTankDetail_MSDS_Click;
            this.btnMSDS.ImageDisabled = null;
            this.btnMSDS.ImageMouseOver = global::SDMS.Properties.Resources.PSMTankDetail_MSDS_Click;
            this.btnMSDS.ImageNormal = global::SDMS.Properties.Resources.PSMTankDetail_MSDS_Default;
            this.btnMSDS.Location = new System.Drawing.Point(144, 9);
            this.btnMSDS.Name = "btnMSDS";
            this.btnMSDS.Owner = null;
            this.btnMSDS.Size = new System.Drawing.Size(50, 28);
            this.btnMSDS.TabIndex = 13;
            this.btnMSDS.TabStop = false;
            this.btnMSDS.TextColor = System.Drawing.Color.Black;
            this.btnMSDS.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMSDS.ToolTipText = "";
            this.btnMSDS.UseToolTip = false;
            this.btnMSDS.WindowRateWidth = 1F;
            this.btnMSDS.Click += new System.EventHandler(this.btnMSDS_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.BackgroundImage = global::SDMS.Properties.Resources.PSMTankDetail_Background1;
            this.panelLeft.Controls.Add(this.imgPSMUsual);
            this.panelLeft.Location = new System.Drawing.Point(0, 31);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(190, 292);
            this.panelLeft.TabIndex = 14;
            // 
            // panelRight
            // 
            this.panelRight.BackgroundImage = global::SDMS.Properties.Resources.PSMTankDetail_Background2;
            this.panelRight.Controls.Add(this.textBoxRemains);
            this.panelRight.Controls.Add(this.lblValueCapacity);
            this.panelRight.Controls.Add(this.chkCCTV);
            this.panelRight.Controls.Add(this.btnSelectUsed);
            this.panelRight.Controls.Add(this.chkMonitoring);
            this.panelRight.Controls.Add(this.btnMSDS);
            this.panelRight.Controls.Add(this.btnPSMMaterial);
            this.panelRight.Controls.Add(this.lblValueRemains);
            this.panelRight.Controls.Add(this.lblTitleOutAmount);
            this.panelRight.Controls.Add(this.cmbSelectUsed);
            this.panelRight.Controls.Add(this.lblTitleInAmount);
            this.panelRight.Controls.Add(this.lblValueInAmount);
            this.panelRight.Controls.Add(this.lblValueTankLocation);
            this.panelRight.Controls.Add(this.lblValueOutAmount);
            this.panelRight.Controls.Add(this.lblValueMaterialName);
            this.panelRight.Controls.Add(this.picMaterialName);
            this.panelRight.Controls.Add(this.picTankLocation);
            this.panelRight.Controls.Add(this.picRemains);
            this.panelRight.Controls.Add(this.picCapacity);
            this.panelRight.Location = new System.Drawing.Point(190, 31);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(269, 292);
            this.panelRight.TabIndex = 15;
            // 
            // picMaterialName
            // 
            this.picMaterialName.BackColor = System.Drawing.Color.Transparent;
            this.picMaterialName.Image = global::SDMS.Properties.Resources.PSMTankDetail_MaterialName;
            this.picMaterialName.Location = new System.Drawing.Point(19, 62);
            this.picMaterialName.Name = "picMaterialName";
            this.picMaterialName.Size = new System.Drawing.Size(231, 50);
            this.picMaterialName.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMaterialName.TabIndex = 28;
            this.picMaterialName.TabStop = false;
            // 
            // picTankLocation
            // 
            this.picTankLocation.BackColor = System.Drawing.Color.Transparent;
            this.picTankLocation.Image = global::SDMS.Properties.Resources.PSMTankDetail_TankLocation;
            this.picTankLocation.Location = new System.Drawing.Point(19, 118);
            this.picTankLocation.Name = "picTankLocation";
            this.picTankLocation.Size = new System.Drawing.Size(231, 50);
            this.picTankLocation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTankLocation.TabIndex = 27;
            this.picTankLocation.TabStop = false;
            // 
            // picRemains
            // 
            this.picRemains.BackColor = System.Drawing.Color.Transparent;
            this.picRemains.Image = global::SDMS.Properties.Resources.PSMTankDetail_Remains;
            this.picRemains.Location = new System.Drawing.Point(19, 174);
            this.picRemains.Name = "picRemains";
            this.picRemains.Size = new System.Drawing.Size(231, 50);
            this.picRemains.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRemains.TabIndex = 26;
            this.picRemains.TabStop = false;
            // 
            // picCapacity
            // 
            this.picCapacity.BackColor = System.Drawing.Color.Transparent;
            this.picCapacity.Image = global::SDMS.Properties.Resources.PSMTankDetail_Capacity;
            this.picCapacity.Location = new System.Drawing.Point(19, 230);
            this.picCapacity.Name = "picCapacity";
            this.picCapacity.Size = new System.Drawing.Size(231, 50);
            this.picCapacity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCapacity.TabIndex = 25;
            this.picCapacity.TabStop = false;
            // 
            // FormPSMTankDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BackgroundImage = global::SDMS.Properties.Resources.PSMTankDetail_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(459, 323);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pnlMonitor);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.labelRemains);
            this.DoubleBuffered = true;
            this.Name = "FormPSMTankDetail";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PSM Tank - Detail Information";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPSMTankDetail_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.imgPSMUsual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPSMMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMSDS)).EndInit();
            this.panelLeft.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMaterialName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTankLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRemains)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCapacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitleOutAmount;
        private System.Windows.Forms.Label lblTitleInAmount;
        private UnE.Controls.GifPictureBox imgPSMUsual;
        private System.Windows.Forms.ComboBox cmbSelectUsed;
        private System.Windows.Forms.Button btnSelectUsed;
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
        private System.Windows.Forms.TextBox textBoxRemains;
        private System.Windows.Forms.LabelEx labelRemains;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnPSMMaterial;
        private UnE.GUI.ImageButton btnMSDS;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.PictureBox picTankLocation;
        private System.Windows.Forms.PictureBox picRemains;
        private System.Windows.Forms.PictureBox picCapacity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picMaterialName;
    }
}