namespace TeamEditor.Popup
{
    partial class FormRegularMember
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
            this.treeRegularTeam = new System.Windows.Forms.TreeView();
            this.gridCompanyMember = new TeamEditor.TeamGrid();
            this.lblTeamPath = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridCompanyMember)).BeginInit();
            this.SuspendLayout();
            // 
            // treeRegularTeam
            // 
            this.treeRegularTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeRegularTeam.Location = new System.Drawing.Point(3, 3);
            this.treeRegularTeam.Name = "treeRegularTeam";
            this.treeRegularTeam.Size = new System.Drawing.Size(292, 416);
            this.treeRegularTeam.TabIndex = 0;
            this.treeRegularTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeRegularTeam_AfterSelect);
            // 
            // gridCompanyMember
            // 
            this.gridCompanyMember.AllowUserToAddRows = false;
            this.gridCompanyMember.AllowUserToDeleteRows = false;
            this.gridCompanyMember.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCompanyMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridCompanyMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCompanyMember.CurrentTeam = null;
            this.gridCompanyMember.groupPosition = null;
            this.gridCompanyMember.LinkedTree = null;
            this.gridCompanyMember.Location = new System.Drawing.Point(301, 27);
            this.gridCompanyMember.MultiSelect = false;
            this.gridCompanyMember.Name = "gridCompanyMember";
            this.gridCompanyMember.NoSort = false;
            this.gridCompanyMember.ReadOnly = true;
            this.gridCompanyMember.RowHeadersVisible = false;
            this.gridCompanyMember.RowTemplate.Height = 23;
            this.gridCompanyMember.Size = new System.Drawing.Size(450, 392);
            this.gridCompanyMember.TabIndex = 1;
            this.gridCompanyMember.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            this.gridCompanyMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCompanyMember_CellClick);
            // 
            // lblTeamPath
            // 
            this.lblTeamPath.AutoSize = true;
            this.lblTeamPath.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPath.Location = new System.Drawing.Point(301, 12);
            this.lblTeamPath.Name = "lblTeamPath";
            this.lblTeamPath.Size = new System.Drawing.Size(40, 12);
            this.lblTeamPath.TabIndex = 5;
            this.lblTeamPath.Text = "       ";
            // 
            // FormRegularMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 422);
            this.Controls.Add(this.lblTeamPath);
            this.Controls.Add(this.gridCompanyMember);
            this.Controls.Add(this.treeRegularTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRegularMember";
            this.Text = "FormRegularMember";
            ((System.ComponentModel.ISupportInitialize)(this.gridCompanyMember)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeRegularTeam;
        private TeamGrid gridCompanyMember;
        private System.Windows.Forms.Label lblTeamPath;
    }
}