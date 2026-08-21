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
            m_netMgr.ReleaseThread();

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
            this.axSkinFramework = new AxXtremeSkinFramework.AxSkinFramework();
            this.axCommandBars = new AxXtremeCommandBars.AxCommandBars();
            this.axImageManager = new AxXtremeCommandBars.AxImageManager();
            this.panelMain = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axCommandBars)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axImageManager)).BeginInit();
            this.SuspendLayout();
            // 
            // axSkinFramework
            // 
            this.axSkinFramework.Enabled = true;
            this.axSkinFramework.Location = new System.Drawing.Point(12, 12);
            this.axSkinFramework.Name = "axSkinFramework";
            this.axSkinFramework.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework.OcxState")));
            this.axSkinFramework.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework.TabIndex = 0;
            // 
            // axCommandBars
            // 
            this.axCommandBars.Enabled = true;
            this.axCommandBars.Location = new System.Drawing.Point(44, 12);
            this.axCommandBars.Name = "axCommandBars";
            this.axCommandBars.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axCommandBars.OcxState")));
            this.axCommandBars.Size = new System.Drawing.Size(24, 24);
            this.axCommandBars.TabIndex = 1;
            this.axCommandBars.Execute += new AxXtremeCommandBars._DCommandBarsEvents_ExecuteEventHandler(this.axCommandBars_Execute);
            this.axCommandBars.UpdateEvent += new AxXtremeCommandBars._DCommandBarsEvents_UpdateEventHandler(this.axCommandBars_UpdateEvent);
            this.axCommandBars.ResizeEvent += new System.EventHandler(this.axCommandBars_ResizeEvent);
            // 
            // axImageManager
            // 
            this.axImageManager.Enabled = true;
            this.axImageManager.Location = new System.Drawing.Point(76, 12);
            this.axImageManager.Name = "axImageManager";
            this.axImageManager.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axImageManager.OcxState")));
            this.axImageManager.Size = new System.Drawing.Size(24, 24);
            this.axImageManager.TabIndex = 2;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelMain.Location = new System.Drawing.Point(19, 110);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1103, 608);
            this.panelMain.TabIndex = 3;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 730);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.axImageManager);
            this.Controls.Add(this.axCommandBars);
            this.Controls.Add(this.axSkinFramework);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "SOP Monitoring System";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axCommandBars)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axImageManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework;
        private AxXtremeCommandBars.AxCommandBars axCommandBars;
        private AxXtremeCommandBars.AxImageManager axImageManager;
        private System.Windows.Forms.Panel panelMain;
    }
}

