namespace SOPMonitoringSystem.Popup.SOPClose
{
    partial class PopupSOPClose
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
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbCloseTime = new System.Windows.Forms.Label();
            this.lbSOPName = new System.Windows.Forms.Label();
            this.tmrClose = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(41, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(116, 24);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "종료";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(175, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbCloseTime);
            this.groupBox1.Controls.Add(this.lbSOPName);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(41, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 83);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // lbCloseTime
            // 
            this.lbCloseTime.Location = new System.Drawing.Point(27, 48);
            this.lbCloseTime.Name = "lbCloseTime";
            this.lbCloseTime.Size = new System.Drawing.Size(203, 24);
            this.lbCloseTime.TabIndex = 1;
            this.lbCloseTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbSOPName
            // 
            this.lbSOPName.Location = new System.Drawing.Point(27, 23);
            this.lbSOPName.Name = "lbSOPName";
            this.lbSOPName.Size = new System.Drawing.Size(203, 24);
            this.lbSOPName.TabIndex = 0;
            // 
            // tmrClose
            // 
            this.tmrClose.Interval = 1000;
            this.tmrClose.Tick += new System.EventHandler(this.tmrClose_Tick);
            // 
            // PopupSOPClose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 135);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.MaximizeBox = false;
            this.Name = "PopupSOPClose";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SOP 종료";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PopupSOPClose_FormClosing);
            this.Load += new System.EventHandler(this.PopupSOPClose_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbCloseTime;
        private System.Windows.Forms.Label lbSOPName;
        private System.Windows.Forms.Timer tmrClose;
    }
}