namespace BroadcastServer
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuChangeSiteID = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuStopBroadcast = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuRunBroadcast = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tsMenuAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuAddNoSiren = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuPause = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuResume = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "TTS 방송 서버";
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuChangeSiteID,
            this.tsMenuStopBroadcast,
            this.tsMenuRunBroadcast,
            this.toolStripSeparator1,
            this.tsMenuClose,
            this.tsMenuAdd,
            this.tsMenuAddNoSiren,
            this.tsMenuPause,
            this.tsMenuResume});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 208);
            // 
            // tsMenuChangeSiteID
            // 
            this.tsMenuChangeSiteID.Name = "tsMenuChangeSiteID";
            this.tsMenuChangeSiteID.Size = new System.Drawing.Size(180, 22);
            this.tsMenuChangeSiteID.Text = "Site ID 변경";
            this.tsMenuChangeSiteID.Click += new System.EventHandler(this.tsMenuChangeSiteID_Click);
            // 
            // tsMenuStopBroadcast
            // 
            this.tsMenuStopBroadcast.Enabled = false;
            this.tsMenuStopBroadcast.Name = "tsMenuStopBroadcast";
            this.tsMenuStopBroadcast.Size = new System.Drawing.Size(180, 22);
            this.tsMenuStopBroadcast.Text = "방송 중단";
            this.tsMenuStopBroadcast.Click += new System.EventHandler(this.tsMenuStopBroadcast_Click);
            // 
            // tsMenuRunBroadcast
            // 
            this.tsMenuRunBroadcast.Enabled = false;
            this.tsMenuRunBroadcast.Name = "tsMenuRunBroadcast";
            this.tsMenuRunBroadcast.Size = new System.Drawing.Size(180, 22);
            this.tsMenuRunBroadcast.Text = "방송 재개";
            this.tsMenuRunBroadcast.Click += new System.EventHandler(this.tsMenuRunBroadcast_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(180, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // tsMenuAdd
            // 
            this.tsMenuAdd.Name = "tsMenuAdd";
            this.tsMenuAdd.Size = new System.Drawing.Size(180, 22);
            this.tsMenuAdd.Text = "방송추가(사이렌)";
            this.tsMenuAdd.Click += new System.EventHandler(this.tsMenuAdd_Click);
            // 
            // tsMenuAddNoSiren
            // 
            this.tsMenuAddNoSiren.Name = "tsMenuAddNoSiren";
            this.tsMenuAddNoSiren.Size = new System.Drawing.Size(180, 22);
            this.tsMenuAddNoSiren.Text = "방송추가";
            this.tsMenuAddNoSiren.Click += new System.EventHandler(this.tsMenuAddNoSiren_Click);
            // 
            // tsMenuPause
            // 
            this.tsMenuPause.Name = "tsMenuPause";
            this.tsMenuPause.Size = new System.Drawing.Size(180, 22);
            this.tsMenuPause.Text = "일시정지";
            this.tsMenuPause.Click += new System.EventHandler(this.tsMenuPause_Click);
            // 
            // tsMenuResume
            // 
            this.tsMenuResume.Name = "tsMenuResume";
            this.tsMenuResume.Size = new System.Drawing.Size(180, 22);
            this.tsMenuResume.Text = "다시시작";
            this.tsMenuResume.Click += new System.EventHandler(this.tsMenuResume_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 261);
            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuChangeSiteID;
        private System.Windows.Forms.ToolStripMenuItem tsMenuStopBroadcast;
        private System.Windows.Forms.ToolStripMenuItem tsMenuRunBroadcast;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAdd;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAddNoSiren;
        private System.Windows.Forms.ToolStripMenuItem tsMenuPause;
        private System.Windows.Forms.ToolStripMenuItem tsMenuResume;
    }
}