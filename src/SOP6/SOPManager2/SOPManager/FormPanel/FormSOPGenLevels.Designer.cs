namespace SOPManager.FormPanel
{
    partial class FormSOPGenLevels
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
            this.panelLevels = new System.Windows.Forms.Panel();
            this.pbFirstLevel = new System.Windows.Forms.PictureBox();
            this.labelFirstLevelName = new System.Windows.Forms.Label();
            this.panelLevels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFirstLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLevels
            // 
            this.panelLevels.AutoScroll = true;
            this.panelLevels.Controls.Add(this.pbFirstLevel);
            this.panelLevels.Controls.Add(this.labelFirstLevelName);
            this.panelLevels.Location = new System.Drawing.Point(12, 12);
            this.panelLevels.Name = "panelLevels";
            this.panelLevels.Size = new System.Drawing.Size(329, 228);
            this.panelLevels.TabIndex = 0;
            // 
            // pbFirstLevel
            // 
            this.pbFirstLevel.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
            this.pbFirstLevel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbFirstLevel.Location = new System.Drawing.Point(13, 16);
            this.pbFirstLevel.Name = "pbFirstLevel";
            this.pbFirstLevel.Size = new System.Drawing.Size(21, 21);
            this.pbFirstLevel.TabIndex = 99;
            this.pbFirstLevel.TabStop = false;
            this.pbFirstLevel.Visible = false;
            this.pbFirstLevel.Click += new System.EventHandler(this.pbLevel_Click);
            // 
            // labelFirstLevelName
            // 
            this.labelFirstLevelName.AutoSize = true;
            this.labelFirstLevelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFirstLevelName.ForeColor = System.Drawing.Color.White;
            this.labelFirstLevelName.Location = new System.Drawing.Point(35, 18);
            this.labelFirstLevelName.Name = "labelFirstLevelName";
            this.labelFirstLevelName.Size = new System.Drawing.Size(105, 20);
            this.labelFirstLevelName.TabIndex = 100;
            this.labelFirstLevelName.Text = "첫번째 등급 이름";
            this.labelFirstLevelName.Visible = false;
            this.labelFirstLevelName.Click += new System.EventHandler(this.labelLevelName_Click);
            // 
            // FormSOPGenLevels
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(352, 253);
            this.Controls.Add(this.panelLevels);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSOPGenLevels";
            this.Text = "FormSOPGenLevels";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSOPGenLevels_FormClosing);
            this.panelLevels.ResumeLayout(false);
            this.panelLevels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFirstLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLevels;
        private System.Windows.Forms.PictureBox pbFirstLevel;
        private System.Windows.Forms.Label labelFirstLevelName;
    }
}