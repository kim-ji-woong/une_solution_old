namespace UBMLViewerDotNet
{
    partial class Form3
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
            this.btnImportView1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.textureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiddenLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.removePOIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFront = new System.Windows.Forms.Button();
            this.btnTop = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnReal = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnFit = new System.Windows.Forms.Button();
            this.btnImport2 = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnAddPOI = new System.Windows.Forms.Button();
            this.cmbPoiType = new System.Windows.Forms.ComboBox();
            //this.panel1 = new System.Windows.Forms.Label();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImportView1
            // 
            this.btnImportView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportView1.Location = new System.Drawing.Point(761, 582);
            this.btnImportView1.Name = "btnImportView1";
            this.btnImportView1.Size = new System.Drawing.Size(91, 26);
            this.btnImportView1.TabIndex = 1;
            this.btnImportView1.Text = "Import1";
            this.btnImportView1.UseVisualStyleBackColor = true;
            this.btnImportView1.Click += new System.EventHandler(this.btnImportView1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dae";
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // popupMenu
            // 
            this.popupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectToolStripMenuItem,
            this.clearSelectToolStripMenuItem,
            this.toolStripSeparator1,
            this.textureToolStripMenuItem,
            this.hiddenLineToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItem1,
            this.removePOIToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolStripMenuItem2,
            this.toolStripSeparator4,
            this.toolStripMenuItem3});
            this.popupMenu.Name = "popupMenu";
            this.popupMenu.Size = new System.Drawing.Size(153, 226);
            this.popupMenu.Opening += new System.ComponentModel.CancelEventHandler(this.popupMenu_Opening);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.selectToolStripMenuItem.Text = "Select";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // clearSelectToolStripMenuItem
            // 
            this.clearSelectToolStripMenuItem.Name = "clearSelectToolStripMenuItem";
            this.clearSelectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearSelectToolStripMenuItem.Text = "Clear Select";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // textureToolStripMenuItem
            // 
            this.textureToolStripMenuItem.Checked = true;
            this.textureToolStripMenuItem.CheckOnClick = true;
            this.textureToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            this.textureToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textureToolStripMenuItem.Text = "Texture";
            // 
            // hiddenLineToolStripMenuItem
            // 
            this.hiddenLineToolStripMenuItem.CheckOnClick = true;
            this.hiddenLineToolStripMenuItem.Name = "hiddenLineToolStripMenuItem";
            this.hiddenLineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hiddenLineToolStripMenuItem.Text = "HiddenLine";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem1.Text = "Add POI";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // removePOIToolStripMenuItem
            // 
            this.removePOIToolStripMenuItem.Name = "removePOIToolStripMenuItem";
            this.removePOIToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removePOIToolStripMenuItem.Text = "Remove POI";
            this.removePOIToolStripMenuItem.Click += new System.EventHandler(this.removePOIToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "Add TextPOI";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // btnFront
            // 
            this.btnFront.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFront.Location = new System.Drawing.Point(12, 583);
            this.btnFront.Name = "btnFront";
            this.btnFront.Size = new System.Drawing.Size(54, 25);
            this.btnFront.TabIndex = 3;
            this.btnFront.Text = "Front";
            this.btnFront.UseVisualStyleBackColor = true;
            this.btnFront.Click += new System.EventHandler(this.btnFront_Click);
            // 
            // btnTop
            // 
            this.btnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTop.Location = new System.Drawing.Point(72, 584);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(54, 25);
            this.btnTop.TabIndex = 4;
            this.btnTop.Text = "Top";
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLeft.Location = new System.Drawing.Point(132, 584);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(54, 25);
            this.btnLeft.TabIndex = 5;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRight.Location = new System.Drawing.Point(192, 584);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(54, 25);
            this.btnRight.TabIndex = 6;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnReal
            // 
            this.btnReal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReal.Location = new System.Drawing.Point(252, 583);
            this.btnReal.Name = "btnReal";
            this.btnReal.Size = new System.Drawing.Size(54, 25);
            this.btnReal.TabIndex = 7;
            this.btnReal.Text = "Rear";
            this.btnReal.UseVisualStyleBackColor = true;
            this.btnReal.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnZoomIn.Location = new System.Drawing.Point(312, 584);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(54, 25);
            this.btnZoomIn.TabIndex = 8;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnZoomOut.Location = new System.Drawing.Point(372, 584);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(54, 25);
            this.btnZoomOut.TabIndex = 9;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            // 
            // btnFit
            // 
            this.btnFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFit.Location = new System.Drawing.Point(432, 584);
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(54, 25);
            this.btnFit.TabIndex = 10;
            this.btnFit.Text = "Fit";
            this.btnFit.UseVisualStyleBackColor = true;
            this.btnFit.Click += new System.EventHandler(this.btnFit_Click);
            // 
            // btnImport2
            // 
            this.btnImport2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport2.Location = new System.Drawing.Point(858, 582);
            this.btnImport2.Name = "btnImport2";
            this.btnImport2.Size = new System.Drawing.Size(91, 26);
            this.btnImport2.TabIndex = 12;
            this.btnImport2.Text = "Import2";
            this.btnImport2.UseVisualStyleBackColor = true;
            this.btnImport2.Click += new System.EventHandler(this.btnImport2_Click);
            // 
            // btnHome
            // 
            this.btnHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHome.Location = new System.Drawing.Point(492, 584);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(54, 25);
            this.btnHome.TabIndex = 14;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click_1);
            // 
            // btnAddPOI
            // 
            this.btnAddPOI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddPOI.Location = new System.Drawing.Point(589, 584);
            this.btnAddPOI.Name = "btnAddPOI";
            this.btnAddPOI.Size = new System.Drawing.Size(54, 25);
            this.btnAddPOI.TabIndex = 15;
            this.btnAddPOI.Text = "POI";
            this.btnAddPOI.UseVisualStyleBackColor = true;
            this.btnAddPOI.Click += new System.EventHandler(this.btnAddPOI_Click);
            // 
            // cmbPoiType
            // 
            this.cmbPoiType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbPoiType.FormattingEnabled = true;
            this.cmbPoiType.Items.AddRange(new object[] {
            "비산먼지",
            "오염",
            "제방붕괴",
            "지진",
            "침수",
            "태풍",
            "테러",
            "폭발",
            "폭설",
            "화재"});
            this.cmbPoiType.Location = new System.Drawing.Point(649, 587);
            this.cmbPoiType.Name = "cmbPoiType";
            this.cmbPoiType.Size = new System.Drawing.Size(105, 20);
            this.cmbPoiType.TabIndex = 16;
            this.cmbPoiType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem3.Text = "Add Fire";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 616);            
            this.Controls.Add(this.cmbPoiType);
            this.Controls.Add(this.btnAddPOI);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnImport2);
            this.Controls.Add(this.btnFit);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnReal);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnTop);
            this.Controls.Add(this.btnFront);
            this.Controls.Add(this.btnImportView1);
            this.Name = "Form3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.popupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImportView1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ContextMenuStrip popupMenu;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem textureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hiddenLineToolStripMenuItem;
        private System.Windows.Forms.Button btnFront;
        private System.Windows.Forms.Button btnTop;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnReal;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnFit;
        private System.Windows.Forms.Button btnImport2;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem removePOIToolStripMenuItem;
        private System.Windows.Forms.Button btnAddPOI;
        private System.Windows.Forms.ComboBox cmbPoiType;
        private System.Windows.Forms.Label panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
    }
}

