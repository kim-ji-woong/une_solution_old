namespace TeamManagementSystem
{
    partial class FormLeftTeamTree
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
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.treeViewEmergency = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.BackColor = System.Drawing.Color.Lavender;
            this.treeViewTeam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTeam.Location = new System.Drawing.Point(0, 0);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(284, 262);
            this.treeViewTeam.TabIndex = 0;
            this.treeViewTeam.Visible = false;
            // 
            // treeViewEmergency
            // 
            this.treeViewEmergency.BackColor = System.Drawing.SystemColors.Info;
            this.treeViewEmergency.Location = new System.Drawing.Point(0, 122);
            this.treeViewEmergency.Name = "treeViewEmergency";
            this.treeViewEmergency.Size = new System.Drawing.Size(284, 140);
            this.treeViewEmergency.TabIndex = 1;
            this.treeViewEmergency.Visible = false;
            // 
            // FormLeftTeamTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.treeViewEmergency);
            this.Controls.Add(this.treeViewTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLeftTeamTree";
            this.Text = "조직체계";
            this.SizeChanged += new System.EventHandler(this.FormLeftTeamTree_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.TreeView treeViewEmergency;
    }
}