namespace KpxPipeMonitoring.Popups
{
    partial class BeginWorkSelectPipe
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
            this.label_pipeName = new System.Windows.Forms.Label();
            this.panelPipeButtonArea = new System.Windows.Forms.Panel();
            this.pictureBox_cancel = new System.Windows.Forms.PictureBox();
            this.pictureBox_begin = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_begin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label_pipeName
            // 
            this.label_pipeName.BackColor = System.Drawing.Color.Transparent;
            this.label_pipeName.Font = new System.Drawing.Font("나눔바른고딕", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_pipeName.Location = new System.Drawing.Point(266, 228);
            this.label_pipeName.Name = "label_pipeName";
            this.label_pipeName.Size = new System.Drawing.Size(191, 35);
            this.label_pipeName.TabIndex = 7;
            this.label_pipeName.Text = "TK-201";
            this.label_pipeName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelPipeButtonArea
            // 
            this.panelPipeButtonArea.Location = new System.Drawing.Point(124, 332);
            this.panelPipeButtonArea.Name = "panelPipeButtonArea";
            this.panelPipeButtonArea.Size = new System.Drawing.Size(831, 318);
            this.panelPipeButtonArea.TabIndex = 17;
            this.panelPipeButtonArea.Tag = "5,5";
            this.panelPipeButtonArea.Visible = false;
            // 
            // pictureBox_cancel
            // 
            this.pictureBox_cancel.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_cancel.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Cancel_Normal;
            this.pictureBox_cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_cancel.Location = new System.Drawing.Point(800, 670);
            this.pictureBox_cancel.Name = "pictureBox_cancel";
            this.pictureBox_cancel.Size = new System.Drawing.Size(155, 58);
            this.pictureBox_cancel.TabIndex = 11;
            this.pictureBox_cancel.TabStop = false;
            this.pictureBox_cancel.Click += new System.EventHandler(this.pictureBox_cancel_Click);
            this.pictureBox_cancel.MouseEnter += new System.EventHandler(this.pictureBox_cancel_MouseEnter);
            this.pictureBox_cancel.MouseLeave += new System.EventHandler(this.pictureBox_cancel_MouseLeave);
            // 
            // pictureBox_begin
            // 
            this.pictureBox_begin.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_begin.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Begin_Normal;
            this.pictureBox_begin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_begin.Location = new System.Drawing.Point(640, 670);
            this.pictureBox_begin.Name = "pictureBox_begin";
            this.pictureBox_begin.Size = new System.Drawing.Size(155, 58);
            this.pictureBox_begin.TabIndex = 10;
            this.pictureBox_begin.TabStop = false;
            this.pictureBox_begin.Click += new System.EventHandler(this.pictureBox_begin_Click);
            this.pictureBox_begin.MouseEnter += new System.EventHandler(this.pictureBox_begin_MouseEnter);
            this.pictureBox_begin.MouseLeave += new System.EventHandler(this.pictureBox_begin_MouseLeave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelectPipe;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1044, 809);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // BeginWorkSelectPipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1044, 809);
            this.Controls.Add(this.panelPipeButtonArea);
            this.Controls.Add(this.pictureBox_cancel);
            this.Controls.Add(this.pictureBox_begin);
            this.Controls.Add(this.label_pipeName);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BeginWorkSelectPipe";
            this.Text = "BeginWorkSelectItem";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(247)))), ((int)(((byte)(2)))));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_begin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_pipeName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox_begin;
        private System.Windows.Forms.PictureBox pictureBox_cancel;
        private System.Windows.Forms.Panel panelPipeButtonArea;
    }
}