namespace CCTVWeb
{
    partial class CCTVWebViewer
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnIrisIn = new System.Windows.Forms.Button();
            this.btnIrisOut = new System.Windows.Forms.Button();
            this.btnFocusIn = new System.Windows.Forms.Button();
            this.btnFocusOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnAuto = new System.Windows.Forms.Button();
            this.btnDownRight = new System.Windows.Forms.Button();
            this.btnDownLeft = new System.Windows.Forms.Button();
            this.btnUpRight = new System.Windows.Forms.Button();
            this.btnUpLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnFullScreen = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(449, 286);
            this.panel1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.comboBox1.Location = new System.Drawing.Point(75, 292);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.Text = "1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 295);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Camera :";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.btnIrisIn);
            this.groupBox1.Controls.Add(this.btnIrisOut);
            this.groupBox1.Controls.Add(this.btnFocusIn);
            this.groupBox1.Controls.Add(this.btnFocusOut);
            this.groupBox1.Controls.Add(this.btnZoomIn);
            this.groupBox1.Controls.Add(this.btnZoomOut);
            this.groupBox1.Controls.Add(this.btnAuto);
            this.groupBox1.Controls.Add(this.btnDownRight);
            this.groupBox1.Controls.Add(this.btnDownLeft);
            this.groupBox1.Controls.Add(this.btnUpRight);
            this.groupBox1.Controls.Add(this.btnUpLeft);
            this.groupBox1.Controls.Add(this.btnRight);
            this.groupBox1.Controls.Add(this.btnLeft);
            this.groupBox1.Controls.Add(this.btnDown);
            this.groupBox1.Controls.Add(this.btnUp);
            this.groupBox1.Location = new System.Drawing.Point(13, 322);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 109);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ptz";
            // 
            // btnIrisIn
            // 
            this.btnIrisIn.Location = new System.Drawing.Point(262, 78);
            this.btnIrisIn.Name = "btnIrisIn";
            this.btnIrisIn.Size = new System.Drawing.Size(75, 23);
            this.btnIrisIn.TabIndex = 14;
            this.btnIrisIn.Text = "IRIS In";
            this.btnIrisIn.UseVisualStyleBackColor = true;
            this.btnIrisIn.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnIrisOut
            // 
            this.btnIrisOut.Location = new System.Drawing.Point(343, 78);
            this.btnIrisOut.Name = "btnIrisOut";
            this.btnIrisOut.Size = new System.Drawing.Size(75, 23);
            this.btnIrisOut.TabIndex = 13;
            this.btnIrisOut.Text = "IRIS Out";
            this.btnIrisOut.UseVisualStyleBackColor = true;
            this.btnIrisOut.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnFocusIn
            // 
            this.btnFocusIn.Location = new System.Drawing.Point(262, 49);
            this.btnFocusIn.Name = "btnFocusIn";
            this.btnFocusIn.Size = new System.Drawing.Size(75, 23);
            this.btnFocusIn.TabIndex = 12;
            this.btnFocusIn.Text = "FocusIn";
            this.btnFocusIn.UseVisualStyleBackColor = true;
            this.btnFocusIn.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnFocusOut
            // 
            this.btnFocusOut.Location = new System.Drawing.Point(343, 49);
            this.btnFocusOut.Name = "btnFocusOut";
            this.btnFocusOut.Size = new System.Drawing.Size(75, 23);
            this.btnFocusOut.TabIndex = 11;
            this.btnFocusOut.Text = "FocusOut";
            this.btnFocusOut.UseVisualStyleBackColor = true;
            this.btnFocusOut.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(262, 20);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(75, 23);
            this.btnZoomIn.TabIndex = 10;
            this.btnZoomIn.Text = "ZoomIn";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(343, 20);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(75, 23);
            this.btnZoomOut.TabIndex = 9;
            this.btnZoomOut.Text = "ZoomOut";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnAuto
            // 
            this.btnAuto.Location = new System.Drawing.Point(87, 49);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(75, 23);
            this.btnAuto.TabIndex = 8;
            this.btnAuto.Text = "Auto";
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnDownRight
            // 
            this.btnDownRight.Location = new System.Drawing.Point(168, 78);
            this.btnDownRight.Name = "btnDownRight";
            this.btnDownRight.Size = new System.Drawing.Size(75, 23);
            this.btnDownRight.TabIndex = 7;
            this.btnDownRight.Text = "DownRight";
            this.btnDownRight.UseVisualStyleBackColor = true;
            this.btnDownRight.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnDownLeft
            // 
            this.btnDownLeft.Location = new System.Drawing.Point(6, 78);
            this.btnDownLeft.Name = "btnDownLeft";
            this.btnDownLeft.Size = new System.Drawing.Size(75, 23);
            this.btnDownLeft.TabIndex = 6;
            this.btnDownLeft.Text = "DownLeft";
            this.btnDownLeft.UseVisualStyleBackColor = true;
            this.btnDownLeft.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnUpRight
            // 
            this.btnUpRight.Location = new System.Drawing.Point(168, 20);
            this.btnUpRight.Name = "btnUpRight";
            this.btnUpRight.Size = new System.Drawing.Size(75, 23);
            this.btnUpRight.TabIndex = 5;
            this.btnUpRight.Text = "UpRight";
            this.btnUpRight.UseVisualStyleBackColor = true;
            this.btnUpRight.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnUpLeft
            // 
            this.btnUpLeft.Location = new System.Drawing.Point(6, 20);
            this.btnUpLeft.Name = "btnUpLeft";
            this.btnUpLeft.Size = new System.Drawing.Size(75, 23);
            this.btnUpLeft.TabIndex = 4;
            this.btnUpLeft.Text = "UpLeft";
            this.btnUpLeft.UseVisualStyleBackColor = true;
            this.btnUpLeft.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(168, 49);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 3;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(6, 49);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 2;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(87, 78);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 1;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(87, 20);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.OnBtnClickPtz);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFullScreen.Location = new System.Drawing.Point(326, 290);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(111, 23);
            this.btnFullScreen.TabIndex = 6;
            this.btnFullScreen.Text = "Full Screen";
            this.btnFullScreen.UseVisualStyleBackColor = true;
            this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
            // 
            // CCTVWebViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 441);
            this.Controls.Add(this.btnFullScreen);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "CCTVWebViewer";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnIrisIn;
        private System.Windows.Forms.Button btnIrisOut;
        private System.Windows.Forms.Button btnFocusIn;
        private System.Windows.Forms.Button btnFocusOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Button btnDownRight;
        private System.Windows.Forms.Button btnDownLeft;
        private System.Windows.Forms.Button btnUpRight;
        private System.Windows.Forms.Button btnUpLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnFullScreen;


    }
}

