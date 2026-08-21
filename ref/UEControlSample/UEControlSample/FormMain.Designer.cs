namespace UEControlSample
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clockControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ribbonBtnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ribbonGaroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textPictureBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.기타ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noFrameSizableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlToolStripMenuItem,
            this.기타ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(387, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clockControlToolStripMenuItem,
            this.ribbonBtnToolStripMenuItem,
            this.ribbonGaroToolStripMenuItem,
            this.imageButtonToolStripMenuItem,
            this.textPictureBoxToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.controlToolStripMenuItem.Text = "컨트롤";
            // 
            // clockControlToolStripMenuItem
            // 
            this.clockControlToolStripMenuItem.Name = "clockControlToolStripMenuItem";
            this.clockControlToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.clockControlToolStripMenuItem.Text = "시계 컨트롤";
            this.clockControlToolStripMenuItem.Click += new System.EventHandler(this.clockControlToolStripMenuItem_Click);
            // 
            // ribbonBtnToolStripMenuItem
            // 
            this.ribbonBtnToolStripMenuItem.Name = "ribbonBtnToolStripMenuItem";
            this.ribbonBtnToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.ribbonBtnToolStripMenuItem.Text = "리본 세로 버튼";
            this.ribbonBtnToolStripMenuItem.Click += new System.EventHandler(this.ribbonBtnToolStripMenuItem_Click);
            // 
            // ribbonGaroToolStripMenuItem
            // 
            this.ribbonGaroToolStripMenuItem.Name = "ribbonGaroToolStripMenuItem";
            this.ribbonGaroToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.ribbonGaroToolStripMenuItem.Text = "리본 가로 버튼";
            this.ribbonGaroToolStripMenuItem.Click += new System.EventHandler(this.ribbonGaroToolStripMenuItem_Click);
            // 
            // imageButtonToolStripMenuItem
            // 
            this.imageButtonToolStripMenuItem.Name = "imageButtonToolStripMenuItem";
            this.imageButtonToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.imageButtonToolStripMenuItem.Text = "이미지 버튼";
            this.imageButtonToolStripMenuItem.Click += new System.EventHandler(this.imageButtonToolStripMenuItem_Click);
            // 
            // textPictureBoxToolStripMenuItem
            // 
            this.textPictureBoxToolStripMenuItem.Name = "textPictureBoxToolStripMenuItem";
            this.textPictureBoxToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.textPictureBoxToolStripMenuItem.Text = "텍스트 픽쳐박스";
            this.textPictureBoxToolStripMenuItem.Click += new System.EventHandler(this.textPictureBoxToolStripMenuItem_Click);
            // 
            // 기타ToolStripMenuItem
            // 
            this.기타ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noFrameSizableToolStripMenuItem});
            this.기타ToolStripMenuItem.Name = "기타ToolStripMenuItem";
            this.기타ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.기타ToolStripMenuItem.Text = "기타";
            // 
            // noFrameSizableToolStripMenuItem
            // 
            this.noFrameSizableToolStripMenuItem.Name = "noFrameSizableToolStripMenuItem";
            this.noFrameSizableToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.noFrameSizableToolStripMenuItem.Text = "NoFrameSizable";
            this.noFrameSizableToolStripMenuItem.Click += new System.EventHandler(this.noFrameSizableToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 400);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "UEControl 예제";
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clockControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ribbonBtnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ribbonGaroToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageButtonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textPictureBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 기타ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noFrameSizableToolStripMenuItem;

    }
}

