namespace BIMViewer
{
    partial class FormPOI
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
            this.poiTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // poiTree
            // 
            this.poiTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.poiTree.Location = new System.Drawing.Point(0, 0);
            this.poiTree.Name = "poiTree";
            this.poiTree.Size = new System.Drawing.Size(281, 294);
            this.poiTree.TabIndex = 0;
            this.poiTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.poiTree_MouseDown);
            // 
            // FormPOI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 294);
            this.Controls.Add(this.poiTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPOI";
            this.Text = "POI List";
            this.Load += new System.EventHandler(this.FormPOI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView poiTree;
    }
}