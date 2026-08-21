namespace SOPManager
{
    partial class PopupUserDisaster
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblDisasterName = new System.Windows.Forms.Label();
            this.textDisaster = new System.Windows.Forms.TextBox();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(576, 78);
            this.panel2.TabIndex = 27;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.lblDisasterName);
            this.panel3.Controls.Add(this.textDisaster);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Font = new System.Drawing.Font(Program.prgFont, 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(570, 72);
            this.panel3.TabIndex = 100;
            // 
            // lblDisasterName
            // 
            this.lblDisasterName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDisasterName.ForeColor = System.Drawing.Color.White;
            this.lblDisasterName.Location = new System.Drawing.Point(20, 20);
            this.lblDisasterName.Name = "lblDisasterName";
            this.lblDisasterName.Size = new System.Drawing.Size(78, 27);
            this.lblDisasterName.TabIndex = 25;
            this.lblDisasterName.Text = "재난명";
            this.lblDisasterName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textDisaster
            // 
            this.textDisaster.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textDisaster.Location = new System.Drawing.Point(100, 22);
            this.textDisaster.Name = "textDisaster";
            this.textDisaster.Size = new System.Drawing.Size(366, 25);
            this.textDisaster.TabIndex = 0;
            this.textDisaster.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDisaster_KeyDown);
            // 
            // btnOK
            // 
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
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
            this.btnOK.Location = new System.Drawing.Point(470, 18);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 26;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupUserDisaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(576, 78);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupUserDisaster";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "재난 이름 설정";
            this.Load += new System.EventHandler(this.PopupUserDisaster_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupUserDisaster_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupUserDisaster_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupUserDisaster_MouseUp);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label lblDisasterName;
        private System.Windows.Forms.TextBox textDisaster;
        private UnE.GUI.RibbonButton btnOK;

	}
}