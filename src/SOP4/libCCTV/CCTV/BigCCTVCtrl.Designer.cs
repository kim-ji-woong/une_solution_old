namespace UnE.CCTV
{
    partial class BigCCTVCtrl
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

            //if( cctvCtrl1 != null)
            //{
            //    cctvCtrl1.Disconnect();
            //}
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Black;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(12, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(143, 25);
            this.lbTitle.TabIndex = 6;
            this.lbTitle.Text = "CCTV정보 없음";
            this.lbTitle.Click += new System.EventHandler(this.lbTitle_Click);
            this.lbTitle.DoubleClick += new System.EventHandler(this.lbTitle_DoubleClick);
            // 
            // BigCCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 457);
            this.Controls.Add(this.lbTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BigCCTVCtrl";
            this.Text = "BigCCTVCtrl";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BigCCTVCtrl_FormClosing);
            this.Load += new System.EventHandler(this.BigCCTVCtrl_Load);
            this.Shown += new System.EventHandler(this.BigCCTVCtrl_Shown);
            this.SizeChanged += new System.EventHandler(this.BigCCTVCtrl_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BigCCTVCtrl_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BigCCTVCtrl_MouseDown);
            this.Resize += new System.EventHandler(this.BigCCTVCtrl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
    }
}