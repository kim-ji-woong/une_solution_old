namespace SOPManager
{
	partial class FormRibbon
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRibbon));
            this.btnOpenXML = new UnE.GUI.RibbonButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnStart = new UnE.GUI.RibbonButton();
            this.btnFinish = new UnE.GUI.RibbonButton();
            this.tabControlEx1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.doubleBufferedPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlEx1
            // 
            this.tabControlEx1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControlEx1.Size = new System.Drawing.Size(1175, 120);
            this.tabControlEx1.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            // 
            // tabPage1
            // 
            this.tabPage1.Size = new System.Drawing.Size(1167, 92);
            this.tabPage1.Text = "SOP";
            // 
            // doubleBufferedPanel1
            // 
            this.doubleBufferedPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.doubleBufferedPanel1.BackgroundImage = global::SOPManager.Properties.Resources.Background;
            this.doubleBufferedPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.doubleBufferedPanel1.Controls.Add(this.btnFinish);
            this.doubleBufferedPanel1.Controls.Add(this.btnStart);
            this.doubleBufferedPanel1.Controls.Add(this.pictureBox1);
            this.doubleBufferedPanel1.Controls.Add(this.btnOpenXML);
            this.doubleBufferedPanel1.Size = new System.Drawing.Size(1167, 92);
            // 
            // btnOpenXML
            // 
            this.btnOpenXML.BackColor = System.Drawing.Color.Transparent;
            this.btnOpenXML.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOpenXML.CheckButton = false;
            this.btnOpenXML.CheckedBkgndImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnOpenXML.CheckedImage = global::SOPManager.Properties.Resources.열기_normal;
            this.btnOpenXML.ClickedBackgroundImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnOpenXML.ClickedImage = global::SOPManager.Properties.Resources.열기_normal;
            this.btnOpenXML.CustomImageRect = new System.Drawing.Rectangle(20, 20, 32, 32);
            this.btnOpenXML.DisabledBkgndImage = null;
            this.btnOpenXML.DisabledImage = global::SOPManager.Properties.Resources.열기_disabled;
            this.btnOpenXML.ID = -1;
            this.btnOpenXML.InitButtonWidth = 70;
            this.btnOpenXML.IsChecked = false;
            this.btnOpenXML.Location = new System.Drawing.Point(162, 1);
            this.btnOpenXML.MouseOverBkgndImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnOpenXML.MouseOverImage = global::SOPManager.Properties.Resources.열기_normal;
            this.btnOpenXML.Name = "btnOpenXML";
            this.btnOpenXML.NormalImage = global::SOPManager.Properties.Resources.열기_normal;
            this.btnOpenXML.Owner = null;
            this.btnOpenXML.Size = new System.Drawing.Size(70, 90);
            this.btnOpenXML.TabIndex = 17;
            this.btnOpenXML.Text = "XML열기";
            this.btnOpenXML.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOpenXML.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOpenXML.ToolTipText = "XML열기";
            this.btnOpenXML.UseCustomImageRect = true;
            this.btnOpenXML.UseTextLocation = false;
            this.btnOpenXML.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.pictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.skin_line_img;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(158, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(2, 89);
            this.pictureBox1.TabIndex = 28;
            this.pictureBox1.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStart.CheckButton = false;
            this.btnStart.CheckedBkgndImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnStart.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnStart.CheckedImage")));
            this.btnStart.ClickedBackgroundImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnStart.ClickedImage = ((System.Drawing.Image)(resources.GetObject("btnStart.ClickedImage")));
            this.btnStart.CustomImageRect = new System.Drawing.Rectangle(20, 20, 32, 32);
            this.btnStart.DisabledBkgndImage = null;
            this.btnStart.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnStart.DisabledImage")));
            this.btnStart.Enabled = false;
            this.btnStart.ID = -1;
            this.btnStart.InitButtonWidth = 70;
            this.btnStart.IsChecked = false;
            this.btnStart.Location = new System.Drawing.Point(15, 1);
            this.btnStart.MouseOverBkgndImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnStart.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("btnStart.MouseOverImage")));
            this.btnStart.Name = "btnStart";
            this.btnStart.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnStart.NormalImage")));
            this.btnStart.Owner = null;
            this.btnStart.Size = new System.Drawing.Size(70, 90);
            this.btnStart.TabIndex = 30;
            this.btnStart.Text = "시작하기";
            this.btnStart.TextLocation = new System.Drawing.Point(0, 0);
            this.btnStart.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnStart.ToolTipText = "시작하기";
            this.btnStart.UseCustomImageRect = true;
            this.btnStart.UseTextLocation = false;
            this.btnStart.UseVisualStyleBackColor = false;
            // 
            // btnFinish
            // 
            this.btnFinish.BackColor = System.Drawing.Color.Transparent;
            this.btnFinish.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFinish.CheckButton = false;
            this.btnFinish.CheckedBkgndImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnFinish.CheckedImage = global::SOPManager.Properties.Resources.중지_normal;
            this.btnFinish.ClickedBackgroundImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnFinish.ClickedImage = global::SOPManager.Properties.Resources.중지_normal;
            this.btnFinish.CustomImageRect = new System.Drawing.Rectangle(20, 20, 32, 32);
            this.btnFinish.DisabledBkgndImage = null;
            this.btnFinish.DisabledImage = global::SOPManager.Properties.Resources.중지_disable;
            this.btnFinish.Enabled = false;
            this.btnFinish.ID = -1;
            this.btnFinish.InitButtonWidth = 70;
            this.btnFinish.IsChecked = false;
            this.btnFinish.Location = new System.Drawing.Point(86, 1);
            this.btnFinish.MouseOverBkgndImage = global::SOPManager.Properties.Resources.Checked_back;
            this.btnFinish.MouseOverImage = global::SOPManager.Properties.Resources.중지_normal;
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.NormalImage = global::SOPManager.Properties.Resources.중지_normal;
            this.btnFinish.Owner = null;
            this.btnFinish.Size = new System.Drawing.Size(70, 90);
            this.btnFinish.TabIndex = 30;
            this.btnFinish.Text = "중지하기";
            this.btnFinish.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFinish.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFinish.ToolTipText = "중지하기";
            this.btnFinish.UseCustomImageRect = true;
            this.btnFinish.UseTextLocation = false;
            this.btnFinish.UseVisualStyleBackColor = false;
            // 
            // FormRibbon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1175, 120);
            this.Name = "FormRibbon";
            this.Load += new System.EventHandler(this.FormRibbon_Load);
            this.tabControlEx1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.doubleBufferedPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private UnE.GUI.RibbonButton btnOpenXML;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UnE.GUI.RibbonButton btnStart;
        private UnE.GUI.RibbonButton btnFinish;
	}
}