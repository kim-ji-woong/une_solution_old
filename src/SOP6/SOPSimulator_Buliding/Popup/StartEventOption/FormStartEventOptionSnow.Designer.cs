namespace SOPMonitoringSystem.Popup
{
    partial class FormStartEventOptionSnow
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
            this.groupBoxAmountSnowfall = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxAmountSnowfall = new System.Windows.Forms.TextBox();
            this.groupBoxAmountSnowfall.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAmountSnowfall
            // 
            this.groupBoxAmountSnowfall.Controls.Add(this.label1);
            this.groupBoxAmountSnowfall.Controls.Add(this.textBoxAmountSnowfall);
            this.groupBoxAmountSnowfall.Location = new System.Drawing.Point(15, 12);
            this.groupBoxAmountSnowfall.Name = "groupBoxAmountSnowfall";
            this.groupBoxAmountSnowfall.Size = new System.Drawing.Size(152, 60);
            this.groupBoxAmountSnowfall.TabIndex = 18;
            this.groupBoxAmountSnowfall.TabStop = false;
            this.groupBoxAmountSnowfall.Text = "적설량";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(123, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "cm";
            // 
            // textBoxAmountSnowfall
            // 
            this.textBoxAmountSnowfall.Location = new System.Drawing.Point(21, 22);
            this.textBoxAmountSnowfall.Name = "textBoxAmountSnowfall";
            this.textBoxAmountSnowfall.Size = new System.Drawing.Size(100, 21);
            this.textBoxAmountSnowfall.TabIndex = 0;
            this.textBoxAmountSnowfall.TextChanged += new System.EventHandler(this.textBoxAmountSnowfall_TextChanged);
            // 
            // FormStartEventOptionSnow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(174, 132);
            this.Controls.Add(this.groupBoxAmountSnowfall);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormStartEventOptionSnow";
            this.ShowInTaskbar = false;
            this.Text = "FormStartEventOptionSnow";
            this.groupBoxAmountSnowfall.ResumeLayout(false);
            this.groupBoxAmountSnowfall.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAmountSnowfall;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxAmountSnowfall;
    }
}