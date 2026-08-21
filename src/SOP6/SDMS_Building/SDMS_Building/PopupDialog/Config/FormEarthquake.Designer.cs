namespace SDMS_Building.PopupDialog.Config
{
    partial class FormEarthquake
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
            this.tbStep1Min = new System.Windows.Forms.TextBox();
            this.tbStep1Max = new System.Windows.Forms.TextBox();
            this.tbStep2Min = new System.Windows.Forms.TextBox();
            this.tbStep3Min = new System.Windows.Forms.TextBox();
            this.tbStep4Min = new System.Windows.Forms.TextBox();
            this.tbStep2Max = new System.Windows.Forms.TextBox();
            this.tbStep3Max = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbStep1Min
            // 
            this.tbStep1Min.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep1Min.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep1Min.Location = new System.Drawing.Point(258, 116);
            this.tbStep1Min.Name = "tbStep1Min";
            this.tbStep1Min.Size = new System.Drawing.Size(43, 20);
            this.tbStep1Min.TabIndex = 36;
            this.tbStep1Min.Text = "2.0";
            // 
            // tbStep1Max
            // 
            this.tbStep1Max.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep1Max.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep1Max.Location = new System.Drawing.Point(258, 176);
            this.tbStep1Max.Name = "tbStep1Max";
            this.tbStep1Max.Size = new System.Drawing.Size(43, 20);
            this.tbStep1Max.TabIndex = 37;
            this.tbStep1Max.Text = "2.5";
            // 
            // tbStep2Min
            // 
            this.tbStep2Min.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep2Min.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep2Min.Location = new System.Drawing.Point(442, 116);
            this.tbStep2Min.Name = "tbStep2Min";
            this.tbStep2Min.Size = new System.Drawing.Size(43, 20);
            this.tbStep2Min.TabIndex = 38;
            this.tbStep2Min.Text = "2.0";
            // 
            // tbStep3Min
            // 
            this.tbStep3Min.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep3Min.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep3Min.Location = new System.Drawing.Point(626, 116);
            this.tbStep3Min.Name = "tbStep3Min";
            this.tbStep3Min.Size = new System.Drawing.Size(43, 20);
            this.tbStep3Min.TabIndex = 39;
            this.tbStep3Min.Text = "2.0";
            // 
            // tbStep4Min
            // 
            this.tbStep4Min.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep4Min.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep4Min.Location = new System.Drawing.Point(810, 116);
            this.tbStep4Min.Name = "tbStep4Min";
            this.tbStep4Min.Size = new System.Drawing.Size(43, 20);
            this.tbStep4Min.TabIndex = 40;
            this.tbStep4Min.Text = "2.0";
            // 
            // tbStep2Max
            // 
            this.tbStep2Max.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep2Max.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep2Max.Location = new System.Drawing.Point(442, 176);
            this.tbStep2Max.Name = "tbStep2Max";
            this.tbStep2Max.Size = new System.Drawing.Size(43, 20);
            this.tbStep2Max.TabIndex = 41;
            this.tbStep2Max.Text = "2.5";
            // 
            // tbStep3Max
            // 
            this.tbStep3Max.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbStep3Max.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbStep3Max.Location = new System.Drawing.Point(626, 176);
            this.tbStep3Max.Name = "tbStep3Max";
            this.tbStep3Max.Size = new System.Drawing.Size(43, 20);
            this.tbStep3Max.TabIndex = 42;
            this.tbStep3Max.Text = "2.5";
            // 
            // FormEarthquake
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(960, 500);
            this.Controls.Add(this.tbStep3Max);
            this.Controls.Add(this.tbStep2Max);
            this.Controls.Add(this.tbStep4Min);
            this.Controls.Add(this.tbStep3Min);
            this.Controls.Add(this.tbStep2Min);
            this.Controls.Add(this.tbStep1Max);
            this.Controls.Add(this.tbStep1Min);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEarthquake";
            this.ShowInTaskbar = false;
            this.Text = "FormEarthquake";
            this.Load += new System.EventHandler(this.FormEarthquake_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormEarthquake_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbStep1Min;
        private System.Windows.Forms.TextBox tbStep1Max;
        private System.Windows.Forms.TextBox tbStep2Min;
        private System.Windows.Forms.TextBox tbStep3Min;
        private System.Windows.Forms.TextBox tbStep4Min;
        private System.Windows.Forms.TextBox tbStep2Max;
        private System.Windows.Forms.TextBox tbStep3Max;
    }
}