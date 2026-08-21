
namespace ServerMonitoring
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnConnectIntegration = new System.Windows.Forms.Button();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.lblServerStatusIntegration = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblClientStatusIntegration = new System.Windows.Forms.Label();
            this.lblClientStatus = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(290, 52);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(113, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "전용서버로 연결";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnConnectIntegration
            // 
            this.btnConnectIntegration.Enabled = false;
            this.btnConnectIntegration.Location = new System.Drawing.Point(290, 88);
            this.btnConnectIntegration.Name = "btnConnectIntegration";
            this.btnConnectIntegration.Size = new System.Drawing.Size(113, 23);
            this.btnConnectIntegration.TabIndex = 1;
            this.btnConnectIntegration.Text = "통합서버로 연결";
            this.btnConnectIntegration.UseVisualStyleBackColor = true;
            this.btnConnectIntegration.Click += new System.EventHandler(this.btnConnectIntegration_Click);
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblServerStatus.Location = new System.Drawing.Point(92, 52);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(73, 22);
            this.lblServerStatus.TabIndex = 2;
            this.lblServerStatus.Text = "label1";
            this.lblServerStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblServerStatusIntegration
            // 
            this.lblServerStatusIntegration.BackColor = System.Drawing.SystemColors.Control;
            this.lblServerStatusIntegration.Location = new System.Drawing.Point(92, 89);
            this.lblServerStatusIntegration.Name = "lblServerStatusIntegration";
            this.lblServerStatusIntegration.Size = new System.Drawing.Size(73, 22);
            this.lblServerStatusIntegration.TabIndex = 3;
            this.lblServerStatusIntegration.Text = "label2";
            this.lblServerStatusIntegration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 22);
            this.label1.TabIndex = 4;
            this.label1.Text = "전용 서버";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 22);
            this.label2.TabIndex = 5;
            this.label2.Text = "통합 서버";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(92, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 22);
            this.label3.TabIndex = 6;
            this.label3.Text = "서버 상태";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(171, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 22);
            this.label4.TabIndex = 9;
            this.label4.Text = "클라이언트 상태";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblClientStatusIntegration
            // 
            this.lblClientStatusIntegration.BackColor = System.Drawing.SystemColors.Control;
            this.lblClientStatusIntegration.Location = new System.Drawing.Point(171, 89);
            this.lblClientStatusIntegration.Name = "lblClientStatusIntegration";
            this.lblClientStatusIntegration.Size = new System.Drawing.Size(113, 22);
            this.lblClientStatusIntegration.TabIndex = 8;
            this.lblClientStatusIntegration.Text = "label2";
            this.lblClientStatusIntegration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblClientStatus
            // 
            this.lblClientStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblClientStatus.Location = new System.Drawing.Point(171, 52);
            this.lblClientStatus.Name = "lblClientStatus";
            this.lblClientStatus.Size = new System.Drawing.Size(113, 22);
            this.lblClientStatus.TabIndex = 7;
            this.lblClientStatus.Text = "label1";
            this.lblClientStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.종료ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(99, 26);
            // 
            // 종료ToolStripMenuItem
            // 
            this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            this.종료ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.종료ToolStripMenuItem.Text = "종료";
            this.종료ToolStripMenuItem.Click += new System.EventHandler(this.종료ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 134);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblClientStatusIntegration);
            this.Controls.Add(this.lblClientStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblServerStatusIntegration);
            this.Controls.Add(this.lblServerStatus);
            this.Controls.Add(this.btnConnectIntegration);
            this.Controls.Add(this.btnConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form1";
            this.Text = "서버 상태 정보";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnConnectIntegration;
        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.Label lblServerStatusIntegration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblClientStatusIntegration;
        private System.Windows.Forms.Label lblClientStatus;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
    }
}

