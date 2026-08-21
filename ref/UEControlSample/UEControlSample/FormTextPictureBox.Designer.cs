namespace UEControlSample
{
    partial class FormTextPictureBox
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
            this.textPictureBox1 = new UnE.GUI.TextPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.textPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textPictureBox1
            // 
            this.textPictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.textPictureBox1.BackgroundImage = global::UEControlSample.Properties.Resources.RibbonChecked_bkgnd;
            this.textPictureBox1.Location = new System.Drawing.Point(51, 39);
            this.textPictureBox1.Name = "textPictureBox1";
            this.textPictureBox1.PictureBoxText = "Text";
            this.textPictureBox1.Size = new System.Drawing.Size(100, 131);
            this.textPictureBox1.TabIndex = 0;
            this.textPictureBox1.TabStop = false;
            this.textPictureBox1.TextColor = System.Drawing.Color.White;
            // 
            // FormTextPictureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.textPictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTextPictureBox";
            this.Text = "FormTextPictureBox";
            this.Load += new System.EventHandler(this.FormTextPictureBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textPictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.TextPictureBox textPictureBox1;

    }
}