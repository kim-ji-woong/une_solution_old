namespace SOPGen
{
    partial class FlowMap
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
            this.contextSectionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sectionAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextSectionMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextSectionMenu
            // 
            this.contextSectionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sectionAdd});
            this.contextSectionMenu.Name = "contextSectionMenu";
            this.contextSectionMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // sectionAdd
            // 
            this.sectionAdd.Name = "sectionAdd";
            this.sectionAdd.Size = new System.Drawing.Size(152, 22);
            this.sectionAdd.Text = "Add";
            this.sectionAdd.Click += new System.EventHandler(this.sectionAdd_Click);
            // 
            // FlowMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 466);
            this.Name = "FlowMap";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FlowMap_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FlowMap_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FlowMap_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FlowMap_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FlowMap_MouseUp);
            this.contextSectionMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextSectionMenu;
        private System.Windows.Forms.ToolStripMenuItem sectionAdd;
    }
}

