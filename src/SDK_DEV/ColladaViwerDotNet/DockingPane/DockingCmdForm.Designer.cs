namespace UBMLViewer
{
    partial class DockingCmdForm
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
            this.m_HistoryListBox = new System.Windows.Forms.ListBox();
            this.m_CmdTextBox = new System.Windows.Forms.TextBox();
            this.m_LoggerTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // m_HistoryListBox
            // 
            this.m_HistoryListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_HistoryListBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.m_HistoryListBox.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.m_HistoryListBox.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.m_HistoryListBox.FormattingEnabled = true;
            this.m_HistoryListBox.ItemHeight = 12;
            this.m_HistoryListBox.Location = new System.Drawing.Point(2, 0);
            this.m_HistoryListBox.Name = "m_HistoryListBox";
            this.m_HistoryListBox.Size = new System.Drawing.Size(651, 172);
            this.m_HistoryListBox.TabIndex = 0;
            this.m_HistoryListBox.SelectedValueChanged += new System.EventHandler(this.m_HistoryListBox_SelectedValueChanged);
            this.m_HistoryListBox.DoubleClick += new System.EventHandler(this.HistoryListBox_DoubleClick);
            this.m_HistoryListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_HistoryListBox_MouseDoubleClick);
            // 
            // m_CmdTextBox
            // 
            this.m_CmdTextBox.AcceptsTab = true;
            this.m_CmdTextBox.AllowDrop = true;
            this.m_CmdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_CmdTextBox.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.m_CmdTextBox.Location = new System.Drawing.Point(2, 171);
            this.m_CmdTextBox.Name = "m_CmdTextBox";
            this.m_CmdTextBox.Size = new System.Drawing.Size(651, 21);
            this.m_CmdTextBox.TabIndex = 1;
            this.m_CmdTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CommandText_KeyDown);
            // 
            // m_LoggerTimer
            // 
            this.m_LoggerTimer.Interval = 500;
            this.m_LoggerTimer.Tick += new System.EventHandler(this.LoggerTimer_Tick);
            // 
            // DockingCmdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 194);
            this.Controls.Add(this.m_CmdTextBox);
            this.Controls.Add(this.m_HistoryListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingCmdForm";
            this.Text = "DockingLogForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DockingCmdForm_FormClosing);
            this.SizeChanged += new System.EventHandler(this.DockingLogForm_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_HistoryListBox;
        private System.Windows.Forms.TextBox m_CmdTextBox;
        private System.Windows.Forms.Timer m_LoggerTimer;
        

    }
}