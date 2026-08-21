namespace UnE.CCTV
{
    partial class BigCCTVCtrlOwner
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

            if (cctvCtrl1 != null)
            {
                cctvCtrl1.Disconnect();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BigCCTVCtrlOwner));
            this.lbTitle = new System.Windows.Forms.Label();
            this.panelPTZ = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboPreset_Control = new System.Windows.Forms.ComboBox();
            this.btnPresetMove = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPTZ = new System.Windows.Forms.Button();
            this.btnPTZEdit = new System.Windows.Forms.Button();
            this.panelPTZEdit = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblDefaultPreset = new System.Windows.Forms.Label();
            this.cboPreset_PTZEdit = new System.Windows.Forms.ComboBox();
            this.btnPTZSave = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panelPTZ.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelPTZEdit.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.panelPTZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPTZ.BackColor = System.Drawing.Color.Black;
            this.panelPTZ.Controls.Add(this.panel1);
            this.panelPTZ.Location = new System.Drawing.Point(515, 33);
            this.panelPTZ.Name = "panelPTZ";
            this.panelPTZ.Size = new System.Drawing.Size(87, 208);
            this.panelPTZ.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.cboPreset_Control);
            this.panel1.Controls.Add(this.btnPresetMove);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnRight);
            this.panel1.Controls.Add(this.btnLeft);
            this.panel1.Controls.Add(this.btnDown);
            this.panel1.Controls.Add(this.btnUp);
            this.panel1.Controls.Add(this.btnZoomOut);
            this.panel1.Controls.Add(this.btnZoomIn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(83, 203);
            this.panel1.TabIndex = 9;
            // 
            // cboPreset_Control
            // 
            this.cboPreset_Control.BackColor = System.Drawing.Color.White;
            this.cboPreset_Control.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPreset_Control.FormattingEnabled = true;
            this.cboPreset_Control.Location = new System.Drawing.Point(1, 151);
            this.cboPreset_Control.Name = "cboPreset_Control";
            this.cboPreset_Control.Size = new System.Drawing.Size(82, 20);
            this.cboPreset_Control.TabIndex = 15;
            // 
            // btnPresetMove
            // 
            this.btnPresetMove.Location = new System.Drawing.Point(3, 176);
            this.btnPresetMove.Name = "btnPresetMove";
            this.btnPresetMove.Size = new System.Drawing.Size(77, 23);
            this.btnPresetMove.TabIndex = 15;
            this.btnPresetMove.Text = "이동";
            this.btnPresetMove.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnStop.Location = new System.Drawing.Point(29, 53);
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
            // btnRight
            // 
            this.btnRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRight.BackgroundImage")));
            this.btnRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRight.Location = new System.Drawing.Point(57, 53);
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
            this.btnLeft.Location = new System.Drawing.Point(1, 53);
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
            this.btnDown.Location = new System.Drawing.Point(29, 81);
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
            this.btnUp.Location = new System.Drawing.Point(29, 26);
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
            this.btnZoomOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.BackgroundImage")));
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomOut.Location = new System.Drawing.Point(47, 111);
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
            this.btnZoomIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.BackgroundImage")));
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnZoomIn.Location = new System.Drawing.Point(10, 111);
            this.btnZoomIn.Margin = new System.Windows.Forms.Padding(0);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(28, 25);
            this.btnZoomIn.TabIndex = 6;
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonDown);
            this.btnZoomIn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnCommandButtonUp);
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
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPTZ
            // 
            this.btnPTZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPTZ.BackColor = System.Drawing.Color.White;
            this.btnPTZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPTZ.Location = new System.Drawing.Point(556, 9);
            this.btnPTZ.Name = "btnPTZ";
            this.btnPTZ.Size = new System.Drawing.Size(46, 22);
            this.btnPTZ.TabIndex = 10;
            this.btnPTZ.Text = "제어";
            this.btnPTZ.UseVisualStyleBackColor = false;
            this.btnPTZ.Click += new System.EventHandler(this.btnPTZ_Click);
            // 
            // btnPTZEdit
            // 
            this.btnPTZEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPTZEdit.BackColor = System.Drawing.Color.White;
            this.btnPTZEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPTZEdit.Location = new System.Drawing.Point(463, 9);
            this.btnPTZEdit.Name = "btnPTZEdit";
            this.btnPTZEdit.Size = new System.Drawing.Size(87, 22);
            this.btnPTZEdit.TabIndex = 12;
            this.btnPTZEdit.Text = "Preset";
            this.btnPTZEdit.UseVisualStyleBackColor = false;
            this.btnPTZEdit.Click += new System.EventHandler(this.btnPTZEdit_Click);
            // 
            // panelPTZEdit
            // 
            this.panelPTZEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPTZEdit.BackColor = System.Drawing.Color.Black;
            this.panelPTZEdit.Controls.Add(this.panel3);
            this.panelPTZEdit.Location = new System.Drawing.Point(370, 9);
            this.panelPTZEdit.Name = "panelPTZEdit";
            this.panelPTZEdit.Size = new System.Drawing.Size(87, 110);
            this.panelPTZEdit.TabIndex = 13;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.lblDefaultPreset);
            this.panel3.Controls.Add(this.cboPreset_PTZEdit);
            this.panel3.Controls.Add(this.btnPTZSave);
            this.panel3.Controls.Add(this.button4);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(2, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(83, 105);
            this.panel3.TabIndex = 10;
            // 
            // lblDefaultPreset
            // 
            this.lblDefaultPreset.Location = new System.Drawing.Point(1, 28);
            this.lblDefaultPreset.Name = "lblDefaultPreset";
            this.lblDefaultPreset.Size = new System.Drawing.Size(80, 23);
            this.lblDefaultPreset.TabIndex = 16;
            this.lblDefaultPreset.Text = "None";
            this.lblDefaultPreset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboPreset_PTZEdit
            // 
            this.cboPreset_PTZEdit.BackColor = System.Drawing.Color.White;
            this.cboPreset_PTZEdit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPreset_PTZEdit.FormattingEnabled = true;
            this.cboPreset_PTZEdit.Location = new System.Drawing.Point(1, 53);
            this.cboPreset_PTZEdit.Name = "cboPreset_PTZEdit";
            this.cboPreset_PTZEdit.Size = new System.Drawing.Size(82, 20);
            this.cboPreset_PTZEdit.TabIndex = 15;
            this.cboPreset_PTZEdit.SelectedIndexChanged += new System.EventHandler(this.cboPreset_PTZEdit_SelectedIndexChanged);
            // 
            // btnPTZSave
            // 
            this.btnPTZSave.Location = new System.Drawing.Point(3, 80);
            this.btnPTZSave.Name = "btnPTZSave";
            this.btnPTZSave.Size = new System.Drawing.Size(77, 23);
            this.btnPTZSave.TabIndex = 15;
            this.btnPTZSave.Text = "저장";
            this.btnPTZSave.UseVisualStyleBackColor = true;
            this.btnPTZSave.Click += new System.EventHandler(this.btnPTZSave_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(63, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(20, 20);
            this.button4.TabIndex = 13;
            this.button4.Text = "x";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 21);
            this.label2.TabIndex = 12;
            this.label2.Text = "Preset";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cctvCtrl1
            // 
            this.cctvCtrl1.CCTVID = 0;
            this.cctvCtrl1.CCTVOwner = null;
            this.cctvCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
            this.cctvCtrl1.Name = "cctvCtrl1";
            this.cctvCtrl1.PositionIndex = -1;
            this.cctvCtrl1.Size = new System.Drawing.Size(630, 496);
            this.cctvCtrl1.TabIndex = 11;
            this.cctvCtrl1.Load += new System.EventHandler(this.cctvCtrl1_Load);
            this.cctvCtrl1.SizeChanged += new System.EventHandler(this.cctvCtrl1_SizeChanged);
            // 
            // BigCCTVCtrlOwner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(630, 496);
            this.Controls.Add(this.panelPTZEdit);
            this.Controls.Add(this.btnPTZEdit);
            this.Controls.Add(this.panelPTZ);
            this.Controls.Add(this.btnPTZ);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.cctvCtrl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BigCCTVCtrlOwner";
            this.Text = "BigCCTVCtrl";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BigCCTVCtrl_FormClosing);
            this.Load += new System.EventHandler(this.BigCCTVCtrl_Load);
            this.SizeChanged += new System.EventHandler(this.BigCCTVCtrl_SizeChanged);
            this.DoubleClick += new System.EventHandler(this.BigCCTVCtrl_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BigCCTVCtrl_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigCCTVCtrl_MouseDown);
            this.Resize += new System.EventHandler(this.BigCCTVCtrl_Resize);
            this.panelPTZ.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelPTZEdit.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnPresetMove;
        private System.Windows.Forms.ComboBox cboPreset_Control;
        private System.Windows.Forms.Button btnPTZEdit;
        private System.Windows.Forms.Panel panelPTZEdit;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox cboPreset_PTZEdit;
        private System.Windows.Forms.Button btnPTZSave;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDefaultPreset;
    }
}