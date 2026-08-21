namespace HSMS
{
    partial class FormVirtualCCTV
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVirtualCCTV));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.CCTVCrtl = new CCTVViewer.CCTVCrtl(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CCTVCrtl)).BeginInit();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // CCTVCrtl
            // 
            this.CCTVCrtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CCTVCrtl.Location = new System.Drawing.Point(0, 0);
            this.CCTVCrtl.Name = "CCTVCrtl";
            this.CCTVCrtl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("CCTVCrtl.OcxState")));
            this.CCTVCrtl.Size = new System.Drawing.Size(624, 416);
            this.CCTVCrtl.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Font = new System.Drawing.Font("돋움", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 23);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Disconnection";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Visible = false;
            // 
            // FormVirtualCCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 416);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.CCTVCrtl);
            this.Name = "FormVirtualCCTV";
            this.ShowIcon = false;
            this.Text = "FormVirtualCCTV";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVirtualCCTV_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormVirtualCCTV_FormClosed);
            this.Load += new System.EventHandler(this.FormVirtualCCTV_Load);
            this.Resize += new System.EventHandler(this.FormVirtualCCTV_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.CCTVCrtl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CCTVViewer.CCTVCrtl CCTVCrtl;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblStatus;
    }
}