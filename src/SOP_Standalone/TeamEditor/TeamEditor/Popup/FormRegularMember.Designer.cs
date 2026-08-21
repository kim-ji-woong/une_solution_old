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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.treeRegularTeam = new System.Windows.Forms.TreeView();
            this.lblTeamPath = new System.Windows.Forms.Label();
            this.gridCompanyMember = new TeamEditor.TeamGrid();
            ((System.ComponentModel.ISupportInitialize)(this.gridCompanyMember)).BeginInit();
            this.SuspendLayout();
            // 
            // treeRegularTeam
            // 
            this.treeRegularTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeRegularTeam.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeRegularTeam.Location = new System.Drawing.Point(3, 3);
            this.treeRegularTeam.Name = "treeRegularTeam";
            this.treeRegularTeam.Size = new System.Drawing.Size(292, 498);
            this.treeRegularTeam.TabIndex = 0;
            this.treeRegularTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeRegularTeam_AfterSelect);
            // 
            // lblTeamPath
            // 
            this.lblTeamPath.AutoSize = true;
            this.lblTeamPath.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTeamPath.Location = new System.Drawing.Point(304, 7);
            this.lblTeamPath.Name = "lblTeamPath";
            this.lblTeamPath.Size = new System.Drawing.Size(45, 21);
            this.lblTeamPath.TabIndex = 5;
            this.lblTeamPath.Text = "       ";
            // 
            // gridCompanyMember
            // 
            this.gridCompanyMember.AllowUserToAddRows = false;
            this.gridCompanyMember.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.gridCompanyMember.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCompanyMember.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCompanyMember.BackgroundColor = System.Drawing.Color.White;
            this.gridCompanyMember.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridCompanyMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCompanyMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridCompanyMember.ColumnHeadersHeight = 40;
            this.gridCompanyMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridCompanyMember.CurrentTeam = null;
            this.gridCompanyMember.CurrentTeamRow = null;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridCompanyMember.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridCompanyMember.GridColor = System.Drawing.Color.Black;
            this.gridCompanyMember.groupPosition = null;
            this.gridCompanyMember.LinkedTree = null;
            this.gridCompanyMember.Location = new System.Drawing.Point(301, 31);
            this.gridCompanyMember.MultiSelect = false;
            this.gridCompanyMember.Name = "gridCompanyMember";
            this.gridCompanyMember.NoSort = false;
            this.gridCompanyMember.Owner = null;
            this.gridCompanyMember.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCompanyMember.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridCompanyMember.RowHeadersVisible = false;
            this.gridCompanyMember.RowHeight = 40;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("나눔스퀘어 Bold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.gridCompanyMember.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.gridCompanyMember.RowTemplate.Height = 23;
            this.gridCompanyMember.Size = new System.Drawing.Size(629, 470);
            this.gridCompanyMember.TabIndex = 1;
            this.gridCompanyMember.Type = TeamEditor.TeamGrid.GridType.RegularMember;
            this.gridCompanyMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCompanyMember_CellClick);
            this.gridCompanyMember.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridCompanyMember_CellPainting);
            // 
            // FormRegularMember
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(932, 504);
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