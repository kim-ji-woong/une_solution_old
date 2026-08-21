namespace SOPGen
{
    partial class FormTeam
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
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.contextCheckMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.VisibleCheckBoxMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.contextCheckMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewTeam.Location = new System.Drawing.Point(0, 0);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(284, 262);
            this.treeViewTeam.TabIndex = 0;
            this.treeViewTeam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewTeam_MouseDown);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(132, 268);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(73, 28);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "선택";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(211, 268);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // contextCheckMenu
            // 
            this.contextCheckMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VisibleCheckBoxMenu});
            this.contextCheckMenu.Name = "contextCheckMenu";
            this.contextCheckMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // VisibleCheckBoxMenu
            // 
            this.VisibleCheckBoxMenu.Name = "VisibleCheckBoxMenu";
            this.VisibleCheckBoxMenu.Size = new System.Drawing.Size(152, 22);
            this.VisibleCheckBoxMenu.Text = "CheckBox";
            this.VisibleCheckBoxMenu.Click += new System.EventHandler(this.VisibleCheckBoxMenu_Click);
            // 
            // FormTeam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 298);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.treeViewTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormTeam";
            this.ShowInTaskbar = false;
            this.Text = "FormTeam";
            this.Load += new System.EventHandler(this.FormTeam_Load);
            this.contextCheckMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip contextCheckMenu;
        private System.Windows.Forms.ToolStripMenuItem VisibleCheckBoxMenu;
    }
}