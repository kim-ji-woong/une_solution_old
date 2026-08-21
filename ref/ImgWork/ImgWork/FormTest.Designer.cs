namespace ImgWork
{
    partial class FormTest
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
            this.mImageView = new ImgWork.ImageViewCtrl();
            this.mBtnFitView = new System.Windows.Forms.Button();
            this.mImageView.SuspendLayout();
            this.SuspendLayout();
            // 
            // mImageView
            // 
            this.mImageView.BackColor = System.Drawing.Color.White;
            this.mImageView.BillboardHeight = 32;
            this.mImageView.BillboardWidth = 32;
            this.mImageView.Controls.Add(this.mBtnFitView);
            this.mImageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mImageView.DrawBillBoard = true;
            this.mImageView.Location = new System.Drawing.Point(0, 0);
            this.mImageView.Name = "mImageView";
            this.mImageView.RectZoomMode = false;
            this.mImageView.RotationMode = false;
            this.mImageView.Size = new System.Drawing.Size(418, 354);
            this.mImageView.TabIndex = 0;
            this.mImageView.TranslateMode = false;
            this.mImageView.SizeChanged += new System.EventHandler(this.ImageView_SizeChanged);
            this.mImageView.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageView_Paint);
            this.mImageView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseDown);
            this.mImageView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseMove);
            this.mImageView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseUp);
            this.mImageView.Resize += new System.EventHandler(this.ImageView_Resize);
            // 
            // mBtnFitView
            // 
            this.mBtnFitView.Location = new System.Drawing.Point(328, 29);
            this.mBtnFitView.Name = "mBtnFitView";
            this.mBtnFitView.Size = new System.Drawing.Size(43, 28);
            this.mBtnFitView.TabIndex = 1;
            this.mBtnFitView.Text = "FitView";
            this.mBtnFitView.UseVisualStyleBackColor = true;
            this.mBtnFitView.Click += new System.EventHandler(this.ImageView_FitView);            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 354);
            this.Controls.Add(this.mImageView);
            this.Name = "FormTest";
            this.Text = "FormTest";
            this.Load += new System.EventHandler(this.FormTest_Load);
            this.mImageView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageViewCtrl mImageView;
        private System.Windows.Forms.Button mBtnFitView;
    }
}