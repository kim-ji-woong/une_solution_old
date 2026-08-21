namespace UBMLViewer
{
    partial class DockingNodePropertiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockingNodePropertiesForm));
            this.m_NodePropertyGrid = new AxXtremePropertyGrid.AxPropertyGrid();
            this.m_MaterialPropertyGrid = new AxXtremePropertyGrid.AxPropertyGrid();
            this.m_ObjectPropertyGrid = new AxXtremePropertyGrid.AxPropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.m_NodePropertyGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_MaterialPropertyGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_ObjectPropertyGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // m_NodePropertyGrid
            // 
            this.m_NodePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_NodePropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.m_NodePropertyGrid.Name = "m_NodePropertyGrid";
            this.m_NodePropertyGrid.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_NodePropertyGrid.OcxState")));
            this.m_NodePropertyGrid.Size = new System.Drawing.Size(316, 271);
            this.m_NodePropertyGrid.TabIndex = 0;
            this.m_NodePropertyGrid.ValueChanged += new AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEventHandler(this.NodePropertyGrid_ValueChanged);
            this.m_NodePropertyGrid.CancelEdit += new AxXtremePropertyGrid._DPropertyGridEvents_CancelEditEventHandler(this.NodePropertyGrid_CancelEdit);
            this.m_NodePropertyGrid.AfterEdit += new AxXtremePropertyGrid._DPropertyGridEvents_AfterEditEventHandler(this.NodePropertyGrid_AfterEdit);
            this.m_NodePropertyGrid.MouseDownEvent += new AxXtremePropertyGrid._DPropertyGridEvents_MouseDownEventHandler(this.NodePropertyGrid_MouseDownEvent);
            // 
            // m_MaterialPropertyGrid
            // 
            this.m_MaterialPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_MaterialPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.m_MaterialPropertyGrid.Name = "m_MaterialPropertyGrid";
            this.m_MaterialPropertyGrid.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_MaterialPropertyGrid.OcxState")));
            this.m_MaterialPropertyGrid.Size = new System.Drawing.Size(316, 271);
            this.m_MaterialPropertyGrid.TabIndex = 0;
            this.m_MaterialPropertyGrid.Visible = false;
            // 
            // m_ObjectPropertyGrid
            // 
            this.m_ObjectPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ObjectPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.m_ObjectPropertyGrid.Name = "m_ObjectPropertyGrid";
            this.m_ObjectPropertyGrid.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_ObjectPropertyGrid.OcxState")));
            this.m_ObjectPropertyGrid.Size = new System.Drawing.Size(316, 271);
            this.m_ObjectPropertyGrid.TabIndex = 0;
            this.m_ObjectPropertyGrid.Visible = false;
            // 
            // DockingNodePropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 271);
            this.Controls.Add(this.m_NodePropertyGrid);
            this.Controls.Add(this.m_MaterialPropertyGrid);
            this.Controls.Add(this.m_ObjectPropertyGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingNodePropertiesForm";
            this.Text = "DockingNodePropertiesForm";
            ((System.ComponentModel.ISupportInitialize)(this.m_NodePropertyGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_MaterialPropertyGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_ObjectPropertyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremePropertyGrid.AxPropertyGrid m_NodePropertyGrid;
        private AxXtremePropertyGrid.AxPropertyGrid m_MaterialPropertyGrid;
        private AxXtremePropertyGrid.AxPropertyGrid m_ObjectPropertyGrid;
    }
}