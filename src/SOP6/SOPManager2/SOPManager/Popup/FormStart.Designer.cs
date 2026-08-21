namespace SOPManager
{
	partial class FormStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnCanel = new UnE.GUI.RibbonButton();
            this.grbStart = new System.Windows.Forms.GroupBox();
            this.lblOpenXML = new System.Windows.Forms.Label();
            this.lblNewSOP = new System.Windows.Forms.Label();
            this.picOpenXML = new System.Windows.Forms.PictureBox();
            this.picOpenSOP = new System.Windows.Forms.PictureBox();
            this.picNewSOP = new System.Windows.Forms.PictureBox();
            this.lblOpenSOP = new System.Windows.Forms.Label();
            this.btnConSetting = new System.Windows.Forms.Button();
            this.rbOpenXML = new System.Windows.Forms.RadioButton();
            this.rbOpenSOP = new System.Windows.Forms.RadioButton();
            this.rbNewSOP = new System.Windows.Forms.RadioButton();
            this.panelMain.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.grbStart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOpenXML)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOpenSOP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNewSOP)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelContent);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(567, 207);
            this.panelMain.TabIndex = 0;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panelContent.Controls.Add(this.btnOK);
            this.panelContent.Controls.Add(this.btnCanel);
            this.panelContent.Controls.Add(this.grbStart);
            this.panelContent.Controls.Add(this.btnConSetting);
            this.panelContent.Controls.Add(this.rbOpenXML);
            this.panelContent.Controls.Add(this.rbOpenSOP);
            this.panelContent.Controls.Add(this.rbNewSOP);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(567, 207);
            this.panelContent.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 69;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(427, 167);
            this.btnOK.Margin = new System.Windows.Forms.Padding(0);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 4;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanel
            // 
            this.btnCanel.CheckButton = false;
            this.btnCanel.CheckedBkgndImage = null;
            this.btnCanel.CheckedImage = null;
            this.btnCanel.ClickedBackgroundImage = null;
            this.btnCanel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCanel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnCanel.DisabledBkgndImage = null;
            this.btnCanel.DisabledImage = null;
            this.btnCanel.ID = -1;
            this.btnCanel.InitButtonWidth = 69;
            this.btnCanel.IsChecked = false;
            this.btnCanel.Location = new System.Drawing.Point(496, 167);
            this.btnCanel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCanel.MouseOverBkgndImage = null;
            this.btnCanel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCanel.Name = "btnCanel";
            this.btnCanel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCanel.Owner = null;
            this.btnCanel.Size = new System.Drawing.Size(69, 37);
            this.btnCanel.TabIndex = 5;
            this.btnCanel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCanel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCanel.ToolTipText = "";
            this.btnCanel.UseCustomImageRect = true;
            this.btnCanel.UseTextLocation = false;
            this.btnCanel.UseVisualStyleBackColor = true;
            this.btnCanel.Click += new System.EventHandler(this.btnCanel_Click);
            // 
            // grbStart
            // 
            this.grbStart.BackColor = System.Drawing.Color.Transparent;
            this.grbStart.Controls.Add(this.lblOpenXML);
            this.grbStart.Controls.Add(this.lblNewSOP);
            this.grbStart.Controls.Add(this.picOpenXML);
            this.grbStart.Controls.Add(this.picOpenSOP);
            this.grbStart.Controls.Add(this.picNewSOP);
            this.grbStart.Controls.Add(this.lblOpenSOP);
            this.grbStart.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.grbStart.ForeColor = System.Drawing.Color.White;
            this.grbStart.Location = new System.Drawing.Point(12, 10);
            this.grbStart.Name = "grbStart";
            this.grbStart.Size = new System.Drawing.Size(546, 150);
            this.grbStart.TabIndex = 0;
            this.grbStart.TabStop = false;
            this.grbStart.Text = "시작하기";
            // 
            // lblOpenXML
            // 
            this.lblOpenXML.AutoSize = true;
            this.lblOpenXML.BackColor = System.Drawing.Color.Transparent;
            this.lblOpenXML.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblOpenXML.ForeColor = System.Drawing.Color.White;
            this.lblOpenXML.Location = new System.Drawing.Point(32, 103);
            this.lblOpenXML.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.lblOpenXML.Name = "lblOpenXML";
            this.lblOpenXML.Size = new System.Drawing.Size(369, 18);
            this.lblOpenXML.TabIndex = 88;
            this.lblOpenXML.Text = "XML 불러오기 - SOP XML문서를 열기로 시작합니다.";
            this.lblOpenXML.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblOpenXML.Click += new System.EventHandler(this.OpenXML_Click);
            // 
            // lblNewSOP
            // 
            this.lblNewSOP.AutoSize = true;
            this.lblNewSOP.BackColor = System.Drawing.Color.Transparent;
            this.lblNewSOP.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNewSOP.ForeColor = System.Drawing.Color.White;
            this.lblNewSOP.Location = new System.Drawing.Point(32, 35);
            this.lblNewSOP.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.lblNewSOP.Name = "lblNewSOP";
            this.lblNewSOP.Size = new System.Drawing.Size(322, 18);
            this.lblNewSOP.TabIndex = 83;
            this.lblNewSOP.Text = "새 SOP 시작하기 -  새로운 SOP를 생성합니다.";
            this.lblNewSOP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNewSOP.Click += new System.EventHandler(this.NewSOP_Click);
            // 
            // picOpenXML
            // 
            this.picOpenXML.BackColor = System.Drawing.Color.Transparent;
            this.picOpenXML.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picOpenXML.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picOpenXML.Location = new System.Drawing.Point(10, 101);
            this.picOpenXML.Margin = new System.Windows.Forms.Padding(3, 10, 0, 0);
            this.picOpenXML.Name = "picOpenXML";
            this.picOpenXML.Size = new System.Drawing.Size(22, 22);
            this.picOpenXML.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picOpenXML.TabIndex = 87;
            this.picOpenXML.TabStop = false;
            this.picOpenXML.Click += new System.EventHandler(this.OpenXML_Click);
            // 
            // picOpenSOP
            // 
            this.picOpenSOP.BackColor = System.Drawing.Color.Transparent;
            this.picOpenSOP.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picOpenSOP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picOpenSOP.Location = new System.Drawing.Point(10, 67);
            this.picOpenSOP.Margin = new System.Windows.Forms.Padding(3, 10, 0, 0);
            this.picOpenSOP.Name = "picOpenSOP";
            this.picOpenSOP.Size = new System.Drawing.Size(22, 22);
            this.picOpenSOP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picOpenSOP.TabIndex = 85;
            this.picOpenSOP.TabStop = false;
            this.picOpenSOP.Click += new System.EventHandler(this.OpenSOP_Click);
            // 
            // picNewSOP
            // 
            this.picNewSOP.BackColor = System.Drawing.Color.Transparent;
            this.picNewSOP.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Enable2;
            this.picNewSOP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picNewSOP.Location = new System.Drawing.Point(10, 33);
            this.picNewSOP.Margin = new System.Windows.Forms.Padding(3, 10, 0, 0);
            this.picNewSOP.Name = "picNewSOP";
            this.picNewSOP.Size = new System.Drawing.Size(22, 22);
            this.picNewSOP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picNewSOP.TabIndex = 84;
            this.picNewSOP.TabStop = false;
            this.picNewSOP.Click += new System.EventHandler(this.NewSOP_Click);
            // 
            // lblOpenSOP
            // 
            this.lblOpenSOP.AutoSize = true;
            this.lblOpenSOP.BackColor = System.Drawing.Color.Transparent;
            this.lblOpenSOP.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblOpenSOP.ForeColor = System.Drawing.Color.White;
            this.lblOpenSOP.Location = new System.Drawing.Point(32, 69);
            this.lblOpenSOP.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.lblOpenSOP.Name = "lblOpenSOP";
            this.lblOpenSOP.Size = new System.Drawing.Size(424, 18);
            this.lblOpenSOP.TabIndex = 86;
            this.lblOpenSOP.Text = "기존 SOP 불러오기 - 이미 작성된 SOP를 DB에서 불러옵니다.";
            this.lblOpenSOP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblOpenSOP.Click += new System.EventHandler(this.OpenSOP_Click);
            // 
            // btnConSetting
            // 
            this.btnConSetting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnConSetting.Location = new System.Drawing.Point(953, 440);
            this.btnConSetting.Name = "btnConSetting";
            this.btnConSetting.Size = new System.Drawing.Size(101, 28);
            this.btnConSetting.TabIndex = 3;
            this.btnConSetting.Text = "연결 설정";
            this.btnConSetting.UseVisualStyleBackColor = true;
            this.btnConSetting.Visible = false;
            this.btnConSetting.Click += new System.EventHandler(this.btnConSetting_Click);
            // 
            // rbOpenXML
            // 
            this.rbOpenXML.AutoSize = true;
            this.rbOpenXML.BackColor = System.Drawing.Color.Transparent;
            this.rbOpenXML.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbOpenXML.ForeColor = System.Drawing.Color.White;
            this.rbOpenXML.Location = new System.Drawing.Point(1060, 447);
            this.rbOpenXML.Name = "rbOpenXML";
            this.rbOpenXML.Size = new System.Drawing.Size(390, 21);
            this.rbOpenXML.TabIndex = 2;
            this.rbOpenXML.Text = "XML 불러오기 - SOP XML문서를 열기로 시작합니다.";
            this.rbOpenXML.UseVisualStyleBackColor = false;
            this.rbOpenXML.Visible = false;
            this.rbOpenXML.CheckedChanged += new System.EventHandler(this.rbOpenXML_CheckedChanged);
            // 
            // rbOpenSOP
            // 
            this.rbOpenSOP.AutoSize = true;
            this.rbOpenSOP.BackColor = System.Drawing.Color.Transparent;
            this.rbOpenSOP.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbOpenSOP.ForeColor = System.Drawing.Color.White;
            this.rbOpenSOP.Location = new System.Drawing.Point(1060, 418);
            this.rbOpenSOP.Name = "rbOpenSOP";
            this.rbOpenSOP.Size = new System.Drawing.Size(446, 21);
            this.rbOpenSOP.TabIndex = 1;
            this.rbOpenSOP.Text = "기존 SOP 불러오기 - 이미 작성된 SOP를 DB에서 불러옵니다.";
            this.rbOpenSOP.UseVisualStyleBackColor = false;
            this.rbOpenSOP.Visible = false;
            this.rbOpenSOP.CheckedChanged += new System.EventHandler(this.rbOpenSOP_CheckedChanged);
            // 
            // rbNewSOP
            // 
            this.rbNewSOP.AutoSize = true;
            this.rbNewSOP.BackColor = System.Drawing.Color.Transparent;
            this.rbNewSOP.Checked = true;
            this.rbNewSOP.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbNewSOP.ForeColor = System.Drawing.Color.White;
            this.rbNewSOP.Location = new System.Drawing.Point(1060, 391);
            this.rbNewSOP.Name = "rbNewSOP";
            this.rbNewSOP.Size = new System.Drawing.Size(345, 21);
            this.rbNewSOP.TabIndex = 0;
            this.rbNewSOP.TabStop = true;
            this.rbNewSOP.Text = "새 SOP 시작하기 -  새로운 SOP를 생성합니다.";
            this.rbNewSOP.UseVisualStyleBackColor = false;
            this.rbNewSOP.Visible = false;
            this.rbNewSOP.CheckedChanged += new System.EventHandler(this.rbNewSOP_CheckedChanged);
            // 
            // FormStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 207);
            this.ControlBox = false;
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "새로운 시작";
            this.TopMost = true;
            this.panelMain.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.grbStart.ResumeLayout(false);
            this.grbStart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOpenXML)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picOpenSOP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNewSOP)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button btnConSetting;
		private System.Windows.Forms.GroupBox grbStart;
		private System.Windows.Forms.RadioButton rbOpenXML;
		private System.Windows.Forms.RadioButton rbOpenSOP;
		private System.Windows.Forms.RadioButton rbNewSOP;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnCanel;
        private System.Windows.Forms.PictureBox picOpenXML;
        private System.Windows.Forms.PictureBox picOpenSOP;
        private System.Windows.Forms.PictureBox picNewSOP;
        private System.Windows.Forms.Label lblNewSOP;
        private System.Windows.Forms.Label lblOpenSOP;
        private System.Windows.Forms.Label lblOpenXML;
	}
}