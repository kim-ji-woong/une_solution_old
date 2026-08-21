namespace ImageDivider
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxHorizontal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxVertical = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cboAddPixel = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "너비 :";
            // 
            // textBoxHorizontal
            // 
            this.textBoxHorizontal.Location = new System.Drawing.Point(105, 52);
            this.textBoxHorizontal.Name = "textBoxHorizontal";
            this.textBoxHorizontal.Size = new System.Drawing.Size(32, 21);
            this.textBoxHorizontal.TabIndex = 1;
            this.textBoxHorizontal.Text = "256";
            this.textBoxHorizontal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(141, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "pixel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "높이 :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(283, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "pixel";
            // 
            // textBoxVertical
            // 
            this.textBoxVertical.Location = new System.Drawing.Point(247, 52);
            this.textBoxVertical.Name = "textBoxVertical";
            this.textBoxVertical.Size = new System.Drawing.Size(32, 21);
            this.textBoxVertical.TabIndex = 1;
            this.textBoxVertical.Text = "256";
            this.textBoxVertical.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "원본파일 :";
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(105, 25);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(285, 21);
            this.textBoxFilePath.TabIndex = 1;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(396, 25);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(27, 23);
            this.btnOpenFile.TabIndex = 2;
            this.btnOpenFile.Text = "...";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnDivide
            // 
            this.btnDivide.Location = new System.Drawing.Point(336, 55);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(54, 23);
            this.btnDivide.TabIndex = 3;
            this.btnDivide.Text = "쪼개기";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.Click += new System.EventHandler(this.btnDivide_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(62, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "여유픽셀 :";
            // 
            // cboAddPixel
            // 
            this.cboAddPixel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAddPixel.FormattingEnabled = true;
            this.cboAddPixel.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cboAddPixel.Location = new System.Drawing.Point(129, 79);
            this.cboAddPixel.Name = "cboAddPixel";
            this.cboAddPixel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboAddPixel.Size = new System.Drawing.Size(52, 20);
            this.cboAddPixel.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(187, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "pixel";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 117);
            this.Controls.Add(this.cboAddPixel);
            this.Controls.Add(this.btnDivide);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.textBoxVertical);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.textBoxHorizontal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "이미지 쪼개기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxHorizontal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxVertical;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboAddPixel;
        private System.Windows.Forms.Label label7;
    }
}

