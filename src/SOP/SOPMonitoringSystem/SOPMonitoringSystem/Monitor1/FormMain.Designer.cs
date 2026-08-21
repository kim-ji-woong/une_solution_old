namespace SOPMonitoringSystem
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
            this.axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.panelProcess = new System.Windows.Forms.Panel();
            this.axSkinFramework1 = new AxXtremeSkinFramework.AxSkinFramework();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).BeginInit();
            this.SuspendLayout();
            // 
            // axDockingPane
            // 
            this.axDockingPane.Dock = System.Windows.Forms.DockStyle.Left;
            this.axDockingPane.Enabled = true;
            this.axDockingPane.Location = new System.Drawing.Point(0, 0);
            this.axDockingPane.Name = "axDockingPane";
            this.axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane.OcxState")));
            this.axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.axDockingPane.TabIndex = 0;
            this.axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.axDockingPane_AttachPaneEvent);
            this.axDockingPane.ResizeEvent += new System.EventHandler(this.axDockingPane_ResizeEvent);
            // 
            // panelProcess
            // 
            this.panelProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelProcess.BackColor = System.Drawing.SystemColors.Control;
            this.panelProcess.Location = new System.Drawing.Point(260, 0);
            this.panelProcess.Name = "panelProcess";
            this.panelProcess.Size = new System.Drawing.Size(562, 314);
            this.panelProcess.TabIndex = 1;
            // 
            // axSkinFramework1
            // 
            this.axSkinFramework1.Enabled = true;
            this.axSkinFramework1.Location = new System.Drawing.Point(24, 0);
            this.axSkinFramework1.Name = "axSkinFramework1";
            this.axSkinFramework1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework1.OcxState")));
            this.axSkinFramework1.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework1.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 726);
            this.Controls.Add(this.axSkinFramework1);
            this.Controls.Add(this.panelProcess);
            this.Controls.Add(this.axDockingPane);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "발전소 지킴이";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private System.Windows.Forms.Panel panelProcess;
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework1;
    }
}

