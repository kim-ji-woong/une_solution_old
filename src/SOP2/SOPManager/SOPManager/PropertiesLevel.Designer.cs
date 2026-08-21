namespace SOPManager
{
    partial class PropertiesLevel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesLevel));
            this.axPropertyGrid = new AxXtremePropertyGrid.AxPropertyGrid();
            this.labelTitle = new System.Windows.Forms.Label();
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
            this.axPropertyGrid.TabIndex = 15;
            this.axPropertyGrid.ValueChanged += new AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEventHandler(this.axPropertyGrid_ValueChanged);
            this.axPropertyGrid.InplaceButtonDown += new AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEventHandler(this.axPropertyGrid_InplaceButtonDown);
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(284, 20);
            this.labelTitle.TabIndex = 14;
            this.labelTitle.Text = "타이틀";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PropertiesLevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.axPropertyGrid);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PropertiesLevel";
            this.Text = "단계속성";
            ((System.ComponentModel.ISupportInitialize)(this.axPropertyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxXtremePropertyGrid.AxPropertyGrid axPropertyGrid;
        private System.Windows.Forms.Label labelTitle;
    }
}