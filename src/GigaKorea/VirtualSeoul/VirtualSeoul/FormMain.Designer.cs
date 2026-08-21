namespace VirtualSeoul
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            UnE.Geometry.Vertex2D vertex2D2 = new UnE.Geometry.Vertex2D();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnAddUpLevel = new System.Windows.Forms.Button();
            this.btnSortLevel = new System.Windows.Forms.Button();
            this.btnDeleteLevel = new System.Windows.Forms.Button();
            this.btnAddDownLevel = new System.Windows.Forms.Button();
            this.radioMovePOI = new System.Windows.Forms.RadioButton();
            this.radioDeletePOI = new System.Windows.Forms.RadioButton();
            this.radioAddPOI = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLevelElevation = new System.Windows.Forms.TextBox();
            this.labelPOICode = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboLevels = new System.Windows.Forms.ComboBox();
            this.cboPOIs = new System.Windows.Forms.ComboBox();
            this.checkBoxEditPOI = new System.Windows.Forms.CheckBox();
            this.gridLayer = new System.Windows.Forms.DataGridView();
            this.colShow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewButtonColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpenDXF = new System.Windows.Forms.ToolStripMenuItem();
            this.웹서비스ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuLoadFromServer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSaveToServer = new System.Windows.Forms.ToolStripMenuItem();
            this.dxfControl = new DXFViewer.DXFControl();
            this.tsMenuExportPML = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuImportPML = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLayer)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1700);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1600, 42);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(276, 37);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.btnAddUpLevel);
            this.panelLeft.Controls.Add(this.btnSortLevel);
            this.panelLeft.Controls.Add(this.btnDeleteLevel);
            this.panelLeft.Controls.Add(this.btnAddDownLevel);
            this.panelLeft.Controls.Add(this.radioMovePOI);
            this.panelLeft.Controls.Add(this.radioDeletePOI);
            this.panelLeft.Controls.Add(this.radioAddPOI);
            this.panelLeft.Controls.Add(this.label2);
            this.panelLeft.Controls.Add(this.textBoxLevelElevation);
            this.panelLeft.Controls.Add(this.labelPOICode);
            this.panelLeft.Controls.Add(this.label3);
            this.panelLeft.Controls.Add(this.label1);
            this.panelLeft.Controls.Add(this.cboLevels);
            this.panelLeft.Controls.Add(this.cboPOIs);
            this.panelLeft.Controls.Add(this.checkBoxEditPOI);
            this.panelLeft.Controls.Add(this.gridLayer);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 53);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(400, 1647);
            this.panelLeft.TabIndex = 3;
            // 
            // btnAddUpLevel
            // 
            this.btnAddUpLevel.Enabled = false;
            this.btnAddUpLevel.Location = new System.Drawing.Point(224, 972);
            this.btnAddUpLevel.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnAddUpLevel.Name = "btnAddUpLevel";
            this.btnAddUpLevel.Size = new System.Drawing.Size(164, 56);
            this.btnAddUpLevel.TabIndex = 7;
            this.btnAddUpLevel.Text = "위층 추가";
            this.btnAddUpLevel.UseVisualStyleBackColor = true;
            this.btnAddUpLevel.Click += new System.EventHandler(this.btnAddUpLevel_Click);
            // 
            // btnSortLevel
            // 
            this.btnSortLevel.Enabled = false;
            this.btnSortLevel.Location = new System.Drawing.Point(224, 1042);
            this.btnSortLevel.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnSortLevel.Name = "btnSortLevel";
            this.btnSortLevel.Size = new System.Drawing.Size(164, 56);
            this.btnSortLevel.TabIndex = 7;
            this.btnSortLevel.Text = "층 정렬";
            this.btnSortLevel.UseVisualStyleBackColor = true;
            this.btnSortLevel.Click += new System.EventHandler(this.btnSortLevel_Click);
            // 
            // btnDeleteLevel
            // 
            this.btnDeleteLevel.Enabled = false;
            this.btnDeleteLevel.Location = new System.Drawing.Point(48, 1042);
            this.btnDeleteLevel.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnDeleteLevel.Name = "btnDeleteLevel";
            this.btnDeleteLevel.Size = new System.Drawing.Size(164, 56);
            this.btnDeleteLevel.TabIndex = 7;
            this.btnDeleteLevel.Text = "현재층 삭제";
            this.btnDeleteLevel.UseVisualStyleBackColor = true;
            this.btnDeleteLevel.Click += new System.EventHandler(this.btnDeleteLevel_Click);
            // 
            // btnAddDownLevel
            // 
            this.btnAddDownLevel.Enabled = false;
            this.btnAddDownLevel.Location = new System.Drawing.Point(48, 972);
            this.btnAddDownLevel.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnAddDownLevel.Name = "btnAddDownLevel";
            this.btnAddDownLevel.Size = new System.Drawing.Size(164, 56);
            this.btnAddDownLevel.TabIndex = 7;
            this.btnAddDownLevel.Text = "아래층 추가";
            this.btnAddDownLevel.UseVisualStyleBackColor = true;
            this.btnAddDownLevel.Click += new System.EventHandler(this.btnAddDownLevel_Click);
            // 
            // radioMovePOI
            // 
            this.radioMovePOI.AutoSize = true;
            this.radioMovePOI.Enabled = false;
            this.radioMovePOI.Location = new System.Drawing.Point(24, 1510);
            this.radioMovePOI.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radioMovePOI.Name = "radioMovePOI";
            this.radioMovePOI.Size = new System.Drawing.Size(129, 33);
            this.radioMovePOI.TabIndex = 6;
            this.radioMovePOI.Text = "POI 이동";
            this.radioMovePOI.UseVisualStyleBackColor = true;
            // 
            // radioDeletePOI
            // 
            this.radioDeletePOI.AutoSize = true;
            this.radioDeletePOI.Enabled = false;
            this.radioDeletePOI.Location = new System.Drawing.Point(24, 1443);
            this.radioDeletePOI.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radioDeletePOI.Name = "radioDeletePOI";
            this.radioDeletePOI.Size = new System.Drawing.Size(129, 33);
            this.radioDeletePOI.TabIndex = 6;
            this.radioDeletePOI.Text = "POI 삭제";
            this.radioDeletePOI.UseVisualStyleBackColor = true;
            // 
            // radioAddPOI
            // 
            this.radioAddPOI.AutoSize = true;
            this.radioAddPOI.Checked = true;
            this.radioAddPOI.Enabled = false;
            this.radioAddPOI.Location = new System.Drawing.Point(24, 1375);
            this.radioAddPOI.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.radioAddPOI.Name = "radioAddPOI";
            this.radioAddPOI.Size = new System.Drawing.Size(129, 33);
            this.radioAddPOI.TabIndex = 6;
            this.radioAddPOI.TabStop = true;
            this.radioAddPOI.Text = "POI 추가";
            this.radioAddPOI.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 1141);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 29);
            this.label2.TabIndex = 5;
            this.label2.Text = "cm";
            // 
            // textBoxLevelElevation
            // 
            this.textBoxLevelElevation.Enabled = false;
            this.textBoxLevelElevation.Location = new System.Drawing.Point(126, 1133);
            this.textBoxLevelElevation.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.textBoxLevelElevation.Name = "textBoxLevelElevation";
            this.textBoxLevelElevation.Size = new System.Drawing.Size(118, 35);
            this.textBoxLevelElevation.TabIndex = 4;
            this.textBoxLevelElevation.Text = "300";
            this.textBoxLevelElevation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelPOICode
            // 
            this.labelPOICode.AutoSize = true;
            this.labelPOICode.Location = new System.Drawing.Point(160, 1225);
            this.labelPOICode.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelPOICode.Name = "labelPOICode";
            this.labelPOICode.Size = new System.Drawing.Size(119, 29);
            this.labelPOICode.TabIndex = 3;
            this.labelPOICode.Text = "POI Code";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 1225);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 29);
            this.label3.TabIndex = 3;
            this.label3.Text = "POI Type :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 1141);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "층높이 :";
            // 
            // cboLevels
            // 
            this.cboLevels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLevels.Enabled = false;
            this.cboLevels.FormattingEnabled = true;
            this.cboLevels.Location = new System.Drawing.Point(244, 899);
            this.cboLevels.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.cboLevels.Name = "cboLevels";
            this.cboLevels.Size = new System.Drawing.Size(140, 37);
            this.cboLevels.TabIndex = 2;
            this.cboLevels.SelectedIndexChanged += new System.EventHandler(this.cboLevels_SelectedIndexChanged);
            // 
            // cboPOIs
            // 
            this.cboPOIs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPOIs.Enabled = false;
            this.cboPOIs.FormattingEnabled = true;
            this.cboPOIs.Location = new System.Drawing.Point(24, 1276);
            this.cboPOIs.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.cboPOIs.Name = "cboPOIs";
            this.cboPOIs.Size = new System.Drawing.Size(360, 37);
            this.cboPOIs.TabIndex = 2;
            this.cboPOIs.SelectedIndexChanged += new System.EventHandler(this.cboPOIs_SelectedIndexChanged);
            // 
            // checkBoxEditPOI
            // 
            this.checkBoxEditPOI.AutoSize = true;
            this.checkBoxEditPOI.Location = new System.Drawing.Point(24, 904);
            this.checkBoxEditPOI.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.checkBoxEditPOI.Name = "checkBoxEditPOI";
            this.checkBoxEditPOI.Size = new System.Drawing.Size(130, 33);
            this.checkBoxEditPOI.TabIndex = 1;
            this.checkBoxEditPOI.Text = "POI 편집";
            this.checkBoxEditPOI.UseVisualStyleBackColor = true;
            // 
            // gridLayer
            // 
            this.gridLayer.AllowUserToAddRows = false;
            this.gridLayer.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridLayer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gridLayer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLayer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShow,
            this.colLayerName,
            this.colColor});
            this.gridLayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridLayer.Location = new System.Drawing.Point(0, 0);
            this.gridLayer.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.gridLayer.MultiSelect = false;
            this.gridLayer.Name = "gridLayer";
            this.gridLayer.RowHeadersVisible = false;
            this.gridLayer.RowTemplate.Height = 23;
            this.gridLayer.Size = new System.Drawing.Size(400, 863);
            this.gridLayer.TabIndex = 0;
            this.gridLayer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLayer_CellContentClick);
            this.gridLayer.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLayer_CellValueChanged);
            // 
            // colShow
            // 
            this.colShow.HeaderText = "";
            this.colShow.Name = "colShow";
            this.colShow.Width = 25;
            // 
            // colLayerName
            // 
            this.colLayerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colLayerName.DefaultCellStyle = dataGridViewCellStyle4;
            this.colLayerName.HeaderText = "Layer 이름";
            this.colLayerName.Name = "colLayerName";
            this.colLayerName.ReadOnly = true;
            this.colLayerName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colColor
            // 
            this.colColor.HeaderText = "색상";
            this.colColor.Name = "colColor";
            this.colColor.Width = 40;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.웹서비스ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(12, 5, 0, 5);
            this.menuStrip1.Size = new System.Drawing.Size(1600, 53);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuOpenDXF,
            this.toolStripSeparator1,
            this.tsMenuExportPML,
            this.tsMenuImportPML});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(83, 43);
            this.fileToolStripMenuItem.Text = "파일";
            // 
            // tsMenuOpenDXF
            // 
            this.tsMenuOpenDXF.Name = "tsMenuOpenDXF";
            this.tsMenuOpenDXF.Size = new System.Drawing.Size(401, 42);
            this.tsMenuOpenDXF.Text = "DXF 열기";
            this.tsMenuOpenDXF.Click += new System.EventHandler(this.tsMenuOpenDXF_Click);
            // 
            // 웹서비스ToolStripMenuItem
            // 
            this.웹서비스ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuLoadFromServer,
            this.tsMenuSaveToServer});
            this.웹서비스ToolStripMenuItem.Name = "웹서비스ToolStripMenuItem";
            this.웹서비스ToolStripMenuItem.Size = new System.Drawing.Size(137, 43);
            this.웹서비스ToolStripMenuItem.Text = "웹서비스";
            // 
            // tsMenuLoadFromServer
            // 
            this.tsMenuLoadFromServer.Enabled = false;
            this.tsMenuLoadFromServer.Name = "tsMenuLoadFromServer";
            this.tsMenuLoadFromServer.Size = new System.Drawing.Size(284, 42);
            this.tsMenuLoadFromServer.Text = "POI 불러오기";
            this.tsMenuLoadFromServer.Click += new System.EventHandler(this.tsMenuLoadFromServer_Click);
            // 
            // tsMenuSaveToServer
            // 
            this.tsMenuSaveToServer.Enabled = false;
            this.tsMenuSaveToServer.Name = "tsMenuSaveToServer";
            this.tsMenuSaveToServer.Size = new System.Drawing.Size(284, 42);
            this.tsMenuSaveToServer.Text = "POI 저장하기";
            this.tsMenuSaveToServer.Click += new System.EventHandler(this.tsMenuSaveToServer_Click);
            // 
            // dxfControl
            // 
            this.dxfControl.AntiAliasing = true;
            this.dxfControl.BackColor = System.Drawing.Color.Black;
            this.dxfControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxfControl.DrawHatchFirst = true;
            this.dxfControl.ExternalPainter = null;
            this.dxfControl.GroupItemDistance = 30;
            this.dxfControl.GroupItemMinCount = 3;
            this.dxfControl.Location = new System.Drawing.Point(400, 53);
            this.dxfControl.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.dxfControl.MinimumSize = new System.Drawing.Size(200, 242);
            this.dxfControl.MovedVertex = vertex2D2;
            this.dxfControl.Name = "dxfControl";
            this.dxfControl.ObjectBR = null;
            this.dxfControl.ObjectTL = null;
            this.dxfControl.OpenNRefresh = true;
            this.dxfControl.Panning = false;
            this.dxfControl.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl.PrintDocument = null;
            this.dxfControl.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfControl.Size = new System.Drawing.Size(1200, 1647);
            this.dxfControl.TabIndex = 5;
            this.dxfControl.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl.UseGroupItem = false;
            this.dxfControl.UseLastViewport = false;
            this.dxfControl.UseMouseWheel = true;
            this.dxfControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dxfControl_MouseDown);
            this.dxfControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl_MouseMove);
            this.dxfControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dxfControl_MouseUp);
            // 
            // tsMenuExportPML
            // 
            this.tsMenuExportPML.Enabled = false;
            this.tsMenuExportPML.Name = "tsMenuExportPML";
            this.tsMenuExportPML.Size = new System.Drawing.Size(401, 42);
            this.tsMenuExportPML.Text = "POI 배치정보 내보내기";
            this.tsMenuExportPML.Click += new System.EventHandler(this.tsMenuExportPML_Click);
            // 
            // tsMenuImportPML
            // 
            this.tsMenuImportPML.Enabled = false;
            this.tsMenuImportPML.Name = "tsMenuImportPML";
            this.tsMenuImportPML.Size = new System.Drawing.Size(401, 42);
            this.tsMenuImportPML.Text = "POI 배치정보 가져오기";
            this.tsMenuImportPML.Click += new System.EventHandler(this.tsMenuImportPML_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(398, 6);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 1742);
            this.Controls.Add(this.dxfControl);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "FormMain";
            this.Text = "Virtual Seoul";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLayer)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.DataGridView gridLayer;
        private DXFViewer.DXFControl dxfControl;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenDXF;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLayerName;
        private System.Windows.Forms.DataGridViewButtonColumn colColor;
        private System.Windows.Forms.RadioButton radioDeletePOI;
        private System.Windows.Forms.RadioButton radioAddPOI;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLevelElevation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboLevels;
        private System.Windows.Forms.ComboBox cboPOIs;
        private System.Windows.Forms.CheckBox checkBoxEditPOI;
        private System.Windows.Forms.Button btnAddUpLevel;
        private System.Windows.Forms.Button btnSortLevel;
        private System.Windows.Forms.Button btnDeleteLevel;
        private System.Windows.Forms.Button btnAddDownLevel;
        private System.Windows.Forms.ToolStripMenuItem 웹서비스ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsMenuLoadFromServer;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSaveToServer;
        private System.Windows.Forms.Label labelPOICode;
        private System.Windows.Forms.RadioButton radioMovePOI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuExportPML;
        private System.Windows.Forms.ToolStripMenuItem tsMenuImportPML;
    }
}

