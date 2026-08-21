namespace SOPMonitoringSystem
{
    partial class DockingLeftScenario
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockingLeftScenario));
            this.rButtonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AllStopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AllDelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridScenario = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewImageColumn();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.axShortcutBar = new AxXtremeShortcutBar.AxShortcutBar();
            this.rButtonMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridScenario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axShortcutBar)).BeginInit();
            this.SuspendLayout();
            // 
            // rButtonMenu
            // 
            this.rButtonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteMenuItem,
            this.stopMenuItem,
            this.AllStopMenuItem,
            this.AllDelMenuItem});
            this.rButtonMenu.Name = "rButtonMenu";
            this.rButtonMenu.Size = new System.Drawing.Size(147, 92);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Enabled = false;
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(146, 22);
            this.deleteMenuItem.Text = "삭제";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // stopMenuItem
            // 
            this.stopMenuItem.Name = "stopMenuItem";
            this.stopMenuItem.Size = new System.Drawing.Size(146, 22);
            this.stopMenuItem.Text = "실행취소";
            this.stopMenuItem.Click += new System.EventHandler(this.stopMenuItem_Click);
            // 
            // AllStopMenuItem
            // 
            this.AllStopMenuItem.Name = "AllStopMenuItem";
            this.AllStopMenuItem.Size = new System.Drawing.Size(146, 22);
            this.AllStopMenuItem.Text = "모두실행취소";
            this.AllStopMenuItem.Click += new System.EventHandler(this.AllStopMenuItem_Click);
            // 
            // AllDelMenuItem
            // 
            this.AllDelMenuItem.Enabled = false;
            this.AllDelMenuItem.Name = "AllDelMenuItem";
            this.AllDelMenuItem.Size = new System.Drawing.Size(146, 22);
            this.AllDelMenuItem.Text = "모두삭제";
            this.AllDelMenuItem.Click += new System.EventHandler(this.AllDelMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.splitContainer1.Panel1.Controls.Add(this.dataGridScenario);
            this.splitContainer1.Panel1MinSize = 70;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.splitContainer1.Panel2.Controls.Add(this.axShortcutBar);
            this.splitContainer1.Panel2MinSize = 130;
            this.splitContainer1.Size = new System.Drawing.Size(284, 397);
            this.splitContainer1.SplitterDistance = 196;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 1;
            // 
            // dataGridScenario
            // 
            this.dataGridScenario.AllowUserToAddRows = false;
            this.dataGridScenario.AllowUserToDeleteRows = false;
            this.dataGridScenario.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridScenario.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridScenario.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridScenario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridScenario.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.path});
            this.dataGridScenario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridScenario.Location = new System.Drawing.Point(0, 0);
            this.dataGridScenario.MultiSelect = false;
            this.dataGridScenario.Name = "dataGridScenario";
            this.dataGridScenario.ReadOnly = true;
            this.dataGridScenario.RowHeadersVisible = false;
            this.dataGridScenario.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridScenario.RowTemplate.Height = 23;
            this.dataGridScenario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridScenario.Size = new System.Drawing.Size(284, 196);
            this.dataGridScenario.TabIndex = 1;
            this.dataGridScenario.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridScenario_CellMouseClick);
            this.dataGridScenario.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridScenario_KeyDown);
            this.dataGridScenario.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridScenario_MouseDown);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "실제";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Width = 25;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "등록";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column2.Width = 25;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "평일";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column3.Width = 25;
            // 
            // path
            // 
            this.path.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.path.DefaultCellStyle = dataGridViewCellStyle2;
            this.path.HeaderText = "시나리오";
            this.path.Name = "path";
            this.path.ReadOnly = true;
            this.path.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.path.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // axShortcutBar
            // 
            this.axShortcutBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axShortcutBar.Location = new System.Drawing.Point(0, 0);
            this.axShortcutBar.Name = "axShortcutBar";
            this.axShortcutBar.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axShortcutBar.OcxState")));
            this.axShortcutBar.Size = new System.Drawing.Size(284, 199);
            this.axShortcutBar.TabIndex = 0;
            // 
            // DockingLeftScenario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 397);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingLeftScenario";
            this.Text = "운용 중 시나리오";
            this.rButtonMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridScenario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axShortcutBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip rButtonMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridScenario;
        private AxXtremeShortcutBar.AxShortcutBar axShortcutBar;
        private System.Windows.Forms.DataGridViewImageColumn Column1;
        private System.Windows.Forms.DataGridViewImageColumn Column2;
        private System.Windows.Forms.DataGridViewImageColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn path;
        private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AllStopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AllDelMenuItem;
    }
}