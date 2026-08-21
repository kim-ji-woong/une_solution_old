namespace DXFView
{
    partial class LeftPanel
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
            this.dataGridViewLayer = new System.Windows.Forms.DataGridView();
            this.colShow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.layerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataGridViewBlock = new System.Windows.Forms.DataGridView();
            this.colShow_ = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colBlockName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor_ = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBlock)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewLayer
            // 
            this.dataGridViewLayer.AllowUserToAddRows = false;
            this.dataGridViewLayer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLayer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShow,
            this.layerName,
            this.colColor});
            this.dataGridViewLayer.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewLayer.Name = "dataGridViewLayer";
            this.dataGridViewLayer.RowHeadersVisible = false;
            this.dataGridViewLayer.RowTemplate.Height = 23;
            this.dataGridViewLayer.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewLayer.TabIndex = 0;
            this.dataGridViewLayer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewLayer_CellContentClick);
            this.dataGridViewLayer.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewLayer_CellValueChanged);
            // 
            // colShow
            // 
            this.colShow.HeaderText = "";
            this.colShow.Name = "colShow";
            this.colShow.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colShow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colShow.Width = 25;
            // 
            // layerName
            // 
            this.layerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.layerName.HeaderText = "Layer 이름";
            this.layerName.Name = "layerName";
            this.layerName.ReadOnly = true;
            // 
            // colColor
            // 
            this.colColor.HeaderText = "색상";
            this.colColor.Name = "colColor";
            this.colColor.ReadOnly = true;
            this.colColor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colColor.Width = 40;
            // 
            // dataGridViewBlock
            // 
            this.dataGridViewBlock.AllowUserToAddRows = false;
            this.dataGridViewBlock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBlock.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShow_,
            this.colBlockName,
            this.colColor_});
            this.dataGridViewBlock.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBlock.Name = "dataGridViewBlock";
            this.dataGridViewBlock.RowHeadersVisible = false;
            this.dataGridViewBlock.RowTemplate.Height = 23;
            this.dataGridViewBlock.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewBlock.TabIndex = 0;
            // 
            // colShow_
            // 
            this.colShow_.HeaderText = "";
            this.colShow_.Name = "colShow_";
            this.colShow_.Width = 25;
            // 
            // colBlockName
            // 
            this.colBlockName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colBlockName.HeaderText = "Block 이름";
            this.colBlockName.Name = "colBlockName";
            this.colBlockName.ReadOnly = true;
            // 
            // colColor_
            // 
            this.colColor_.HeaderText = "색상";
            this.colColor_.Name = "colColor_";
            this.colColor_.ReadOnly = true;
            this.colColor_.Width = 40;
            // 
            // LeftPanel
            // 
            this.SizeChanged += new System.EventHandler(this.LeftPanel_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBlock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewLayer;
        private System.Windows.Forms.DataGridView dataGridViewBlock;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn layerName;
        private System.Windows.Forms.DataGridViewButtonColumn colColor;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShow_;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBlockName;
        private System.Windows.Forms.DataGridViewButtonColumn colColor_;
    }
}
