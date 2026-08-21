namespace UnE.GUI.Contorl
{
	partial class FormRibbonTab
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
			this.tabControlEx1 = new UnE.Controls.TabControlEx();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabControlEx1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlEx1
			// 
			this.tabControlEx1.CloseBtnImage = null;
			this.tabControlEx1.Controls.Add(this.tabPage1);
			this.tabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlEx1.ItemSize = new System.Drawing.Size(80, 20);
			this.tabControlEx1.Location = new System.Drawing.Point(0, 0);
			this.tabControlEx1.Margin = new System.Windows.Forms.Padding(0);
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.Padding = new System.Drawing.Point(0, 0);
			this.tabControlEx1.SelectedIndex = 0;
			this.tabControlEx1.SelectedTabColor = System.Drawing.Color.DarkGray;
			this.tabControlEx1.Size = new System.Drawing.Size(871, 109);
			this.tabControlEx1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabControlEx1.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.tabControlEx1.TabForeColor = System.Drawing.Color.White;
			this.tabControlEx1.TabIndex = 10;
			this.tabControlEx1.UseCloseButton = false;
			this.tabControlEx1.OnTabDoubleClicked += new UnE.Controls.TabDoubleClicked(this.tabControlEx1_OnTabDoubleClicked);
			this.tabControlEx1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlEx1_Selected);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.panel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 24);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(863, 81);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "홈";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.panel1.BackgroundImage = global::UnE.GUI.Properties.Resources.RibbonBar_Middle;
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(863, 81);
			this.panel1.TabIndex = 7;
			// 
			// FormRibbon
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(871, 109);
			this.Controls.Add(this.tabControlEx1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRibbon";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "FormRibbon";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRibbon_FormClosing);
			this.Load += new System.EventHandler(this.FormRibbon_Load);
			this.ParentChanged += new System.EventHandler(this.FormRibbon_ParentChanged);
			this.tabControlEx1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		protected Controls.TabControlEx tabControlEx1;
		protected System.Windows.Forms.TabPage tabPage1;
		protected System.Windows.Forms.Panel panel1;

	}
}