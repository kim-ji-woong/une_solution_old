namespace SDMS
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
            this.pictureBoxCross = new System.Windows.Forms.PictureBox();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCross)).BeginInit();
            this.SuspendLayout();
            // 
            // axxpressStrm1
            // 
            this.axxpressStrm1.Dock = System.Windows.Forms.DockStyle.Left;
            this.axxpressStrm1.Enabled = true;
            this.axxpressStrm1.Location = new System.Drawing.Point(0, 0);
            this.axxpressStrm1.Name = "axxpressStrm1";
            this.axxpressStrm1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axxpressStrm1.OcxState")));
            this.axxpressStrm1.Size = new System.Drawing.Size(456, 457);
            this.axxpressStrm1.TabIndex = 0;
            this.axxpressStrm1.Notify += new AxxpressStrmLib._DxpressStrmEvents_NotifyEventHandler(this.axxpressStrm1_Notify);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackgroundImage = global::SDMS.Properties.Resources.CCTV_ZoomIn;
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomIn.Location = new System.Drawing.Point(481, 348);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(49, 42);
            this.btnZoomIn.TabIndex = 4;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomIn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // pictureBoxCross
            // 
            this.pictureBoxCross.BackgroundImage = global::SDMS.Properties.Resources.CCTV_CENTER_CROSS;
            this.pictureBoxCross.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxCross.Location = new System.Drawing.Point(527, 90);
            this.pictureBoxCross.Name = "pictureBoxCross";
            this.pictureBoxCross.Size = new System.Drawing.Size(37, 36);
            this.pictureBoxCross.TabIndex = 3;
            this.pictureBoxCross.TabStop = false;
            // 
            // btnRight
            // 
            this.btnRight.BackgroundImage = global::SDMS.Properties.Resources.CCTV_RIGHT_ARROW;
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRight.Location = new System.Drawing.Point(575, 90);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(35, 36);
            this.btnRight.TabIndex = 2;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnLeft
            // 
            this.btnLeft.BackgroundImage = global::SDMS.Properties.Resources.CCTV_LEFT_ARROW;
            this.btnLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLeft.Location = new System.Drawing.Point(481, 90);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(35, 36);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnDown
            // 
            this.btnDown.BackgroundImage = global::SDMS.Properties.Resources.CCTV_DOWN_ARROW;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(527, 139);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(36, 32);
            this.btnDown.TabIndex = 1;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnUp
            // 
            this.btnUp.BackgroundImage = global::SDMS.Properties.Resources.CCTV_UP_Arrow;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(527, 46);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(36, 32);
            this.btnUp.TabIndex = 1;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackgroundImage = global::SDMS.Properties.Resources.CCTV_ZoomOut;
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomOut.Location = new System.Drawing.Point(561, 348);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(49, 42);
            this.btnZoomOut.TabIndex = 4;
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // BigCCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(632, 457);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.pictureBoxCross);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCross)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxxpressStrmLib.AxxpressStrm axxpressStrm1;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.PictureBox pictureBoxCross;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
    }
}