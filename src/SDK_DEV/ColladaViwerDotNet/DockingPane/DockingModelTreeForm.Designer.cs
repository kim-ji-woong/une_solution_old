namespace UBMLViewer
{
    partial class DockingModelTreeForm
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
            this.m_TreeView = new System.Windows.Forms.TreeView();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // m_TreeView
            // 
            this.m_TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_TreeView.Location = new System.Drawing.Point(0, 0);
            this.m_TreeView.Name = "m_TreeView";
            this.m_TreeView.Size = new System.Drawing.Size(302, 470);
            this.m_TreeView.StateImageList = this.mImageList;
            this.m_TreeView.TabIndex = 0;
            this.m_TreeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.ModelTreeView_BeforeCheck);
            this.m_TreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ModelTreeView_AfterCheck);
            this.m_TreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.ModelTreeView_BeforeSelect);
            this.m_TreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ModelTreeView_AfterSelect);
            this.m_TreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ModelTreeView_NodeMouseClick);
            this.m_TreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ModelTreeView_NodeMouseDoubleClick);
            // 
            // mImageList
            // 
            this.mImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.mImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // DockingModelTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 470);
            this.Controls.Add(this.m_TreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingModelTreeForm";
            this.Text = "DockingModelTreeForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView m_TreeView;
        private System.Windows.Forms.ImageList mImageList;
        
        
    }
}