namespace UnE.Utility.Print
{
	partial class FormPrintPreview
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrintPreview));
			this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripButton4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem2 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem1 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem3 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem4 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem5 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem6 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem7 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem8 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStripMenuItem9 = new UnE.Utility.IDToolStripMenuItem();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// printPreviewControl1
			// 
			this.printPreviewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.printPreviewControl1.Location = new System.Drawing.Point(0, 25);
			this.printPreviewControl1.Name = "printPreviewControl1";
			this.printPreviewControl1.Size = new System.Drawing.Size(731, 564);
			this.printPreviewControl1.TabIndex = 1;
			this.printPreviewControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.printPreviewControl1_MouseDown);
			this.printPreviewControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.printPreviewControl1_MouseMove);
			this.printPreviewControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.printPreviewControl1_MouseUp);
			// 
			// toolStrip1
			// 
			this.toolStrip1.AllowMerge = false;
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.CanOverflow = false;
			this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripDropDownButton1,
            this.toolStripButton4,
            this.toolStripButton3});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
			this.toolStrip1.Size = new System.Drawing.Size(731, 25);
			this.toolStrip1.Stretch = true;
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.Image = global::RoadMan.Properties.Resources._64Print_normal;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(51, 22);
			this.toolStripButton1.Text = "인쇄";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem1,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9});
			this.toolStripDropDownButton1.Image = global::RoadMan.Properties.Resources._64Zoom_normal;
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(60, 22);
			this.toolStripDropDownButton1.Text = "확대";
			// 
			// toolStripButton4
			// 
			this.toolStripButton4.Name = "toolStripButton4";
			this.toolStripButton4.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(35, 22);
			this.toolStripButton3.Text = "닫기";
			this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Checked = true;
			this.toolStripMenuItem2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem2.CommandID = -1;
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem2.Text = "자동";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.CommandID = -1;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem1.Tag = "500";
			this.toolStripMenuItem1.Text = "500 %";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.CommandID = -1;
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem3.Tag = "200";
			this.toolStripMenuItem3.Text = "200%";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.CommandID = -1;
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem4.Tag = "150";
			this.toolStripMenuItem4.Text = "150%";
			this.toolStripMenuItem4.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.CommandID = -1;
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem5.Tag = "100";
			this.toolStripMenuItem5.Text = "100%";
			this.toolStripMenuItem5.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.CommandID = -1;
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem6.Tag = "75";
			this.toolStripMenuItem6.Text = "75%";
			this.toolStripMenuItem6.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.CommandID = -1;
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem7.Tag = "50";
			this.toolStripMenuItem7.Text = "50%";
			this.toolStripMenuItem7.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.CommandID = -1;
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem8.Tag = "25";
			this.toolStripMenuItem8.Text = "25%";
			this.toolStripMenuItem8.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.CommandID = -1;
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(109, 22);
			this.toolStripMenuItem9.Tag = "10";
			this.toolStripMenuItem9.Text = "10%";
			this.toolStripMenuItem9.Click += new System.EventHandler(this.scaleMenuItemClicked);
			// 
			// FormPrintPreview
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(731, 589);
			this.Controls.Add(this.printPreviewControl1);
			this.Controls.Add(this.toolStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "FormPrintPreview";
			this.Text = "인쇄 미리보기";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PrintPreviewControl printPreviewControl1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.ToolStripSeparator toolStripButton4;
		private IDToolStripMenuItem toolStripMenuItem2;
		private IDToolStripMenuItem toolStripMenuItem1;
		private IDToolStripMenuItem toolStripMenuItem3;
		private IDToolStripMenuItem toolStripMenuItem4;
		private IDToolStripMenuItem toolStripMenuItem5;
		private IDToolStripMenuItem toolStripMenuItem6;
		private IDToolStripMenuItem toolStripMenuItem7;
		private IDToolStripMenuItem toolStripMenuItem8;
		private IDToolStripMenuItem toolStripMenuItem9;
	}
}