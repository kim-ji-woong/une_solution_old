namespace ShapeFileReader
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxTransparentFill = new System.Windows.Forms.CheckBox();
            this.checkBoxTransparentLine = new System.Windows.Forms.CheckBox();
            this.textBoxLineThick = new System.Windows.Forms.TextBox();
            this.btnFillColor = new System.Windows.Forms.Button();
            this.btnLineColor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPointSize = new System.Windows.Forms.TextBox();
            this.cboPointShape = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.dxfControl1 = new DXFViewer.DXFControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelLeft.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.White;
            this.panelLeft.Controls.Add(this.groupBox2);
            this.panelLeft.Controls.Add(this.groupBox1);
            this.panelLeft.Location = new System.Drawing.Point(0, 24);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(200, 373);
            this.panelLeft.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxTransparentFill);
            this.groupBox2.Controls.Add(this.checkBoxTransparentLine);
            this.groupBox2.Controls.Add(this.textBoxLineThick);
            this.groupBox2.Controls.Add(this.btnFillColor);
            this.groupBox2.Controls.Add(this.btnLineColor);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(171, 115);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "기타";
            // 
            // checkBoxTransparentFill
            // 
            this.checkBoxTransparentFill.AutoSize = true;
            this.checkBoxTransparentFill.Location = new System.Drawing.Point(65, 47);
            this.checkBoxTransparentFill.Name = "checkBoxTransparentFill";
            this.checkBoxTransparentFill.Size = new System.Drawing.Size(48, 16);
            this.checkBoxTransparentFill.TabIndex = 3;
            this.checkBoxTransparentFill.Text = "투명";
            this.checkBoxTransparentFill.UseVisualStyleBackColor = true;
            this.checkBoxTransparentFill.CheckedChanged += new System.EventHandler(this.checkBoxTransparentFill_CheckedChanged);
            // 
            // checkBoxTransparentLine
            // 
            this.checkBoxTransparentLine.AutoSize = true;
            this.checkBoxTransparentLine.Location = new System.Drawing.Point(65, 21);
            this.checkBoxTransparentLine.Name = "checkBoxTransparentLine";
            this.checkBoxTransparentLine.Size = new System.Drawing.Size(48, 16);
            this.checkBoxTransparentLine.TabIndex = 3;
            this.checkBoxTransparentLine.Text = "투명";
            this.checkBoxTransparentLine.UseVisualStyleBackColor = true;
            this.checkBoxTransparentLine.CheckedChanged += new System.EventHandler(this.checkBoxTransparentLine_CheckedChanged);
            // 
            // textBoxLineThick
            // 
            this.textBoxLineThick.Location = new System.Drawing.Point(65, 75);
            this.textBoxLineThick.Name = "textBoxLineThick";
            this.textBoxLineThick.Size = new System.Drawing.Size(88, 21);
            this.textBoxLineThick.TabIndex = 2;
            this.textBoxLineThick.Text = "1";
            this.textBoxLineThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnFillColor
            // 
            this.btnFillColor.BackColor = System.Drawing.Color.Red;
            this.btnFillColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFillColor.Location = new System.Drawing.Point(127, 43);
            this.btnFillColor.Name = "btnFillColor";
            this.btnFillColor.Size = new System.Drawing.Size(26, 23);
            this.btnFillColor.TabIndex = 1;
            this.btnFillColor.UseVisualStyleBackColor = false;
            this.btnFillColor.Click += new System.EventHandler(this.btnFillColor_Click);
            // 
            // btnLineColor
            // 
            this.btnLineColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLineColor.Location = new System.Drawing.Point(127, 18);
            this.btnLineColor.Name = "btnLineColor";
            this.btnLineColor.Size = new System.Drawing.Size(26, 23);
            this.btnLineColor.TabIndex = 1;
            this.btnLineColor.UseVisualStyleBackColor = true;
            this.btnLineColor.Click += new System.EventHandler(this.btnLineColor_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "선두께 : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "채움색 : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "선색상 : ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxPointSize);
            this.groupBox1.Controls.Add(this.cboPointShape);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 82);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Point";
            // 
            // textBoxPointSize
            // 
            this.textBoxPointSize.Location = new System.Drawing.Point(53, 44);
            this.textBoxPointSize.Name = "textBoxPointSize";
            this.textBoxPointSize.Size = new System.Drawing.Size(100, 21);
            this.textBoxPointSize.TabIndex = 2;
            this.textBoxPointSize.Text = "5";
            this.textBoxPointSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cboPointShape
            // 
            this.cboPointShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPointShape.FormattingEnabled = true;
            this.cboPointShape.Items.AddRange(new object[] {
            "사각형",
            "원"});
            this.cboPointShape.Location = new System.Drawing.Point(53, 20);
            this.cboPointShape.Name = "cboPointShape";
            this.cboPointShape.Size = new System.Drawing.Size(100, 20);
            this.cboPointShape.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "크기 : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "모양 : ";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Black;
            this.panelMain.Controls.Add(this.dxfControl1);
            this.panelMain.Location = new System.Drawing.Point(200, 24);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(444, 373);
            this.panelMain.TabIndex = 2;
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
            this.dxfControl1.Name = "dxfControl1";
            this.dxfControl1.ObjectBR = null;
            this.dxfControl1.ObjectTL = null;
            this.dxfControl1.OpenNRefresh = true;
            this.dxfControl1.Panning = false;
            this.dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl1.PrintDocument = null;
            this.dxfControl1.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfControl1.Size = new System.Drawing.Size(444, 373);
            this.dxfControl1.TabIndex = 0;
            this.dxfControl1.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl1.UseGroupItem = false;
            this.dxfControl1.UseLastViewport = false;
            this.dxfControl1.UseMouseWheel = true;
            this.dxfControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseDown);
            this.dxfControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFile});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(644, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItemFile
            // 
            this.toolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpen});
            this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
            this.toolStripMenuItemFile.Size = new System.Drawing.Size(57, 20);
            this.toolStripMenuItemFile.Text = "파일(&F)";
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.Size = new System.Drawing.Size(115, 22);
            this.menuOpen.Text = "열기(&O)";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 397);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(644, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatusLabel1.Text = "좌표값";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 419);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Shape File Viewer";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelLeft.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.Panel panelMain;
        private DXFViewer.DXFControl dxfControl1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnFillColor;
        private System.Windows.Forms.Button btnLineColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPointSize;
        private System.Windows.Forms.ComboBox cboPointShape;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLineThick;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.CheckBox checkBoxTransparentFill;
        private System.Windows.Forms.CheckBox checkBoxTransparentLine;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

