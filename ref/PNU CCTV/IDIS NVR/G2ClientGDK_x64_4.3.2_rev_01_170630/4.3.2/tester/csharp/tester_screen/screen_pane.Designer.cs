namespace GDK_tester
{
    partial class screen_pane
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SCREEN_FRAME = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.SCREEN_FRAME)).BeginInit();
            this.SuspendLayout();
            // 
            // SCREEN_FRAME 
            // 
            this.SCREEN_FRAME.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.SCREEN_FRAME.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SCREEN_FRAME.Location = new System.Drawing.Point(0, 0);
            this.SCREEN_FRAME.Name = "SCREEN_FRAME";
            this.SCREEN_FRAME.Size = new System.Drawing.Size(640, 360);
            this.SCREEN_FRAME.TabIndex = 0;
            this.SCREEN_FRAME.TabStop = false;
            this.SCREEN_FRAME.Paint += new System.Windows.Forms.PaintEventHandler(this.on_paint);
            this.SCREEN_FRAME.MouseClick += new System.Windows.Forms.MouseEventHandler(this.on_mouse_click);
            this.SCREEN_FRAME.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.on_mouse_double_click);
            // 
            // screen_pane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SCREEN_FRAME);
            this.Name = "screen_pane";
            this.Size = new System.Drawing.Size(640, 360);
            ((System.ComponentModel.ISupportInitialize)(this.SCREEN_FRAME)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        protected System.Windows.Forms.PictureBox SCREEN_FRAME;
    }
}
