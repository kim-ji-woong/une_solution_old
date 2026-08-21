namespace SOPBulletin
{
    partial class DockingProgress2
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
            this.labelCurrent = new System.Windows.Forms.Label();
            this.labelMax = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelMin = new System.Windows.Forms.Label();
            this.progressBar = new ColorProgressBar.ColorProgressBar();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCurrent
            // 
            this.labelCurrent.AutoSize = true;
            this.labelCurrent.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrent.ForeColor = System.Drawing.Color.Red;
            this.labelCurrent.Location = new System.Drawing.Point(125, 43);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(26, 13);
            this.labelCurrent.TabIndex = 14;
            this.labelCurrent.Text = "0%";
            this.labelCurrent.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            // 
            // labelMax
            // 
            this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMax.AutoSize = true;
            this.labelMax.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMax.ForeColor = System.Drawing.Color.Black;
            this.labelMax.Location = new System.Drawing.Point(795, 43);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(40, 13);
            this.labelMax.TabIndex = 13;
            this.labelMax.Text = "100%";
            this.labelMax.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 32);
            this.label1.TabIndex = 12;
            this.label1.Text = "진행률";
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(63)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(107, 66);
            this.panel1.TabIndex = 15;
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            // 
            // labelMin
            // 
            this.labelMin.AutoSize = true;
            this.labelMin.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMin.ForeColor = System.Drawing.Color.Black;
            this.labelMin.Location = new System.Drawing.Point(125, 43);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(26, 13);
            this.labelMin.TabIndex = 16;
            this.labelMin.Text = "0%";
            this.labelMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BarColor = System.Drawing.Color.White;
            this.progressBar.BorderColor = System.Drawing.SystemColors.Control;
            this.progressBar.FillStyle = ColorProgressBar.ColorProgressBar.FillStyles.Solid;
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(37)))), ((int)(((byte)(35)))));
            this.progressBar.Location = new System.Drawing.Point(128, 12);
            this.progressBar.Maximum = 100;
            this.progressBar.Minimum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(672, 28);
            this.progressBar.Step = 10;
            this.progressBar.TabIndex = 11;
            this.progressBar.Value = 0;
            this.progressBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            // 
            // DockingProgress2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 66);
            this.Controls.Add(this.labelMin);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelCurrent);
            this.Controls.Add(this.labelMax);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingProgress2";
            this.Text = "DockingProgress2";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DockingProgress2_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.Label label1;
        private ColorProgressBar.ColorProgressBar progressBar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelMin;
    }
}