namespace ServerMonitor
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.stateMonitor = new System.Windows.Forms.Label();
            this.grpMonitor = new System.Windows.Forms.GroupBox();
            this.lbName1 = new System.Windows.Forms.Label();
            this.btnStop1 = new System.Windows.Forms.Button();
            this.btnStart1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.m_CheckTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUpdateImmediately = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.종료하기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnLogFolder = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUploadFile = new System.Windows.Forms.Button();
            this.grpMonitor.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // stateMonitor
            // 
            this.stateMonitor.BackColor = System.Drawing.Color.Red;
            this.stateMonitor.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.stateMonitor.ForeColor = System.Drawing.Color.White;
            this.stateMonitor.Location = new System.Drawing.Point(194, 26);
            this.stateMonitor.Name = "stateMonitor";
            this.stateMonitor.Size = new System.Drawing.Size(106, 26);
            this.stateMonitor.TabIndex = 0;
            this.stateMonitor.Text = "연결안됨";
            this.stateMonitor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpMonitor
            // 
            this.grpMonitor.Controls.Add(this.lbName1);
            this.grpMonitor.Controls.Add(this.btnStop1);
            this.grpMonitor.Controls.Add(this.btnStart1);
            this.grpMonitor.Controls.Add(this.stateMonitor);
            this.grpMonitor.Location = new System.Drawing.Point(13, 12);
            this.grpMonitor.Name = "grpMonitor";
            this.grpMonitor.Size = new System.Drawing.Size(319, 108);
            this.grpMonitor.TabIndex = 3;
            this.grpMonitor.TabStop = false;
            this.grpMonitor.Text = "서버상태";
            // 
            // lbName1
            // 
            this.lbName1.AutoSize = true;
            this.lbName1.Location = new System.Drawing.Point(18, 33);
            this.lbName1.Name = "lbName1";
            this.lbName1.Size = new System.Drawing.Size(53, 12);
            this.lbName1.TabIndex = 9;
            this.lbName1.Text = "모니터링";
            // 
            // btnStop1
            // 
            this.btnStop1.Enabled = false;
            this.btnStop1.Location = new System.Drawing.Point(147, 26);
            this.btnStop1.Name = "btnStop1";
            this.btnStop1.Size = new System.Drawing.Size(44, 26);
            this.btnStop1.TabIndex = 4;
            this.btnStop1.Text = "종료";
            this.btnStop1.UseVisualStyleBackColor = true;
            // 
            // btnStart1
            // 
            this.btnStart1.Enabled = true;
            this.btnStart1.Location = new System.Drawing.Point(101, 26);
            this.btnStart1.Name = "btnStart1";
            this.btnStart1.Size = new System.Drawing.Size(44, 26);
            this.btnStart1.TabIndex = 3;
            this.btnStart1.Text = "시작";
            this.btnStart1.UseVisualStyleBackColor = true;
            this.btnStart1.Click += new System.EventHandler(this.btnStart1_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(264, 135);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(68, 26);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // m_CheckTimer
            // 
            this.m_CheckTimer.Interval = 1000;
            this.m_CheckTimer.Tick += new System.EventHandler(this.m_CheckTimer_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnUpdateImmediately);
            this.groupBox2.Location = new System.Drawing.Point(364, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(319, 56);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "시스템 업데이트";
            this.groupBox2.Visible = false;
            // 
            // btnUpdateImmediately
            // 
            this.btnUpdateImmediately.Location = new System.Drawing.Point(101, 20);
            this.btnUpdateImmediately.Name = "btnUpdateImmediately";
            this.btnUpdateImmediately.Size = new System.Drawing.Size(90, 23);
            this.btnUpdateImmediately.TabIndex = 0;
            this.btnUpdateImmediately.Text = "즉시 업데이트";
            this.btnUpdateImmediately.UseVisualStyleBackColor = true;
            this.btnUpdateImmediately.Click += new System.EventHandler(this.btnUpdateImmediately_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "e재난 서버 모니터";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "e재난 서버 모니터";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.열기ToolStripMenuItem,
            this.종료하기ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 48);
            // 
            // 열기ToolStripMenuItem
            // 
            this.열기ToolStripMenuItem.Name = "열기ToolStripMenuItem";
            this.열기ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.열기ToolStripMenuItem.Text = "열기";
            this.열기ToolStripMenuItem.Click += new System.EventHandler(this.열기ToolStripMenuItem_Click);
            // 
            // 종료하기ToolStripMenuItem
            // 
            this.종료하기ToolStripMenuItem.Name = "종료하기ToolStripMenuItem";
            this.종료하기ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.종료하기ToolStripMenuItem.Text = "종료하기";
            this.종료하기ToolStripMenuItem.Click += new System.EventHandler(this.종료하기ToolStripMenuItem_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnLogFolder);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(364, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(319, 108);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "시스템 로그";
            this.groupBox3.Visible = false;
            // 
            // btnLogFolder
            // 
            this.btnLogFolder.Location = new System.Drawing.Point(147, 56);
            this.btnLogFolder.Name = "btnLogFolder";
            this.btnLogFolder.Size = new System.Drawing.Size(108, 26);
            this.btnLogFolder.TabIndex = 20;
            this.btnLogFolder.Text = "백업 폴더 지정";
            this.btnLogFolder.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(147, 20);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(108, 26);
            this.button6.TabIndex = 21;
            this.button6.Text = "로그 백업 받기";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(64, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "로그경로";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(64, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "서버로그";
            // 
            // btnUploadFile
            // 
            this.btnUploadFile.Location = new System.Drawing.Point(33, 135);
            this.btnUploadFile.Name = "btnUploadFile";
            this.btnUploadFile.Size = new System.Drawing.Size(94, 26);
            this.btnUploadFile.TabIndex = 8;
            this.btnUploadFile.Text = "업데이트 파일";
            this.btnUploadFile.UseVisualStyleBackColor = true;
            this.btnUploadFile.Click += new System.EventHandler(this.btnUploadFile_Clicked);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 183);
            this.Controls.Add(this.btnUploadFile);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.grpMonitor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "e-재난 서버 모니터";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.grpMonitor.ResumeLayout(false);
            this.grpMonitor.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label stateMonitor;
        private System.Windows.Forms.GroupBox grpMonitor;
        private System.Windows.Forms.Button btnStop1;
        private System.Windows.Forms.Button btnStart1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lbName1;
        private System.Windows.Forms.Timer m_CheckTimer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnUpdateImmediately;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 열기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 종료하기ToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnLogFolder;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUploadFile;


    }
}

