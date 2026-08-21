namespace SDMS
{
    partial class Form4CCTV
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4CCTV));
            this.SuspendLayout();
            // 
            // Form4CCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(71)))), ((int)(((byte)(103)))));
            this.ClientSize = new System.Drawing.Size(769, 501);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form4CCTV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form4CCTV";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form4CCTV_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.Form4CCTV_VisibleChanged);
            this.Resize += new System.EventHandler(this.Form4CCTV_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}