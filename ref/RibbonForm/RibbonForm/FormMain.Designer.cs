namespace UnE.GUI
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
			this.panelRibbon = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.sdfsdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sdfsdfToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.sdfdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sdfsdfToolStripMenuItem,
            this.sdfsdfToolStripMenuItem1});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(914, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// panelRibbon
			// 
			this.panelRibbon.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelRibbon.Location = new System.Drawing.Point(0, 24);
			this.panelRibbon.Name = "panelRibbon";
			this.panelRibbon.Size = new System.Drawing.Size(914, 100);
			this.panelRibbon.TabIndex = 1;
			// 
			// panel2
			// 
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 124);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(914, 454);
			this.panel2.TabIndex = 2;
			// 
			// sdfsdfToolStripMenuItem
			// 
			this.sdfsdfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sdfdfToolStripMenuItem});
			this.sdfsdfToolStripMenuItem.Name = "sdfsdfToolStripMenuItem";
			this.sdfsdfToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
			this.sdfsdfToolStripMenuItem.Text = "sdfsdf";
			this.sdfsdfToolStripMenuItem.Click += new System.EventHandler(this.sdfsdfToolStripMenuItem_Click);
			// 
			// sdfsdfToolStripMenuItem1
			// 
			this.sdfsdfToolStripMenuItem1.Name = "sdfsdfToolStripMenuItem1";
			this.sdfsdfToolStripMenuItem1.Size = new System.Drawing.Size(51, 20);
			this.sdfsdfToolStripMenuItem1.Text = "sdfsdf";
			// 
			// sdfdfToolStripMenuItem
			// 
			this.sdfdfToolStripMenuItem.Name = "sdfdfToolStripMenuItem";
			this.sdfdfToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.sdfdfToolStripMenuItem.Text = "sdfdf";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(914, 578);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panelRibbon);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormMain";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem sdfsdfToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sdfsdfToolStripMenuItem1;
		private System.Windows.Forms.Panel panelRibbon;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ToolStripMenuItem sdfdfToolStripMenuItem;


	}
}

