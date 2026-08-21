namespace ServerRestartController
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.labelServiceNameSample = new System.Windows.Forms.Label();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnTTS = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSOP = new System.Windows.Forms.Label();
            this.lblTTS = new System.Windows.Forms.Label();
            this.btnTTSStop = new System.Windows.Forms.Button();
            this.btnSOPStop = new System.Windows.Forms.Button();
            this.btnTTSStart = new System.Windows.Forms.Button();
            this.btnSOPStart = new System.Windows.Forms.Button();
            this.pnSOPServer = new System.Windows.Forms.Panel();
            this.pnTTSServer = new System.Windows.Forms.Panel();
            this.pnSOPServer.SuspendLayout();
            this.pnTTSServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelServiceNameSample
            // 
            this.labelServiceNameSample.AutoSize = true;
            this.labelServiceNameSample.Location = new System.Drawing.Point(3, 10);
            this.labelServiceNameSample.Name = "labelServiceNameSample";
            this.labelServiceNameSample.Size = new System.Drawing.Size(144, 12);
            this.labelServiceNameSample.TabIndex = 1;
            this.labelServiceNameSample.Text = "시스템 서버 (SOPServer)";
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(448, 5);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(63, 23);
            this.btnRestart.TabIndex = 2;
            this.btnRestart.Text = "재시작";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Visible = false;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnTTS
            // 
            this.btnTTS.Location = new System.Drawing.Point(448, 6);
            this.btnTTS.Name = "btnTTS";
            this.btnTTS.Size = new System.Drawing.Size(63, 23);
            this.btnTTS.TabIndex = 4;
            this.btnTTS.Text = "재시작";
            this.btnTTS.UseVisualStyleBackColor = true;
            this.btnTTS.Visible = false;
            this.btnTTS.Click += new System.EventHandler(this.btnTTS_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "방송 서버 (TTSServer)";
            // 
            // lblSOP
            // 
            this.lblSOP.AutoSize = true;
            this.lblSOP.Location = new System.Drawing.Point(176, 10);
            this.lblSOP.Name = "lblSOP";
            this.lblSOP.Size = new System.Drawing.Size(53, 12);
            this.lblSOP.TabIndex = 5;
            this.lblSOP.Text = "알수없음";
            // 
            // lblTTS
            // 
            this.lblTTS.AutoSize = true;
            this.lblTTS.Location = new System.Drawing.Point(176, 11);
            this.lblTTS.Name = "lblTTS";
            this.lblTTS.Size = new System.Drawing.Size(53, 12);
            this.lblTTS.TabIndex = 6;
            this.lblTTS.Text = "알수없음";
            // 
            // btnTTSStop
            // 
            this.btnTTSStop.Location = new System.Drawing.Point(249, 6);
            this.btnTTSStop.Name = "btnTTSStop";
            this.btnTTSStop.Size = new System.Drawing.Size(63, 23);
            this.btnTTSStop.TabIndex = 8;
            this.btnTTSStop.Text = "중지";
            this.btnTTSStop.UseVisualStyleBackColor = true;
            this.btnTTSStop.Click += new System.EventHandler(this.btnTTSStop_Click);
            // 
            // btnSOPStop
            // 
            this.btnSOPStop.Location = new System.Drawing.Point(249, 5);
            this.btnSOPStop.Name = "btnSOPStop";
            this.btnSOPStop.Size = new System.Drawing.Size(63, 23);
            this.btnSOPStop.TabIndex = 7;
            this.btnSOPStop.Text = "중지";
            this.btnSOPStop.UseVisualStyleBackColor = true;
            this.btnSOPStop.Click += new System.EventHandler(this.btnSOPStop_Click);
            // 
            // btnTTSStart
            // 
            this.btnTTSStart.Location = new System.Drawing.Point(318, 6);
            this.btnTTSStart.Name = "btnTTSStart";
            this.btnTTSStart.Size = new System.Drawing.Size(63, 23);
            this.btnTTSStart.TabIndex = 10;
            this.btnTTSStart.Text = "시작";
            this.btnTTSStart.UseVisualStyleBackColor = true;
            this.btnTTSStart.Click += new System.EventHandler(this.btnTTSStart_Click);
            // 
            // btnSOPStart
            // 
            this.btnSOPStart.Location = new System.Drawing.Point(318, 5);
            this.btnSOPStart.Name = "btnSOPStart";
            this.btnSOPStart.Size = new System.Drawing.Size(63, 23);
            this.btnSOPStart.TabIndex = 9;
            this.btnSOPStart.Text = "시작";
            this.btnSOPStart.UseVisualStyleBackColor = true;
            this.btnSOPStart.Click += new System.EventHandler(this.btnSOPStart_Click);
            // 
            // pnSOPServer
            // 
            this.pnSOPServer.Controls.Add(this.labelServiceNameSample);
            this.pnSOPServer.Controls.Add(this.btnRestart);
            this.pnSOPServer.Controls.Add(this.btnSOPStart);
            this.pnSOPServer.Controls.Add(this.lblSOP);
            this.pnSOPServer.Controls.Add(this.btnSOPStop);
            this.pnSOPServer.Location = new System.Drawing.Point(12, 12);
            this.pnSOPServer.Name = "pnSOPServer";
            this.pnSOPServer.Size = new System.Drawing.Size(526, 34);
            this.pnSOPServer.TabIndex = 11;
            // 
            // pnTTSServer
            // 
            this.pnTTSServer.Controls.Add(this.label1);
            this.pnTTSServer.Controls.Add(this.btnTTS);
            this.pnTTSServer.Controls.Add(this.btnTTSStart);
            this.pnTTSServer.Controls.Add(this.lblTTS);
            this.pnTTSServer.Controls.Add(this.btnTTSStop);
            this.pnTTSServer.Location = new System.Drawing.Point(12, 52);
            this.pnTTSServer.Name = "pnTTSServer";
            this.pnTTSServer.Size = new System.Drawing.Size(526, 36);
            this.pnTTSServer.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 98);
            this.Controls.Add(this.pnTTSServer);
            this.Controls.Add(this.pnSOPServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "서버 원격 제어";
            this.pnSOPServer.ResumeLayout(false);
            this.pnSOPServer.PerformLayout();
            this.pnTTSServer.ResumeLayout(false);
            this.pnTTSServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelServiceNameSample;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnTTS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSOP;
        private System.Windows.Forms.Label lblTTS;
        private System.Windows.Forms.Button btnTTSStop;
        private System.Windows.Forms.Button btnSOPStop;
        private System.Windows.Forms.Button btnTTSStart;
        private System.Windows.Forms.Button btnSOPStart;
        private System.Windows.Forms.Panel pnSOPServer;
        private System.Windows.Forms.Panel pnTTSServer;
    }
}

