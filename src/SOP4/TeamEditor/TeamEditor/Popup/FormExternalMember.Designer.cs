namespace TeamEditor.Popup
{
    partial class FormExternalMember
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
            this.gridExternalCompanyMember = new TeamEditor.TeamGrid();
            this.treeExternalCompanyTeam = new System.Windows.Forms.TreeView();
            this.lblTeamPath = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridExternalCompanyMember)).BeginInit();
            this.SuspendLayout();
            // 
            // gridExternalCompanyMember
            // 
            this.gridExternalCompanyMember.AllowUserToAddRows = false;
            this.gridExternalCompanyMember.AllowUserToDeleteRows = false;
            this.gridExternalCompanyMember.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridExternalCompanyMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridExternalCompanyMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridExternalCompanyMember.CurrentTeam = null;
            this.gridExternalCompanyMember.groupPosition = null;
            this.gridExternalCompanyMember.LinkedTree = null;
            this.gridExternalCompanyMember.Location = new System.Drawing.Point(301, 27);
            this.gridExternalCompanyMember.MultiSelect = false;
            this.gridExternalCompanyMember.Name = "gridExternalCompanyMember";
            this.gridExternalCompanyMember.NoSort = false;
            this.gridExternalCompanyMember.ReadOnly = true;
            this.gridExternalCompanyMember.RowHeadersVisible = false;
            this.gridExternalCompanyMember.RowTemplate.Height = 23;
            this.gridExternalCompanyMember.Size = new System.Drawing.Size(450, 392);
            this.gridExternalCompanyMember.TabIndex = 3;
            this.gridExternalCompanyMember.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            this.gridExternalCompanyMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridExternalCompanyMember_CellClick);
            // 
            // treeExternalCompanyTeam
            // 
            this.treeExternalCompanyTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeExternalCompanyTeam.Location = new System.Drawing.Point(3, 3);
            this.treeExternalCompanyTeam.Name = "treeExternalCompanyTeam";
            this.treeExternalCompanyTeam.Size = new System.Drawing.Size(292, 416);
            this.treeExternalCompanyTeam.TabIndex = 2;
            this.treeExternalCompanyTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeExternalCompanyTeam_AfterSelect);
            // 
            // lblTeamPath
            // 
            this.lblTeamPath.AutoSize = true;
            this.lblTeamPath.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPath.Location = new System.Drawing.Point(301, 12);
            this.lblTeamPath.Name = "lblTeamPath";
            this.lblTeamPath.Size = new System.Drawing.Size(40, 12);
            this.lblTeamPath.TabIndex = 4;
            this.lblTeamPath.Text = "       ";
            // 
            // FormExternalMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 422);
            this.Controls.Add(this.lblTeamPath);
            this.Controls.Add(this.gridExternalCompanyMember);
            this.Controls.Add(this.treeExternalCompanyTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormExternalMember";
            this.Text = "FormExternalMember";
            ((System.ComponentModel.ISupportInitialize)(this.gridExternalCompanyMember)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TeamGrid gridExternalCompanyMember;
        private System.Windows.Forms.TreeView treeExternalCompanyTeam;
        private System.Windows.Forms.Label lblTeamPath;
    }
}