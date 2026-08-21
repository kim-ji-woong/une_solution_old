namespace SOPManager
{
    partial class PopupPreListenMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupPreListenMessage));
            this.textBoxPreview = new System.Windows.Forms.TextBox();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.btnStop = new UnE.GUI.ImageButton();
            this.btnApply = new UnE.GUI.RibbonButton();
            this.btnPreListen = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.panelBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreListen)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxPreview
            // 
            this.textBoxPreview.BackColor = System.Drawing.Color.White;
            this.textBoxPreview.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPreview.Location = new System.Drawing.Point(3, 7);
            this.textBoxPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPreview.Multiline = true;
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxPreview.Size = new System.Drawing.Size(510, 302);
            this.textBoxPreview.TabIndex = 18;
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panelBackground.Controls.Add(this.btnStop);
            this.panelBackground.Controls.Add(this.btnApply);
            this.panelBackground.Controls.Add(this.btnPreListen);
            this.panelBackground.Controls.Add(this.btnOK);
            this.panelBackground.Controls.Add(this.textBoxPreview);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(516, 361);
            this.panelBackground.TabIndex = 19;
            // 
            // btnStop
            // 
            this.btnStop.ButtonText = "";
            this.btnStop.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStop.ImageClicked = global::SOPManager.Properties.Resources.PreListen_Stop_Click;
            this.btnStop.ImageDisabled = null;
            this.btnStop.ImageMouseOver = global::SOPManager.Properties.Resources.PreListen_Stop_Click;
            this.btnStop.ImageNormal = global::SOPManager.Properties.Resources.PreListen_Stop;
            this.btnStop.Location = new System.Drawing.Point(52, 316);
            this.btnStop.Name = "btnStop";
            this.btnStop.Owner = null;
            this.btnStop.Size = new System.Drawing.Size(37, 37);
            this.btnStop.TabIndex = 108;
            this.btnStop.TabStop = false;
            this.btnStop.TextColor = System.Drawing.Color.Black;
            this.btnStop.TextFont = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStop.ToolTipText = "";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnApply
            // 
            this.btnApply.CheckButton = false;
            this.btnApply.CheckedBkgndImage = null;
            this.btnApply.CheckedImage = null;
            this.btnApply.ClickedBackgroundImage = null;
            this.btnApply.ClickedImage = global::SOPManager.Properties.Resources.Apply_Click;
            this.btnApply.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnApply.DisabledBkgndImage = null;
            this.btnApply.DisabledImage = null;
            this.btnApply.ID = -1;
            this.btnApply.InitButtonWidth = 69;
            this.btnApply.IsChecked = false;
            this.btnApply.Location = new System.Drawing.Point(377, 316);
            this.btnApply.MouseOverBkgndImage = null;
            this.btnApply.MouseOverImage = global::SOPManager.Properties.Resources.Apply_Click;
            this.btnApply.Name = "btnApply";
            this.btnApply.NormalImage = global::SOPManager.Properties.Resources.Apply;
            this.btnApply.Owner = null;
            this.btnApply.Size = new System.Drawing.Size(69, 37);
            this.btnApply.TabIndex = 107;
            this.btnApply.TextLocation = new System.Drawing.Point(0, 0);
            this.btnApply.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnApply.ToolTipText = "";
            this.btnApply.UseCustomImageRect = true;
            this.btnApply.UseTextLocation = false;
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnPreListen
            // 
            this.btnPreListen.ButtonText = "";
            this.btnPreListen.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreListen.ImageClicked = global::SOPManager.Properties.Resources.PreListen_Play_Click;
            this.btnPreListen.ImageDisabled = null;
            this.btnPreListen.ImageMouseOver = global::SOPManager.Properties.Resources.PreListen_Play_Click;
            this.btnPreListen.ImageNormal = global::SOPManager.Properties.Resources.PreListen_Play;
            this.btnPreListen.Location = new System.Drawing.Point(12, 316);
            this.btnPreListen.Name = "btnPreListen";
            this.btnPreListen.Owner = null;
            this.btnPreListen.Size = new System.Drawing.Size(37, 37);
            this.btnPreListen.TabIndex = 106;
            this.btnPreListen.TabStop = false;
            this.btnPreListen.TextColor = System.Drawing.Color.Black;
            this.btnPreListen.TextFont = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPreListen.ToolTipText = "";
            this.btnPreListen.Click += new System.EventHandler(this.btnPreListen_Click);
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
            this.btnOK.Location = new System.Drawing.Point(447, 316);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 95;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupPreListenMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(516, 361);
            this.Controls.Add(this.panelBackground);
            this.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(516, 345);
            this.Name = "PopupPreListenMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "방송메시지 미리보기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PopupPreListenMessage_FormClosing);
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreListen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TextBox textBoxPreview;
        private System.Windows.Forms.Panel panelBackground;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.ImageButton btnPreListen;
        private UnE.GUI.RibbonButton btnApply;
        private UnE.GUI.ImageButton btnStop;
    }
}