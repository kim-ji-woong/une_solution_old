namespace SOPManager
{
	partial class FormLevelProperties
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.mPropertyGrid = new SOPManager.PropertyGridEx();
            this.panelTop = new System.Windows.Forms.Panel();
            this.mTitle = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.mPropertyGrid);
            this.panel2.Controls.Add(this.panelTop);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 291);
            this.panel2.TabIndex = 0;
            // 
            // mPropertyGrid
            // 
            this.mPropertyGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.mPropertyGrid.CategoryForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.mPropertyGrid.CategorySplitterColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.mPropertyGrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.mPropertyGrid.CommandsForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.mPropertyGrid.DisabledItemForeColor = System.Drawing.SystemColors.ControlDark;
            this.mPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPropertyGrid.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mPropertyGrid.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.mPropertyGrid.HelpForeColor = System.Drawing.Color.White;
            this.mPropertyGrid.LineColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.mPropertyGrid.Location = new System.Drawing.Point(0, 21);
            this.mPropertyGrid.Margin = new System.Windows.Forms.Padding(0);
            this.mPropertyGrid.Name = "mPropertyGrid";
            this.mPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.mPropertyGrid.Size = new System.Drawing.Size(284, 270);
            this.mPropertyGrid.TabIndex = 1;
            this.mPropertyGrid.ToolbarVisible = false;
            this.mPropertyGrid.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelTop.BackgroundImage = global::SOPManager.Properties.Resources.panelTitle;
            this.panelTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelTop.Controls.Add(this.mTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(284, 21);
            this.panelTop.TabIndex = 1;
            // 
            // mTitle
            // 
            this.mTitle.BackColor = System.Drawing.Color.Transparent;
            this.mTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTitle.Font = new System.Drawing.Font("나눔스퀘어", 9.75F, System.Drawing.FontStyle.Bold);
            this.mTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mTitle.Location = new System.Drawing.Point(0, 0);
            this.mTitle.Name = "mTitle";
            this.mTitle.Size = new System.Drawing.Size(284, 21);
            this.mTitle.TabIndex = 0;
            this.mTitle.Text = "  SOP단계 속성";
            this.mTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormLevelProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 291);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLevelProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PropertiesEndPoint";
            this.Load += new System.EventHandler(this.FormProperties_Load);
            this.panel2.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label mTitle;
        internal PropertyGridEx mPropertyGrid;
    }
}