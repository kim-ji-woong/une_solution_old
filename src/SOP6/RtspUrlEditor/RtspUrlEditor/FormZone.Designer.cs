namespace RtspUrlEditor
{
    partial class FormZone
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
            this.treeZones = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeZones
            // 
            this.treeZones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeZones.Location = new System.Drawing.Point(0, 0);
            this.treeZones.Name = "treeZones";
            this.treeZones.Size = new System.Drawing.Size(428, 450);
            this.treeZones.TabIndex = 0;
            this.treeZones.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeZones_AfterSelect);
            // 
            // FormZone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 450);
            this.Controls.Add(this.treeZones);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormZone";
            this.Text = "Zone List";
            this.Load += new System.EventHandler(this.FormZone_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeZones;
    }
}