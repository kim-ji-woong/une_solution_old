namespace CCTVSingleViewer
{
    partial class CCTVPanel
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbTitle = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExpand = new UnE.GUI.ImageButton();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExpand)).BeginInit();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.AllowDrop = true;
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.White;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.Black;
            this.lbTitle.Location = new System.Drawing.Point(12, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(143, 25);
            this.lbTitle.TabIndex = 9;
            this.lbTitle.Text = "CCTV정보 없음";
            this.lbTitle.DragDrop += new System.Windows.Forms.DragEventHandler(this.CCTVPanel_DragDrop);
            this.lbTitle.DragEnter += new System.Windows.Forms.DragEventHandler(this.CCTVPanel_DragEnter);
            this.lbTitle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseClick);
            this.lbTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuDisconnect});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
            // 
            // tsMenuDisconnect
            // 
            this.tsMenuDisconnect.Name = "tsMenuDisconnect";
            this.tsMenuDisconnect.Size = new System.Drawing.Size(122, 22);
            this.tsMenuDisconnect.Text = "연결해제";
            this.tsMenuDisconnect.Click += new System.EventHandler(this.tsMenuDisconnect_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpand.ButtonText = "";
            this.btnExpand.ImageClicked = global::CCTVSingleViewer.Properties.Resources.ext_click;
            this.btnExpand.ImageDisabled = null;
            this.btnExpand.ImageMouseOver = global::CCTVSingleViewer.Properties.Resources.ext_hover;
            this.btnExpand.ImageNormal = global::CCTVSingleViewer.Properties.Resources.ext_normal;
            this.btnExpand.Location = new System.Drawing.Point(247, 9);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Owner = null;
            this.btnExpand.Size = new System.Drawing.Size(37, 31);
            this.btnExpand.TabIndex = 10;
            this.btnExpand.TabStop = false;
            this.btnExpand.TextColor = System.Drawing.Color.Black;
            this.btnExpand.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnExpand.ToolTipText = "";
            this.btnExpand.UseToolTip = false;
            this.btnExpand.WindowRateWidth = 1F;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // CCTVPanel
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.lbTitle);
            this.Name = "CCTVPanel";
            this.Size = new System.Drawing.Size(295, 265);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CCTVPanel_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CCTVPanel_DragEnter);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CCTVPanel_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CCTVPanel_MouseDoubleClick);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnExpand)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDisconnect;
        private UnE.GUI.ImageButton btnExpand;
    }
}
