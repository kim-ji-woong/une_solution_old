namespace FireManagement
{
    partial class FormPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPanel));
            this.dxfControl1 = new DXFViewer.DXFControl();
            this.axDockingPane = new AxXtremeDockingPane.AxDockingPane();
            this.labelZoneName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).BeginInit();
            this.SuspendLayout();
            // 
            // dxfControl1
            // 
            this.dxfControl1.BackColor = System.Drawing.Color.Black;
            this.dxfControl1.Location = new System.Drawing.Point(131, 98);
            this.dxfControl1.Name = "dxfControl1";
            this.dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl1.Size = new System.Drawing.Size(150, 150);
            this.dxfControl1.TabIndex = 1;
            this.dxfControl1.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl1.UseMouseWheel = true;
            this.dxfControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dxfControl1_KeyDown);
            this.dxfControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseDown);
            this.dxfControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dxfControl1_MouseMove);
            // 
            // axDockingPane
            // 
            this.axDockingPane.Enabled = true;
            this.axDockingPane.Location = new System.Drawing.Point(12, 12);
            this.axDockingPane.Name = "axDockingPane";
            this.axDockingPane.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axDockingPane.OcxState")));
            this.axDockingPane.Size = new System.Drawing.Size(24, 24);
            this.axDockingPane.TabIndex = 0;
            this.axDockingPane.AttachPaneEvent += new AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEventHandler(this.axDockingPane_AttachPaneEvent);
            // 
            // labelZoneName
            // 
            this.labelZoneName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.labelZoneName.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelZoneName.ForeColor = System.Drawing.Color.Black;
            this.labelZoneName.Location = new System.Drawing.Point(91, 27);
            this.labelZoneName.Name = "labelZoneName";
            this.labelZoneName.Size = new System.Drawing.Size(86, 41);
            this.labelZoneName.TabIndex = 2;
            this.labelZoneName.Text = "label1";
            this.labelZoneName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.labelZoneName);
            this.Controls.Add(this.dxfControl1);
            this.Controls.Add(this.axDockingPane);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPanel";
            this.Text = "FormPanel";
            this.Resize += new System.EventHandler(this.FormPanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.axDockingPane)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremeDockingPane.AxDockingPane axDockingPane;
        private DXFViewer.DXFControl dxfControl1;
        private System.Windows.Forms.Label labelZoneName;
    }
}