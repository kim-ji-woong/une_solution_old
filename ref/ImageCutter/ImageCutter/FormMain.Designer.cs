namespace ImageCutter
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.btnImagePath = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.labelImageSize = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxVert = new System.Windows.Forms.TextBox();
            this.textBoxHorz = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTileInfo = new System.Windows.Forms.Label();
            this.groupBoxTileInfo = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBoxTileInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "이미지 경로 :";
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(95, 21);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(147, 21);
            this.textBoxFilePath.TabIndex = 0;
            this.textBoxFilePath.TextChanged += new System.EventHandler(this.textBoxFilePath_TextChanged);
            // 
            // btnImagePath
            // 
            this.btnImagePath.Location = new System.Drawing.Point(248, 21);
            this.btnImagePath.Name = "btnImagePath";
            this.btnImagePath.Size = new System.Drawing.Size(28, 23);
            this.btnImagePath.TabIndex = 2;
            this.btnImagePath.Text = "...";
            this.btnImagePath.UseVisualStyleBackColor = true;
            this.btnImagePath.Click += new System.EventHandler(this.btnImagePath_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(221, 130);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(55, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "실행";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // labelImageSize
            // 
            this.labelImageSize.AutoSize = true;
            this.labelImageSize.Location = new System.Drawing.Point(93, 56);
            this.labelImageSize.Name = "labelImageSize";
            this.labelImageSize.Size = new System.Drawing.Size(69, 12);
            this.labelImageSize.TabIndex = 5;
            this.labelImageSize.Text = "이미지 크기";
            this.labelImageSize.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxVert);
            this.groupBox1.Controls.Add(this.textBoxHorz);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(110, 80);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "타일 이미지";
            // 
            // textBoxVert
            // 
            this.textBoxVert.Location = new System.Drawing.Point(53, 46);
            this.textBoxVert.Name = "textBoxVert";
            this.textBoxVert.Size = new System.Drawing.Size(42, 21);
            this.textBoxVert.TabIndex = 1;
            this.textBoxVert.TextChanged += new System.EventHandler(this.textBoxVert_TextChanged);
            // 
            // textBoxHorz
            // 
            this.textBoxHorz.Location = new System.Drawing.Point(53, 20);
            this.textBoxHorz.Name = "textBoxHorz";
            this.textBoxHorz.Size = new System.Drawing.Size(42, 21);
            this.textBoxHorz.TabIndex = 0;
            this.textBoxHorz.TextChanged += new System.EventHandler(this.textBoxHorz_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "세로";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "가로";
            // 
            // labelTileInfo
            // 
            this.labelTileInfo.AutoSize = true;
            this.labelTileInfo.Location = new System.Drawing.Point(9, 33);
            this.labelTileInfo.Name = "labelTileInfo";
            this.labelTileInfo.Size = new System.Drawing.Size(57, 12);
            this.labelTileInfo.TabIndex = 5;
            this.labelTileInfo.Text = "타일 개수";
            // 
            // groupBoxTileInfo
            // 
            this.groupBoxTileInfo.Controls.Add(this.labelTileInfo);
            this.groupBoxTileInfo.Location = new System.Drawing.Point(130, 81);
            this.groupBoxTileInfo.Name = "groupBoxTileInfo";
            this.groupBoxTileInfo.Size = new System.Drawing.Size(85, 80);
            this.groupBoxTileInfo.TabIndex = 7;
            this.groupBoxTileInfo.TabStop = false;
            this.groupBoxTileInfo.Text = "타일 개수";
            this.groupBoxTileInfo.Visible = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 184);
            this.Controls.Add(this.groupBoxTileInfo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelImageSize);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnImagePath);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.label3);
            this.Name = "FormMain";
            this.Text = "이미지 뽀개기";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxTileInfo.ResumeLayout(false);
            this.groupBoxTileInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button btnImagePath;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label labelImageSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxVert;
        private System.Windows.Forms.TextBox textBoxHorz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTileInfo;
        private System.Windows.Forms.GroupBox groupBoxTileInfo;
    }
}

