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
            this.panelTitleBar = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
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
            this.panelTitleBar.SuspendLayout();
            this.panelBackground.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTitleBar
            // 
            this.panelTitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.panelTitleBar.Controls.Add(this.labelTitle);
            this.panelTitleBar.Location = new System.Drawing.Point(0, 0);
            this.panelTitleBar.Name = "panelTitleBar";
            this.panelTitleBar.Size = new System.Drawing.Size(284, 35);
            this.panelTitleBar.TabIndex = 17;
            this.panelTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.panelTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.panelTitleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(7, 4);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(88, 25);
            this.labelTitle.TabIndex = 15;
            this.labelTitle.Text = "미리보기";
            this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.labelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);
            // 
            // textBoxPreview
            // 
            this.textBoxPreview.BackColor = System.Drawing.Color.White;
            this.textBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.textBoxPreview.Multiline = true;
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ReadOnly = true;
            this.textBoxPreview.Size = new System.Drawing.Size(278, 182);
            this.textBoxPreview.TabIndex = 18;
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.SystemColors.Control;
            this.panelBackground.Controls.Add(this.panel2);
            this.panelBackground.Controls.Add(this.panel1);
            this.panelBackground.Controls.Add(this.btnOK);
            this.panelBackground.Controls.Add(this.textBoxPreview);
            this.panelBackground.Location = new System.Drawing.Point(3, 38);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(278, 255);
            this.panelBackground.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radioIgnoreDay);
            this.panel2.Controls.Add(this.radioDay);
            this.panel2.Controls.Add(this.radioNight);
            this.panel2.Location = new System.Drawing.Point(85, 181);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(115, 72);
            this.panel2.TabIndex = 21;
            // 
            // radioIgnoreDay
            // 
            this.radioIgnoreDay.AutoSize = true;
            this.radioIgnoreDay.Checked = true;
            this.radioIgnoreDay.Location = new System.Drawing.Point(0, 6);
            this.radioIgnoreDay.Name = "radioIgnoreDay";
            this.radioIgnoreDay.Size = new System.Drawing.Size(47, 16);
            this.radioIgnoreDay.TabIndex = 20;
            this.radioIgnoreDay.TabStop = true;
            this.radioIgnoreDay.Text = "무시";
            this.radioIgnoreDay.UseVisualStyleBackColor = true;
            this.radioIgnoreDay.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioDay
            // 
            this.radioDay.AutoSize = true;
            this.radioDay.Location = new System.Drawing.Point(0, 29);
            this.radioDay.Name = "radioDay";
            this.radioDay.Size = new System.Drawing.Size(99, 16);
            this.radioDay.TabIndex = 20;
            this.radioDay.Text = "평일 주간모드";
            this.radioDay.UseVisualStyleBackColor = true;
            this.radioDay.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioNight
            // 
            this.radioNight.AutoSize = true;
            this.radioNight.Location = new System.Drawing.Point(0, 51);
            this.radioNight.Name = "radioNight";
            this.radioNight.Size = new System.Drawing.Size(115, 16);
            this.radioNight.TabIndex = 20;
            this.radioNight.Text = "야간 및 휴일모드";
            this.radioNight.UseVisualStyleBackColor = true;
            this.radioNight.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioIgnoreVirtual);
            this.panel1.Controls.Add(this.radioReal);
            this.panel1.Controls.Add(this.radioVirtual);
            this.panel1.Location = new System.Drawing.Point(11, 181);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(70, 72);
            this.panel1.TabIndex = 21;
            // 
            // radioIgnoreVirtual
            // 
            this.radioIgnoreVirtual.AutoSize = true;
            this.radioIgnoreVirtual.Location = new System.Drawing.Point(0, 6);
            this.radioIgnoreVirtual.Name = "radioIgnoreVirtual";
            this.radioIgnoreVirtual.Size = new System.Drawing.Size(47, 16);
            this.radioIgnoreVirtual.TabIndex = 20;
            this.radioIgnoreVirtual.Text = "무시";
            this.radioIgnoreVirtual.UseVisualStyleBackColor = true;
            this.radioIgnoreVirtual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioReal
            // 
            this.radioReal.AutoSize = true;
            this.radioReal.Location = new System.Drawing.Point(0, 51);
            this.radioReal.Name = "radioReal";
            this.radioReal.Size = new System.Drawing.Size(71, 16);
            this.radioReal.TabIndex = 20;
            this.radioReal.Text = "실제모드";
            this.radioReal.UseVisualStyleBackColor = true;
            this.radioReal.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioVirtual
            // 
            this.radioVirtual.AutoSize = true;
            this.radioVirtual.Checked = true;
            this.radioVirtual.Location = new System.Drawing.Point(0, 29);
            this.radioVirtual.Name = "radioVirtual";
            this.radioVirtual.Size = new System.Drawing.Size(71, 16);
            this.radioVirtual.TabIndex = 20;
            this.radioVirtual.TabStop = true;
            this.radioVirtual.Text = "훈련모드";
            this.radioVirtual.UseVisualStyleBackColor = true;
            this.radioVirtual.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnOK.Location = new System.Drawing.Point(206, 222);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(66, 27);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupPreviewMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(284, 296);
            this.Controls.Add(this.panelBackground);
            this.Controls.Add(this.panelTitleBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupPreviewMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPreviewMessage";
            this.panelTitleBar.ResumeLayout(false);
            this.panelTitleBar.PerformLayout();
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTitleBar;
        private System.Windows.Forms.TextBox textBoxPreview;
        private System.Windows.Forms.Panel panelBackground;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelTitle;
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