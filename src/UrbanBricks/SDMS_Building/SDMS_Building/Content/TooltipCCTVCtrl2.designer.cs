namespace SDMS_Building.Content
{
    partial class TooltipCCTVCtrl2
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
            this.components = new System.ComponentModel.Container();
            this.cctvPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnTop = new System.Windows.Forms.Panel();
            this.pictureBoxIndex = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // cctvPanel
            // 
            this.cctvPanel.Location = new System.Drawing.Point(0, 39);
            this.cctvPanel.Name = "cctvPanel";
            this.cctvPanel.Size = new System.Drawing.Size(300, 235);
            this.cctvPanel.TabIndex = 0;
            this.cctvPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cctvPanel_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // pnTop
            // 
            this.pnTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(101)))), ((int)(((byte)(150)))));
            this.pnTop.Controls.Add(this.pictureBoxIndex);
            this.pnTop.Controls.Add(this.lblTitle);
            this.pnTop.Controls.Add(this.panel3);
            this.pnTop.Controls.Add(this.btnClose);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(300, 33);
            this.pnTop.TabIndex = 2;
            this.pnTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseDown);
            this.pnTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseMove);
            this.pnTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseUp);
            // 
            // pictureBoxIndex
            // 
            this.pictureBoxIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxIndex.BackgroundImage = global::SDMS_Building.Properties.Resources._1;
            this.pictureBoxIndex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxIndex.Location = new System.Drawing.Point(231, 2);
            this.pictureBoxIndex.Name = "pictureBoxIndex";
            this.pictureBoxIndex.Size = new System.Drawing.Size(29, 29);
            this.pictureBoxIndex.TabIndex = 18;
            this.pictureBoxIndex.TabStop = false;
            this.pictureBoxIndex.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(38, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(49, 19);
            this.lblTitle.TabIndex = 17;
            this.lblTitle.Text = "CCTV";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseDown);
            this.lblTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseMove);
            this.lblTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseUp);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(20, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(7, 7);
            this.panel3.TabIndex = 16;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::SDMS_Building.Properties.Resources.close_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS_Building.Properties.Resources.close_Hover;
            this.btnClose.ImageNormal = global::SDMS_Building.Properties.Resources.close_Normal;
            this.btnClose.Location = new System.Drawing.Point(273, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(15, 15);
            this.btnClose.TabIndex = 15;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // TooltipCCTVCtrl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 286);
            this.Controls.Add(this.pnTop);
            this.Controls.Add(this.cctvPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(840, 800);
            this.MinimizeBox = false;
            this.Name = "TooltipCCTVCtrl2";
            this.Text = "TooltipCCTVCtrl2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TooltipCCTVCtrl2_FormClosing);
            this.Resize += new System.EventHandler(this.TooltipCCTVCtrl2_Resize);
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cctvPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.PictureBox pictureBoxIndex;
    }
}