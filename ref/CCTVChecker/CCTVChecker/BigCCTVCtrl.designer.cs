namespace CCTVChecker
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
            this.axxpressStrm1 = new AxxpressStrmLib.AxxpressStrm();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.labelID = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).BeginInit();
            this.SuspendLayout();
            // 
            // axxpressStrm1
            // 
            this.axxpressStrm1.Dock = System.Windows.Forms.DockStyle.Top;
            this.axxpressStrm1.Enabled = true;
            this.axxpressStrm1.Location = new System.Drawing.Point(0, 0);
            this.axxpressStrm1.Name = "axxpressStrm1";
            this.axxpressStrm1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axxpressStrm1.OcxState")));
            this.axxpressStrm1.Size = new System.Drawing.Size(397, 268);
            this.axxpressStrm1.TabIndex = 0;
            this.axxpressStrm1.Notify += new AxxpressStrmLib._DxpressStrmEvents_NotifyEventHandler(this.axxpressStrm1_Notify);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomIn.BackgroundImage = global::CCTVChecker.Properties.Resources.CCTV_ZoomIn;
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomIn.Location = new System.Drawing.Point(269, 305);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(49, 42);
            this.btnZoomIn.TabIndex = 4;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomIn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRight.BackgroundImage = global::CCTVChecker.Properties.Resources.CCTV_RIGHT_ARROW;
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRight.Location = new System.Drawing.Point(166, 284);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(35, 36);
            this.btnRight.TabIndex = 2;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeft.BackgroundImage = global::CCTVChecker.Properties.Resources.CCTV_LEFT_ARROW;
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLeft.Location = new System.Drawing.Point(41, 284);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(35, 36);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.BackgroundImage = global::CCTVChecker.Properties.Resources.CCTV_DOWN_ARROW;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(124, 284);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(36, 32);
            this.btnDown.TabIndex = 1;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.BackgroundImage = global::CCTVChecker.Properties.Resources.CCTV_UP_Arrow;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(82, 284);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(36, 32);
            this.btnUp.TabIndex = 1;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomOut.BackgroundImage = global::CCTVChecker.Properties.Resources.CCTV_ZoomOut;
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomOut.Location = new System.Drawing.Point(324, 305);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(49, 42);
            this.btnZoomOut.TabIndex = 4;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Font = new System.Drawing.Font("굴림", 16F);
            this.labelID.Location = new System.Drawing.Point(186, 288);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(62, 22);
            this.labelID.TabIndex = 5;
            this.labelID.Text = "label1";
            this.labelID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelID_MouseDown);
            // 
            // BigCCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(397, 353);
            this.Controls.Add(this.labelID);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.axxpressStrm1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BigCCTVCtrl";
            this.Text = "BigCCTVCtrl";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BigCCTVCtrl_FormClosing);
            this.Load += new System.EventHandler(this.BigCCTVCtrl_Load);
            this.Shown += new System.EventHandler(this.BigCCTVCtrl_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BigCCTVCtrl_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigCCTVCtrl_MouseDown);
            this.Resize += new System.EventHandler(this.BigCCTVCtrl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxxpressStrmLib.AxxpressStrm axxpressStrm1;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Label labelID;
    }
}