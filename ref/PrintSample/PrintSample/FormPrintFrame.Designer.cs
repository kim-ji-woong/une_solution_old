namespace UnE.Utility.Print
{
    partial class FormPrintFrame
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrintFrame));
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
			this.panelTop.Size = new System.Drawing.Size(909, 20);
			// 
			// panelLeft
			// 
			this.panelLeft.Size = new System.Drawing.Size(5, 595);
			// 
			// panelRight
			// 
			this.panelRight.Location = new System.Drawing.Point(904, 20);
			this.panelRight.Size = new System.Drawing.Size(5, 595);
			// 
			// panelBottom
			// 
			this.panelBottom.Location = new System.Drawing.Point(5, 615);
			this.panelBottom.Size = new System.Drawing.Size(899, 5);
			// 
			// panelLB
			// 
			this.panelLB.Location = new System.Drawing.Point(0, 615);
			// 
			// panelRB
			// 
			this.panelRB.Location = new System.Drawing.Point(904, 615);
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(23, 10);
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(887, 2);
			// 
			// btnMax
			// 
			this.btnMax.Location = new System.Drawing.Point(869, 2);
			// 
			// btnMin
			// 
			this.btnMin.Location = new System.Drawing.Point(851, 2);
			// 
			// pictureBoxTitle
			// 
			this.pictureBoxTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
			// 
			// FormPrintFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.ClientSize = new System.Drawing.Size(909, 620);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrintFrame";
			this.ShowCloseButton = true;
			this.ShowInTaskbar = false;
			this.ShowMaxButton = true;
			this.ShowMinButton = true;
			this.ShowPictureBoxTitle = true;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.TitleBarBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
			this.TitleTextWidth = 122;
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
    }
}