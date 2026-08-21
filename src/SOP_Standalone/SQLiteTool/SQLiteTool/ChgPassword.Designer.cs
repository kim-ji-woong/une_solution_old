namespace SQLiteTool
{
    partial class ChgPassword
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnChg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(161, 21);
            this.textBox1.TabIndex = 0;
            // 
            // btnChg
            // 
            this.btnChg.Location = new System.Drawing.Point(180, 13);
            this.btnChg.Name = "btnChg";
            this.btnChg.Size = new System.Drawing.Size(75, 23);
            this.btnChg.TabIndex = 2;
            this.btnChg.Text = "변경";
            this.btnChg.UseVisualStyleBackColor = true;
            this.btnChg.Click += new System.EventHandler(this.btnChg_Click);
            // 
            // ChgPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 46);
            this.Controls.Add(this.btnChg);
            this.Controls.Add(this.textBox1);
            this.Name = "ChgPassword";
            this.Text = "ChgPassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnChg;
    }
}