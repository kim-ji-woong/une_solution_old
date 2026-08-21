namespace CCTVAlarmWatcher
{
    partial class FormMain
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
            this.cboNVR = new System.Windows.Forms.ComboBox();
            this.btnMakeAlarm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboNVR
            // 
            this.cboNVR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNVR.FormattingEnabled = true;
            this.cboNVR.Items.AddRange(new object[] {
            "4:1",
            "4:2",
            "4:5",
            "4:4"});
            this.cboNVR.Location = new System.Drawing.Point(607, 342);
            this.cboNVR.Name = "cboNVR";
            this.cboNVR.Size = new System.Drawing.Size(75, 20);
            this.cboNVR.TabIndex = 0;
            // 
            // btnMakeAlarm
            // 
            this.btnMakeAlarm.Location = new System.Drawing.Point(607, 368);
            this.btnMakeAlarm.Name = "btnMakeAlarm";
            this.btnMakeAlarm.Size = new System.Drawing.Size(75, 23);
            this.btnMakeAlarm.TabIndex = 1;
            this.btnMakeAlarm.Text = "알람 발생";
            this.btnMakeAlarm.UseVisualStyleBackColor = true;
            this.btnMakeAlarm.Click += new System.EventHandler(this.btnMakeAlarm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(605, 318);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Test Alarm 발생";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMakeAlarm);
            this.Controls.Add(this.cboNVR);
            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboNVR;
        private System.Windows.Forms.Button btnMakeAlarm;
        private System.Windows.Forms.Label label1;
    }
}