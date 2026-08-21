namespace SOPMonitoringSystem
{
    partial class FormLegend
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
            this.labelSkip = new System.Windows.Forms.Label();
            this.labelRunning = new System.Windows.Forms.Label();
            this.labelWait = new System.Windows.Forms.Label();
            this.labelNotProcessed = new System.Windows.Forms.Label();
            this.labelComplete = new System.Windows.Forms.Label();
            this.labelProcessed = new System.Windows.Forms.Label();
            this.labelCurrent = new System.Windows.Forms.Label();
            this.pictureBoxCurrent = new System.Windows.Forms.PictureBox();
            this.pictureBoxNotProcessed = new System.Windows.Forms.PictureBox();
            this.pictureBoxComplete = new System.Windows.Forms.PictureBox();
            this.pictureBoxProcessed = new System.Windows.Forms.PictureBox();
            this.pictureBoxWait = new System.Windows.Forms.PictureBox();
            this.pictureBoxRunning = new System.Windows.Forms.PictureBox();
            this.pictureBoxSkip = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNotProcessed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComplete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProcessed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRunning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSkip)).BeginInit();
            this.SuspendLayout();
            // 
            // labelSkip
            // 
            this.labelSkip.AutoSize = true;
            this.labelSkip.Location = new System.Drawing.Point(46, 132);
            this.labelSkip.Name = "labelSkip";
            this.labelSkip.Size = new System.Drawing.Size(97, 12);
            this.labelSkip.TabIndex = 1;
            this.labelSkip.Text = "건너 뛴 프로세스";
            this.labelSkip.Visible = false;
            // 
            // labelRunning
            // 
            this.labelRunning.AutoSize = true;
            this.labelRunning.Location = new System.Drawing.Point(43, 72);
            this.labelRunning.Name = "labelRunning";
            this.labelRunning.Size = new System.Drawing.Size(105, 12);
            this.labelRunning.TabIndex = 8;
            this.labelRunning.Text = "실행중인 프로세스";
            // 
            // labelWait
            // 
            this.labelWait.AutoSize = true;
            this.labelWait.Location = new System.Drawing.Point(43, 52);
            this.labelWait.Name = "labelWait";
            this.labelWait.Size = new System.Drawing.Size(133, 12);
            this.labelWait.TabIndex = 9;
            this.labelWait.Text = "실행하지 않은 프로세스";
            // 
            // labelNotProcessed
            // 
            this.labelNotProcessed.AutoSize = true;
            this.labelNotProcessed.Location = new System.Drawing.Point(42, 14);
            this.labelNotProcessed.Name = "labelNotProcessed";
            this.labelNotProcessed.Size = new System.Drawing.Size(133, 12);
            this.labelNotProcessed.TabIndex = 10;
            this.labelNotProcessed.Text = "실행하지 않은 프로세스";
            // 
            // labelComplete
            // 
            this.labelComplete.AutoSize = true;
            this.labelComplete.Location = new System.Drawing.Point(43, 92);
            this.labelComplete.Name = "labelComplete";
            this.labelComplete.Size = new System.Drawing.Size(93, 12);
            this.labelComplete.TabIndex = 11;
            this.labelComplete.Text = "완료된 프로세스";
            // 
            // labelProcessed
            // 
            this.labelProcessed.AutoSize = true;
            this.labelProcessed.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelProcessed.Location = new System.Drawing.Point(43, 33);
            this.labelProcessed.Name = "labelProcessed";
            this.labelProcessed.Size = new System.Drawing.Size(86, 11);
            this.labelProcessed.TabIndex = 12;
            this.labelProcessed.Text = "실행한 프로세스";
            // 
            // labelCurrent
            // 
            this.labelCurrent.AutoSize = true;
            this.labelCurrent.Location = new System.Drawing.Point(43, 112);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(81, 12);
            this.labelCurrent.TabIndex = 14;
            this.labelCurrent.Text = "현재 프로세스";
            // 
            // pictureBoxCurrent
            // 
            this.pictureBoxCurrent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pictureBoxCurrent.Location = new System.Drawing.Point(11, 112);
            this.pictureBoxCurrent.Name = "pictureBoxCurrent";
            this.pictureBoxCurrent.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxCurrent.TabIndex = 13;
            this.pictureBoxCurrent.TabStop = false;
            // 
            // pictureBoxNotProcessed
            // 
            this.pictureBoxNotProcessed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pictureBoxNotProcessed.Location = new System.Drawing.Point(11, 12);
            this.pictureBoxNotProcessed.Name = "pictureBoxNotProcessed";
            this.pictureBoxNotProcessed.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxNotProcessed.TabIndex = 7;
            this.pictureBoxNotProcessed.TabStop = false;
            // 
            // pictureBoxComplete
            // 
            this.pictureBoxComplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pictureBoxComplete.Location = new System.Drawing.Point(11, 92);
            this.pictureBoxComplete.Name = "pictureBoxComplete";
            this.pictureBoxComplete.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxComplete.TabIndex = 6;
            this.pictureBoxComplete.TabStop = false;
            // 
            // pictureBoxProcessed
            // 
            this.pictureBoxProcessed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pictureBoxProcessed.Location = new System.Drawing.Point(11, 32);
            this.pictureBoxProcessed.Name = "pictureBoxProcessed";
            this.pictureBoxProcessed.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxProcessed.TabIndex = 5;
            this.pictureBoxProcessed.TabStop = false;
            // 
            // pictureBoxWait
            // 
            this.pictureBoxWait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pictureBoxWait.Location = new System.Drawing.Point(11, 52);
            this.pictureBoxWait.Name = "pictureBoxWait";
            this.pictureBoxWait.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxWait.TabIndex = 4;
            this.pictureBoxWait.TabStop = false;
            // 
            // pictureBoxRunning
            // 
            this.pictureBoxRunning.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRunning.Location = new System.Drawing.Point(12, 72);
            this.pictureBoxRunning.Name = "pictureBoxRunning";
            this.pictureBoxRunning.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxRunning.TabIndex = 3;
            this.pictureBoxRunning.TabStop = false;
            // 
            // pictureBoxSkip
            // 
            this.pictureBoxSkip.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxSkip.Location = new System.Drawing.Point(12, 132);
            this.pictureBoxSkip.Name = "pictureBoxSkip";
            this.pictureBoxSkip.Size = new System.Drawing.Size(25, 14);
            this.pictureBoxSkip.TabIndex = 2;
            this.pictureBoxSkip.TabStop = false;
            this.pictureBoxSkip.Visible = false;
            // 
            // FormLegend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(172, 166);
            this.Controls.Add(this.labelCurrent);
            this.Controls.Add(this.pictureBoxCurrent);
            this.Controls.Add(this.labelProcessed);
            this.Controls.Add(this.labelComplete);
            this.Controls.Add(this.labelNotProcessed);
            this.Controls.Add(this.labelWait);
            this.Controls.Add(this.labelRunning);
            this.Controls.Add(this.pictureBoxNotProcessed);
            this.Controls.Add(this.pictureBoxComplete);
            this.Controls.Add(this.pictureBoxProcessed);
            this.Controls.Add(this.pictureBoxWait);
            this.Controls.Add(this.pictureBoxRunning);
            this.Controls.Add(this.pictureBoxSkip);
            this.Controls.Add(this.labelSkip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLegend";
            this.Text = "FormLegend";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNotProcessed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxComplete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProcessed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRunning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSkip)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSkip;
        private System.Windows.Forms.PictureBox pictureBoxSkip;
        private System.Windows.Forms.PictureBox pictureBoxRunning;
        private System.Windows.Forms.PictureBox pictureBoxWait;
        private System.Windows.Forms.PictureBox pictureBoxProcessed;
        private System.Windows.Forms.PictureBox pictureBoxComplete;
        private System.Windows.Forms.PictureBox pictureBoxNotProcessed;
        private System.Windows.Forms.Label labelRunning;
        private System.Windows.Forms.Label labelWait;
        private System.Windows.Forms.Label labelNotProcessed;
        private System.Windows.Forms.Label labelComplete;
        private System.Windows.Forms.Label labelProcessed;
        private System.Windows.Forms.PictureBox pictureBoxCurrent;
        private System.Windows.Forms.Label labelCurrent;


    }
}