namespace UnE.SenarioMaker
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
            this.mSecionTab = new UnE.Controls.TabControlEx();
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
            // mSecionTab
            // 
            resources.ApplyResources(this.mSecionTab, "mSecionTab");
            this.mSecionTab.CloseBtnImage = global::UnE.SenarioMaker.Properties.Resources.CloseWindow_Normal;
            this.mSecionTab.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mSecionTab.HotTrack = true;
            this.mSecionTab.Multiline = true;
            this.mSecionTab.Name = "mSecionTab";
            this.mSecionTab.SelectedIndex = 0;
            this.mSecionTab.SelectedTabColor = System.Drawing.Color.DarkGray;
            this.mSecionTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mSecionTab.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.mSecionTab.TabDisabledForeColor = System.Drawing.Color.DarkGray;
            this.mSecionTab.TabForeColor = System.Drawing.Color.White;
            this.mSecionTab.UseCloseButton = true;
            this.mSecionTab.OnTabPageDeleted += new UnE.Controls.TabPageDeleted(this.mSecionTab_OnTabPageDeleted);
            this.mSecionTab.OnTabPageDeleting += new UnE.Controls.TabPageDeleting(this.mSecionTab_OnTabPageDeleting);
            this.mSecionTab.SelectedIndexChanged += new System.EventHandler(this.mSecionTab_SelectedIndexChanged);
            this.mSecionTab.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.mSecionTab_Selecting);
            // 
            // FormContent
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.Controls.Add(this.mSecionTab);
            this.Controls.Add(this.mSenarioTitle);
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

        
        private System.Windows.Forms.Timer mWheelEndCheckTimer;
        internal System.Windows.Forms.Label mSenarioTitle;
        private UnE.Controls.TabControlEx mSecionTab;

    }
}