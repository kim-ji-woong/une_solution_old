namespace SOPMonitoringSystem
{
    partial class PropertiesTransmission
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesTransmission));
            this.axPropertyGrid = new AxXtremePropertyGrid.AxPropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // axPropertyGrid
            // 
            this.axPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axPropertyGrid.Location = new System.Drawing.Point(0, 20);
            this.axPropertyGrid.Name = "axPropertyGrid";
            this.axPropertyGrid.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPropertyGrid.OcxState")));
            this.axPropertyGrid.Size = new System.Drawing.Size(284, 242);
            this.axPropertyGrid.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "상황전파 속성";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PropertiesTransmission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.axPropertyGrid);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PropertiesTransmission";
            this.Text = "PropertiesTransmission";
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremePropertyGrid.AxPropertyGrid axPropertyGrid;
        private System.Windows.Forms.Label label1;
    }
}