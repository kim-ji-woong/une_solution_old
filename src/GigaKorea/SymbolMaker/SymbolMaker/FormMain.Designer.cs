namespace SymbolMaker
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            UnE.Geometry.Vertex2D vertex2D1 = new UnE.Geometry.Vertex2D();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.파일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpenDXF = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpenDXF4Wire = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuExport1By1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panelDXF = new DXFViewer.DXFControl();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.labelCoord = new System.Windows.Forms.Label();
            this.tsMenuOpenDXFFromLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            this.파일ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuOpenDXFFromLayer,
            this.tsMenuOpenDXF,
            this.tsMenuOpenDXF4Wire,
            this.tsMenuExport,
            this.tsMenuExport1By1});
            this.파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            this.파일ToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.파일ToolStripMenuItem.Text = "파일(&F)";
            // 
            // tsMenuOpenDXF
            // 
            this.tsMenuOpenDXF.Name = "tsMenuOpenDXF";
            this.tsMenuOpenDXF.Size = new System.Drawing.Size(222, 22);
            this.tsMenuOpenDXF.Text = "DXF 열기(POI)";
            this.tsMenuOpenDXF.Click += new System.EventHandler(this.tsMenuOpenDXF_Click);
            // 
            // tsMenuOpenDXF4Wire
            // 
            this.tsMenuOpenDXF4Wire.Name = "tsMenuOpenDXF4Wire";
            this.tsMenuOpenDXF4Wire.Size = new System.Drawing.Size(222, 22);
            this.tsMenuOpenDXF4Wire.Text = "DXF 열기(배선)";
            this.tsMenuOpenDXF4Wire.Click += new System.EventHandler(this.tsMenuOpenDXF4Wire_Click);
            // 
            // tsMenuExport
            // 
            this.tsMenuExport.Enabled = false;
            this.tsMenuExport.Name = "tsMenuExport";
            this.tsMenuExport.Size = new System.Drawing.Size(222, 22);
            this.tsMenuExport.Text = "한 파일로 내보내기";
            this.tsMenuExport.Click += new System.EventHandler(this.tsMenuExport_Click);
            // 
            // tsMenuExport1By1
            // 
            this.tsMenuExport1By1.Enabled = false;
            this.tsMenuExport1By1.Name = "tsMenuExport1By1";
            this.tsMenuExport1By1.Size = new System.Drawing.Size(222, 22);
            this.tsMenuExport1By1.Text = "개별 파일로 내보내기";
            this.tsMenuExport1By1.Click += new System.EventHandler(this.tsMenuExport1By1_Click);
            // 
            // panelDXF
            // 
            this.panelDXF.AntiAliasing = true;
            this.panelDXF.BackColor = System.Drawing.Color.Black;
            this.panelDXF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDXF.DrawHatchFirst = true;
            this.panelDXF.ExternalPainter = null;
            this.panelDXF.GroupItemDistance = 30;
            this.panelDXF.GroupItemMinCount = 3;
            this.panelDXF.Location = new System.Drawing.Point(0, 24);
            this.panelDXF.MinimumSize = new System.Drawing.Size(100, 100);
            this.panelDXF.MovedVertex = vertex2D1;
            this.panelDXF.Name = "panelDXF";
            this.panelDXF.ObjectBR = null;
            this.panelDXF.ObjectTL = null;
            this.panelDXF.OpenNRefresh = true;
            this.panelDXF.Panning = false;
            this.panelDXF.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.panelDXF.PrintDocument = null;
            this.panelDXF.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.panelDXF.Size = new System.Drawing.Size(800, 426);
            this.panelDXF.TabIndex = 1;
            this.panelDXF.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.panelDXF.UseGroupItem = false;
            this.panelDXF.UseLastViewport = false;
            this.panelDXF.UseMouseWheel = true;
            this.panelDXF.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelDXF_MouseMove);
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.labelCoord);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 427);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(800, 23);
            this.panelStatus.TabIndex = 2;
            // 
            // labelCoord
            // 
            this.labelCoord.AutoSize = true;
            this.labelCoord.Location = new System.Drawing.Point(6, 5);
            this.labelCoord.Name = "labelCoord";
            this.labelCoord.Size = new System.Drawing.Size(53, 12);
            this.labelCoord.TabIndex = 0;
            this.labelCoord.Text = "좌표정보";
            // 
            // tsMenuOpenDXFFromLayer
            // 
            this.tsMenuOpenDXFFromLayer.Name = "tsMenuOpenDXFFromLayer";
            this.tsMenuOpenDXFFromLayer.Size = new System.Drawing.Size(222, 22);
            this.tsMenuOpenDXFFromLayer.Text = "DXF 열기(POI, 한글 레이어)";
            this.tsMenuOpenDXFFromLayer.Click += new System.EventHandler(this.tsMenuOpenDXFFromLayer_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panelDXF);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "POI 심볼 제작";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenDXF;
        private DXFViewer.DXFControl panelDXF;
        private System.Windows.Forms.ToolStripMenuItem tsMenuExport;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label labelCoord;
        private System.Windows.Forms.ToolStripMenuItem tsMenuExport1By1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenDXF4Wire;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenDXFFromLayer;
    }
}

