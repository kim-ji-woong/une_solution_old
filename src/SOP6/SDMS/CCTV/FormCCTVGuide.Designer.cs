namespace SDMS
{
    partial class FormCCTVGuide
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
            this.labelZone = new System.Windows.Forms.Label();
            this.labelCCTV = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelZone
            // 
            this.labelZone.AutoSize = true;
            this.labelZone.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelZone.Location = new System.Drawing.Point(34, 38);
            this.labelZone.Name = "labelZone";
            this.labelZone.Size = new System.Drawing.Size(105, 25);
            this.labelZone.TabIndex = 0;
            this.labelZone.Text = "CCTV 위치";
            // 
            // labelCCTV
            // 
            this.labelCCTV.AutoSize = true;
            this.labelCCTV.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCCTV.Location = new System.Drawing.Point(34, 120);
            this.labelCCTV.Name = "labelCCTV";
            this.labelCCTV.Size = new System.Drawing.Size(143, 25);
            this.labelCCTV.TabIndex = 0;
            this.labelCCTV.Text = "CCTV 연결정보";
            // 
            // FormCCTVGuide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.labelCCTV);
            this.Controls.Add(this.labelZone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCCTVGuide";
            this.Text = "FormCCTVGuide";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelZone;
        private System.Windows.Forms.Label labelCCTV;
    }
}