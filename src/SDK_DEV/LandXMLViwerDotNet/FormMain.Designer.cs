namespace UBMLViewer
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
			this.m_ColorDialog = new System.Windows.Forms.ColorDialog();
			this.m_FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.m_FontDialog = new System.Windows.Forms.FontDialog();
			this.m_OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.m_SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.m_MainTimer = new System.Windows.Forms.Timer(this.components);
			this.m_MainPanel = new System.Windows.Forms.Panel();
			this.m_axImageManager = new AxXtremeCommandBars.AxImageManager();
			this.m_axCommandBars = new AxXtremeCommandBars.AxCommandBars();
			this.m_axSkinFramework = new AxXtremeSkinFramework.AxSkinFramework();
			((System.ComponentModel.ISupportInitialize)(this.m_axImageManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_axCommandBars)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_axSkinFramework)).BeginInit();
			this.SuspendLayout();
			// 
			// m_OpenFileDialog
			// 
			this.m_OpenFileDialog.FileName = "openFileDialog1";
			// 
			// m_MainTimer
			// 
			this.m_MainTimer.Interval = 1000;
			this.m_MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
			// 
			// m_MainPanel
			// 
			this.m_MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_MainPanel.BackColor = System.Drawing.Color.White;
			this.m_MainPanel.Location = new System.Drawing.Point(0, 174);
			this.m_MainPanel.Name = "m_MainPanel";
			this.m_MainPanel.Size = new System.Drawing.Size(1264, 688);
			this.m_MainPanel.TabIndex = 4;
			// 
			// m_axImageManager
			// 
			this.m_axImageManager.Enabled = true;
			this.m_axImageManager.Location = new System.Drawing.Point(75, 13);
			this.m_axImageManager.Name = "m_axImageManager";
			this.m_axImageManager.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_axImageManager.OcxState")));
			this.m_axImageManager.Size = new System.Drawing.Size(24, 24);
			this.m_axImageManager.TabIndex = 3;
			// 
			// m_axCommandBars
			// 
			this.m_axCommandBars.Enabled = true;
			this.m_axCommandBars.Location = new System.Drawing.Point(44, 13);
			this.m_axCommandBars.Name = "m_axCommandBars";
			this.m_axCommandBars.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_axCommandBars.OcxState")));
			this.m_axCommandBars.Size = new System.Drawing.Size(24, 24);
			this.m_axCommandBars.TabIndex = 2;
			this.m_axCommandBars.Execute += new AxXtremeCommandBars._DCommandBarsEvents_ExecuteEventHandler(this.CommandBars_Execute);
			this.m_axCommandBars.UpdateEvent += new AxXtremeCommandBars._DCommandBarsEvents_UpdateEventHandler(this.CommandBars_UpdateEvent);
			this.m_axCommandBars.ResizeEvent += new System.EventHandler(this.CommandBars_ResizeEvent);
			// 
			// m_axSkinFramework
			// 
			this.m_axSkinFramework.Enabled = true;
			this.m_axSkinFramework.Location = new System.Drawing.Point(13, 13);
			this.m_axSkinFramework.Name = "m_axSkinFramework";
			this.m_axSkinFramework.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_axSkinFramework.OcxState")));
			this.m_axSkinFramework.Size = new System.Drawing.Size(24, 24);
			this.m_axSkinFramework.TabIndex = 1;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1264, 862);
			this.Controls.Add(this.m_MainPanel);
			this.Controls.Add(this.m_axImageManager);
			this.Controls.Add(this.m_axCommandBars);
			this.Controls.Add(this.m_axSkinFramework);
			this.DoubleBuffered = true;
			this.Name = "FormMain";
			this.Text = "LandXML Viewer";
			this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this.m_axImageManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_axCommandBars)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_axSkinFramework)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog m_ColorDialog;
        private System.Windows.Forms.FolderBrowserDialog m_FolderBrowserDialog;
        private System.Windows.Forms.FontDialog m_FontDialog;
        private System.Windows.Forms.OpenFileDialog m_OpenFileDialog;
        private System.Windows.Forms.SaveFileDialog m_SaveFileDialog;
        private System.Windows.Forms.Timer m_MainTimer;
        private AxXtremeSkinFramework.AxSkinFramework m_axSkinFramework;
        private AxXtremeCommandBars.AxCommandBars m_axCommandBars;
        private AxXtremeCommandBars.AxImageManager m_axImageManager;
        private System.Windows.Forms.Panel m_MainPanel;
    }
}

