namespace UBMLViewer
{
    partial class DockingLogForm
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
            this.m_LogTextBox = new System.Windows.Forms.RichTextBox();
            this.m_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_LogTextBox
            // 
            this.m_LogTextBox.BackColor = System.Drawing.Color.Beige;
            this.m_LogTextBox.Location = new System.Drawing.Point(49, 36);
            this.m_LogTextBox.Name = "m_LogTextBox";
            this.m_LogTextBox.ReadOnly = true;
            this.m_LogTextBox.Size = new System.Drawing.Size(540, 96);
            this.m_LogTextBox.TabIndex = 0;
            this.m_LogTextBox.Text = "";
            this.m_LogTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_LogTextBox_MouseDown);
            // 
            // m_ContextMenuStrip
            // 
            this.m_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteAllToolStripMenuItem,
            this.copyAllToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.m_ContextMenuStrip.Name = "m_ContextMenuStrip";
            this.m_ContextMenuStrip.Size = new System.Drawing.Size(153, 92);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteAllToolStripMenuItem.Text = "지우기";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // copyAllToolStripMenuItem
            // 
            this.copyAllToolStripMenuItem.Name = "copyAllToolStripMenuItem";
            this.copyAllToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.copyAllToolStripMenuItem.Text = "모두 복사";
            this.copyAllToolStripMenuItem.Click += new System.EventHandler(this.copyAllToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "파일 저장";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // DockingLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 194);
            this.Controls.Add(this.m_LogTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingLogForm";
            this.Text = "DockingLogForm";
            this.SizeChanged += new System.EventHandler(this.DockingLogForm_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DockingLogForm_MouseDown);
            this.m_ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox m_LogTextBox;
        private System.Windows.Forms.ContextMenuStrip m_ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;



    }
}