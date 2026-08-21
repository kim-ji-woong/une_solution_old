namespace TestWebService
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
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPW = new System.Windows.Forms.TextBox();
            this.btnRegist = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPhoneNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxBirth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPOIFilePath = new System.Windows.Forms.TextBox();
            this.btnPOIFile = new System.Windows.Forms.Button();
            this.btnRegistPOIFile = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPOICode = new System.Windows.Forms.TextBox();
            this.btnSearchPOI = new System.Windows.Forms.Button();
            this.btnDownloadPOIFile = new System.Windows.Forms.Button();
            this.btnUpdatePOIFile = new System.Windows.Forms.Button();
            this.btnSearchAllPOIs = new System.Windows.Forms.Button();
            this.btnPOIFolder = new System.Windows.Forms.Button();
            this.textBoxPOIFolderPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnRegistPOIFolder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHazardPOIFolder = new System.Windows.Forms.Button();
            this.textBoxHazardPOIFolderPath = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnRegistHazardPOIFolder = new System.Windows.Forms.Button();
            this.btnHazardPOIFile = new System.Windows.Forms.Button();
            this.textBoxHazardPOIFilePath = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnDownloadHazardPOIFile = new System.Windows.Forms.Button();
            this.btnSearchHazardPOI = new System.Windows.Forms.Button();
            this.btnUpdateHazardPOIFile = new System.Windows.Forms.Button();
            this.btnRegistHazardPOIFile = new System.Windows.Forms.Button();
            this.textBoxHazardPOICode = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "사용자 이름 : ";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(99, 15);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(108, 21);
            this.textBoxName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "비밀번호 : ";
            // 
            // textBoxPW
            // 
            this.textBoxPW.Location = new System.Drawing.Point(99, 42);
            this.textBoxPW.Name = "textBoxPW";
            this.textBoxPW.Size = new System.Drawing.Size(108, 21);
            this.textBoxPW.TabIndex = 1;
            // 
            // btnRegist
            // 
            this.btnRegist.Location = new System.Drawing.Point(19, 77);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(80, 23);
            this.btnRegist.TabIndex = 4;
            this.btnRegist.Text = "사용자 등록";
            this.btnRegist.UseVisualStyleBackColor = true;
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(224, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "전화번호 : ";
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(306, 15);
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(108, 21);
            this.textBoxPhoneNumber.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(224, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "생년월일 : ";
            // 
            // textBoxBirth
            // 
            this.textBoxBirth.Location = new System.Drawing.Point(306, 42);
            this.textBoxBirth.Name = "textBoxBirth";
            this.textBoxBirth.Size = new System.Drawing.Size(108, 21);
            this.textBoxBirth.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(224, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "(8자리)";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(105, 77);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "사용자 삭제";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 140);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "POI 파일 :";
            // 
            // textBoxPOIFilePath
            // 
            this.textBoxPOIFilePath.Location = new System.Drawing.Point(83, 137);
            this.textBoxPOIFilePath.Name = "textBoxPOIFilePath";
            this.textBoxPOIFilePath.Size = new System.Drawing.Size(297, 21);
            this.textBoxPOIFilePath.TabIndex = 6;
            // 
            // btnPOIFile
            // 
            this.btnPOIFile.Location = new System.Drawing.Point(386, 136);
            this.btnPOIFile.Name = "btnPOIFile";
            this.btnPOIFile.Size = new System.Drawing.Size(28, 23);
            this.btnPOIFile.TabIndex = 7;
            this.btnPOIFile.Text = "...";
            this.btnPOIFile.UseVisualStyleBackColor = true;
            this.btnPOIFile.Click += new System.EventHandler(this.btnPOIFile_Click);
            // 
            // btnRegistPOIFile
            // 
            this.btnRegistPOIFile.Location = new System.Drawing.Point(19, 164);
            this.btnRegistPOIFile.Name = "btnRegistPOIFile";
            this.btnRegistPOIFile.Size = new System.Drawing.Size(80, 23);
            this.btnRegistPOIFile.TabIndex = 4;
            this.btnRegistPOIFile.Text = "POI 등록";
            this.btnRegistPOIFile.UseVisualStyleBackColor = true;
            this.btnRegistPOIFile.Click += new System.EventHandler(this.btnRegistPOIFile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 281);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "POI 코드 : ";
            // 
            // textBoxPOICode
            // 
            this.textBoxPOICode.Location = new System.Drawing.Point(87, 278);
            this.textBoxPOICode.Name = "textBoxPOICode";
            this.textBoxPOICode.Size = new System.Drawing.Size(108, 21);
            this.textBoxPOICode.TabIndex = 0;
            // 
            // btnSearchPOI
            // 
            this.btnSearchPOI.Location = new System.Drawing.Point(19, 305);
            this.btnSearchPOI.Name = "btnSearchPOI";
            this.btnSearchPOI.Size = new System.Drawing.Size(80, 23);
            this.btnSearchPOI.TabIndex = 4;
            this.btnSearchPOI.Text = "POI 조회";
            this.btnSearchPOI.UseVisualStyleBackColor = true;
            this.btnSearchPOI.Click += new System.EventHandler(this.btnSearchPOIFile_Click);
            // 
            // btnDownloadPOIFile
            // 
            this.btnDownloadPOIFile.Location = new System.Drawing.Point(105, 305);
            this.btnDownloadPOIFile.Name = "btnDownloadPOIFile";
            this.btnDownloadPOIFile.Size = new System.Drawing.Size(90, 23);
            this.btnDownloadPOIFile.TabIndex = 4;
            this.btnDownloadPOIFile.Text = "POI 다운로드";
            this.btnDownloadPOIFile.UseVisualStyleBackColor = true;
            this.btnDownloadPOIFile.Click += new System.EventHandler(this.btnDownloadPOIFile_Click);
            // 
            // btnUpdatePOIFile
            // 
            this.btnUpdatePOIFile.Location = new System.Drawing.Point(105, 164);
            this.btnUpdatePOIFile.Name = "btnUpdatePOIFile";
            this.btnUpdatePOIFile.Size = new System.Drawing.Size(80, 23);
            this.btnUpdatePOIFile.TabIndex = 4;
            this.btnUpdatePOIFile.Text = "POI 수정";
            this.btnUpdatePOIFile.UseVisualStyleBackColor = true;
            this.btnUpdatePOIFile.Click += new System.EventHandler(this.btnUpdatePOIFile_Click);
            // 
            // btnSearchAllPOIs
            // 
            this.btnSearchAllPOIs.Location = new System.Drawing.Point(201, 305);
            this.btnSearchAllPOIs.Name = "btnSearchAllPOIs";
            this.btnSearchAllPOIs.Size = new System.Drawing.Size(80, 23);
            this.btnSearchAllPOIs.TabIndex = 4;
            this.btnSearchAllPOIs.Text = "전체 POI 조회";
            this.btnSearchAllPOIs.UseVisualStyleBackColor = true;
            this.btnSearchAllPOIs.Click += new System.EventHandler(this.btnSearchAllPOIs_Click);
            // 
            // btnPOIFolder
            // 
            this.btnPOIFolder.Location = new System.Drawing.Point(386, 208);
            this.btnPOIFolder.Name = "btnPOIFolder";
            this.btnPOIFolder.Size = new System.Drawing.Size(28, 23);
            this.btnPOIFolder.TabIndex = 11;
            this.btnPOIFolder.Text = "...";
            this.btnPOIFolder.UseVisualStyleBackColor = true;
            this.btnPOIFolder.Click += new System.EventHandler(this.btnPOIFolder_Click);
            // 
            // textBoxPOIFolderPath
            // 
            this.textBoxPOIFolderPath.Location = new System.Drawing.Point(83, 209);
            this.textBoxPOIFolderPath.Name = "textBoxPOIFolderPath";
            this.textBoxPOIFolderPath.Size = new System.Drawing.Size(297, 21);
            this.textBoxPOIFolderPath.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 212);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "POI 폴더 :";
            // 
            // btnRegistPOIFolder
            // 
            this.btnRegistPOIFolder.Location = new System.Drawing.Point(19, 236);
            this.btnRegistPOIFolder.Name = "btnRegistPOIFolder";
            this.btnRegistPOIFolder.Size = new System.Drawing.Size(113, 23);
            this.btnRegistPOIFolder.TabIndex = 8;
            this.btnRegistPOIFolder.Text = "POI 등록 및 수정";
            this.btnRegistPOIFolder.UseVisualStyleBackColor = true;
            this.btnRegistPOIFolder.Click += new System.EventHandler(this.btnRegistPOIFolder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 243);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "소방시설물 POI";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHazardPOIFolder);
            this.groupBox2.Controls.Add(this.textBoxHazardPOIFolderPath);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.btnRegistHazardPOIFolder);
            this.groupBox2.Controls.Add(this.btnHazardPOIFile);
            this.groupBox2.Controls.Add(this.textBoxHazardPOIFilePath);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.btnDownloadHazardPOIFile);
            this.groupBox2.Controls.Add(this.btnSearchHazardPOI);
            this.groupBox2.Controls.Add(this.btnUpdateHazardPOIFile);
            this.groupBox2.Controls.Add(this.btnRegistHazardPOIFile);
            this.groupBox2.Controls.Add(this.textBoxHazardPOICode);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Location = new System.Drawing.Point(12, 374);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(478, 240);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "위험물 POI";
            // 
            // btnHazardPOIFolder
            // 
            this.btnHazardPOIFolder.Location = new System.Drawing.Point(374, 97);
            this.btnHazardPOIFolder.Name = "btnHazardPOIFolder";
            this.btnHazardPOIFolder.Size = new System.Drawing.Size(28, 23);
            this.btnHazardPOIFolder.TabIndex = 25;
            this.btnHazardPOIFolder.Text = "...";
            this.btnHazardPOIFolder.UseVisualStyleBackColor = true;
            this.btnHazardPOIFolder.Click += new System.EventHandler(this.btnHazardPOIFolder_Click);
            // 
            // textBoxHazardPOIFolderPath
            // 
            this.textBoxHazardPOIFolderPath.Location = new System.Drawing.Point(71, 98);
            this.textBoxHazardPOIFolderPath.Name = "textBoxHazardPOIFolderPath";
            this.textBoxHazardPOIFolderPath.Size = new System.Drawing.Size(297, 21);
            this.textBoxHazardPOIFolderPath.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "POI 폴더 :";
            // 
            // btnRegistHazardPOIFolder
            // 
            this.btnRegistHazardPOIFolder.Location = new System.Drawing.Point(7, 125);
            this.btnRegistHazardPOIFolder.Name = "btnRegistHazardPOIFolder";
            this.btnRegistHazardPOIFolder.Size = new System.Drawing.Size(113, 23);
            this.btnRegistHazardPOIFolder.TabIndex = 22;
            this.btnRegistHazardPOIFolder.Text = "POI 등록 및 수정";
            this.btnRegistHazardPOIFolder.UseVisualStyleBackColor = true;
            this.btnRegistHazardPOIFolder.Click += new System.EventHandler(this.btnRegistHazardPOIFolder_Click);
            // 
            // btnHazardPOIFile
            // 
            this.btnHazardPOIFile.Location = new System.Drawing.Point(374, 25);
            this.btnHazardPOIFile.Name = "btnHazardPOIFile";
            this.btnHazardPOIFile.Size = new System.Drawing.Size(28, 23);
            this.btnHazardPOIFile.TabIndex = 21;
            this.btnHazardPOIFile.Text = "...";
            this.btnHazardPOIFile.UseVisualStyleBackColor = true;
            this.btnHazardPOIFile.Click += new System.EventHandler(this.btnHazardPOIFile_Click);
            // 
            // textBoxHazardPOIFilePath
            // 
            this.textBoxHazardPOIFilePath.Location = new System.Drawing.Point(71, 26);
            this.textBoxHazardPOIFilePath.Name = "textBoxHazardPOIFilePath";
            this.textBoxHazardPOIFilePath.Size = new System.Drawing.Size(297, 21);
            this.textBoxHazardPOIFilePath.TabIndex = 20;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 12);
            this.label10.TabIndex = 19;
            this.label10.Text = "POI 파일 :";
            // 
            // btnDownloadHazardPOIFile
            // 
            this.btnDownloadHazardPOIFile.Location = new System.Drawing.Point(93, 194);
            this.btnDownloadHazardPOIFile.Name = "btnDownloadHazardPOIFile";
            this.btnDownloadHazardPOIFile.Size = new System.Drawing.Size(90, 23);
            this.btnDownloadHazardPOIFile.TabIndex = 14;
            this.btnDownloadHazardPOIFile.Text = "POI 다운로드";
            this.btnDownloadHazardPOIFile.UseVisualStyleBackColor = true;
            this.btnDownloadHazardPOIFile.Click += new System.EventHandler(this.btnDownloadHazardPOIFile_Click);
            // 
            // btnSearchHazardPOI
            // 
            this.btnSearchHazardPOI.Location = new System.Drawing.Point(7, 194);
            this.btnSearchHazardPOI.Name = "btnSearchHazardPOI";
            this.btnSearchHazardPOI.Size = new System.Drawing.Size(80, 23);
            this.btnSearchHazardPOI.TabIndex = 16;
            this.btnSearchHazardPOI.Text = "POI 조회";
            this.btnSearchHazardPOI.UseVisualStyleBackColor = true;
            this.btnSearchHazardPOI.Click += new System.EventHandler(this.btnSearchHazardPOI_Click);
            // 
            // btnUpdateHazardPOIFile
            // 
            this.btnUpdateHazardPOIFile.Location = new System.Drawing.Point(93, 53);
            this.btnUpdateHazardPOIFile.Name = "btnUpdateHazardPOIFile";
            this.btnUpdateHazardPOIFile.Size = new System.Drawing.Size(80, 23);
            this.btnUpdateHazardPOIFile.TabIndex = 17;
            this.btnUpdateHazardPOIFile.Text = "POI 수정";
            this.btnUpdateHazardPOIFile.UseVisualStyleBackColor = true;
            this.btnUpdateHazardPOIFile.Click += new System.EventHandler(this.btnUpdateHazardPOIFile_Click);
            // 
            // btnRegistHazardPOIFile
            // 
            this.btnRegistHazardPOIFile.Location = new System.Drawing.Point(7, 53);
            this.btnRegistHazardPOIFile.Name = "btnRegistHazardPOIFile";
            this.btnRegistHazardPOIFile.Size = new System.Drawing.Size(80, 23);
            this.btnRegistHazardPOIFile.TabIndex = 18;
            this.btnRegistHazardPOIFile.Text = "POI 등록";
            this.btnRegistHazardPOIFile.UseVisualStyleBackColor = true;
            this.btnRegistHazardPOIFile.Click += new System.EventHandler(this.btnRegistHazardPOIFile_Click);
            // 
            // textBoxHazardPOICode
            // 
            this.textBoxHazardPOICode.Location = new System.Drawing.Point(75, 167);
            this.textBoxHazardPOICode.Name = "textBoxHazardPOICode";
            this.textBoxHazardPOICode.Size = new System.Drawing.Size(108, 21);
            this.textBoxHazardPOICode.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 170);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 13;
            this.label11.Text = "POI 코드 : ";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 634);
            this.Controls.Add(this.btnPOIFolder);
            this.Controls.Add(this.textBoxPOIFolderPath);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnRegistPOIFolder);
            this.Controls.Add(this.btnPOIFile);
            this.Controls.Add(this.textBoxPOIFilePath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnDownloadPOIFile);
            this.Controls.Add(this.btnSearchAllPOIs);
            this.Controls.Add(this.btnSearchPOI);
            this.Controls.Add(this.btnUpdatePOIFile);
            this.Controls.Add(this.btnRegistPOIFile);
            this.Controls.Add(this.btnRegist);
            this.Controls.Add(this.textBoxBirth);
            this.Controls.Add(this.textBoxPW);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPhoneNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPOICode);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPW;
        private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPhoneNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxBirth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPOIFilePath;
        private System.Windows.Forms.Button btnPOIFile;
        private System.Windows.Forms.Button btnRegistPOIFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPOICode;
        private System.Windows.Forms.Button btnSearchPOI;
        private System.Windows.Forms.Button btnDownloadPOIFile;
        private System.Windows.Forms.Button btnUpdatePOIFile;
        private System.Windows.Forms.Button btnSearchAllPOIs;
        private System.Windows.Forms.Button btnPOIFolder;
        private System.Windows.Forms.TextBox textBoxPOIFolderPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnRegistPOIFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnHazardPOIFolder;
        private System.Windows.Forms.TextBox textBoxHazardPOIFolderPath;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnRegistHazardPOIFolder;
        private System.Windows.Forms.Button btnHazardPOIFile;
        private System.Windows.Forms.TextBox textBoxHazardPOIFilePath;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnDownloadHazardPOIFile;
        private System.Windows.Forms.Button btnSearchHazardPOI;
        private System.Windows.Forms.Button btnUpdateHazardPOIFile;
        private System.Windows.Forms.Button btnRegistHazardPOIFile;
        private System.Windows.Forms.TextBox textBoxHazardPOICode;
        private System.Windows.Forms.Label label11;
    }
}

