namespace TeamEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFrame));
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Size = new System.Drawing.Size(970, 20);
			// 
			// panelLeft
			// 
			this.panelLeft.Size = new System.Drawing.Size(5, 638);
			// 
			// panelRight
			// 
			this.panelRight.Location = new System.Drawing.Point(965, 20);
			this.panelRight.Size = new System.Drawing.Size(5, 638);
			// 
			// panelBottom
			// 
			this.panelBottom.Location = new System.Drawing.Point(5, 658);
			this.panelBottom.Size = new System.Drawing.Size(960, 5);
			// 
			// panelLB
			// 
			this.panelLB.Location = new System.Drawing.Point(0, 658);
			// 
			// panelRB
			// 
			this.panelRB.Location = new System.Drawing.Point(965, 658);
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(23, 10);
			this.labelTitle.Size = new System.Drawing.Size(68, 15);
			this.labelTitle.Text = "FormFrame";
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(948, 2);
			// 
			// btnMax
			// 
			this.btnMax.Location = new System.Drawing.Point(930, 2);
			// 
			// btnMin
			// 
			this.btnMin.Location = new System.Drawing.Point(912, 2);
			// 
			// FormFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(970, 663);
			this.Icon = ((System.Drawing.Icon)(global::TeamEditor.Properties.Resources.teamedit_logo1));
			this.Name = "FormFrame";
			this.ShowCloseButton = true;
			this.ShowMaxButton = true;
			this.ShowMinButton = true;
			this.ShowPictureBoxTitle = true;
			this.Text = "FormFrame";
			this.TitleTextWidth = 68;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFrame_FormClosing);
			this.Load += new System.EventHandler(this.FormFrame_Load);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
    }
}