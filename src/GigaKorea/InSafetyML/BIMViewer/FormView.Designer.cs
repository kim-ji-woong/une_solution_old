namespace BIMViewer
{
    partial class FormView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormView));
            this.panelStatus = new System.Windows.Forms.Panel();
            this.checkBoxFixedPOIScale = new System.Windows.Forms.CheckBox();
            this.labelCoord = new System.Windows.Forms.Label();
            this.panelBody = new BIMViewer.GdiPanel();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.checkBoxFixedPOIScale);
            this.panelStatus.Controls.Add(this.labelCoord);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 428);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(800, 22);
            this.panelStatus.TabIndex = 1;
            this.panelStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelStatus_MouseDown);
            // 
            // checkBoxFixedPOIScale
            // 
            this.checkBoxFixedPOIScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFixedPOIScale.AutoSize = true;
            this.checkBoxFixedPOIScale.Location = new System.Drawing.Point(681, 3);
            this.checkBoxFixedPOIScale.Name = "checkBoxFixedPOIScale";
            this.checkBoxFixedPOIScale.Size = new System.Drawing.Size(115, 16);
            this.checkBoxFixedPOIScale.TabIndex = 1;
            this.checkBoxFixedPOIScale.Text = "Fixed POI Scale";
            this.checkBoxFixedPOIScale.UseVisualStyleBackColor = true;
            this.checkBoxFixedPOIScale.CheckedChanged += new System.EventHandler(this.checkBoxFixedPOIScale_CheckedChanged);
            // 
            // labelCoord
            // 
            this.labelCoord.AutoSize = true;
            this.labelCoord.Location = new System.Drawing.Point(7, 4);
            this.labelCoord.Name = "labelCoord";
            this.labelCoord.Size = new System.Drawing.Size(21, 12);
            this.labelCoord.TabIndex = 0;
            this.labelCoord.Text = "XY";
            // 
            // panelBody
            // 
            this.panelBody.AllowDrop = true;
            this.panelBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(40)))), ((int)(((byte)(48)))));
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.DXFLayers = null;
            this.panelBody.Layers = null;
            this.panelBody.Level = null;
            this.panelBody.Location = new System.Drawing.Point(0, 0);
            this.panelBody.Name = "panelBody";
            this.panelBody.Project = null;
            this.panelBody.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.panelBody.Size = new System.Drawing.Size(800, 450);
            this.panelBody.TabIndex = 0;
            // 

            // FormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panelBody);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormView";
            this.Text = "FormView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormView_FormClosing);
            this.Load += new System.EventHandler(this.FormView_Load);
            this.ResizeEnd += new System.EventHandler(this.FormView_ResizeEnd);
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GdiPanel panelBody;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label labelCoord;
        private System.Windows.Forms.CheckBox checkBoxFixedPOIScale;
    }
}