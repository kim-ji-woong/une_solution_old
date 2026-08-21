namespace PreSafe
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
            this.labelMessage2 = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.panelWhiteBoard = new System.Windows.Forms.Panel();
            this.panelWhiteBoard.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMessage2
            // 
            this.labelMessage2.AutoSize = true;
            this.labelMessage2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMessage2.Location = new System.Drawing.Point(35, 52);
            this.labelMessage2.Name = "labelMessage2";
            this.labelMessage2.Size = new System.Drawing.Size(60, 15);
            this.labelMessage2.TabIndex = 1;
            this.labelMessage2.Text = "Message2";
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMessage.Location = new System.Drawing.Point(35, 33);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(53, 15);
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "Message";
            // 
            // panelWhiteBoard
            // 
            this.panelWhiteBoard.BackColor = System.Drawing.Color.White;
            this.panelWhiteBoard.Controls.Add(this.labelMessage);
            this.panelWhiteBoard.Controls.Add(this.labelMessage2);
            this.panelWhiteBoard.Location = new System.Drawing.Point(7, 8);
            this.panelWhiteBoard.Name = "panelWhiteBoard";
            this.panelWhiteBoard.Size = new System.Drawing.Size(490, 95);
            this.panelWhiteBoard.TabIndex = 3;
            // 
            // PopupTransparentMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(56)))), ((int)(((byte)(71)))));
            this.ClientSize = new System.Drawing.Size(506, 112);
            this.Controls.Add(this.panelWhiteBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupTransparentMessage";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PopupTransparentMessage";
            this.panelWhiteBoard.ResumeLayout(false);
            this.panelWhiteBoard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMessage2;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Panel panelWhiteBoard;
    }
}