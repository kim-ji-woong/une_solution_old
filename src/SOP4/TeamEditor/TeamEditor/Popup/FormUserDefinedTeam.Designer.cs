namespace TeamEditor.Popup
{
    partial class FormUserDefinedTeam
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
            this.gridUserDefinedTeam = new TeamEditor.TeamGrid();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedTeam)).BeginInit();
            this.SuspendLayout();
            // 
            // gridUserDefinedTeam
            // 
            this.gridUserDefinedTeam.AllowUserToAddRows = false;
            this.gridUserDefinedTeam.AllowUserToDeleteRows = false;
            this.gridUserDefinedTeam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridUserDefinedTeam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUserDefinedTeam.CurrentTeam = null;
            this.gridUserDefinedTeam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUserDefinedTeam.groupPosition = null;
            this.gridUserDefinedTeam.LinkedTree = null;
            this.gridUserDefinedTeam.Location = new System.Drawing.Point(0, 0);
            this.gridUserDefinedTeam.MultiSelect = false;
            this.gridUserDefinedTeam.Name = "gridUserDefinedTeam";
            this.gridUserDefinedTeam.NoSort = false;
            this.gridUserDefinedTeam.ReadOnly = true;
            this.gridUserDefinedTeam.RowHeadersVisible = false;
            this.gridUserDefinedTeam.RowTemplate.Height = 23;
            this.gridUserDefinedTeam.Size = new System.Drawing.Size(754, 422);
            this.gridUserDefinedTeam.TabIndex = 4;
            this.gridUserDefinedTeam.Type = TeamEditor.TeamGrid.GridType.UserDefinedTeam;
            // 
            // FormUserDefinedTeam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 422);
            this.Controls.Add(this.gridUserDefinedTeam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormUserDefinedTeam";
            this.Text = "FormUserDefinedTeam";
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedTeam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TeamGrid gridUserDefinedTeam;

    }
}