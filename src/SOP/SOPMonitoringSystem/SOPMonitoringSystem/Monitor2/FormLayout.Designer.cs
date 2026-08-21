namespace SOPDisasterSystem
{
    partial class FormLayout
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
            this.components = new System.ComponentModel.Container();
            this.panelLayout1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLayout2 = new System.Windows.Forms.Panel();
            this.panelLayout3 = new System.Windows.Forms.Panel();
            this.panelLayout4 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelLayout1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLayout1
            // 
            this.panelLayout1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelLayout1.Controls.Add(this.label1);
            this.panelLayout1.Location = new System.Drawing.Point(0, 0);
            this.panelLayout1.Name = "panelLayout1";
            this.panelLayout1.Size = new System.Drawing.Size(249, 249);
            this.panelLayout1.TabIndex = 0;
            this.panelLayout1.SizeChanged += new System.EventHandler(this.panelLayout1_SizeChanged);
            this.panelLayout1.MouseEnter += new System.EventHandler(this.panelLayout1_MouseEnter);
            this.panelLayout1.MouseLeave += new System.EventHandler(this.panelLayout1_MouseLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(29, 221);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "File Download 중입니다.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // panelLayout2
            // 
            this.panelLayout2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelLayout2.Location = new System.Drawing.Point(251, 0);
            this.panelLayout2.Name = "panelLayout2";
            this.panelLayout2.Size = new System.Drawing.Size(249, 249);
            this.panelLayout2.TabIndex = 1;
            this.panelLayout2.SizeChanged += new System.EventHandler(this.panelLayout2_SizeChanged);
            this.panelLayout2.MouseEnter += new System.EventHandler(this.panelLayout2_MouseEnter);
            this.panelLayout2.MouseLeave += new System.EventHandler(this.panelLayout2_MouseLeave);
            // 
            // panelLayout3
            // 
            this.panelLayout3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panelLayout3.Location = new System.Drawing.Point(0, 251);
            this.panelLayout3.Name = "panelLayout3";
            this.panelLayout3.Size = new System.Drawing.Size(249, 249);
            this.panelLayout3.TabIndex = 2;
            this.panelLayout3.SizeChanged += new System.EventHandler(this.panelLayout3_SizeChanged);
            this.panelLayout3.MouseEnter += new System.EventHandler(this.panelLayout3_MouseEnter);
            this.panelLayout3.MouseLeave += new System.EventHandler(this.panelLayout3_MouseLeave);
            // 
            // panelLayout4
            // 
            this.panelLayout4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panelLayout4.Location = new System.Drawing.Point(251, 251);
            this.panelLayout4.Name = "panelLayout4";
            this.panelLayout4.Size = new System.Drawing.Size(249, 249);
            this.panelLayout4.TabIndex = 3;
            this.panelLayout4.SizeChanged += new System.EventHandler(this.panelLayout4_SizeChanged);
            this.panelLayout4.MouseEnter += new System.EventHandler(this.panelLayout4_MouseEnter);
            this.panelLayout4.MouseLeave += new System.EventHandler(this.panelLayout4_MouseLeave);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.panelLayout4);
            this.Controls.Add(this.panelLayout3);
            this.Controls.Add(this.panelLayout2);
            this.Controls.Add(this.panelLayout1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLayout";
            this.Text = "FormLayout1";
            this.Load += new System.EventHandler(this.FormLayout_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLayout_MouseDown);
            this.Resize += new System.EventHandler(this.FormLayout_Resize);
            this.panelLayout1.ResumeLayout(false);
            this.panelLayout1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLayout1;
        private System.Windows.Forms.Panel panelLayout2;
        private System.Windows.Forms.Panel panelLayout3;
        private System.Windows.Forms.Panel panelLayout4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
    }
}