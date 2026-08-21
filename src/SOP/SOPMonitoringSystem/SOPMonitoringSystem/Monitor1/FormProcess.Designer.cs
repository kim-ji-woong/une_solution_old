namespace SOPMonitoringSystem
{
    partial class FormProcess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcess));
            this.tsSOPControlMenu = new System.Windows.Forms.ToolStrip();
            this.tsbtnPlay = new System.Windows.Forms.ToolStripButton();
            this.tsbtnPause = new System.Windows.Forms.ToolStripButton();
            this.tsbtnStop = new System.Windows.Forms.ToolStripButton();
            this.tsbtnRestart = new System.Windows.Forms.ToolStripButton();
            this.tsbtnPan = new System.Windows.Forms.ToolStripButton();
            this.tsbtnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.tsbtnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.tsbtnFullScreen = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label4Scroll = new System.Windows.Forms.Label();
            this.tsSOPControlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsSOPControlMenu
            // 
            this.tsSOPControlMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsSOPControlMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnPlay,
            this.tsbtnPause,
            this.tsbtnStop,
            this.tsbtnRestart,
            this.tsbtnPan,
            this.tsbtnZoomOut,
            this.tsbtnZoomIn,
            this.tsbtnFullScreen,
            this.toolStripLabel1,
            this.toolStripLabel2});
            this.tsSOPControlMenu.Location = new System.Drawing.Point(0, 0);
            this.tsSOPControlMenu.Name = "tsSOPControlMenu";
            this.tsSOPControlMenu.Size = new System.Drawing.Size(470, 25);
            this.tsSOPControlMenu.TabIndex = 0;
            this.tsSOPControlMenu.Text = "toolStrip1";
            // 
            // tsbtnPlay
            // 
            this.tsbtnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPlay.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPlay.Image")));
            this.tsbtnPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPlay.Name = "tsbtnPlay";
            this.tsbtnPlay.Size = new System.Drawing.Size(23, 22);
            this.tsbtnPlay.Text = "재생";
            this.tsbtnPlay.Click += new System.EventHandler(this.tsbtnPlay_Click);
            // 
            // tsbtnPause
            // 
            this.tsbtnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPause.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPause.Image")));
            this.tsbtnPause.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPause.Name = "tsbtnPause";
            this.tsbtnPause.Size = new System.Drawing.Size(23, 22);
            this.tsbtnPause.Text = "일시정지";
            this.tsbtnPause.Click += new System.EventHandler(this.tsbtnPause_Click);
            // 
            // tsbtnStop
            // 
            this.tsbtnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnStop.Image")));
            this.tsbtnStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnStop.Name = "tsbtnStop";
            this.tsbtnStop.Size = new System.Drawing.Size(23, 22);
            this.tsbtnStop.Text = "정지";
            this.tsbtnStop.Click += new System.EventHandler(this.tsbtnStop_Click);
            // 
            // tsbtnRestart
            // 
            this.tsbtnRestart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnRestart.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnRestart.Image")));
            this.tsbtnRestart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnRestart.Name = "tsbtnRestart";
            this.tsbtnRestart.Size = new System.Drawing.Size(23, 22);
            this.tsbtnRestart.Text = "다시시작";
            this.tsbtnRestart.Click += new System.EventHandler(this.tsbtnRestart_Click);
            // 
            // tsbtnPan
            // 
            this.tsbtnPan.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbtnPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnPan.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnPan.Image")));
            this.tsbtnPan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnPan.Name = "tsbtnPan";
            this.tsbtnPan.Size = new System.Drawing.Size(23, 22);
            this.tsbtnPan.Text = "이동";
            // 
            // tsbtnZoomOut
            // 
            this.tsbtnZoomOut.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbtnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnZoomOut.Image")));
            this.tsbtnZoomOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnZoomOut.Name = "tsbtnZoomOut";
            this.tsbtnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.tsbtnZoomOut.Text = "축소";
            // 
            // tsbtnZoomIn
            // 
            this.tsbtnZoomIn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbtnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnZoomIn.Image")));
            this.tsbtnZoomIn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnZoomIn.Name = "tsbtnZoomIn";
            this.tsbtnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.tsbtnZoomIn.Text = "확대";
            // 
            // tsbtnFullScreen
            // 
            this.tsbtnFullScreen.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbtnFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtnFullScreen.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFullScreen.Image")));
            this.tsbtnFullScreen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbtnFullScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFullScreen.Name = "tsbtnFullScreen";
            this.tsbtnFullScreen.Size = new System.Drawing.Size(23, 22);
            this.tsbtnFullScreen.Text = "전체화면";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(88, 22);
            this.toolStripLabel1.Text = "toolStripLabel1";
            this.toolStripLabel1.Visible = false;
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(88, 22);
            this.toolStripLabel2.Text = "toolStripLabel2";
            this.toolStripLabel2.Visible = false;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "toolbar_SOPcontrol.png");
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label4Scroll
            // 
            this.label4Scroll.AutoSize = true;
            this.label4Scroll.Location = new System.Drawing.Point(375, 307);
            this.label4Scroll.Name = "label4Scroll";
            this.label4Scroll.Size = new System.Drawing.Size(70, 12);
            this.label4Scroll.TabIndex = 1;
            this.label4Scroll.Text = "label4Scroll";
            // 
            // FormProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(20, 20);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(470, 435);
            this.Controls.Add(this.label4Scroll);
            this.Controls.Add(this.tsSOPControlMenu);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormProcess";
            this.Text = "FormProcess";
            this.Load += new System.EventHandler(this.FormProcess_Load);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FormProcess_Scroll);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormProcess_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormProcess_MouseDown);
            this.Resize += new System.EventHandler(this.FormProcess_Resize);
            this.tsSOPControlMenu.ResumeLayout(false);
            this.tsSOPControlMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsSOPControlMenu;
        private System.Windows.Forms.ToolStripButton tsbtnPlay;
        private System.Windows.Forms.ToolStripButton tsbtnPause;
        private System.Windows.Forms.ToolStripButton tsbtnStop;
        private System.Windows.Forms.ToolStripButton tsbtnRestart;
        private System.Windows.Forms.ToolStripButton tsbtnFullScreen;
        private System.Windows.Forms.ToolStripButton tsbtnZoomIn;
        private System.Windows.Forms.ToolStripButton tsbtnZoomOut;
        private System.Windows.Forms.ToolStripButton tsbtnPan;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.Label label4Scroll;


    }
}