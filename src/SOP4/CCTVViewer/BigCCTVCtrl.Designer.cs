namespace UnE.CCTV
{
    partial class BigCCTVCtrl
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

            //if( cctvCtrl1 != null)
            //{
            //    cctvCtrl1.Disconnect();
            //}
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BigCCTVCtrl));
            this.lbTitle = new System.Windows.Forms.Label();
            this.panelPTZ = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnStop = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnPTZ = new System.Windows.Forms.Button();
            //this.cctvCtrl1 = new UnE.Control.CCTVCtrl();
            this.panelPTZ.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Black;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(12, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(143, 25);
            this.lbTitle.TabIndex = 6;
            this.lbTitle.Text = "CCTV정보 없음";
            this.lbTitle.Click += new System.EventHandler(this.lbTitle_Click);
            this.lbTitle.DoubleClick += new System.EventHandler(this.lbTitle_DoubleClick);
            // 
            // panelPTZ
            // 
            this.panelPTZ.BackColor = System.Drawing.Color.Black;
            this.panelPTZ.Controls.Add(this.panel1);
            this.panelPTZ.Location = new System.Drawing.Point(705, 9);
            this.panelPTZ.Name = "panelPTZ";
            this.panelPTZ.Size = new System.Drawing.Size(87, 136);
            this.panelPTZ.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnRight);
            this.panel1.Controls.Add(this.btnLeft);
            this.panel1.Controls.Add(this.btnDown);
            this.panel1.Controls.Add(this.btnUp);
            this.panel1.Controls.Add(this.btnZoomOut);
            this.panel1.Controls.Add(this.btnZoomIn);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(83, 132);
            this.panel1.TabIndex = 9;
            // 
            // btnStop
            // 
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnStop.Location = new System.Drawing.Point(29, 48);
            this.btnStop.Margin = new System.Windows.Forms.Padding(0);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(25, 25);
            this.btnStop.TabIndex = 14;
            this.btnStop.Text = "■";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            this.btnStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(63, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(20, 20);
            this.button1.TabIndex = 13;
            this.button1.Text = "x";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 21);
            this.label1.TabIndex = 12;
            this.label1.Text = "제어";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRight
            // 
            this.btnRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRight.BackgroundImage")));
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRight.Location = new System.Drawing.Point(57, 48);
            this.btnRight.Margin = new System.Windows.Forms.Padding(0);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(25, 25);
            this.btnRight.TabIndex = 9;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnLeft
            // 
            this.btnLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLeft.BackgroundImage")));
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLeft.Location = new System.Drawing.Point(1, 48);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(25, 25);
            this.btnLeft.TabIndex = 10;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnDown
            // 
            this.btnDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDown.BackgroundImage")));
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(29, 76);
            this.btnDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(25, 25);
            this.btnDown.TabIndex = 7;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnUp
            // 
            this.btnUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUp.BackgroundImage")));
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(29, 21);
            this.btnUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(25, 25);
            this.btnUp.TabIndex = 8;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnZoomOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.BackgroundImage")));
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomOut.Location = new System.Drawing.Point(47, 102);
            this.btnZoomOut.Margin = new System.Windows.Forms.Padding(0);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(28, 25);
            this.btnZoomOut.TabIndex = 5;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnZoomIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.BackgroundImage")));
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomIn.Location = new System.Drawing.Point(10, 102);
            this.btnZoomIn.Margin = new System.Windows.Forms.Padding(0);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(28, 25);
            this.btnZoomIn.TabIndex = 6;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomIn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnPTZ
            // 
            this.btnPTZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPTZ.BackColor = System.Drawing.Color.White;
            this.btnPTZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPTZ.Location = new System.Drawing.Point(746, 9);
            this.btnPTZ.Name = "button2";
            this.btnPTZ.Size = new System.Drawing.Size(46, 22);
            this.btnPTZ.TabIndex = 10;
            this.btnPTZ.Text = "제어";
            this.btnPTZ.UseVisualStyleBackColor = false;
            this.btnPTZ.Click += new System.EventHandler(this.btnPTZ_Click);
            // 
            // cctvCtrl1
            // 
            this.cctvCtrl1.BackColor = System.Drawing.Color.Black;
            this.cctvCtrl1.CCTVOwner = null;
            this.cctvCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
            this.cctvCtrl1.Name = "cctvCtrl1";
            this.cctvCtrl1.Size = new System.Drawing.Size(801, 408);
            this.cctvCtrl1.TabIndex = 7;
            this.cctvCtrl1.Load += new System.EventHandler(this.cctvCtrl1_Load);
            this.cctvCtrl1.SizeChanged += new System.EventHandler(this.cctvCtrl1_SizeChanged);
            // 
            // BigCCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(801, 408);
            this.Controls.Add(this.panelPTZ);
            this.Controls.Add(this.btnPTZ);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.cctvCtrl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BigCCTVCtrl";
            this.Text = "BigCCTVCtrl";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BigCCTVCtrl_FormClosing);
            this.Load += new System.EventHandler(this.BigCCTVCtrl_Load);
            this.Shown += new System.EventHandler(this.BigCCTVCtrl_Shown);
            this.SizeChanged += new System.EventHandler(this.BigCCTVCtrl_SizeChanged);
            this.DoubleClick += new System.EventHandler(this.BigCCTVCtrl_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BigCCTVCtrl_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigCCTVCtrl_MouseDown);
            this.Resize += new System.EventHandler(this.BigCCTVCtrl_Resize);
            this.panelPTZ.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private UnE.Control.CCTVCtrl cctvCtrl1;
        private System.Windows.Forms.Panel panelPTZ;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnPTZ;
        private System.Windows.Forms.Button btnStop;
    }
}