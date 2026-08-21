namespace Poi2Image
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
            this.textBoxFolderPath = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btnCreateImage = new System.Windows.Forms.Button();
            this.rbtnBlack = new System.Windows.Forms.RadioButton();
            this.rbtnYellow = new System.Windows.Forms.RadioButton();
            this.yellowColor = new System.Windows.Forms.Button();
            this.blackColor = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "POI 폴더 :";
            // 
            // textBoxFolderPath
            // 
            this.textBoxFolderPath.Location = new System.Drawing.Point(79, 16);
            this.textBoxFolderPath.Name = "textBoxFolderPath";
            this.textBoxFolderPath.Size = new System.Drawing.Size(346, 21);
            this.textBoxFolderPath.TabIndex = 1;
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(431, 16);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(36, 22);
            this.btnFolder.TabIndex = 2;
            this.btnFolder.Text = "...";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // btnCreateImage
            // 
            this.btnCreateImage.Location = new System.Drawing.Point(14, 52);
            this.btnCreateImage.Name = "btnCreateImage";
            this.btnCreateImage.Size = new System.Drawing.Size(81, 23);
            this.btnCreateImage.TabIndex = 3;
            this.btnCreateImage.Text = "이미지 생성";
            this.btnCreateImage.UseVisualStyleBackColor = true;
            this.btnCreateImage.Click += new System.EventHandler(this.btnCreateImage_Click);
            // 
            // rbtnBlack
            // 
            this.rbtnBlack.AutoSize = true;
            this.rbtnBlack.Checked = true;
            this.rbtnBlack.Location = new System.Drawing.Point(14, 19);
            this.rbtnBlack.Name = "rbtnBlack";
            this.rbtnBlack.Size = new System.Drawing.Size(54, 16);
            this.rbtnBlack.TabIndex = 4;
            this.rbtnBlack.TabStop = true;
            this.rbtnBlack.Text = "Black";
            this.rbtnBlack.UseVisualStyleBackColor = true;
            // 
            // rbtnYellow
            // 
            this.rbtnYellow.AutoSize = true;
            this.rbtnYellow.Location = new System.Drawing.Point(97, 19);
            this.rbtnYellow.Name = "rbtnYellow";
            this.rbtnYellow.Size = new System.Drawing.Size(61, 16);
            this.rbtnYellow.TabIndex = 5;
            this.rbtnYellow.Text = "Yellow";
            this.rbtnYellow.UseVisualStyleBackColor = true;
            // 
            // yellowColor
            // 
            this.yellowColor.BackColor = System.Drawing.Color.Yellow;
            this.yellowColor.Location = new System.Drawing.Point(154, 15);
            this.yellowColor.Name = "yellowColor";
            this.yellowColor.Size = new System.Drawing.Size(20, 20);
            this.yellowColor.TabIndex = 6;
            this.yellowColor.UseVisualStyleBackColor = false;
            // 
            // blackColor
            // 
            this.blackColor.BackColor = System.Drawing.Color.Black;
            this.blackColor.Location = new System.Drawing.Point(64, 15);
            this.blackColor.Name = "blackColor";
            this.blackColor.Size = new System.Drawing.Size(20, 20);
            this.blackColor.TabIndex = 7;
            this.blackColor.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.blackColor);
            this.groupBox1.Controls.Add(this.yellowColor);
            this.groupBox1.Controls.Add(this.rbtnYellow);
            this.groupBox1.Controls.Add(this.rbtnBlack);
            this.groupBox1.Location = new System.Drawing.Point(295, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 43);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "이미지 색상";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 93);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCreateImage);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.textBoxFolderPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "POI To Image";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFolderPath;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Button btnCreateImage;
        private System.Windows.Forms.RadioButton rbtnBlack;
        private System.Windows.Forms.RadioButton rbtnYellow;
        private System.Windows.Forms.Button yellowColor;
        private System.Windows.Forms.Button blackColor;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

