namespace SDMS
{
    partial class TooltipCCTVCtrl2
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
            if (FormMain.Instance.CloseApplication)
                return;

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
            this.components = new System.ComponentModel.Container();
            this.cctvPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // cctvPanel
            // 
            this.cctvPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cctvPanel.Location = new System.Drawing.Point(0, 0);
            this.cctvPanel.Name = "cctvPanel";
            this.cctvPanel.Size = new System.Drawing.Size(290, 272);
            this.cctvPanel.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // TooltipCCTVCtrl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 272);
            this.Controls.Add(this.cctvPanel);
            this.MaximumSize = new System.Drawing.Size(840, 800);
            this.MinimizeBox = false;
            this.Name = "TooltipCCTVCtrl2";
            this.Text = "TooltipCCTVCtrl2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TooltipCCTVCtrl2_FormClosing);
            this.Resize += new System.EventHandler(this.TooltipCCTVCtrl2_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cctvPanel;
        private System.Windows.Forms.Timer timer1;
    }
}