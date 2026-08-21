namespace SOPManager
{
    partial class PopupTransparentMessage
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
            this.panelWhiteBoard = new System.Windows.Forms.Panel();
            this.labelMessage2 = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.panelWhiteBoard.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWhiteBoard
            // 
            this.panelWhiteBoard.BackColor = System.Drawing.Color.White;
            this.panelWhiteBoard.Controls.Add(this.labelMessage2);
            this.panelWhiteBoard.Controls.Add(this.labelMessage);
            this.panelWhiteBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWhiteBoard.Location = new System.Drawing.Point(0, 0);
            this.panelWhiteBoard.Name = "panelWhiteBoard";
            this.panelWhiteBoard.Size = new System.Drawing.Size(350, 74);
            this.panelWhiteBoard.TabIndex = 0;
            // 
            // labelMessage2
            // 
            this.labelMessage2.AutoSize = true;
            this.labelMessage2.Location = new System.Drawing.Point(14, 41);
            this.labelMessage2.Name = "labelMessage2";
            this.labelMessage2.Size = new System.Drawing.Size(64, 12);
            this.labelMessage2.TabIndex = 0;
            this.labelMessage2.Text = "Message2";
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(14, 22);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(58, 12);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Message";
            // 
            // PopupTransparentMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 74);
            this.Controls.Add(this.panelWhiteBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupTransparentMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PopupTransparentMessage";
            this.TopMost = true;
            this.panelWhiteBoard.ResumeLayout(false);
            this.panelWhiteBoard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWhiteBoard;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label labelMessage2;
    }
}