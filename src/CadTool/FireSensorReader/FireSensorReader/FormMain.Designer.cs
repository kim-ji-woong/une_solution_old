namespace FireSensorReader
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
            this.dxfSensors = new DXFViewer.DXFControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsMenuParent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelCoord = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dxfSensors
            // 
            this.dxfSensors.AllowDrop = true;
            this.dxfSensors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dxfSensors.AntiAliasing = true;
            this.dxfSensors.BackColor = System.Drawing.Color.Black;
            this.dxfSensors.DrawHatchFirst = true;
            this.dxfSensors.ExternalPainter = null;
            this.dxfSensors.GroupItemDistance = 30;
            this.dxfSensors.GroupItemMinCount = 3;
            this.dxfSensors.Location = new System.Drawing.Point(12, 12);
            this.dxfSensors.MinimumSize = new System.Drawing.Size(100, 100);
            this.dxfSensors.MovedVertex = vertex2D1;
            this.dxfSensors.Name = "dxfSensors";
            this.dxfSensors.ObjectBR = null;
            this.dxfSensors.ObjectTL = null;
            this.dxfSensors.OpenNRefresh = true;
            this.dxfSensors.Panning = false;
            this.dxfSensors.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfSensors.PrintDocument = null;
            this.dxfSensors.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfSensors.Size = new System.Drawing.Size(976, 686);
            this.dxfSensors.TabIndex = 0;
            this.dxfSensors.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfSensors.UseGroupItem = false;
            this.dxfSensors.UseLastViewport = false;
            this.dxfSensors.UseMouseWheel = true;
            this.dxfSensors.DragDrop += new System.Windows.Forms.DragEventHandler(this.dxfSensors_DragDrop);
            this.dxfSensors.DragEnter += new System.Windows.Forms.DragEventHandler(this.dxfSensors_DragEnter);
            this.dxfSensors.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dxfSensors_MouseDown);
            this.dxfSensors.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfSensors_MouseMove);
            this.dxfSensors.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dxfSensors_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuParent});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1000, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsMenuParent
            // 
            this.tsMenuParent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuLayer,
            this.tsMenuExport});
            this.tsMenuParent.Name = "tsMenuParent";
            this.tsMenuParent.Size = new System.Drawing.Size(43, 20);
            this.tsMenuParent.Text = "메뉴";
            // 
            // tsMenuLayer
            // 
            this.tsMenuLayer.Name = "tsMenuLayer";
            this.tsMenuLayer.Size = new System.Drawing.Size(180, 22);
            this.tsMenuLayer.Text = "Layer";
            this.tsMenuLayer.Click += new System.EventHandler(this.tsMenuLayer_Click);
            // 
            // tsMenuExport
            // 
            this.tsMenuExport.Name = "tsMenuExport";
            this.tsMenuExport.Size = new System.Drawing.Size(180, 22);
            this.tsMenuExport.Text = "내보내기";
            this.tsMenuExport.Click += new System.EventHandler(this.tsMenuExport_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(15, 704);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(53, 12);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "상태정보";
            // 
            // labelCoord
            // 
            this.labelCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCoord.AutoSize = true;
            this.labelCoord.Location = new System.Drawing.Point(409, 704);
            this.labelCoord.Name = "labelCoord";
            this.labelCoord.Size = new System.Drawing.Size(29, 12);
            this.labelCoord.TabIndex = 3;
            this.labelCoord.Text = "좌표";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 721);
            this.Controls.Add(this.labelCoord);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dxfSensors);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "화재센서 탐색기";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DXFViewer.DXFControl dxfSensors;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuParent;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ToolStripMenuItem tsMenuLayer;
        private System.Windows.Forms.ToolStripMenuItem tsMenuExport;
        private System.Windows.Forms.Label labelCoord;
    }
}

