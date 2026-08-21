namespace Sections
{
    partial class PanelSectionEx
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.popupContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuExcec = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuSkip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuDecisionExec = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStripRClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuCloseSOP = new System.Windows.Forms.ToolStripMenuItem();
            this.popupContextMenuStrip.SuspendLayout();
            this.contextMenuStripRClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // popupContextMenuStrip
            // 
            this.popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuDelete,
            this.toolStripSeparator1,
            this.toolStripMenuExcec,
            this.toolStripMenuComplete,
            this.toolStripMenuSkip,
            this.toolStripMenuCancel,
            this.toolStripMenuRestart,
            this.toolStripMenuDecisionExec});
            this.popupContextMenuStrip.Name = "popupContextMenuStrip";
            this.popupContextMenuStrip.Size = new System.Drawing.Size(123, 164);
            // 
            // toolStripMenuDelete
            // 
            this.toolStripMenuDelete.Name = "toolStripMenuDelete";
            this.toolStripMenuDelete.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuDelete.Text = "삭제";
            this.toolStripMenuDelete.ToolTipText = "삭제";
            this.toolStripMenuDelete.Click += new System.EventHandler(this.toolStripMenuDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // toolStripMenuExcec
            // 
            this.toolStripMenuExcec.Name = "toolStripMenuExcec";
            this.toolStripMenuExcec.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuExcec.Text = "실행";
            this.toolStripMenuExcec.Click += new System.EventHandler(this.toolStripMenuExce_Click);
            // 
            // toolStripMenuComplete
            // 
            this.toolStripMenuComplete.Name = "toolStripMenuComplete";
            this.toolStripMenuComplete.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuComplete.Text = "완료";
            this.toolStripMenuComplete.Click += new System.EventHandler(this.toolStripMenuComplete_Click);
            // 
            // toolStripMenuSkip
            // 
            this.toolStripMenuSkip.Name = "toolStripMenuSkip";
            this.toolStripMenuSkip.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuSkip.Text = "건너뛰기";
            this.toolStripMenuSkip.Click += new System.EventHandler(this.toolStripMenuSkip_Click);
            // 
            // toolStripMenuCancel
            // 
            this.toolStripMenuCancel.Name = "toolStripMenuCancel";
            this.toolStripMenuCancel.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuCancel.Text = "실행취소";
            this.toolStripMenuCancel.Click += new System.EventHandler(this.toolStripMenuCancel_Click);
            // 
            // toolStripMenuRestart
            // 
            this.toolStripMenuRestart.Name = "toolStripMenuRestart";
            this.toolStripMenuRestart.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuRestart.Text = "재실행";
            this.toolStripMenuRestart.Click += new System.EventHandler(this.toolStripMenuRestart_Click);
            // 
            // toolStripMenuDecisionExec
            // 
            this.toolStripMenuDecisionExec.Name = "toolStripMenuDecisionExec";
            this.toolStripMenuDecisionExec.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuDecisionExec.Text = "실행";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStripRClick
            // 
            this.contextMenuStripRClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuCloseSOP});
            this.contextMenuStripRClick.Name = "contextMenuStripRClick";
            this.contextMenuStripRClick.Size = new System.Drawing.Size(126, 26);
            // 
            // toolStripMenuCloseSOP
            // 
            this.toolStripMenuCloseSOP.Name = "toolStripMenuCloseSOP";
            this.toolStripMenuCloseSOP.Size = new System.Drawing.Size(125, 22);
            this.toolStripMenuCloseSOP.Text = "SOP 닫기";
            this.toolStripMenuCloseSOP.Click += new System.EventHandler(this.toolStripMenuCloseSOP_Click);
            // 
            // PanelSectionEx
            // 
            this.BackColorChanged += new System.EventHandler(this.PanelSectionEx_BackColorChanged);
            this.SizeChanged += new System.EventHandler(this.PanelSectionEx_SizeChanged);
            this.popupContextMenuStrip.ResumeLayout(false);
            this.contextMenuStripRClick.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip popupContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDelete;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuExcec;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuComplete;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuSkip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCancel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuRestart;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDecisionExec;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRClick;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCloseSOP;

    }
}
