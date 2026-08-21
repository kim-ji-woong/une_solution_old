namespace FireManagement
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
            if (!IsPCMode)
            {
                m_rfidReader.Owner = null;
                m_rfidReader.FinishReading(true);
            }

            if (disposing && (components != null))
                components.Dispose();

            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();

            if (process != null)
                process.Kill();
            else
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.statusScreenCoordStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.axImageManager = new AxXtremeCommandBars.AxImageManager();
            this.axCommandBars = new AxXtremeCommandBars.AxCommandBars();
            this.axSkinFramework = new AxXtremeSkinFramework.AxSkinFramework();
            this.statusScreenCoordStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axImageManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axCommandBars)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelMain.Location = new System.Drawing.Point(63, 79);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(933, 512);
            this.panelMain.TabIndex = 0;
            // 
            // statusScreenCoordStrip
            // 
            this.statusScreenCoordStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusScreenCoordStrip.Location = new System.Drawing.Point(0, 594);
            this.statusScreenCoordStrip.Name = "statusScreenCoordStrip";
            this.statusScreenCoordStrip.Size = new System.Drawing.Size(1028, 22);
            this.statusScreenCoordStrip.TabIndex = 4;
            this.statusScreenCoordStrip.Text = "\"\"";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(121, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // axImageManager
            // 
            this.axImageManager.Enabled = true;
            this.axImageManager.Location = new System.Drawing.Point(97, 27);
            this.axImageManager.Name = "axImageManager";
            this.axImageManager.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axImageManager.OcxState")));
            this.axImageManager.Size = new System.Drawing.Size(24, 24);
            this.axImageManager.TabIndex = 3;
            // 
            // axCommandBars
            // 
            this.axCommandBars.Enabled = true;
            this.axCommandBars.Location = new System.Drawing.Point(63, 27);
            this.axCommandBars.Name = "axCommandBars";
            this.axCommandBars.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axCommandBars.OcxState")));
            this.axCommandBars.Size = new System.Drawing.Size(24, 24);
            this.axCommandBars.TabIndex = 2;
            this.axCommandBars.Execute += new AxXtremeCommandBars._DCommandBarsEvents_ExecuteEventHandler(this.axCommandBars_Execute);
            this.axCommandBars.UpdateEvent += new AxXtremeCommandBars._DCommandBarsEvents_UpdateEventHandler(this.axCommandBars_UpdateEvent);
            this.axCommandBars.ResizeEvent += new System.EventHandler(this.axCommandBars_ResizeEvent);
            // 
            // axSkinFramework
            // 
            this.axSkinFramework.Enabled = true;
            this.axSkinFramework.Location = new System.Drawing.Point(29, 27);
            this.axSkinFramework.Name = "axSkinFramework";
            this.axSkinFramework.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework.OcxState")));
            this.axSkinFramework.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 616);
            this.Controls.Add(this.statusScreenCoordStrip);
            this.Controls.Add(this.axImageManager);
            this.Controls.Add(this.axCommandBars);
            this.Controls.Add(this.axSkinFramework);
            this.Controls.Add(this.panelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "소방설비 관리 시스템";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusScreenCoordStrip.ResumeLayout(false);
            this.statusScreenCoordStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axImageManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axCommandBars)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework;
        private AxXtremeCommandBars.AxCommandBars axCommandBars;
        private AxXtremeCommandBars.AxImageManager axImageManager;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.StatusStrip statusScreenCoordStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

