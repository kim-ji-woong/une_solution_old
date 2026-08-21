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
            this.axPropertyGrid1 = new AxXtremePropertyGrid.AxPropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // axPropertyGrid1
            // 
            this.axPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.axPropertyGrid1.Name = "axPropertyGrid1";
            this.axPropertyGrid1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPropertyGrid1.OcxState")));
            this.axPropertyGrid1.Size = new System.Drawing.Size(316, 271);
            this.axPropertyGrid1.TabIndex = 0;
            // 
            // DockingNodePropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 271);
            this.Controls.Add(this.axPropertyGrid1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingNodePropertiesForm";
            this.Text = "DockingNodePropertiesForm";
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremePropertyGrid.AxPropertyGrid axPropertyGrid1;
    }
}