namespace BIMViewer.uControl
{
    partial class uSaveForm
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
            this.lblLocalPath = new System.Windows.Forms.Label();
            this.rbtnYes = new UnE.GUI.RibbonButton();
            this.rbtnNo = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // lblLocalPath
            // 
            this.lblLocalPath.AutoSize = true;
            this.lblLocalPath.Location = new System.Drawing.Point(48, 23);
            this.lblLocalPath.Name = "lblLocalPath";
            this.lblLocalPath.Size = new System.Drawing.Size(38, 12);
            this.lblLocalPath.TabIndex = 0;
            this.lblLocalPath.Text = "label1";
            // 
            // rbtnYes
            // 
            this.rbtnYes.CheckButton = false;
            this.rbtnYes.CheckedBkgndImage = null;
            this.rbtnYes.CheckedImage = null;
            this.rbtnYes.CheckedMouseOver = null;
            this.rbtnYes.ClickedBackgroundImage = null;
            this.rbtnYes.ClickedImage = null;
            this.rbtnYes.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnYes.DisabledBkgndImage = null;
            this.rbtnYes.DisabledImage = null;
            this.rbtnYes.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnYes.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnYes.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnYes.ForeColorsByTypeUse = false;
            this.rbtnYes.ID = -1;
            this.rbtnYes.InitButtonWidth = 60;
            this.rbtnYes.IsChecked = false;
            this.rbtnYes.Location = new System.Drawing.Point(110, 54);
            this.rbtnYes.MouseOverBkgndImage = null;
            this.rbtnYes.MouseOverImage = null;
            this.rbtnYes.Name = "rbtnYes";
            this.rbtnYes.NormalImage = null;
            this.rbtnYes.Owner = null;
            this.rbtnYes.Size = new System.Drawing.Size(60, 29);
            this.rbtnYes.TabIndex = 1;
            this.rbtnYes.Text = "예";
            this.rbtnYes.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnYes.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnYes.ToolTipText = "예";
            this.rbtnYes.UseCustomImageRect = false;
            this.rbtnYes.UseTextLocation = false;
            this.rbtnYes.UseVisualStyleBackColor = true;
            // 
            // rbtnNo
            // 
            this.rbtnNo.CheckButton = false;
            this.rbtnNo.CheckedBkgndImage = null;
            this.rbtnNo.CheckedImage = null;
            this.rbtnNo.CheckedMouseOver = null;
            this.rbtnNo.ClickedBackgroundImage = null;
            this.rbtnNo.ClickedImage = null;
            this.rbtnNo.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnNo.DisabledBkgndImage = null;
            this.rbtnNo.DisabledImage = null;
            this.rbtnNo.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnNo.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnNo.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnNo.ForeColorsByTypeUse = false;
            this.rbtnNo.ID = -1;
            this.rbtnNo.InitButtonWidth = 60;
            this.rbtnNo.IsChecked = false;
            this.rbtnNo.Location = new System.Drawing.Point(224, 54);
            this.rbtnNo.MouseOverBkgndImage = null;
            this.rbtnNo.MouseOverImage = null;
            this.rbtnNo.Name = "rbtnNo";
            this.rbtnNo.NormalImage = null;
            this.rbtnNo.Owner = null;
            this.rbtnNo.Size = new System.Drawing.Size(60, 29);
            this.rbtnNo.TabIndex = 2;
            this.rbtnNo.Text = "아니오";
            this.rbtnNo.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnNo.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnNo.ToolTipText = "아니오";
            this.rbtnNo.UseCustomImageRect = false;
            this.rbtnNo.UseTextLocation = false;
            this.rbtnNo.UseVisualStyleBackColor = true;
            // 
            // uSaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 111);
            this.Controls.Add(this.rbtnNo);
            this.Controls.Add(this.rbtnYes);
            this.Controls.Add(this.lblLocalPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "uSaveForm";
            this.Text = "uSaveAs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLocalPath;
        private UnE.GUI.RibbonButton rbtnYes;
        private UnE.GUI.RibbonButton rbtnNo;
    }
}