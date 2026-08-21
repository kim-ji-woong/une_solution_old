namespace SDMS
{
    partial class PopupFormBase
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
            this.SuspendLayout();
            // 
            // PopupFormBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupFormBase";
            this.Text = "PopupFormBase";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupFormBase_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupFormBase_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupFormBase_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}