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
			this.arrowPopupContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuDelete2 = new System.Windows.Forms.ToolStripMenuItem();
			this.panelContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.linkContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuDelete3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.groupContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.popupContextMenuStrip.SuspendLayout();
			this.arrowPopupContextMenuStrip.SuspendLayout();
			this.panelContextMenuStrip.SuspendLayout();
			this.linkContextMenuStrip.SuspendLayout();
			this.groupContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// popupContextMenuStrip
			// 
			this.popupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuDelete});
			this.popupContextMenuStrip.Name = "popupContextMenuStrip";
			this.popupContextMenuStrip.Size = new System.Drawing.Size(99, 26);
			// 
			// toolStripMenuDelete
			// 
			this.toolStripMenuDelete.Name = "toolStripMenuDelete";
			this.toolStripMenuDelete.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuDelete.Text = "삭제";
			this.toolStripMenuDelete.ToolTipText = "삭제";
			this.toolStripMenuDelete.Click += new System.EventHandler(this.toolStripMenuDelete_Click);
			// 
			// arrowPopupContextMenuStrip
			// 
			this.arrowPopupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuDelete2});
			this.arrowPopupContextMenuStrip.Name = "arrowPopupContextMenuStrip";
			this.arrowPopupContextMenuStrip.Size = new System.Drawing.Size(99, 26);
			// 
			// toolStripMenuDelete2
			// 
			this.toolStripMenuDelete2.Name = "toolStripMenuDelete2";
			this.toolStripMenuDelete2.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuDelete2.Text = "삭제";
			this.toolStripMenuDelete2.ToolTipText = "삭제";
			this.toolStripMenuDelete2.Click += new System.EventHandler(this.toolStripMenuDelete_Click);
			// 
			// panelContextMenuStrip
			// 
			this.panelContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toBackToolStripMenuItem,
            this.toFrontToolStripMenuItem,
            this.insertBackToolStripMenuItem,
            this.insertFrontToolStripMenuItem,
            this.deleteToolStripMenuItem});
			this.panelContextMenuStrip.Name = "panelContextMenuStrip";
			this.panelContextMenuStrip.Size = new System.Drawing.Size(191, 114);
			// 
			// toBackToolStripMenuItem
			// 
			this.toBackToolStripMenuItem.Name = "toBackToolStripMenuItem";
			this.toBackToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.toBackToolStripMenuItem.Text = "패널을 뒤로 보내기";
			
			// 
			// toFrontToolStripMenuItem
			// 
			this.toFrontToolStripMenuItem.Name = "toFrontToolStripMenuItem";
			this.toFrontToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.toFrontToolStripMenuItem.Text = "패널을 앞으로 보내기";
			
			// 
			// insertBackToolStripMenuItem
			// 
			this.insertBackToolStripMenuItem.Name = "insertBackToolStripMenuItem";
			this.insertBackToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.insertBackToolStripMenuItem.Text = "왼쪽에 패널 추가";
			
			// 
			// insertFrontToolStripMenuItem
			// 
			this.insertFrontToolStripMenuItem.Name = "insertFrontToolStripMenuItem";
			this.insertFrontToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.insertFrontToolStripMenuItem.Text = "오른쪽에 패널 추가";
			
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.deleteToolStripMenuItem.Text = "패널 삭제";
			
			// 
			// linkContextMenuStrip
			// 
			this.linkContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuDelete3,
            this.toolStripMenuSelection});
			this.linkContextMenuStrip.Name = "linkContextMenuStrip";
			this.linkContextMenuStrip.Size = new System.Drawing.Size(195, 48);
			// 
			// toolStripMenuDelete3
			// 
			this.toolStripMenuDelete3.Name = "toolStripMenuDelete3";
			this.toolStripMenuDelete3.Size = new System.Drawing.Size(194, 22);
			this.toolStripMenuDelete3.Text = "삭제";
			this.toolStripMenuDelete3.Click += new System.EventHandler(this.toolStripMenuDelete_Click);
			// 
			// toolStripMenuSelection
			// 
			this.toolStripMenuSelection.Name = "toolStripMenuSelection";
			this.toolStripMenuSelection.Size = new System.Drawing.Size(194, 22);
			this.toolStripMenuSelection.Text = "링크될 대상 객체 선택";
			this.toolStripMenuSelection.Click += new System.EventHandler(this.toolStripMenuSelection_Click);
			// 
			// groupContextMenuStrip
			// 
			this.groupContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
			this.groupContextMenuStrip.Name = "popupContextMenuStrip";
			this.groupContextMenuStrip.Size = new System.Drawing.Size(123, 76);
			this.groupContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.groupContextMenuStrip_ItemClicked);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(122, 22);
			this.toolStripMenuItem1.Text = "삭제";
			this.toolStripMenuItem1.ToolTipText = "삭제";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(122, 22);
			this.toolStripMenuItem2.Text = "그룹화";
			this.toolStripMenuItem2.ToolTipText = "그룹화";
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(122, 22);
			this.toolStripMenuItem3.Text = "그룹해제";
			this.toolStripMenuItem3.ToolTipText = "그룹해제";
			// 
			// PanelSectionEx
			// 
			this.BackColorChanged += new System.EventHandler(this.PanelSectionEx_BackColorChanged);
			this.SizeChanged += new System.EventHandler(this.PanelSectionEx_SizeChanged);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.PanelSectionEx_PreviewKeyDown);
			this.popupContextMenuStrip.ResumeLayout(false);
			this.arrowPopupContextMenuStrip.ResumeLayout(false);
			this.panelContextMenuStrip.ResumeLayout(false);
			this.linkContextMenuStrip.ResumeLayout(false);
			this.groupContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip popupContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDelete;
        private System.Windows.Forms.ContextMenuStrip arrowPopupContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDelete2;
        private System.Windows.Forms.ContextMenuStrip panelContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toFrontToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip linkContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDelete3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuSelection;
        private System.Windows.Forms.ToolStripMenuItem insertBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertFrontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip groupContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;

    }
}
