namespace KpxPipeMonitoring.Popups
{
    partial class FormBase
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
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.pictureBoxTitle2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.OptionClose_normal;
            this.btnClose.Location = new System.Drawing.Point(305, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(18, 18);
            this.btnClose.TabIndex = 26;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // pictureBoxTitle2
            // 
            this.pictureBoxTitle2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTitle2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTitle2.Image = global::KpxPipeMonitoring.Properties.Resources.Top;
            this.pictureBoxTitle2.Location = new System.Drawing.Point(0, -1);
            this.pictureBoxTitle2.Name = "pictureBoxTitle2";
            this.pictureBoxTitle2.Size = new System.Drawing.Size(330, 30);
            this.pictureBoxTitle2.TabIndex = 9;
            this.pictureBoxTitle2.TabStop = false;
            this.pictureBoxTitle2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.pictureBoxTitle2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.pictureBoxTitle2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // FormBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 350);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pictureBoxTitle2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBase";
            this.Text = "FormBase";
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.PictureBox pictureBoxTitle2;
        protected System.Windows.Forms.PictureBox btnClose;
    }
}