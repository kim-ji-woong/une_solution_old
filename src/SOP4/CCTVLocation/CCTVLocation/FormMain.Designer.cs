namespace CCTVLocation
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
            UnE.Geometry.Vertex2D vertex2D1 = new UnE.Geometry.Vertex2D();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dxfControl1 = new DXFViewer.DXFControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.파일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpenDXF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpenDataFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSaveDataFile = new System.Windows.Forms.ToolStripMenuItem();
            this.보기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuShowCCTVList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuShowCoordText = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuPolylineText = new System.Windows.Forms.ToolStripMenuItem();
            this.tsDeleteAllVertex = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 469);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(596, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(121, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // dxfControl1
            // 
            this.dxfControl1.AntiAliasing = true;
            this.dxfControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxfControl1.DrawHatchFirst = true;
            this.dxfControl1.ExternalPainter = null;
            this.dxfControl1.GroupItemDistance = 30;
            this.dxfControl1.GroupItemMinCount = 3;
            this.dxfControl1.Location = new System.Drawing.Point(0, 24);
            this.dxfControl1.MinimumSize = new System.Drawing.Size(100, 100);
            this.dxfControl1.MovedVertex = vertex2D1;
            this.dxfControl1.Name = "dxfControl1";
            this.dxfControl1.ObjectBR = null;
            this.dxfControl1.ObjectTL = null;
            this.dxfControl1.OpenNRefresh = true;
            this.dxfControl1.Panning = false;
            this.dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl1.PrintDocument = null;
            this.dxfControl1.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfControl1.Size = new System.Drawing.Size(596, 445);
            this.dxfControl1.TabIndex = 2;
            this.dxfControl1.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl1.UseGroupItem = false;
            this.dxfControl1.UseLastViewport = false;
            this.dxfControl1.UseMouseWheel = true;
            this.dxfControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseClick);
            this.dxfControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일ToolStripMenuItem,
            this.보기ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(596, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            this.파일ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuOpenDXF,
            this.tsMenuOpenDataFile,
            this.tsMenuSaveDataFile});
            this.파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            this.파일ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.파일ToolStripMenuItem.Text = "파일";
            // 
            // tsMenuOpenDXF
            // 
            this.tsMenuOpenDXF.Name = "tsMenuOpenDXF";
            this.tsMenuOpenDXF.Size = new System.Drawing.Size(127, 22);
            this.tsMenuOpenDXF.Text = "DXF 열기";
            this.tsMenuOpenDXF.Click += new System.EventHandler(this.tsMenuOpenDXF_Click);
            // 
            // tsMenuOpenDataFile
            // 
            this.tsMenuOpenDataFile.Name = "tsMenuOpenDataFile";
            this.tsMenuOpenDataFile.Size = new System.Drawing.Size(127, 22);
            this.tsMenuOpenDataFile.Text = "Data 열기";
            this.tsMenuOpenDataFile.Click += new System.EventHandler(this.tsMenuOpenDataFile_Click);
            // 
            // tsMenuSaveDataFile
            // 
            this.tsMenuSaveDataFile.Enabled = false;
            this.tsMenuSaveDataFile.Name = "tsMenuSaveDataFile";
            this.tsMenuSaveDataFile.Size = new System.Drawing.Size(127, 22);
            this.tsMenuSaveDataFile.Text = "저장";
            this.tsMenuSaveDataFile.Click += new System.EventHandler(this.tsMenuSaveDataFile_Click);
            // 
            // 보기ToolStripMenuItem
            // 
            this.보기ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuShowCCTVList,
            this.tsMenuShowCoordText,
            this.tsMenuPolylineText,
            this.tsDeleteAllVertex});
            this.보기ToolStripMenuItem.Name = "보기ToolStripMenuItem";
            this.보기ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.보기ToolStripMenuItem.Text = "보기";
            // 
            // tsMenuShowCCTVList
            // 
            this.tsMenuShowCCTVList.Enabled = false;
            this.tsMenuShowCCTVList.Name = "tsMenuShowCCTVList";
            this.tsMenuShowCCTVList.Size = new System.Drawing.Size(152, 22);
            this.tsMenuShowCCTVList.Text = "CCTV List";
            this.tsMenuShowCCTVList.Click += new System.EventHandler(this.tsMenuShowCCTVList_Click);
            // 
            // tsMenuShowCoordText
            // 
            this.tsMenuShowCoordText.Name = "tsMenuShowCoordText";
            this.tsMenuShowCoordText.Size = new System.Drawing.Size(152, 22);
            this.tsMenuShowCoordText.Text = "Coord Text";
            this.tsMenuShowCoordText.Click += new System.EventHandler(this.tsMenuShowCoordText_Click);
            // 
            // tsMenuPolylineText
            // 
            this.tsMenuPolylineText.Name = "tsMenuPolylineText";
            this.tsMenuPolylineText.Size = new System.Drawing.Size(152, 22);
            this.tsMenuPolylineText.Text = "Polyline Text";
            this.tsMenuPolylineText.Click += new System.EventHandler(this.tsMenuPolylineText_Click);
            // 
            // tsDeleteAllVertex
            // 
            this.tsDeleteAllVertex.Name = "tsDeleteAllVertex";
            this.tsDeleteAllVertex.Size = new System.Drawing.Size(152, 22);
            this.tsDeleteAllVertex.Text = "모두 지우기";
            this.tsDeleteAllVertex.Click += new System.EventHandler(this.tsDeleteAllVertex_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 491);
            this.Controls.Add(this.dxfControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "CCTV 위치 입력기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private DXFViewer.DXFControl dxfControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenDXF;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenDataFile;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSaveDataFile;
        private System.Windows.Forms.ToolStripMenuItem 보기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMenuShowCCTVList;
        private System.Windows.Forms.ToolStripMenuItem tsMenuShowCoordText;
        private System.Windows.Forms.ToolStripMenuItem tsMenuPolylineText;
        private System.Windows.Forms.ToolStripMenuItem tsDeleteAllVertex;
    }
}

