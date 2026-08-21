namespace SVMSTest
{
    partial class FormCCTV
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCCTV));
            this.axRTSPLiveScreen1 = new AxRTSPLiveScreenLib.AxRTSPLiveScreen();
            ((System.ComponentModel.ISupportInitialize)(this.axRTSPLiveScreen1)).BeginInit();
            this.SuspendLayout();
            // 
            // axRTSPLiveScreen1
            // 
            this.axRTSPLiveScreen1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRTSPLiveScreen1.Enabled = true;
            this.axRTSPLiveScreen1.Location = new System.Drawing.Point(0, 0);
            this.axRTSPLiveScreen1.Name = "axRTSPLiveScreen1";
            this.axRTSPLiveScreen1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRTSPLiveScreen1.OcxState")));
            this.axRTSPLiveScreen1.Size = new System.Drawing.Size(394, 300);
            this.axRTSPLiveScreen1.TabIndex = 0;
            // 
            // FormCCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 300);
            this.Controls.Add(this.axRTSPLiveScreen1);
            this.Name = "FormCCTV";
            this.Text = "FormCCTV";
            this.Load += new System.EventHandler(this.FormCCTV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axRTSPLiveScreen1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxRTSPLiveScreenLib.AxRTSPLiveScreen axRTSPLiveScreen1;
    }
}