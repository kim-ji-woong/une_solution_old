namespace SOPMonitoringSystem
{
    partial class FormLeftScenario
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
            this.dataGridScenario = new System.Windows.Forms.DataGridView();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rButtonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridScenario)).BeginInit();
            this.rButtonMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridScenario
            // 
            this.dataGridScenario.AllowUserToAddRows = false;
            this.dataGridScenario.AllowUserToDeleteRows = false;
            this.dataGridScenario.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridScenario.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridScenario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridScenario.ColumnHeadersVisible = false;
            this.dataGridScenario.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.path});
            this.dataGridScenario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridScenario.Location = new System.Drawing.Point(0, 0);
            this.dataGridScenario.MultiSelect = false;
            this.dataGridScenario.Name = "dataGridScenario";
            this.dataGridScenario.ReadOnly = true;
            this.dataGridScenario.RowHeadersVisible = false;
            this.dataGridScenario.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridScenario.RowTemplate.Height = 23;
            this.dataGridScenario.Size = new System.Drawing.Size(284, 262);
            this.dataGridScenario.TabIndex = 0;
            this.dataGridScenario.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridScenario_CellMouseClick);
            this.dataGridScenario.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridScenario_KeyDown);
            // 
            // path
            // 
            this.path.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.path.HeaderText = "Column1";
            this.path.Name = "path";
            this.path.ReadOnly = true;
            this.path.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.path.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // rButtonMenu
            // 
            this.rButtonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteMenuItem});
            this.rButtonMenu.Name = "rButtonMenu";
            this.rButtonMenu.Size = new System.Drawing.Size(99, 26);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(98, 22);
            this.deleteMenuItem.Text = "삭제";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // FormLeftScenario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.dataGridScenario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLeftScenario";
            this.Text = "운용 중 시나리오";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridScenario)).EndInit();
            this.rButtonMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridScenario;
        private System.Windows.Forms.DataGridViewTextBoxColumn path;
        private System.Windows.Forms.ContextMenuStrip rButtonMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
    }
}