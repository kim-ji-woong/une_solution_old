namespace SOPBulletin
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.axSkinFramework = new AxXtremeSkinFramework.AxSkinFramework();
            this.axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.OnProcessedTimer);
            // 
            // axSkinFramework
            // 
            this.axSkinFramework.Enabled = true;
            this.axSkinFramework.Location = new System.Drawing.Point(42, 493);
            this.axSkinFramework.Name = "axSkinFramework";
            this.axSkinFramework.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework.OcxState")));
            this.axSkinFramework.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework.TabIndex = 6;
            // 
            // axDockingPane
            // 
            this.axDockingPane.Enabled = true;
            this.axDockingPane.Location = new System.Drawing.Point(12, 493);
            this.axDockingPane.Name = "axDockingPane";
            this.axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane.OcxState")));
            this.axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.axDockingPane.TabIndex = 4;
            this.axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.axDockingPane_AttachPaneEvent);
            this.axDockingPane.ResizeClient += new AxXtremeDockingPane._DDockingPaneEvents_ResizeClientEventHandler(this.axDockingPane_ResizeClient);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(942, 529);
            this.Controls.Add(this.axSkinFramework);
            this.Controls.Add(this.axDockingPane);
            this.Name = "FormMain";
            this.Text = "SOP Bulletin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}

