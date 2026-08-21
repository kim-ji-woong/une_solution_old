namespace SOPMonitoringSystem
{
    partial class BarLevelTree
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.panelLine = new System.Windows.Forms.Panel();
            this.btnNext = new UnE.GUI.RibbonButton();
            this.btnPrev = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(284, 201);
            this.treeView.TabIndex = 0;
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeSelect);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // panelLine
            // 
            this.panelLine.BackColor = System.Drawing.Color.DimGray;
            this.panelLine.Location = new System.Drawing.Point(0, 203);
            this.panelLine.Name = "panelLine";
            this.panelLine.Size = new System.Drawing.Size(276, 5);
            this.panelLine.TabIndex = 7;
            // 
            // btnNext
            // 
            this.btnNext.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.go;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNext.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.go_click;
            this.btnNext.CheckedImage = null;
            this.btnNext.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnNext.DisabledBkgndImage = global::SOPMonitoringSystem.Properties.Resources.go_disable;
            this.btnNext.DisabledImage = null;
            this.btnNext.ID = -1;
            this.btnNext.InitButtonWidth = 60;
            this.btnNext.IsChecked = false;
            this.btnNext.Location = new System.Drawing.Point(90, 218);
            this.btnNext.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.go_over;
            this.btnNext.Name = "btnNext";
            this.btnNext.NormalImage = null;
            this.btnNext.Owner = null;
            this.btnNext.Size = new System.Drawing.Size(60, 31);
            this.btnNext.TabIndex = 6;
            this.btnNext.TextLocation = new System.Drawing.Point(0, 0);
            this.btnNext.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnNext.UseCustomImageRect = false;
            this.btnNext.UseTextLocation = false;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnPrevNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.back;
            this.btnPrev.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrev.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.back_click;
            this.btnPrev.CheckedImage = null;
            this.btnPrev.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnPrev.DisabledBkgndImage = global::SOPMonitoringSystem.Properties.Resources.back_disable;
            this.btnPrev.DisabledImage = null;
            this.btnPrev.ID = -1;
            this.btnPrev.InitButtonWidth = 60;
            this.btnPrev.IsChecked = false;
            this.btnPrev.Location = new System.Drawing.Point(15, 218);
            this.btnPrev.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.back_over;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.NormalImage = null;
            this.btnPrev.Owner = null;
            this.btnPrev.Size = new System.Drawing.Size(60, 31);
            this.btnPrev.TabIndex = 5;
            this.btnPrev.TextLocation = new System.Drawing.Point(0, 0);
            this.btnPrev.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnPrev.UseCustomImageRect = false;
            this.btnPrev.UseTextLocation = false;
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrevNext_Click);
            // 
            // BarLevelTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panelLine);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.treeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BarLevelTree";
            this.Text = "단계 Tree";
            this.Resize += new System.EventHandler(this.BarLevelTree_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panelLine;
        private UnE.GUI.RibbonButton btnNext;
        private UnE.GUI.RibbonButton btnPrev;
        
    }
}