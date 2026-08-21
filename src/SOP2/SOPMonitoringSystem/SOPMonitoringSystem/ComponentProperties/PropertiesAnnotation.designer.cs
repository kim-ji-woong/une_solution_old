namespace SOPMonitoringSystem
{
    partial class PropertiesAnnotation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesAnnotation));
            this.label1 = new System.Windows.Forms.Label();
            this.axPropertyGrid = new AxXtremePropertyGrid.AxPropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "설명 속성";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // axPropertyGrid
            // 
            this.axPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axPropertyGrid.Location = new System.Drawing.Point(0, 20);
            this.axPropertyGrid.Name = "axPropertyGrid";
            this.axPropertyGrid.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPropertyGrid.OcxState")));
            this.axPropertyGrid.Size = new System.Drawing.Size(284, 242);
            this.axPropertyGrid.TabIndex = 6;
            //this.axPropertyGrid.ValueChanged += new AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEventHandler(this.axPropertyGrid_ValueChanged);
            //this.axPropertyGrid.InplaceButtonDown += new AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEventHandler(this.axPropertyGrid_InplaceButtonDown);
            // 
            // PropertiesAnnotation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.axPropertyGrid);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PropertiesAnnotation";
            this.Text = "설명";
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private AxXtremePropertyGrid.AxPropertyGrid axPropertyGrid;
    }
}