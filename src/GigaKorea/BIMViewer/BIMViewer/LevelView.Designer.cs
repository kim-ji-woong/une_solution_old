namespace BIMViewer
{
    partial class LevelView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridLevel = new System.Windows.Forms.DataGridView();
            this.colProjects = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.contextMenuLevel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuOpenLevel = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridLevel)).BeginInit();
            this.panel1.SuspendLayout();
            this.contextMenuLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLevel
            // 
            this.gridLevel.AllowUserToAddRows = false;
            this.gridLevel.AllowUserToDeleteRows = false;
            this.gridLevel.AllowUserToResizeRows = false;
            this.gridLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridLevel.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(54)))), ((int)(((byte)(84)))));
            this.gridLevel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLevel.ColumnHeadersVisible = false;
            this.gridLevel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProjects});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(218)))), ((int)(((byte)(228)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridLevel.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridLevel.Location = new System.Drawing.Point(0, 23);
            this.gridLevel.MultiSelect = false;
            this.gridLevel.Name = "gridLevel";
            this.gridLevel.ReadOnly = true;
            this.gridLevel.RowHeadersVisible = false;
            this.gridLevel.RowTemplate.Height = 30;
            this.gridLevel.Size = new System.Drawing.Size(320, 302);
            this.gridLevel.TabIndex = 2;
            this.gridLevel.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridLevel_CellMouseClick);
            this.gridLevel.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridLevel_CellMouseDoubleClick);
            // 
            // colProjects
            // 
            this.colProjects.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colProjects.DefaultCellStyle = dataGridViewCellStyle1;
            this.colProjects.HeaderText = "건물";
            this.colProjects.Name = "colProjects";
            this.colProjects.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.popup_title;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 23);
            this.panel1.TabIndex = 3;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(5, 5);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(79, 16);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Floor Plan";
            // 
            // contextMenuLevel
            // 
            this.contextMenuLevel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuOpenLevel});
            this.contextMenuLevel.Name = "contextMenuLevel";
            this.contextMenuLevel.Size = new System.Drawing.Size(99, 26);
            // 
            // tsMenuOpenLevel
            // 
            this.tsMenuOpenLevel.Name = "tsMenuOpenLevel";
            this.tsMenuOpenLevel.Size = new System.Drawing.Size(98, 22);
            this.tsMenuOpenLevel.Text = "열기";
            this.tsMenuOpenLevel.Click += new System.EventHandler(this.tsMenuOpenLevel_Click);
            // 
            // LevelView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLevel);
            this.Controls.Add(this.panel1);
            this.Name = "LevelView";
            this.Size = new System.Drawing.Size(320, 325);
            ((System.ComponentModel.ISupportInitialize)(this.gridLevel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuLevel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjects;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuLevel;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpenLevel;
    }
}
