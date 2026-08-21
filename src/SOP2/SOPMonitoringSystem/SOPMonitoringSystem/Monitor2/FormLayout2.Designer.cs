namespace SOPDisasterSystem
{
    partial class FormLayout2
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setDisasterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseDisasterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLayout4 = new SOPDisasterSystem.VirtoolPanel();
            this.panelLayout3 = new SOPDisasterSystem.VirtoolPanel();
            this.panelLayout2 = new SOPDisasterSystem.VirtoolPanel();
            this.panelLayout1 = new SOPDisasterSystem.VirtoolPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.panelLayout1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setDisasterToolStripMenuItem,
            this.releaseDisasterToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 70);
            // 
            // setDisasterToolStripMenuItem
            // 
            this.setDisasterToolStripMenuItem.Name = "setDisasterToolStripMenuItem";
            this.setDisasterToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.setDisasterToolStripMenuItem.Text = "재난위치 지정";
            this.setDisasterToolStripMenuItem.Click += new System.EventHandler(this.setDisasterToolStripMenuItem_Click);
            // 
            // releaseDisasterToolStripMenuItem
            // 
            this.releaseDisasterToolStripMenuItem.Name = "releaseDisasterToolStripMenuItem";
            this.releaseDisasterToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.releaseDisasterToolStripMenuItem.Text = "재난위치 해지";
            this.releaseDisasterToolStripMenuItem.Click += new System.EventHandler(this.releaseDisasterToolStripMenuItem_Click);
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
            this.Resize += new System.EventHandler(this.FormLayout_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panelLayout1.ResumeLayout(false);
            this.panelLayout1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        VirtoolPanel panelLayout1;
        VirtoolPanel panelLayout2;
        VirtoolPanel panelLayout3;
        VirtoolPanel panelLayout4;
        //private System.Windows.Forms.Panel panelLayout1;
        //private System.Windows.Forms.Panel panelLayout2;
        //private System.Windows.Forms.Panel panelLayout3;
        //private System.Windows.Forms.Panel panelLayout4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setDisasterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem releaseDisasterToolStripMenuItem;
    }
}