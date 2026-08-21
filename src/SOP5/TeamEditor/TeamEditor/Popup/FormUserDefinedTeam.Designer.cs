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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridUserDefinedTeam = new TeamEditor.TeamGrid();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserDefinedTeam)).BeginInit();
            this.SuspendLayout();
            // 
            // gridUserDefinedTeam
            // 
            this.gridUserDefinedTeam.AllowUserToAddRows = false;
            this.gridUserDefinedTeam.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.gridUserDefinedTeam.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridUserDefinedTeam.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.gridUserDefinedTeam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridUserDefinedTeam.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridUserDefinedTeam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUserDefinedTeam.CurrentTeam = null;
            this.gridUserDefinedTeam.CurrentTeamRow = null;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridUserDefinedTeam.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridUserDefinedTeam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUserDefinedTeam.GridColor = System.Drawing.Color.Black;
            this.gridUserDefinedTeam.groupPosition = null;
            this.gridUserDefinedTeam.LinkedTree = null;
            this.gridUserDefinedTeam.Location = new System.Drawing.Point(0, 0);
            this.gridUserDefinedTeam.MultiSelect = false;
            this.gridUserDefinedTeam.Name = "gridUserDefinedTeam";
            this.gridUserDefinedTeam.NoSort = false;
            this.gridUserDefinedTeam.Owner = null;
            this.gridUserDefinedTeam.ReadOnly = true;
            this.gridUserDefinedTeam.RowHeadersVisible = false;
            this.gridUserDefinedTeam.RowHeight = 35;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.gridUserDefinedTeam.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gridUserDefinedTeam.RowTemplate.Height = 23;
            this.gridUserDefinedTeam.Size = new System.Drawing.Size(754, 422);
            this.gridUserDefinedTeam.TabIndex = 4;
            this.gridUserDefinedTeam.Type = TeamEditor.TeamGrid.GridType.UserDefinedTeam;
            this.gridUserDefinedTeam.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridUserDefinedTeam_CellPainting);
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