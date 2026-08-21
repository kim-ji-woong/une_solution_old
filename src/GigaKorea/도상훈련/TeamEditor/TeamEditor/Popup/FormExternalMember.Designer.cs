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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
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
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.gridExternalCompanyMember.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridExternalCompanyMember.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridExternalCompanyMember.BackgroundColor = System.Drawing.Color.White;
            this.gridExternalCompanyMember.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridExternalCompanyMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridExternalCompanyMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridExternalCompanyMember.ColumnHeadersHeight = 40;
            this.gridExternalCompanyMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridExternalCompanyMember.CurrentTeam = null;
            this.gridExternalCompanyMember.CurrentTeamRow = null;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridExternalCompanyMember.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridExternalCompanyMember.GridColor = System.Drawing.Color.Black;
            this.gridExternalCompanyMember.groupPosition = null;
            this.gridExternalCompanyMember.LinkedTree = null;
            this.gridExternalCompanyMember.Location = new System.Drawing.Point(301, 31);
            this.gridExternalCompanyMember.MultiSelect = false;
            this.gridExternalCompanyMember.Name = "gridExternalCompanyMember";
            this.gridExternalCompanyMember.NoSort = false;
            this.gridExternalCompanyMember.Owner = null;
            this.gridExternalCompanyMember.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridExternalCompanyMember.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridExternalCompanyMember.RowHeadersVisible = false;
            this.gridExternalCompanyMember.RowHeight = 35;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.gridExternalCompanyMember.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.gridExternalCompanyMember.RowTemplate.Height = 23;
            this.gridExternalCompanyMember.Size = new System.Drawing.Size(450, 388);
            this.gridExternalCompanyMember.TabIndex = 3;
            this.gridExternalCompanyMember.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            this.gridExternalCompanyMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridExternalCompanyMember_CellClick);
            this.gridExternalCompanyMember.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridExternalCompanyMember_CellPainting);
            // 
            // treeExternalCompanyTeam
            // 
            this.treeExternalCompanyTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeExternalCompanyTeam.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeExternalCompanyTeam.Location = new System.Drawing.Point(3, 3);
            this.treeExternalCompanyTeam.Name = "treeExternalCompanyTeam";
            this.treeExternalCompanyTeam.Size = new System.Drawing.Size(292, 416);
            this.treeExternalCompanyTeam.TabIndex = 2;
            this.treeExternalCompanyTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeExternalCompanyTeam_AfterSelect);
            // 
            // lblTeamPath
            // 
            this.lblTeamPath.AutoSize = true;
            this.lblTeamPath.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPath.Location = new System.Drawing.Point(305, 6);
            this.lblTeamPath.Name = "lblTeamPath";
            this.lblTeamPath.Size = new System.Drawing.Size(45, 21);
            this.lblTeamPath.TabIndex = 4;
            this.lblTeamPath.Text = "       ";
            // 
            // FormExternalMember
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
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