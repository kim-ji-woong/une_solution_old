namespace SDMS
{
    partial class FormReport
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
            this.m_DetectPanel = new System.Windows.Forms.Panel();
            this.m_NotOperationPanel = new System.Windows.Forms.Panel();
            this.m_ActionPanel = new System.Windows.Forms.Panel();
            this.m_SmsPanel = new System.Windows.Forms.Panel();            
            this.SuspendLayout();
            // 
            // m_DetectPanel
            // 
            this.m_DetectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_DetectPanel.Location = new System.Drawing.Point(0, 0);
            this.m_DetectPanel.Name = "m_DetectPanel";
            this.m_DetectPanel.Size = new System.Drawing.Size(761, 422);
            this.m_DetectPanel.TabIndex = 0;
            // 
            // m_NotOperationPanel
            // 
            this.m_NotOperationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_NotOperationPanel.Location = new System.Drawing.Point(0, 0);
            this.m_NotOperationPanel.Name = "m_NotOperationPanel";
            this.m_NotOperationPanel.Size = new System.Drawing.Size(761, 422);
            this.m_NotOperationPanel.TabIndex = 0;
            this.m_NotOperationPanel.Resize += new System.EventHandler(this.m_NotOperationPanel_Resize);
            // 
            // m_ActionPanel
            // 
            this.m_ActionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ActionPanel.Location = new System.Drawing.Point(0, 0);
            this.m_ActionPanel.Name = "m_ActionPanel";
            this.m_ActionPanel.Size = new System.Drawing.Size(761, 422);
            this.m_ActionPanel.TabIndex = 0;

            this.m_SmsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_SmsPanel.Location = new System.Drawing.Point(0, 0);
            this.m_SmsPanel.Name = "m_ActionPanel";
            this.m_SmsPanel.Size = new System.Drawing.Size(761, 422);
            this.m_SmsPanel.TabIndex = 0;
            // 
            // FormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 422);
            this.Controls.Add(this.m_NotOperationPanel);
            this.Controls.Add(this.m_ActionPanel);
            this.Controls.Add(this.m_DetectPanel);
            this.Controls.Add(this.m_SmsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormReport";
            this.Text = "FormReport";
            this.VisibleChanged += new System.EventHandler(this.FormReport_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel m_DetectPanel;
        private System.Windows.Forms.Panel m_NotOperationPanel;
        private System.Windows.Forms.Panel m_ActionPanel;
        private System.Windows.Forms.Panel m_SmsPanel;
    }
}