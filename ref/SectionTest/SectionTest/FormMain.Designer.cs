namespace section
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
            this.mainContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axCommandBars1 = new AxXtremeCommandBars.AxCommandBars();
            this.m_MainPanel = new System.Windows.Forms.Panel();
            this.mainContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axCommandBars1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainContextMenuStrip
            // 
            this.mainContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.mainContextMenuStrip.Name = "contextMenuStrip1";
            this.mainContextMenuStrip.Size = new System.Drawing.Size(95, 26);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.addToolStripMenuItem.Text = "add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.Add_Click);
            // 
            // axCommandBars1
            // 
            this.axCommandBars1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axCommandBars1.Enabled = true;
            this.axCommandBars1.Location = new System.Drawing.Point(32, 46);
            this.axCommandBars1.Name = "axCommandBars1";
            this.axCommandBars1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axCommandBars1.OcxState")));
            this.axCommandBars1.Size = new System.Drawing.Size(24, 24);
            this.axCommandBars1.TabIndex = 12;
            this.axCommandBars1.Execute += new AxXtremeCommandBars._DCommandBarsEvents_ExecuteEventHandler(this.axCommandBars1_Execute);
            this.axCommandBars1.UpdateEvent += new AxXtremeCommandBars._DCommandBarsEvents_UpdateEventHandler(this.axCommandBars1_UpdateEvent);
            this.axCommandBars1.ResizeEvent += new System.EventHandler(this.axCommandBars1_ResizeEvent);
            // 
            // panel1
            // 
            this.m_MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_MainPanel.Location = new System.Drawing.Point(12, 160);
            this.m_MainPanel.Name = "panel1";
            this.m_MainPanel.Size = new System.Drawing.Size(977, 346);
            this.m_MainPanel.TabIndex = 13;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 518);
            this.Controls.Add(this.m_MainPanel);
            this.Controls.Add(this.axCommandBars1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.mainContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axCommandBars1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip mainContextMenuStrip;
        
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private AxXtremeCommandBars.AxCommandBars axCommandBars1;
        private System.Windows.Forms.Panel m_MainPanel;
    }
}

