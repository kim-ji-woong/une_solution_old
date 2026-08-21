namespace SDMS
{
    partial class TooltipCCTVCtrl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TooltipCCTVCtrl));
            this.checkBoxFix = new System.Windows.Forms.CheckBox();
            this.checkBoxLOD = new System.Windows.Forms.CheckBox();
            //this.cctvCtrl1 = new UnE.Control.CCTVCtrl();
            this.btnControl = new System.Windows.Forms.Button();
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
            this.panelPTZ.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxFix
            // 
            this.checkBoxFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFix.AutoSize = true;
            this.checkBoxFix.Location = new System.Drawing.Point(203, 283);
            this.checkBoxFix.Name = "checkBoxFix";
            this.checkBoxFix.Size = new System.Drawing.Size(88, 16);
            this.checkBoxFix.TabIndex = 7;
            this.checkBoxFix.Text = "섬네일 고정";
            this.checkBoxFix.UseVisualStyleBackColor = true;
            this.checkBoxFix.Visible = false;
            // 
            // checkBoxLOD
            // 
            this.checkBoxLOD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLOD.AutoSize = true;
            this.checkBoxLOD.Location = new System.Drawing.Point(12, 283);
            this.checkBoxLOD.Name = "checkBoxLOD";
            this.checkBoxLOD.Size = new System.Drawing.Size(48, 16);
            this.checkBoxLOD.TabIndex = 9;
            this.checkBoxLOD.Text = "활성";
            this.checkBoxLOD.UseVisualStyleBackColor = true;
            this.checkBoxLOD.Visible = false;
            // 
            // cctvCtrl1
            // 
            this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cctvCtrl1.BackColor = System.Drawing.Color.Black;
            this.cctvCtrl1.CCTVOwner = null;
            this.cctvCtrl1.Location = new System.Drawing.Point(12, 12);
            this.cctvCtrl1.Name = "cctvCtrl1";
            this.cctvCtrl1.Size = new System.Drawing.Size(279, 246);
            this.cctvCtrl1.TabIndex = 8;
            // 
            // button2
            // 
            this.btnControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnControl.BackColor = System.Drawing.Color.White;
            this.btnControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnControl.Location = new System.Drawing.Point(245, 12);
            this.btnControl.Name = "button2";
            this.btnControl.Size = new System.Drawing.Size(46, 22);
            this.btnControl.TabIndex = 11;
            this.btnControl.Text = "제어";
            this.btnControl.UseVisualStyleBackColor = false;
            this.btnControl.Click += new System.EventHandler(this.button2_Click);
            // 
            // panelPTZ
            // 
            this.panelPTZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPTZ.BackColor = System.Drawing.Color.Black;
            this.panelPTZ.Controls.Add(this.panel1);
            this.panelPTZ.Location = new System.Drawing.Point(207, 12);
            this.panelPTZ.Name = "panelPTZ";
            this.panelPTZ.Size = new System.Drawing.Size(87, 136);
            this.panelPTZ.TabIndex = 12;
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
            this.btnStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnStop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(63, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(20, 20);
            this.button1.TabIndex = 13;
            this.button1.Text = "x";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
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
            // TooltipCCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(306, 311);
            this.Controls.Add(this.panelPTZ);
            this.Controls.Add(this.btnControl);
            this.Controls.Add(this.cctvCtrl1);
            this.Controls.Add(this.checkBoxLOD);
            this.Controls.Add(this.checkBoxFix);
            this.MaximumSize = new System.Drawing.Size(840, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(316, 296);
            this.Name = "TooltipCCTVCtrl";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CCTV";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TooltipCCTVCtrl_FormClosing);
            this.Shown += new System.EventHandler(this.TooltipCCTVCtrl_Shown);
            this.LocationChanged += new System.EventHandler(this.TooltipCCTVCtrl_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.TooltipCCTVCtrl_SizeChanged);
            this.Move += new System.EventHandler(this.TooltipCCTVCtrl_Move);
            this.Resize += new System.EventHandler(this.TooltipCCTVCtrl_Resize);
            this.panelPTZ.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxFix;
        private UnE.Control.CCTVCtrl cctvCtrl1;
        private System.Windows.Forms.CheckBox checkBoxLOD;
        private System.Windows.Forms.Button btnControl;
        private System.Windows.Forms.Panel panelPTZ;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
    }
}
