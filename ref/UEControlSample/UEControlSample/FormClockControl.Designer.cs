namespace UEControlSample
{
    partial class FormClockControl
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
            timer1.Stop();

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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.clockControl1 = new UnE.GUI.ClockControl();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // clockControl1
            // 
            this.clockControl1.BackColor = System.Drawing.Color.Transparent;
            this.clockControl1.CenterPointBrush = System.Drawing.Color.MidnightBlue;
            this.clockControl1.CenterRectSize = 4;
            this.clockControl1.EdgeColor = System.Drawing.Color.Sienna;
            this.clockControl1.EdgeThick = 3;
            this.clockControl1.Fixed10Visible = false;
            this.clockControl1.Fixed11Visible = false;
            this.clockControl1.Fixed12Visible = true;
            this.clockControl1.Fixed1Visible = true;
            this.clockControl1.Fixed2Visible = true;
            this.clockControl1.Fixed3Visible = true;
            this.clockControl1.Fixed4Visible = true;
            this.clockControl1.Fixed5Visible = true;
            this.clockControl1.Fixed6Visible = true;
            this.clockControl1.Fixed7Visible = true;
            this.clockControl1.Fixed8Visible = true;
            this.clockControl1.Fixed9Visible = true;
            this.clockControl1.FixedBrush = System.Drawing.Color.DarkSlateGray;
            this.clockControl1.FixedHeight = 6;
            this.clockControl1.FixedHour = 0;
            this.clockControl1.FixedMinute = 0;
            this.clockControl1.FixedWidth = 2;
            this.clockControl1.InnerBrush = System.Drawing.Color.Transparent;
            this.clockControl1.Location = new System.Drawing.Point(81, 73);
            this.clockControl1.LongMovementHeight = 20;
            this.clockControl1.LongMovementWidth = 2;
            this.clockControl1.MovementBrush = System.Drawing.Color.DeepSkyBlue;
            this.clockControl1.Name = "clockControl1";
            this.clockControl1.ShortMovementHeight = 12;
            this.clockControl1.ShortMovementWidth = 2;
            this.clockControl1.Size = new System.Drawing.Size(115, 109);
            this.clockControl1.TabIndex = 0;
            this.clockControl1.UseFixedTime = false;
            // 
            // FormClockControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.clockControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormClockControl";
            this.Text = "FormClockControl";
            this.Load += new System.EventHandler(this.FormClockControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private UnE.GUI.ClockControl clockControl1;
    }
}