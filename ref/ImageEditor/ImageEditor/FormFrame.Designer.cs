namespace ImageEditor
{
    partial class FormFrame
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
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Size = new System.Drawing.Size(296, 20);
            // 
            // panelLeft
            // 
            this.panelLeft.Size = new System.Drawing.Size(5, 241);
            // 
            // panelRight
            // 
            this.panelRight.Location = new System.Drawing.Point(291, 20);
            this.panelRight.Size = new System.Drawing.Size(5, 241);
            // 
            // panelBottom
            // 
            this.panelBottom.Location = new System.Drawing.Point(5, 261);
            this.panelBottom.Size = new System.Drawing.Size(286, 5);
            // 
            // panelLB
            // 
            this.panelLB.Location = new System.Drawing.Point(0, 261);
            // 
            // panelRB
            // 
            this.panelRB.Location = new System.Drawing.Point(291, 261);
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(3, 2);
            this.labelTitle.Size = new System.Drawing.Size(68, 15);
            this.labelTitle.Text = "FormFrame";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(274, 2);
            // 
            // btnMax
            // 
            this.btnMax.Location = new System.Drawing.Point(256, 2);
            // 
            // btnMin
            // 
            this.btnMin.Location = new System.Drawing.Point(238, 2);
            // 
            // FormFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 266);
            this.Name = "FormFrame";
            this.ShowCloseButton = true;
            this.ShowMaxButton = true;
            this.ShowMinButton = true;
            this.ShowPictureBoxTitle = true;
            this.Text = "FormFrame";
            this.TitleTextWidth = 68;
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}