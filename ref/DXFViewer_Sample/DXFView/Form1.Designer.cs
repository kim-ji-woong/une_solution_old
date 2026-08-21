namespace DXFView
{
    partial class Form1
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
            UnE.Geometry.Vertex2D vertex2D1 = new UnE.Geometry.Vertex2D();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dxfControl1 = new DXFViewer.DXFControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLeft = new DXFView.LeftPanel(this.components);
            this.tsMenuOnlyBlocks = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dxfControl1
            // 
            this.dxfControl1.AntiAliasing = true;
            this.dxfControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxfControl1.DrawHatchFirst = true;
            this.dxfControl1.ExternalPainter = null;
            this.dxfControl1.GroupItemDistance = 30;
            this.dxfControl1.GroupItemMinCount = 3;
            this.dxfControl1.Location = new System.Drawing.Point(0, 0);
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
            this.dxfControl1.Size = new System.Drawing.Size(672, 429);
            this.dxfControl1.TabIndex = 0;
            this.dxfControl1.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl1.UseGroupItem = false;
            this.dxfControl1.UseLastViewport = false;
            this.dxfControl1.UseMouseWheel = true;
            this.dxfControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseMove);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 407);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(672, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(121, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(672, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.tsMenuOnlyBlocks});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.FileToolStripMenuItem.Text = "파일(&F)";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.OpenToolStripMenuItem.Text = "열기(&O)";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.Blocks = ((System.Collections.ArrayList)(resources.GetObject("panelLeft.Blocks")));
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Layers = ((System.Collections.ArrayList)(resources.GetObject("panelLeft.Layers")));
            this.panelLeft.Location = new System.Drawing.Point(0, 24);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(255, 383);
            this.panelLeft.TabIndex = 3;
            // 
            // tsMenuOnlyBlocks
            // 
            this.tsMenuOnlyBlocks.Name = "tsMenuOnlyBlocks";
            this.tsMenuOnlyBlocks.Size = new System.Drawing.Size(152, 22);
            this.tsMenuOnlyBlocks.Text = "Block만 보기";
            this.tsMenuOnlyBlocks.Click += new System.EventHandler(this.tsMenuOnlyBlocks_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 429);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dxfControl1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DXFViewer.DXFControl dxfControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private LeftPanel panelLeft;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOnlyBlocks;

    }
}

