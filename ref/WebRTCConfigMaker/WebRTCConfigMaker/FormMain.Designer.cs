
namespace WebRTCConfigMaker
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxConfig = new System.Windows.Forms.TextBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWebServerURL = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDBName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboDBType = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Config 파일 경로 :";
            // 
            // textBoxConfig
            // 
            this.textBoxConfig.Location = new System.Drawing.Point(130, 20);
            this.textBoxConfig.Name = "textBoxConfig";
            this.textBoxConfig.Size = new System.Drawing.Size(213, 23);
            this.textBoxConfig.TabIndex = 1;
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(349, 20);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(31, 23);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "...";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "WebServerURL :";
            // 
            // textBoxWebServerURL
            // 
            this.textBoxWebServerURL.Location = new System.Drawing.Point(130, 49);
            this.textBoxWebServerURL.Name = "textBoxWebServerURL";
            this.textBoxWebServerURL.Size = new System.Drawing.Size(213, 23);
            this.textBoxWebServerURL.TabIndex = 1;
            this.textBoxWebServerURL.Text = "http://127.0.0.1:809";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "DBName :";
            // 
            // textBoxDBName
            // 
            this.textBoxDBName.Location = new System.Drawing.Point(130, 78);
            this.textBoxDBName.Name = "textBoxDBName";
            this.textBoxDBName.Size = new System.Drawing.Size(213, 23);
            this.textBoxDBName.TabIndex = 1;
            this.textBoxDBName.Text = "WSOP_11";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "DBType :";
            // 
            // cboDBType
            // 
            this.cboDBType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDBType.FormattingEnabled = true;
            this.cboDBType.Items.AddRange(new object[] {
            "SQL Server",
            "MySQL"});
            this.cboDBType.Location = new System.Drawing.Point(130, 107);
            this.cboDBType.Name = "cboDBType";
            this.cboDBType.Size = new System.Drawing.Size(121, 23);
            this.cboDBType.TabIndex = 3;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(326, 107);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(54, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "실행";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 138);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.cboDBType);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxDBName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxWebServerURL);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxConfig);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "WebRTC Config 만들기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxConfig;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxWebServerURL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDBName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboDBType;
        private System.Windows.Forms.Button btnRun;
    }
}

