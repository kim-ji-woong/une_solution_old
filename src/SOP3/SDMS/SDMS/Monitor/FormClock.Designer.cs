namespace SDMS
{
    partial class FormClock
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panelCalender = new System.Windows.Forms.Panel();
            this.panelClock = new System.Windows.Forms.Panel();
            this.clockControl = new SDMS.DigitalDisplayControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelCalender.SuspendLayout();
            this.panelClock.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Green;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("맑은 고딕", 19F);
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(237, 41);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelCalender
            // 
            this.panelCalender.Controls.Add(this.textBox1);
            this.panelCalender.Location = new System.Drawing.Point(0, 0);
            this.panelCalender.Name = "panelCalender";
            this.panelCalender.Size = new System.Drawing.Size(237, 40);
            this.panelCalender.TabIndex = 2;
            // 
            // panelClock
            // 
            this.panelClock.Controls.Add(this.clockControl);
            this.panelClock.Location = new System.Drawing.Point(0, 40);
            this.panelClock.Name = "panelClock";
            this.panelClock.Size = new System.Drawing.Size(237, 40);
            this.panelClock.TabIndex = 2;
            // 
            // clockControl
            // 
            this.clockControl.BackColor = System.Drawing.Color.Green;
            this.clockControl.DigitColor = System.Drawing.Color.WhiteSmoke;
            this.clockControl.DigitText = "00:00:00";
            this.clockControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clockControl.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.clockControl.Location = new System.Drawing.Point(0, 0);
            this.clockControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.clockControl.Name = "clockControl";
            this.clockControl.Size = new System.Drawing.Size(237, 40);
            this.clockControl.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // FormClock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 87);
            this.Controls.Add(this.panelClock);
            this.Controls.Add(this.panelCalender);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormClock";
            this.Text = "FormClock";
            this.panelCalender.ResumeLayout(false);
            this.panelCalender.PerformLayout();
            this.panelClock.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DigitalDisplayControl clockControl;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panelCalender;
        private System.Windows.Forms.Panel panelClock;
        private System.Windows.Forms.Timer timer1;
    }
}