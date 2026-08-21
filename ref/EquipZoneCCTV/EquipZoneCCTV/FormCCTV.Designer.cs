namespace EquipZoneCCTV
{
    partial class FormCCTV
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
            this.textBoxCCTVID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.labelCameraName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "CCTV 번호 :";
            // 
            // textBoxCCTVID
            // 
            this.textBoxCCTVID.Location = new System.Drawing.Point(93, 21);
            this.textBoxCCTVID.Name = "textBoxCCTVID";
            this.textBoxCCTVID.Size = new System.Drawing.Size(51, 21);
            this.textBoxCCTVID.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "URL :";
            // 
            // textBoxURL
            // 
            this.textBoxURL.Location = new System.Drawing.Point(93, 59);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(360, 21);
            this.textBoxURL.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(93, 106);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "조회하기";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(174, 106);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "적용하기";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // labelCameraName
            // 
            this.labelCameraName.AutoSize = true;
            this.labelCameraName.Location = new System.Drawing.Point(174, 24);
            this.labelCameraName.Name = "labelCameraName";
            this.labelCameraName.Size = new System.Drawing.Size(84, 12);
            this.labelCameraName.TabIndex = 0;
            this.labelCameraName.Text = "CameraName";
            // 
            // FormCCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 174);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.textBoxURL);
            this.Controls.Add(this.textBoxCCTVID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCameraName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormCCTV";
            this.Text = "CCTV URL 설정하기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCCTVID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label labelCameraName;
    }
}