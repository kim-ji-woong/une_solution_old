namespace libExternalUI.Lib
{
    partial class FormEarthquakeDetection
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConfig = new UnE.GUI.ImageButton();
            this.plRank = new System.Windows.Forms.Panel();
            this.lblConnError = new System.Windows.Forms.Label();
            this.lbData = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfig)).BeginInit();
            this.plRank.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnConfig);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 60);
            this.panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = null;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = null;
            this.btnClose.ImageNormal = null;
            this.btnClose.Location = new System.Drawing.Point(341, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(37, 37);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(15, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "지진감지";
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.btnConfig.ButtonText = "";
            this.btnConfig.ImageClicked = null;
            this.btnConfig.ImageDisabled = null;
            this.btnConfig.ImageMouseOver = null;
            this.btnConfig.ImageNormal = null;
            this.btnConfig.Location = new System.Drawing.Point(284, 13);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Owner = null;
            this.btnConfig.Size = new System.Drawing.Size(37, 38);
            this.btnConfig.TabIndex = 0;
            this.btnConfig.TabStop = false;
            this.btnConfig.TextColor = System.Drawing.Color.Black;
            this.btnConfig.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfig.ToolTipText = "";
            this.btnConfig.UseToolTip = false;
            this.btnConfig.WindowRateWidth = 1F;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // plRank
            // 
            this.plRank.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.plRank.Controls.Add(this.lblConnError);
            this.plRank.Controls.Add(this.lbData);
            this.plRank.Controls.Add(this.label2);
            this.plRank.Location = new System.Drawing.Point(0, 60);
            this.plRank.Name = "plRank";
            this.plRank.Size = new System.Drawing.Size(390, 220);
            this.plRank.TabIndex = 2;
            // 
            // lblConnError
            // 
            this.lblConnError.Font = new System.Drawing.Font("맑은 고딕", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblConnError.ForeColor = System.Drawing.Color.White;
            this.lblConnError.Location = new System.Drawing.Point(85, 31);
            this.lblConnError.Name = "lblConnError";
            this.lblConnError.Size = new System.Drawing.Size(226, 158);
            this.lblConnError.TabIndex = 5;
            this.lblConnError.Text = "신호 없음";
            this.lblConnError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConnError.Visible = false;
            // 
            // lbData
            // 
            this.lbData.Font = new System.Drawing.Font("맑은 고딕", 90F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbData.ForeColor = System.Drawing.Color.White;
            this.lbData.Location = new System.Drawing.Point(87, 56);
            this.lbData.Name = "lbData";
            this.lbData.Size = new System.Drawing.Size(226, 138);
            this.lbData.TabIndex = 3;
            this.lbData.Text = "8";
            this.lbData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(170, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 31);
            this.label2.TabIndex = 2;
            this.label2.Text = "진도";
            // 
            // FormEarthquakeDetection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(390, 280);
            this.Controls.Add(this.plRank);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEarthquakeDetection";
            this.Text = "FormEarthquakeDetection";
            this.Load += new System.EventHandler(this.FormEarthquakeDetection_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfig)).EndInit();
            this.plRank.ResumeLayout(false);
            this.plRank.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton btnConfig;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Panel plRank;
        private System.Windows.Forms.Label lbData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblConnError;
    }
}