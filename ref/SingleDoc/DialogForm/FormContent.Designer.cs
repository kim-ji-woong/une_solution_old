namespace PreSafe
{
    partial class FormContent
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
            this.mSectionPanel = new Sections.PanelSectionEx();
            this.SuspendLayout();
            // 
            // mSectionPanel
            // 
            this.mSectionPanel.ActionStepID = -1;
            this.mSectionPanel.ArrowSnapOn = true;
            this.mSectionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.mSectionPanel.Collapse = true;
            this.mSectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSectionPanel.Editable = true;
            this.mSectionPanel.Location = new System.Drawing.Point(0, 0);
            this.mSectionPanel.Name = "mSectionPanel";
            this.mSectionPanel.Size = new System.Drawing.Size(703, 443);
            this.mSectionPanel.TabIndex = 0;
            this.mSectionPanel.TeamID = 0;
            this.mSectionPanel.TeamName = "";
            this.mSectionPanel.TeamType = 0;
            // 
            // FormContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.ClientSize = new System.Drawing.Size(703, 443);
            this.Controls.Add(this.mSectionPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormContent";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormContent";
            this.ResumeLayout(false);

        }

        #endregion

        internal Sections.PanelSectionEx mSectionPanel;

    }
}