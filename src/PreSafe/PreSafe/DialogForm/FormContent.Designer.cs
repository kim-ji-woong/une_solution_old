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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormContent));
            this.mWheelEndCheckTimer = new System.Windows.Forms.Timer(this.components);
            this.mSenarioTitle = new System.Windows.Forms.Label();
            this.mSectionPanel = new Sections.PanelSectionEx();
            this.SuspendLayout();
            // 
            // mWheelEndCheckTimer
            // 
            this.mWheelEndCheckTimer.Interval = 1000;
            this.mWheelEndCheckTimer.Tick += new System.EventHandler(this.mWheelEndCheckTimer_Tick);
            // 
            // mSenarioTitle
            // 
            this.mSenarioTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.mSenarioTitle, "mSenarioTitle");
            this.mSenarioTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mSenarioTitle.Name = "mSenarioTitle";
            // 
            // mSectionPanel
            // 
            this.mSectionPanel.ActionStepID = -1;
            resources.ApplyResources(this.mSectionPanel, "mSectionPanel");
            this.mSectionPanel.ArrowSnapOn = true;
            this.mSectionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.mSectionPanel.Collapse = true;
            this.mSectionPanel.Editable = true;
            this.mSectionPanel.IsModified = false;
            this.mSectionPanel.Name = "mSectionPanel";
            this.mSectionPanel.TeamID = 0;
            this.mSectionPanel.TeamName = "";
            this.mSectionPanel.TeamType = 0;
            // 
            // FormContent
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.Controls.Add(this.mSenarioTitle);
            this.Controls.Add(this.mSectionPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormContent";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormContent_FormClosing);
            this.Load += new System.EventHandler(this.FormContent_Load);
            this.Resize += new System.EventHandler(this.FormContent_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        internal Sections.PanelSectionEx mSectionPanel;
        private System.Windows.Forms.Timer mWheelEndCheckTimer;
        internal System.Windows.Forms.Label mSenarioTitle;

    }
}