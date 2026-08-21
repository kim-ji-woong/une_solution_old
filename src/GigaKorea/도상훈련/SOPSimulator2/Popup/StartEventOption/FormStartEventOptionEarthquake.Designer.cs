namespace SOPMonitoringSystem.Popup
{
    partial class FormStartEventOptionEarthquake
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
            this.radioMagnit = new System.Windows.Forms.RadioButton();
            this.radioIntens = new System.Windows.Forms.RadioButton();
            this.textBoxMagnit = new System.Windows.Forms.TextBox();
            this.cboIntensity = new System.Windows.Forms.ComboBox();
            this.radioUnknown = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // radioMagnit
            // 
            this.radioMagnit.AutoSize = true;
            this.radioMagnit.Location = new System.Drawing.Point(12, 12);
            this.radioMagnit.Name = "radioMagnit";
            this.radioMagnit.Size = new System.Drawing.Size(47, 16);
            this.radioMagnit.TabIndex = 0;
            this.radioMagnit.Text = "규모";
            this.radioMagnit.UseVisualStyleBackColor = true;
            this.radioMagnit.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioIntens
            // 
            this.radioIntens.AutoSize = true;
            this.radioIntens.Location = new System.Drawing.Point(12, 49);
            this.radioIntens.Name = "radioIntens";
            this.radioIntens.Size = new System.Drawing.Size(47, 16);
            this.radioIntens.TabIndex = 0;
            this.radioIntens.Text = "진도";
            this.radioIntens.UseVisualStyleBackColor = true;
            this.radioIntens.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // textBoxMagnit
            // 
            this.textBoxMagnit.Enabled = false;
            this.textBoxMagnit.Location = new System.Drawing.Point(65, 9);
            this.textBoxMagnit.Name = "textBoxMagnit";
            this.textBoxMagnit.Size = new System.Drawing.Size(57, 21);
            this.textBoxMagnit.TabIndex = 1;
            this.textBoxMagnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxMagnit.TextChanged += new System.EventHandler(this.textBoxMagnit_TextChanged);
            // 
            // cboIntensity
            // 
            this.cboIntensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIntensity.Enabled = false;
            this.cboIntensity.FormattingEnabled = true;
            this.cboIntensity.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cboIntensity.Location = new System.Drawing.Point(65, 48);
            this.cboIntensity.Name = "cboIntensity";
            this.cboIntensity.Size = new System.Drawing.Size(57, 20);
            this.cboIntensity.TabIndex = 2;
            this.cboIntensity.SelectedIndexChanged += new System.EventHandler(this.cboIntensity_SelectedIndexChanged);
            // 
            // radioUnknown
            // 
            this.radioUnknown.AutoSize = true;
            this.radioUnknown.Checked = true;
            this.radioUnknown.Location = new System.Drawing.Point(12, 83);
            this.radioUnknown.Name = "radioUnknown";
            this.radioUnknown.Size = new System.Drawing.Size(75, 16);
            this.radioUnknown.TabIndex = 0;
            this.radioUnknown.TabStop = true;
            this.radioUnknown.Text = "알수 없음";
            this.radioUnknown.UseVisualStyleBackColor = true;
            this.radioUnknown.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // FormStartEventOptionEarthquake
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(174, 132);
            this.Controls.Add(this.cboIntensity);
            this.Controls.Add(this.textBoxMagnit);
            this.Controls.Add(this.radioUnknown);
            this.Controls.Add(this.radioIntens);
            this.Controls.Add(this.radioMagnit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormStartEventOptionEarthquake";
            this.ShowInTaskbar = false;
            this.Text = "FormStartEventOptionEarthquake";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioMagnit;
        private System.Windows.Forms.RadioButton radioIntens;
        private System.Windows.Forms.TextBox textBoxMagnit;
        private System.Windows.Forms.ComboBox cboIntensity;
        private System.Windows.Forms.RadioButton radioUnknown;
    }
}