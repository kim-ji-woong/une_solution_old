namespace UEControlSample
{
    partial class FormRibbonHorz
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
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.ribbonButton2 = new UnE.GUI.RibbonButton();
            this.ribbonButton3 = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.CheckedBkgndImage = global::UEControlSample.Properties.Resources.LeftBar_Click_Area;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(80, 10, 70, 70);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ForeColor = System.Drawing.Color.Black;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 230;
            this.ribbonButton1.IsChecked = true;
            this.ribbonButton1.Location = new System.Drawing.Point(60, 12);
            this.ribbonButton1.MouseOverBkgndImage = global::UEControlSample.Properties.Resources.LeftBar_Click_Area;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = global::UEControlSample.Properties.Resources.Load_Icon;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(230, 112);
            this.ribbonButton1.TabIndex = 0;
            this.ribbonButton1.Text = "불러오기";
            this.ribbonButton1.TextLocation = new System.Drawing.Point(90, 100);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = true;
            // 
            // ribbonButton2
            // 
            this.ribbonButton2.CheckedBkgndImage = global::UEControlSample.Properties.Resources.LeftBar_Click_Area;
            this.ribbonButton2.CheckedImage = null;
            this.ribbonButton2.CustomImageRect = new System.Drawing.Rectangle(80, 10, 70, 70);
            this.ribbonButton2.DisabledBkgndImage = null;
            this.ribbonButton2.DisabledImage = null;
            this.ribbonButton2.ForeColor = System.Drawing.Color.Maroon;
            this.ribbonButton2.ID = -1;
            this.ribbonButton2.InitButtonWidth = 230;
            this.ribbonButton2.IsChecked = false;
            this.ribbonButton2.Location = new System.Drawing.Point(60, 130);
            this.ribbonButton2.MouseOverBkgndImage = global::UEControlSample.Properties.Resources.LeftBar_Click_Area;
            this.ribbonButton2.Name = "ribbonButton2";
            this.ribbonButton2.NormalImage = global::UEControlSample.Properties.Resources.Update_Icon;
            this.ribbonButton2.Owner = null;
            this.ribbonButton2.Size = new System.Drawing.Size(230, 112);
            this.ribbonButton2.TabIndex = 0;
            this.ribbonButton2.Text = "업데이트";
            this.ribbonButton2.TextLocation = new System.Drawing.Point(90, 100);
            this.ribbonButton2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton2.UseCustomImageRect = true;
            this.ribbonButton2.UseTextLocation = false;
            this.ribbonButton2.UseVisualStyleBackColor = true;
            // 
            // ribbonButton3
            // 
            this.ribbonButton3.CheckedBkgndImage = global::UEControlSample.Properties.Resources.LeftBar_Click_Area;
            this.ribbonButton3.CheckedImage = null;
            this.ribbonButton3.CustomImageRect = new System.Drawing.Rectangle(80, 10, 70, 70);
            this.ribbonButton3.DisabledBkgndImage = null;
            this.ribbonButton3.DisabledImage = null;
            this.ribbonButton3.ForeColor = System.Drawing.Color.Goldenrod;
            this.ribbonButton3.ID = -1;
            this.ribbonButton3.InitButtonWidth = 230;
            this.ribbonButton3.IsChecked = false;
            this.ribbonButton3.Location = new System.Drawing.Point(60, 248);
            this.ribbonButton3.MouseOverBkgndImage = global::UEControlSample.Properties.Resources.LeftBar_Click_Area;
            this.ribbonButton3.Name = "ribbonButton3";
            this.ribbonButton3.NormalImage = global::UEControlSample.Properties.Resources.Save_icon;
            this.ribbonButton3.Owner = null;
            this.ribbonButton3.Size = new System.Drawing.Size(230, 112);
            this.ribbonButton3.TabIndex = 0;
            this.ribbonButton3.Text = "저장";
            this.ribbonButton3.TextLocation = new System.Drawing.Point(90, 100);
            this.ribbonButton3.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton3.UseCustomImageRect = true;
            this.ribbonButton3.UseTextLocation = false;
            this.ribbonButton3.UseVisualStyleBackColor = true;
            // 
            // FormRibbonHorz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 386);
            this.Controls.Add(this.ribbonButton3);
            this.Controls.Add(this.ribbonButton2);
            this.Controls.Add(this.ribbonButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRibbonHorz";
            this.Text = "FormRibbonHorz";
            this.Load += new System.EventHandler(this.FormRibbonHorz_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton ribbonButton1;
        private UnE.GUI.RibbonButton ribbonButton2;
        private UnE.GUI.RibbonButton ribbonButton3;
    }
}