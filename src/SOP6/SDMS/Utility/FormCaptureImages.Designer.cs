namespace SDMS.Utility
{
    partial class FormCaptureImages
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOutdoorFolder = new System.Windows.Forms.TextBox();
            this.btnOutdoorFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIndoorFolder = new System.Windows.Forms.TextBox();
            this.btnIndoor = new System.Windows.Forms.Button();
            this.radioFire = new System.Windows.Forms.RadioButton();
            this.radioPSM = new System.Windows.Forms.RadioButton();
            this.btnRun = new System.Windows.Forms.Button();
            this.labelProcess = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Outdoor Folder :";
            // 
            // textBoxOutdoorFolder
            // 
            this.textBoxOutdoorFolder.Location = new System.Drawing.Point(119, 6);
            this.textBoxOutdoorFolder.Name = "textBoxOutdoorFolder";
            this.textBoxOutdoorFolder.Size = new System.Drawing.Size(247, 21);
            this.textBoxOutdoorFolder.TabIndex = 1;
            // 
            // btnOutdoorFolder
            // 
            this.btnOutdoorFolder.Location = new System.Drawing.Point(372, 4);
            this.btnOutdoorFolder.Name = "btnOutdoorFolder";
            this.btnOutdoorFolder.Size = new System.Drawing.Size(28, 23);
            this.btnOutdoorFolder.TabIndex = 2;
            this.btnOutdoorFolder.Text = "...";
            this.btnOutdoorFolder.UseVisualStyleBackColor = true;
            this.btnOutdoorFolder.Click += new System.EventHandler(this.btnOutdoorFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Indoor Folder :";
            // 
            // textBoxIndoorFolder
            // 
            this.textBoxIndoorFolder.Location = new System.Drawing.Point(119, 33);
            this.textBoxIndoorFolder.Name = "textBoxIndoorFolder";
            this.textBoxIndoorFolder.Size = new System.Drawing.Size(247, 21);
            this.textBoxIndoorFolder.TabIndex = 1;
            // 
            // btnIndoor
            // 
            this.btnIndoor.Location = new System.Drawing.Point(372, 31);
            this.btnIndoor.Name = "btnIndoor";
            this.btnIndoor.Size = new System.Drawing.Size(28, 23);
            this.btnIndoor.TabIndex = 2;
            this.btnIndoor.Text = "...";
            this.btnIndoor.UseVisualStyleBackColor = true;
            this.btnIndoor.Click += new System.EventHandler(this.btnIndoor_Click);
            // 
            // radioFire
            // 
            this.radioFire.AutoSize = true;
            this.radioFire.Checked = true;
            this.radioFire.Location = new System.Drawing.Point(16, 75);
            this.radioFire.Name = "radioFire";
            this.radioFire.Size = new System.Drawing.Size(71, 16);
            this.radioFire.TabIndex = 3;
            this.radioFire.TabStop = true;
            this.radioFire.Text = "화재탐지";
            this.radioFire.UseVisualStyleBackColor = true;
            // 
            // radioPSM
            // 
            this.radioPSM.AutoSize = true;
            this.radioPSM.Location = new System.Drawing.Point(105, 75);
            this.radioPSM.Name = "radioPSM";
            this.radioPSM.Size = new System.Drawing.Size(71, 16);
            this.radioPSM.TabIndex = 3;
            this.radioPSM.Text = "누출탐지";
            this.radioPSM.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(325, 72);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // labelProcess
            // 
            this.labelProcess.AutoSize = true;
            this.labelProcess.Location = new System.Drawing.Point(14, 105);
            this.labelProcess.Name = "labelProcess";
            this.labelProcess.Size = new System.Drawing.Size(53, 12);
            this.labelProcess.TabIndex = 5;
            this.labelProcess.Text = "진행상황";
            // 
            // FormCaptureImages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 130);
            this.Controls.Add(this.labelProcess);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.radioPSM);
            this.Controls.Add(this.radioFire);
            this.Controls.Add(this.btnIndoor);
            this.Controls.Add(this.btnOutdoorFolder);
            this.Controls.Add(this.textBoxIndoorFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxOutdoorFolder);
            this.Controls.Add(this.label1);
            this.Name = "FormCaptureImages";
            this.Text = "FormCaptureImages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOutdoorFolder;
        private System.Windows.Forms.Button btnOutdoorFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIndoorFolder;
        private System.Windows.Forms.Button btnIndoor;
        private System.Windows.Forms.RadioButton radioFire;
        private System.Windows.Forms.RadioButton radioPSM;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label labelProcess;
    }
}