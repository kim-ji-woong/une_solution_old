namespace CadToSpace
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dxfControl = new DXFViewer.DXFControl();
            this.gridSpaceID = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSpaceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPolyLineLayerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTextLayerName = new System.Windows.Forms.TextBox();
            this.checkBoxRemember = new System.Windows.Forms.CheckBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.checkBoxAll = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxExportFilePath = new System.Windows.Forms.TextBox();
            this.btnExportFilePath = new System.Windows.Forms.Button();
            this.btnExportFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridSpaceID)).BeginInit();
            this.SuspendLayout();
            // 
            // dxfControl
            // 
            this.dxfControl.AllowDrop = true;
            this.dxfControl.AntiAliasing = true;
            this.dxfControl.BackColor = System.Drawing.Color.Black;
            this.dxfControl.DrawHatchFirst = true;
            this.dxfControl.ExternalPainter = null;
            this.dxfControl.GroupItemDistance = 30;
            this.dxfControl.GroupItemMinCount = 3;
            this.dxfControl.Location = new System.Drawing.Point(12, 12);
            this.dxfControl.MinimumSize = new System.Drawing.Size(100, 100);
            this.dxfControl.MovedVertex = vertex2D1;
            this.dxfControl.Name = "dxfControl";
            this.dxfControl.ObjectBR = null;
            this.dxfControl.ObjectTL = null;
            this.dxfControl.OpenNRefresh = true;
            this.dxfControl.Panning = false;
            this.dxfControl.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl.PrintDocument = null;
            this.dxfControl.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfControl.Size = new System.Drawing.Size(576, 380);
            this.dxfControl.TabIndex = 0;
            this.dxfControl.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl.UseGroupItem = false;
            this.dxfControl.UseLastViewport = false;
            this.dxfControl.UseMouseWheel = true;
            this.dxfControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.dxfControl_DragDrop);
            this.dxfControl.DragEnter += new System.Windows.Forms.DragEventHandler(this.dxfControl_DragEnter);
            // 
            // gridSpaceID
            // 
            this.gridSpaceID.AllowDrop = true;
            this.gridSpaceID.AllowUserToAddRows = false;
            this.gridSpaceID.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSpaceID.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridSpaceID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSpaceID.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colSpaceName});
            this.gridSpaceID.Location = new System.Drawing.Point(594, 12);
            this.gridSpaceID.Name = "gridSpaceID";
            this.gridSpaceID.ReadOnly = true;
            this.gridSpaceID.RowHeadersVisible = false;
            this.gridSpaceID.RowTemplate.Height = 23;
            this.gridSpaceID.Size = new System.Drawing.Size(265, 380);
            this.gridSpaceID.TabIndex = 1;
            this.gridSpaceID.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridSpaceID_DragDrop);
            this.gridSpaceID.DragEnter += new System.Windows.Forms.DragEventHandler(this.gridSpaceID_DragEnter);
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 80;
            // 
            // colSpaceName
            // 
            this.colSpaceName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colSpaceName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colSpaceName.HeaderText = "이름";
            this.colSpaceName.Name = "colSpaceName";
            this.colSpaceName.ReadOnly = true;
            this.colSpaceName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 413);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "PolyLine Layer Name :";
            // 
            // textBoxPolyLineLayerName
            // 
            this.textBoxPolyLineLayerName.Location = new System.Drawing.Point(153, 410);
            this.textBoxPolyLineLayerName.Name = "textBoxPolyLineLayerName";
            this.textBoxPolyLineLayerName.Size = new System.Drawing.Size(189, 21);
            this.textBoxPolyLineLayerName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(362, 413);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Text Layer Name :";
            // 
            // textBoxTextLayerName
            // 
            this.textBoxTextLayerName.Location = new System.Drawing.Point(478, 410);
            this.textBoxTextLayerName.Name = "textBoxTextLayerName";
            this.textBoxTextLayerName.Size = new System.Drawing.Size(189, 21);
            this.textBoxTextLayerName.TabIndex = 3;
            // 
            // checkBoxRemember
            // 
            this.checkBoxRemember.AutoSize = true;
            this.checkBoxRemember.Location = new System.Drawing.Point(673, 413);
            this.checkBoxRemember.Name = "checkBoxRemember";
            this.checkBoxRemember.Size = new System.Drawing.Size(48, 16);
            this.checkBoxRemember.TabIndex = 4;
            this.checkBoxRemember.Text = "기억";
            this.checkBoxRemember.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(803, 409);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(56, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "초기화";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // checkBoxAll
            // 
            this.checkBoxAll.AutoSize = true;
            this.checkBoxAll.Location = new System.Drawing.Point(727, 413);
            this.checkBoxAll.Name = "checkBoxAll";
            this.checkBoxAll.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAll.TabIndex = 4;
            this.checkBoxAll.Text = "전체보기";
            this.checkBoxAll.UseVisualStyleBackColor = true;
            this.checkBoxAll.CheckedChanged += new System.EventHandler(this.checkBoxAll_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 444);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "내보내기 파일경로       :";
            // 
            // textBoxExportFilePath
            // 
            this.textBoxExportFilePath.Location = new System.Drawing.Point(153, 441);
            this.textBoxExportFilePath.Name = "textBoxExportFilePath";
            this.textBoxExportFilePath.Size = new System.Drawing.Size(189, 21);
            this.textBoxExportFilePath.TabIndex = 3;
            // 
            // btnExportFilePath
            // 
            this.btnExportFilePath.Location = new System.Drawing.Point(348, 440);
            this.btnExportFilePath.Name = "btnExportFilePath";
            this.btnExportFilePath.Size = new System.Drawing.Size(31, 23);
            this.btnExportFilePath.TabIndex = 6;
            this.btnExportFilePath.Text = "...";
            this.btnExportFilePath.UseVisualStyleBackColor = true;
            this.btnExportFilePath.Click += new System.EventHandler(this.btnExportFilePath_Click);
            // 
            // btnExportFile
            // 
            this.btnExportFile.Location = new System.Drawing.Point(385, 440);
            this.btnExportFile.Name = "btnExportFile";
            this.btnExportFile.Size = new System.Drawing.Size(110, 23);
            this.btnExportFile.TabIndex = 6;
            this.btnExportFile.Text = "파일로 내보내기";
            this.btnExportFile.UseVisualStyleBackColor = true;
            this.btnExportFile.Click += new System.EventHandler(this.btnExportFile_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 474);
            this.Controls.Add(this.btnExportFile);
            this.Controls.Add(this.btnExportFilePath);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.checkBoxAll);
            this.Controls.Add(this.checkBoxRemember);
            this.Controls.Add(this.textBoxTextLayerName);
            this.Controls.Add(this.textBoxExportFilePath);
            this.Controls.Add(this.textBoxPolyLineLayerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridSpaceID);
            this.Controls.Add(this.dxfControl);
            this.Name = "FormMain";
            this.Text = "공간배치도";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridSpaceID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DXFViewer.DXFControl dxfControl;
        private System.Windows.Forms.DataGridView gridSpaceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpaceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPolyLineLayerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTextLayerName;
        private System.Windows.Forms.CheckBox checkBoxRemember;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox checkBoxAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxExportFilePath;
        private System.Windows.Forms.Button btnExportFilePath;
        private System.Windows.Forms.Button btnExportFile;
    }
}

