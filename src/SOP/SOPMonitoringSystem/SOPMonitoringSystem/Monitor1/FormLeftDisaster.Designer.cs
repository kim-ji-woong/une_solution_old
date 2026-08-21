namespace SOPMonitoringSystem
{
    partial class FormLeftDisaster
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
            this.treeViewDisaster = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewDisaster
            // 
            this.treeViewDisaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDisaster.Location = new System.Drawing.Point(0, 0);
            this.treeViewDisaster.Name = "treeViewDisaster";
            this.treeViewDisaster.Size = new System.Drawing.Size(284, 262);
            this.treeViewDisaster.TabIndex = 0;
            this.treeViewDisaster.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewDisaster_BeforeSelect);
            this.treeViewDisaster.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDisaster_AfterSelect);
            // 
            // FormLeftDisaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.treeViewDisaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLeftDisaster";
            this.Text = "재난 Tree";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewDisaster;
    }
}