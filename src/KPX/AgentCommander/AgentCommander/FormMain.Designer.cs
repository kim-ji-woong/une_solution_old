namespace AgentCommander
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
            this.radioUpdate = new System.Windows.Forms.RadioButton();
            this.radioScreenCapture = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.radioClientUpdate = new System.Windows.Forms.RadioButton();
            this.radioTankServerUpdate = new System.Windows.Forms.RadioButton();
            this.radioServerUpdate = new System.Windows.Forms.RadioButton();
            this.radioPushServerUpdate = new System.Windows.Forms.RadioButton();
            this.radioUserAcceptanceUpdate = new System.Windows.Forms.RadioButton();
            this.radioJspFileUpdate = new System.Windows.Forms.RadioButton();
            this.textBox_jspFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioChkStatus = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_serverDll = new System.Windows.Forms.TextBox();
            this.radioServerDllUpdate = new System.Windows.Forms.RadioButton();
            this.radioDownloadZipFile = new System.Windows.Forms.RadioButton();
            this.textBoxZipTargetFolderPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxZipFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNormalTargetFolderPath = new System.Windows.Forms.TextBox();
            this.textBoxNormalFileName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.radioDownloadNormalFile = new System.Windows.Forms.RadioButton();
            this.textBoxSearchFolderPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.radioSearchFolder = new System.Windows.Forms.RadioButton();
            this.radioButton_file = new System.Windows.Forms.RadioButton();
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.radioButton_serviceFile = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox_areaType = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButton_procKill = new System.Windows.Forms.RadioButton();
            this.radioButton_procStart = new System.Windows.Forms.RadioButton();
            this.radioButton_serviceStart = new System.Windows.Forms.RadioButton();
            this.radioButton_serviceStop = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // radioUpdate
            // 
            this.radioUpdate.AutoSize = true;
            this.radioUpdate.Location = new System.Drawing.Point(12, 12);
            this.radioUpdate.Name = "radioUpdate";
            this.radioUpdate.Size = new System.Drawing.Size(98, 16);
            this.radioUpdate.TabIndex = 0;
            this.radioUpdate.TabStop = true;
            this.radioUpdate.Text = "Agent Update";
            this.radioUpdate.UseVisualStyleBackColor = true;
            // 
            // radioScreenCapture
            // 
            this.radioScreenCapture.AutoSize = true;
            this.radioScreenCapture.Location = new System.Drawing.Point(12, 34);
            this.radioScreenCapture.Name = "radioScreenCapture";
            this.radioScreenCapture.Size = new System.Drawing.Size(71, 16);
            this.radioScreenCapture.TabIndex = 0;
            this.radioScreenCapture.TabStop = true;
            this.radioScreenCapture.Text = "화면캡쳐";
            this.radioScreenCapture.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 438);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "쿼리 전송";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // radioClientUpdate
            // 
            this.radioClientUpdate.AutoSize = true;
            this.radioClientUpdate.Location = new System.Drawing.Point(12, 56);
            this.radioClientUpdate.Name = "radioClientUpdate";
            this.radioClientUpdate.Size = new System.Drawing.Size(98, 16);
            this.radioClientUpdate.TabIndex = 2;
            this.radioClientUpdate.TabStop = true;
            this.radioClientUpdate.Text = "Client Update";
            this.radioClientUpdate.UseVisualStyleBackColor = true;
            // 
            // radioTankServerUpdate
            // 
            this.radioTankServerUpdate.AutoSize = true;
            this.radioTankServerUpdate.Location = new System.Drawing.Point(12, 150);
            this.radioTankServerUpdate.Name = "radioTankServerUpdate";
            this.radioTankServerUpdate.Size = new System.Drawing.Size(134, 16);
            this.radioTankServerUpdate.TabIndex = 3;
            this.radioTankServerUpdate.TabStop = true;
            this.radioTankServerUpdate.Text = "Tank Server Update";
            this.radioTankServerUpdate.UseVisualStyleBackColor = true;
            // 
            // radioServerUpdate
            // 
            this.radioServerUpdate.AutoSize = true;
            this.radioServerUpdate.Location = new System.Drawing.Point(12, 78);
            this.radioServerUpdate.Name = "radioServerUpdate";
            this.radioServerUpdate.Size = new System.Drawing.Size(102, 16);
            this.radioServerUpdate.TabIndex = 4;
            this.radioServerUpdate.TabStop = true;
            this.radioServerUpdate.Text = "Server Update";
            this.radioServerUpdate.UseVisualStyleBackColor = true;
            // 
            // radioPushServerUpdate
            // 
            this.radioPushServerUpdate.AutoSize = true;
            this.radioPushServerUpdate.Location = new System.Drawing.Point(12, 172);
            this.radioPushServerUpdate.Name = "radioPushServerUpdate";
            this.radioPushServerUpdate.Size = new System.Drawing.Size(135, 16);
            this.radioPushServerUpdate.TabIndex = 5;
            this.radioPushServerUpdate.TabStop = true;
            this.radioPushServerUpdate.Text = "Push Server Update";
            this.radioPushServerUpdate.UseVisualStyleBackColor = true;
            // 
            // radioUserAcceptanceUpdate
            // 
            this.radioUserAcceptanceUpdate.AutoSize = true;
            this.radioUserAcceptanceUpdate.Location = new System.Drawing.Point(12, 194);
            this.radioUserAcceptanceUpdate.Name = "radioUserAcceptanceUpdate";
            this.radioUserAcceptanceUpdate.Size = new System.Drawing.Size(163, 16);
            this.radioUserAcceptanceUpdate.TabIndex = 6;
            this.radioUserAcceptanceUpdate.TabStop = true;
            this.radioUserAcceptanceUpdate.Text = "User Acceptance Update";
            this.radioUserAcceptanceUpdate.UseVisualStyleBackColor = true;
            // 
            // radioJspFileUpdate
            // 
            this.radioJspFileUpdate.AutoSize = true;
            this.radioJspFileUpdate.Location = new System.Drawing.Point(12, 216);
            this.radioJspFileUpdate.Name = "radioJspFileUpdate";
            this.radioJspFileUpdate.Size = new System.Drawing.Size(112, 16);
            this.radioJspFileUpdate.TabIndex = 7;
            this.radioJspFileUpdate.TabStop = true;
            this.radioJspFileUpdate.Text = "JSP File Update";
            this.radioJspFileUpdate.UseVisualStyleBackColor = true;
            // 
            // textBox_jspFileName
            // 
            this.textBox_jspFileName.Location = new System.Drawing.Point(84, 238);
            this.textBox_jspFileName.Name = "textBox_jspFileName";
            this.textBox_jspFileName.Size = new System.Drawing.Size(198, 21);
            this.textBox_jspFileName.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 242);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "파일명 : ";
            // 
            // radioChkStatus
            // 
            this.radioChkStatus.AutoSize = true;
            this.radioChkStatus.Location = new System.Drawing.Point(12, 265);
            this.radioChkStatus.Name = "radioChkStatus";
            this.radioChkStatus.Size = new System.Drawing.Size(211, 16);
            this.radioChkStatus.TabIndex = 11;
            this.radioChkStatus.TabStop = true;
            this.radioChkStatus.Text = "클라이언트, 서버 상태 로그로 확인";
            this.radioChkStatus.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "dll명 : ";
            // 
            // textBox_serverDll
            // 
            this.textBox_serverDll.Location = new System.Drawing.Point(84, 123);
            this.textBox_serverDll.Name = "textBox_serverDll";
            this.textBox_serverDll.Size = new System.Drawing.Size(177, 21);
            this.textBox_serverDll.TabIndex = 13;
            // 
            // radioServerDllUpdate
            // 
            this.radioServerDllUpdate.AutoSize = true;
            this.radioServerDllUpdate.Location = new System.Drawing.Point(12, 101);
            this.radioServerDllUpdate.Name = "radioServerDllUpdate";
            this.radioServerDllUpdate.Size = new System.Drawing.Size(120, 16);
            this.radioServerDllUpdate.TabIndex = 12;
            this.radioServerDllUpdate.TabStop = true;
            this.radioServerDllUpdate.Text = "Server Dll Update";
            this.radioServerDllUpdate.UseVisualStyleBackColor = true;
            // 
            // radioDownloadZipFile
            // 
            this.radioDownloadZipFile.AutoSize = true;
            this.radioDownloadZipFile.Location = new System.Drawing.Point(12, 287);
            this.radioDownloadZipFile.Name = "radioDownloadZipFile";
            this.radioDownloadZipFile.Size = new System.Drawing.Size(129, 16);
            this.radioDownloadZipFile.TabIndex = 11;
            this.radioDownloadZipFile.TabStop = true;
            this.radioDownloadZipFile.Text = "Zip 파일 Download";
            this.radioDownloadZipFile.UseVisualStyleBackColor = true;
            // 
            // textBoxZipTargetFolderPath
            // 
            this.textBoxZipTargetFolderPath.Location = new System.Drawing.Point(98, 309);
            this.textBoxZipTargetFolderPath.Name = "textBoxZipTargetFolderPath";
            this.textBoxZipTargetFolderPath.Size = new System.Drawing.Size(183, 21);
            this.textBoxZipTargetFolderPath.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 313);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "폴더경로 : ";
            // 
            // textBoxZipFileName
            // 
            this.textBoxZipFileName.Location = new System.Drawing.Point(344, 310);
            this.textBoxZipFileName.Name = "textBoxZipFileName";
            this.textBoxZipFileName.Size = new System.Drawing.Size(198, 21);
            this.textBoxZipFileName.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(287, 314);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "파일명 : ";
            // 
            // textBoxNormalTargetFolderPath
            // 
            this.textBoxNormalTargetFolderPath.Location = new System.Drawing.Point(98, 358);
            this.textBoxNormalTargetFolderPath.Name = "textBoxNormalTargetFolderPath";
            this.textBoxNormalTargetFolderPath.Size = new System.Drawing.Size(183, 21);
            this.textBoxNormalTargetFolderPath.TabIndex = 8;
            // 
            // textBoxNormalFileName
            // 
            this.textBoxNormalFileName.Location = new System.Drawing.Point(344, 359);
            this.textBoxNormalFileName.Name = "textBoxNormalFileName";
            this.textBoxNormalFileName.Size = new System.Drawing.Size(198, 21);
            this.textBoxNormalFileName.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 362);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "폴더경로 : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(287, 363);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "파일명 : ";
            // 
            // radioDownloadNormalFile
            // 
            this.radioDownloadNormalFile.AutoSize = true;
            this.radioDownloadNormalFile.Location = new System.Drawing.Point(12, 336);
            this.radioDownloadNormalFile.Name = "radioDownloadNormalFile";
            this.radioDownloadNormalFile.Size = new System.Drawing.Size(135, 16);
            this.radioDownloadNormalFile.TabIndex = 11;
            this.radioDownloadNormalFile.TabStop = true;
            this.radioDownloadNormalFile.Text = "일반 파일 Download";
            this.radioDownloadNormalFile.UseVisualStyleBackColor = true;
            // 
            // textBoxSearchFolderPath
            // 
            this.textBoxSearchFolderPath.Location = new System.Drawing.Point(98, 407);
            this.textBoxSearchFolderPath.Name = "textBoxSearchFolderPath";
            this.textBoxSearchFolderPath.Size = new System.Drawing.Size(183, 21);
            this.textBoxSearchFolderPath.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 411);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "폴더경로 : ";
            // 
            // radioSearchFolder
            // 
            this.radioSearchFolder.AutoSize = true;
            this.radioSearchFolder.Location = new System.Drawing.Point(12, 385);
            this.radioSearchFolder.Name = "radioSearchFolder";
            this.radioSearchFolder.Size = new System.Drawing.Size(75, 16);
            this.radioSearchFolder.TabIndex = 11;
            this.radioSearchFolder.TabStop = true;
            this.radioSearchFolder.Text = "폴더 탐색";
            this.radioSearchFolder.UseVisualStyleBackColor = true;
            // 
            // radioButton_file
            // 
            this.radioButton_file.AutoSize = true;
            this.radioButton_file.Location = new System.Drawing.Point(569, 35);
            this.radioButton_file.Name = "radioButton_file";
            this.radioButton_file.Size = new System.Drawing.Size(86, 16);
            this.radioButton_file.TabIndex = 15;
            this.radioButton_file.TabStop = true;
            this.radioButton_file.Text = "File Update";
            this.radioButton_file.UseVisualStyleBackColor = true;
            // 
            // textBox_path
            // 
            this.textBox_path.Location = new System.Drawing.Point(623, 215);
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.Size = new System.Drawing.Size(388, 21);
            this.textBox_path.TabIndex = 18;
            // 
            // radioButton_serviceFile
            // 
            this.radioButton_serviceFile.AutoSize = true;
            this.radioButton_serviceFile.Location = new System.Drawing.Point(569, 57);
            this.radioButton_serviceFile.Name = "radioButton_serviceFile";
            this.radioButton_serviceFile.Size = new System.Drawing.Size(132, 16);
            this.radioButton_serviceFile.TabIndex = 17;
            this.radioButton_serviceFile.TabStop = true;
            this.radioButton_serviceFile.Text = "Service File Update";
            this.radioButton_serviceFile.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(575, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "Path : ";
            // 
            // checkBox_areaType
            // 
            this.checkBox_areaType.AutoSize = true;
            this.checkBox_areaType.Location = new System.Drawing.Point(569, 191);
            this.checkBox_areaType.Name = "checkBox_areaType";
            this.checkBox_areaType.Size = new System.Drawing.Size(121, 16);
            this.checkBox_areaType.TabIndex = 20;
            this.checkBox_areaType.Text = "휴게실 PC도 적용";
            this.checkBox_areaType.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(581, 242);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(300, 116);
            this.label9.TabIndex = 21;
            this.label9.Text = "ex) \r\n1. File Update > 경로+파일명 \r\n    C:\\UNE\\KpxMonitoring\\KpxMonitoring.exe\r\n\r\n2. " +
    "Process Kill > 프로세스명 \r\n     KpxMonitoring\r\n\r\n3. Process Start > Full 경로\r\n     C:" +
    "\\UNE\\KpxMonitoring\\KpxMonitoring.exe";
            // 
            // radioButton_procKill
            // 
            this.radioButton_procKill.AutoSize = true;
            this.radioButton_procKill.Location = new System.Drawing.Point(569, 79);
            this.radioButton_procKill.Name = "radioButton_procKill";
            this.radioButton_procKill.Size = new System.Drawing.Size(91, 16);
            this.radioButton_procKill.TabIndex = 22;
            this.radioButton_procKill.TabStop = true;
            this.radioButton_procKill.Text = "Process Kill";
            this.radioButton_procKill.UseVisualStyleBackColor = true;
            // 
            // radioButton_procStart
            // 
            this.radioButton_procStart.AutoSize = true;
            this.radioButton_procStart.Location = new System.Drawing.Point(569, 101);
            this.radioButton_procStart.Name = "radioButton_procStart";
            this.radioButton_procStart.Size = new System.Drawing.Size(99, 16);
            this.radioButton_procStart.TabIndex = 23;
            this.radioButton_procStart.TabStop = true;
            this.radioButton_procStart.Text = "Process Start";
            this.radioButton_procStart.UseVisualStyleBackColor = true;
            // 
            // radioButton_serviceStart
            // 
            this.radioButton_serviceStart.AutoSize = true;
            this.radioButton_serviceStart.Location = new System.Drawing.Point(569, 148);
            this.radioButton_serviceStart.Name = "radioButton_serviceStart";
            this.radioButton_serviceStart.Size = new System.Drawing.Size(94, 16);
            this.radioButton_serviceStart.TabIndex = 25;
            this.radioButton_serviceStart.TabStop = true;
            this.radioButton_serviceStart.Text = "Service Start";
            this.radioButton_serviceStart.UseVisualStyleBackColor = true;
            // 
            // radioButton_serviceStop
            // 
            this.radioButton_serviceStop.AutoSize = true;
            this.radioButton_serviceStop.Location = new System.Drawing.Point(569, 126);
            this.radioButton_serviceStop.Name = "radioButton_serviceStop";
            this.radioButton_serviceStop.Size = new System.Drawing.Size(94, 16);
            this.radioButton_serviceStop.TabIndex = 24;
            this.radioButton_serviceStop.TabStop = true;
            this.radioButton_serviceStop.Text = "Service Stop";
            this.radioButton_serviceStop.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(569, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(248, 12);
            this.label10.TabIndex = 26;
            this.label10.Text = "신규 추가 작업 ! 기존 Command도 사용가능";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1058, 473);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.radioButton_serviceStart);
            this.Controls.Add(this.radioButton_serviceStop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_path);
            this.Controls.Add(this.textBox_serverDll);
            this.Controls.Add(this.radioButton_procStart);
            this.Controls.Add(this.radioServerDllUpdate);
            this.Controls.Add(this.radioButton_file);
            this.Controls.Add(this.radioButton_procKill);
            this.Controls.Add(this.radioSearchFolder);
            this.Controls.Add(this.radioButton_serviceFile);
            this.Controls.Add(this.radioDownloadNormalFile);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.radioDownloadZipFile);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBox_areaType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.radioChkStatus);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxNormalFileName);
            this.Controls.Add(this.textBoxSearchFolderPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxNormalTargetFolderPath);
            this.Controls.Add(this.textBoxZipFileName);
            this.Controls.Add(this.textBoxZipTargetFolderPath);
            this.Controls.Add(this.textBox_jspFileName);
            this.Controls.Add(this.radioJspFileUpdate);
            this.Controls.Add(this.radioUserAcceptanceUpdate);
            this.Controls.Add(this.radioPushServerUpdate);
            this.Controls.Add(this.radioServerUpdate);
            this.Controls.Add(this.radioTankServerUpdate);
            this.Controls.Add(this.radioClientUpdate);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.radioScreenCapture);
            this.Controls.Add(this.radioUpdate);
            this.Name = "FormMain";
            this.Text = "Agent Commander";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioUpdate;
        private System.Windows.Forms.RadioButton radioScreenCapture;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton radioClientUpdate;
        private System.Windows.Forms.RadioButton radioTankServerUpdate;
        private System.Windows.Forms.RadioButton radioServerUpdate;
        private System.Windows.Forms.RadioButton radioPushServerUpdate;
        private System.Windows.Forms.RadioButton radioUserAcceptanceUpdate;
        private System.Windows.Forms.RadioButton radioJspFileUpdate;
        private System.Windows.Forms.TextBox textBox_jspFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioChkStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_serverDll;
        private System.Windows.Forms.RadioButton radioServerDllUpdate;
        private System.Windows.Forms.RadioButton radioDownloadZipFile;
        private System.Windows.Forms.TextBox textBoxZipTargetFolderPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxZipFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNormalTargetFolderPath;
        private System.Windows.Forms.TextBox textBoxNormalFileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioDownloadNormalFile;
        private System.Windows.Forms.TextBox textBoxSearchFolderPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioSearchFolder;
        private System.Windows.Forms.RadioButton radioButton_file;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.RadioButton radioButton_serviceFile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox_areaType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton radioButton_procKill;
        private System.Windows.Forms.RadioButton radioButton_procStart;
        private System.Windows.Forms.RadioButton radioButton_serviceStart;
        private System.Windows.Forms.RadioButton radioButton_serviceStop;
        private System.Windows.Forms.Label label10;
    }
}

