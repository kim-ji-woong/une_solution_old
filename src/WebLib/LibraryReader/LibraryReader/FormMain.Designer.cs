namespace LibraryReader
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.textBoxSheetName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelProgress = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.radioFromBegin = new System.Windows.Forms.RadioButton();
            this.radioFromManual = new System.Windows.Forms.RadioButton();
            this.textBoxFromIndex = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DB 데이터 파일을 선택하세요.";
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(24, 64);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(209, 21);
            this.textBoxFilePath.TabIndex = 1;
            // 
            // textBoxSheetName
            // 
            this.textBoxSheetName.Location = new System.Drawing.Point(24, 122);
            this.textBoxSheetName.Name = "textBoxSheetName";
            this.textBoxSheetName.Size = new System.Drawing.Size(100, 21);
            this.textBoxSheetName.TabIndex = 2;
            this.textBoxSheetName.Text = "공공도서관";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "엑셀 쉬트 이름을 지정하세요.";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(243, 64);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(29, 21);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "...";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(24, 233);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(248, 21);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(22, 267);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(53, 12);
            this.labelProgress.TabIndex = 5;
            this.labelProgress.Text = "진행상황";
            this.labelProgress.Visible = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(197, 290);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "시작";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // radioFromBegin
            // 
            this.radioFromBegin.AutoSize = true;
            this.radioFromBegin.Checked = true;
            this.radioFromBegin.Location = new System.Drawing.Point(24, 158);
            this.radioFromBegin.Name = "radioFromBegin";
            this.radioFromBegin.Size = new System.Drawing.Size(123, 16);
            this.radioFromBegin.TabIndex = 7;
            this.radioFromBegin.TabStop = true;
            this.radioFromBegin.Text = "처음부터 시작하기";
            this.radioFromBegin.UseVisualStyleBackColor = true;
            this.radioFromBegin.CheckedChanged += new System.EventHandler(this.radioFromBegin_CheckedChanged);
            // 
            // radioFromManual
            // 
            this.radioFromManual.AutoSize = true;
            this.radioFromManual.Location = new System.Drawing.Point(24, 174);
            this.radioFromManual.Name = "radioFromManual";
            this.radioFromManual.Size = new System.Drawing.Size(163, 16);
            this.radioFromManual.TabIndex = 7;
            this.radioFromManual.Text = "다음 데이터부터 시작하기";
            this.radioFromManual.UseVisualStyleBackColor = true;
            this.radioFromManual.CheckedChanged += new System.EventHandler(this.radioFromManual_CheckedChanged);
            // 
            // textBoxFromIndex
            // 
            this.textBoxFromIndex.Enabled = false;
            this.textBoxFromIndex.Location = new System.Drawing.Point(42, 194);
            this.textBoxFromIndex.Name = "textBoxFromIndex";
            this.textBoxFromIndex.Size = new System.Drawing.Size(33, 21);
            this.textBoxFromIndex.TabIndex = 8;
            this.textBoxFromIndex.Text = "1";
            this.textBoxFromIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxFromIndex.TextChanged += new System.EventHandler(this.textBoxFromIndex_TextChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 326);
            this.Controls.Add(this.textBoxFromIndex);
            this.Controls.Add(this.radioFromManual);
            this.Controls.Add(this.radioFromBegin);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.textBoxSheetName);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "좌표변환";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.TextBox textBoxSheetName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RadioButton radioFromBegin;
        private System.Windows.Forms.RadioButton radioFromManual;
        private System.Windows.Forms.TextBox textBoxFromIndex;
    }
}

