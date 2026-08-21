namespace CCTVViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.axRASplus_WatSear1 = new AxRASplus_WatSearLib.AxRASplus_WatSear();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelIP = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelCCTVName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axRASplus_WatSear1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // axRASplus_WatSear1
            // 
            this.axRASplus_WatSear1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRASplus_WatSear1.Enabled = true;
            this.axRASplus_WatSear1.Location = new System.Drawing.Point(0, 0);
            this.axRASplus_WatSear1.Name = "axRASplus_WatSear1";
            this.axRASplus_WatSear1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRASplus_WatSear1.OcxState")));
            this.axRASplus_WatSear1.Size = new System.Drawing.Size(284, 248);
            this.axRASplus_WatSear1.TabIndex = 0;
            this.axRASplus_WatSear1.EventLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_EventLoadedEventHandler(this.axRASplus_WatSear1_EventLoaded);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelIP);
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 25);
            this.panel1.TabIndex = 1;
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(6, 7);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(81, 12);
            this.labelIP.TabIndex = 2;
            this.labelIP.Text = "CCTV ID && IP";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.Controls.Add(this.labelCCTVName);
            this.panel2.Location = new System.Drawing.Point(6, 215);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(180, 25);
            this.panel2.TabIndex = 1;
            // 
            // labelCCTVName
            // 
            this.labelCCTVName.AutoSize = true;
            this.labelCCTVName.Location = new System.Drawing.Point(6, 7);
            this.labelCCTVName.Name = "labelCCTVName";
            this.labelCCTVName.Size = new System.Drawing.Size(67, 12);
            this.labelCCTVName.TabIndex = 2;
            this.labelCCTVName.Text = "CCTV 이름";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 248);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.axRASplus_WatSear1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axRASplus_WatSear1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AxRASplus_WatSearLib.AxRASplus_WatSear axRASplus_WatSear1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelCCTVName;
    }
}

