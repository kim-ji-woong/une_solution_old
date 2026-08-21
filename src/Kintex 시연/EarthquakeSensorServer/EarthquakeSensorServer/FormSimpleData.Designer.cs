namespace EarthquakeSensorServer
{
    partial class FormSimpleData
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
            this.cboIntensity = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMagnitude = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLocation = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "진도 :";
            // 
            // cboIntensity
            // 
            this.cboIntensity.DisplayMember = "8";
            this.cboIntensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIntensity.FormattingEnabled = true;
            this.cboIntensity.Items.AddRange(new object[] {
            "사용안함",
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
            this.cboIntensity.Location = new System.Drawing.Point(67, 25);
            this.cboIntensity.Name = "cboIntensity";
            this.cboIntensity.Size = new System.Drawing.Size(83, 20);
            this.cboIntensity.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "규모 :";
            // 
            // textBoxMagnitude
            // 
            this.textBoxMagnitude.Location = new System.Drawing.Point(67, 54);
            this.textBoxMagnitude.Name = "textBoxMagnitude";
            this.textBoxMagnitude.Size = new System.Drawing.Size(83, 21);
            this.textBoxMagnitude.TabIndex = 2;
            this.textBoxMagnitude.Text = "6.5";
            this.textBoxMagnitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "진앙위치";
            // 
            // textBoxLocation
            // 
            this.textBoxLocation.Location = new System.Drawing.Point(26, 121);
            this.textBoxLocation.Name = "textBoxLocation";
            this.textBoxLocation.Size = new System.Drawing.Size(124, 21);
            this.textBoxLocation.TabIndex = 2;
            this.textBoxLocation.Text = "영흥도 서쪽 해상 5km";
            this.textBoxLocation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(95, 168);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(55, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // FormSimpleData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(176, 206);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.textBoxLocation);
            this.Controls.Add(this.textBoxMagnitude);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboIntensity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormSimpleData";
            this.Text = "지진 입력";
            this.Load += new System.EventHandler(this.FormSimpleData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboIntensity;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMagnitude;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLocation;
        private System.Windows.Forms.Button btnSend;

    }
}