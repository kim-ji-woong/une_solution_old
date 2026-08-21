namespace SOPManager
{
    partial class PopupPreviewMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupPreviewMessage));
            this.textBoxPreview = new System.Windows.Forms.TextBox();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioIgnoreDay = new System.Windows.Forms.RadioButton();
            this.radioDay = new System.Windows.Forms.RadioButton();
            this.radioNight = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioIgnoreVirtual = new System.Windows.Forms.RadioButton();
            this.radioReal = new System.Windows.Forms.RadioButton();
            this.radioVirtual = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.panelBackground.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxPreview
            // 
            this.textBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPreview.BackColor = System.Drawing.Color.White;
            this.textBoxPreview.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.textBoxPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPreview.Multiline = true;
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ReadOnly = true;
            this.textBoxPreview.Size = new System.Drawing.Size(516, 240);
            this.textBoxPreview.TabIndex = 18;
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelBackground.Controls.Add(this.panel2);
            this.panelBackground.Controls.Add(this.panel1);
            this.panelBackground.Controls.Add(this.btnOK);
            this.panelBackground.Controls.Add(this.textBoxPreview);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(516, 345);
            this.panelBackground.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.Controls.Add(this.radioIgnoreDay);
            this.panel2.Controls.Add(this.radioDay);
            this.panel2.Controls.Add(this.radioNight);
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Location = new System.Drawing.Point(121, 248);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(141, 90);
            this.panel2.TabIndex = 21;
            // 
            // radioIgnoreDay
            // 
            this.radioIgnoreDay.AutoSize = true;
            this.radioIgnoreDay.Checked = true;
            this.radioIgnoreDay.Location = new System.Drawing.Point(0, 8);
            this.radioIgnoreDay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioIgnoreDay.Name = "radioIgnoreDay";
            this.radioIgnoreDay.Size = new System.Drawing.Size(49, 19);
            this.radioIgnoreDay.TabIndex = 20;
            this.radioIgnoreDay.TabStop = true;
            this.radioIgnoreDay.Text = "무시";
            this.radioIgnoreDay.UseVisualStyleBackColor = true;
            this.radioIgnoreDay.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioDay
            // 
            this.radioDay.AutoSize = true;
            this.radioDay.Location = new System.Drawing.Point(0, 36);
            this.radioDay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioDay.Name = "radioDay";
            this.radioDay.Size = new System.Drawing.Size(101, 19);
            this.radioDay.TabIndex = 20;
            this.radioDay.Text = "평일 주간모드";
            this.radioDay.UseVisualStyleBackColor = true;
            this.radioDay.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioNight
            // 
            this.radioNight.AutoSize = true;
            this.radioNight.Location = new System.Drawing.Point(0, 64);
            this.radioNight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioNight.Name = "radioNight";
            this.radioNight.Size = new System.Drawing.Size(117, 19);
            this.radioNight.TabIndex = 20;
            this.radioNight.Text = "야간 및 휴일모드";
            this.radioNight.UseVisualStyleBackColor = true;
            this.radioNight.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.radioIgnoreVirtual);
            this.panel1.Controls.Add(this.radioReal);
            this.panel1.Controls.Add(this.radioVirtual);
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel1.Location = new System.Drawing.Point(11, 248);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(104, 90);
            this.panel1.TabIndex = 21;
            // 
            // radioIgnoreVirtual
            // 
            this.radioIgnoreVirtual.AutoSize = true;
            this.radioIgnoreVirtual.Location = new System.Drawing.Point(0, 8);
            this.radioIgnoreVirtual.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioIgnoreVirtual.Name = "radioIgnoreVirtual";
            this.radioIgnoreVirtual.Size = new System.Drawing.Size(49, 19);
            this.radioIgnoreVirtual.TabIndex = 20;
            this.radioIgnoreVirtual.Text = "무시";
            this.radioIgnoreVirtual.UseVisualStyleBackColor = true;
            this.radioIgnoreVirtual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioReal
            // 
            this.radioReal.AutoSize = true;
            this.radioReal.Location = new System.Drawing.Point(0, 64);
            this.radioReal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioReal.Name = "radioReal";
            this.radioReal.Size = new System.Drawing.Size(73, 19);
            this.radioReal.TabIndex = 20;
            this.radioReal.Text = "실제모드";
            this.radioReal.UseVisualStyleBackColor = true;
            this.radioReal.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioVirtual
            // 
            this.radioVirtual.AutoSize = true;
            this.radioVirtual.Checked = true;
            this.radioVirtual.Location = new System.Drawing.Point(0, 36);
            this.radioVirtual.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioVirtual.Name = "radioVirtual";
            this.radioVirtual.Size = new System.Drawing.Size(73, 19);
            this.radioVirtual.TabIndex = 20;
            this.radioVirtual.TabStop = true;
            this.radioVirtual.Text = "훈련모드";
            this.radioVirtual.UseVisualStyleBackColor = true;
            this.radioVirtual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnOK.Location = new System.Drawing.Point(390, 298);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(108, 29);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupPreviewMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(516, 345);
            this.Controls.Add(this.panelBackground);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(516, 345);
            this.Name = "PopupPreviewMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "방송메세지 미리보기";
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TextBox textBoxPreview;
        private System.Windows.Forms.Panel panelBackground;
		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton radioVirtual;
        private System.Windows.Forms.RadioButton radioIgnoreVirtual;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioIgnoreDay;
        private System.Windows.Forms.RadioButton radioDay;
        private System.Windows.Forms.RadioButton radioNight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioReal;
    }
}