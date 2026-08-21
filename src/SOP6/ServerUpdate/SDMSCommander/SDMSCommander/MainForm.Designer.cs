namespace SDMSCommander
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txt_GetFileListPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnUpdate = new System.Windows.Forms.Panel();
            this.groupBox_Start = new System.Windows.Forms.GroupBox();
            this.rb_Start_Service = new System.Windows.Forms.RadioButton();
            this.txt_Start_FileName = new System.Windows.Forms.TextBox();
            this.rb_Start_Proc = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox_Down = new System.Windows.Forms.GroupBox();
            this.btn_UploadOpenFolder = new System.Windows.Forms.Button();
            this.btn_UploadOpenFile = new System.Windows.Forms.Button();
            this.txt_Upload_Path = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_Upload_LocalPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox_Kill = new System.Windows.Forms.GroupBox();
            this.rb_Kill_Service = new System.Windows.Forms.RadioButton();
            this.txt_Kill_FileName = new System.Windows.Forms.TextBox();
            this.rb_Kill_Proc = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.chk_Start = new System.Windows.Forms.CheckBox();
            this.chk_Upload = new System.Windows.Forms.CheckBox();
            this.chk_Kill = new System.Windows.Forms.CheckBox();
            this.btn_SendCommand = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_SiteId = new System.Windows.Forms.TextBox();
            this.label_isConnect = new System.Windows.Forms.Label();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.txt_GetProcName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chk_GetAllProc = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Down_Path = new System.Windows.Forms.TextBox();
            this.txt_AgentPath = new System.Windows.Forms.TextBox();
            this.btn_AgentOpenFile = new System.Windows.Forms.Button();
            this.btn_DownloadLogFile = new System.Windows.Forms.Button();
            this.txt_DownloadLocalPath = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txt_LogFilePath = new System.Windows.Forms.TextBox();
            this.btn_SDMSUpdateOpenFile = new System.Windows.Forms.Button();
            this.txt_SDMSUpdate_LocalPath = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.cbCommand = new System.Windows.Forms.ComboBox();
            this.pnAgentUpdate = new System.Windows.Forms.Panel();
            this.pnGetProcList = new System.Windows.Forms.Panel();
            this.pnGetFileList = new System.Windows.Forms.Panel();
            this.label25 = new System.Windows.Forms.Label();
            this.pnDownload = new System.Windows.Forms.Panel();
            this.lblDownloadLog = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.pnSdmsUpdate = new System.Windows.Forms.Panel();
            this.btnSDMSUpdateNow = new System.Windows.Forms.Button();
            this.btnSDMSUpdateShowXML = new System.Windows.Forms.Button();
            this.pnFileCopy = new System.Windows.Forms.Panel();
            this.label24 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.chkFileCopyDelete = new System.Windows.Forms.CheckBox();
            this.txtFileCopyDestFileName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtFileCopySourceFileName = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnRefreshDirectory = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtDirectoryPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_UploadServerPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnNewUpdateXML = new System.Windows.Forms.Button();
            this.pnUpdate.SuspendLayout();
            this.groupBox_Start.SuspendLayout();
            this.groupBox_Down.SuspendLayout();
            this.groupBox_Kill.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnAgentUpdate.SuspendLayout();
            this.pnGetProcList.SuspendLayout();
            this.pnGetFileList.SuspendLayout();
            this.pnDownload.SuspendLayout();
            this.pnSdmsUpdate.SuspendLayout();
            this.pnFileCopy.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_GetFileListPath
            // 
            this.txt_GetFileListPath.Location = new System.Drawing.Point(39, 3);
            this.txt_GetFileListPath.Name = "txt_GetFileListPath";
            this.txt_GetFileListPath.Size = new System.Drawing.Size(390, 21);
            this.txt_GetFileListPath.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "경로 : ";
            // 
            // pnUpdate
            // 
            this.pnUpdate.BackColor = System.Drawing.Color.White;
            this.pnUpdate.Controls.Add(this.groupBox_Start);
            this.pnUpdate.Controls.Add(this.groupBox_Down);
            this.pnUpdate.Controls.Add(this.groupBox_Kill);
            this.pnUpdate.Controls.Add(this.chk_Start);
            this.pnUpdate.Controls.Add(this.chk_Upload);
            this.pnUpdate.Controls.Add(this.chk_Kill);
            this.pnUpdate.Location = new System.Drawing.Point(12, 320);
            this.pnUpdate.Name = "pnUpdate";
            this.pnUpdate.Size = new System.Drawing.Size(494, 345);
            this.pnUpdate.TabIndex = 6;
            // 
            // groupBox_Start
            // 
            this.groupBox_Start.Controls.Add(this.rb_Start_Service);
            this.groupBox_Start.Controls.Add(this.txt_Start_FileName);
            this.groupBox_Start.Controls.Add(this.rb_Start_Proc);
            this.groupBox_Start.Controls.Add(this.label5);
            this.groupBox_Start.Location = new System.Drawing.Point(26, 257);
            this.groupBox_Start.Name = "groupBox_Start";
            this.groupBox_Start.Size = new System.Drawing.Size(449, 73);
            this.groupBox_Start.TabIndex = 14;
            this.groupBox_Start.TabStop = false;
            // 
            // rb_Start_Service
            // 
            this.rb_Start_Service.AutoSize = true;
            this.rb_Start_Service.Location = new System.Drawing.Point(111, 21);
            this.rb_Start_Service.Name = "rb_Start_Service";
            this.rb_Start_Service.Size = new System.Drawing.Size(65, 16);
            this.rb_Start_Service.TabIndex = 9;
            this.rb_Start_Service.Text = "Service";
            this.rb_Start_Service.UseVisualStyleBackColor = true;
            // 
            // txt_Start_FileName
            // 
            this.txt_Start_FileName.Location = new System.Drawing.Point(66, 40);
            this.txt_Start_FileName.Name = "txt_Start_FileName";
            this.txt_Start_FileName.Size = new System.Drawing.Size(326, 21);
            this.txt_Start_FileName.TabIndex = 11;
            this.txt_Start_FileName.Text = "SOPServer2";
            // 
            // rb_Start_Proc
            // 
            this.rb_Start_Proc.AutoSize = true;
            this.rb_Start_Proc.Checked = true;
            this.rb_Start_Proc.Location = new System.Drawing.Point(13, 21);
            this.rb_Start_Proc.Name = "rb_Start_Proc";
            this.rb_Start_Proc.Size = new System.Drawing.Size(70, 16);
            this.rb_Start_Proc.TabIndex = 8;
            this.rb_Start_Proc.TabStop = true;
            this.rb_Start_Proc.Text = "Process";
            this.rb_Start_Proc.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "파일명 : ";
            // 
            // groupBox_Down
            // 
            this.groupBox_Down.Controls.Add(this.btn_UploadOpenFolder);
            this.groupBox_Down.Controls.Add(this.btn_UploadOpenFile);
            this.groupBox_Down.Controls.Add(this.txt_Upload_Path);
            this.groupBox_Down.Controls.Add(this.label14);
            this.groupBox_Down.Controls.Add(this.label13);
            this.groupBox_Down.Controls.Add(this.txt_Upload_LocalPath);
            this.groupBox_Down.Controls.Add(this.label2);
            this.groupBox_Down.Location = new System.Drawing.Point(26, 140);
            this.groupBox_Down.Name = "groupBox_Down";
            this.groupBox_Down.Size = new System.Drawing.Size(449, 82);
            this.groupBox_Down.TabIndex = 13;
            this.groupBox_Down.TabStop = false;
            // 
            // btn_UploadOpenFolder
            // 
            this.btn_UploadOpenFolder.Location = new System.Drawing.Point(396, 19);
            this.btn_UploadOpenFolder.Name = "btn_UploadOpenFolder";
            this.btn_UploadOpenFolder.Size = new System.Drawing.Size(45, 23);
            this.btn_UploadOpenFolder.TabIndex = 28;
            this.btn_UploadOpenFolder.Text = "폴더";
            this.btn_UploadOpenFolder.UseVisualStyleBackColor = true;
            this.btn_UploadOpenFolder.Click += new System.EventHandler(this.btn_UploadOpenFolder_Click);
            // 
            // btn_UploadOpenFile
            // 
            this.btn_UploadOpenFile.Location = new System.Drawing.Point(348, 19);
            this.btn_UploadOpenFile.Name = "btn_UploadOpenFile";
            this.btn_UploadOpenFile.Size = new System.Drawing.Size(45, 23);
            this.btn_UploadOpenFile.TabIndex = 27;
            this.btn_UploadOpenFile.Text = "파일";
            this.btn_UploadOpenFile.UseVisualStyleBackColor = true;
            this.btn_UploadOpenFile.Click += new System.EventHandler(this.btn_OpenFile_Click);
            // 
            // txt_Upload_Path
            // 
            this.txt_Upload_Path.Location = new System.Drawing.Point(91, 47);
            this.txt_Upload_Path.Name = "txt_Upload_Path";
            this.txt_Upload_Path.Size = new System.Drawing.Size(350, 21);
            this.txt_Upload_Path.TabIndex = 8;
            this.txt_Upload_Path.Text = "D:\\SDMSServer";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(84, 12);
            this.label14.TabIndex = 9;
            this.label14.Text = "Upload path : ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(266, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 12);
            this.label13.TabIndex = 7;
            // 
            // txt_Upload_LocalPath
            // 
            this.txt_Upload_LocalPath.Location = new System.Drawing.Point(91, 20);
            this.txt_Upload_LocalPath.Name = "txt_Upload_LocalPath";
            this.txt_Upload_LocalPath.ReadOnly = true;
            this.txt_Upload_LocalPath.Size = new System.Drawing.Size(255, 21);
            this.txt_Upload_LocalPath.TabIndex = 4;
            this.txt_Upload_LocalPath.Text = "D:\\UnESolution\\bin\\SOP\\SDMSServer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Upload 파일 : ";
            // 
            // groupBox_Kill
            // 
            this.groupBox_Kill.Controls.Add(this.rb_Kill_Service);
            this.groupBox_Kill.Controls.Add(this.txt_Kill_FileName);
            this.groupBox_Kill.Controls.Add(this.rb_Kill_Proc);
            this.groupBox_Kill.Controls.Add(this.label4);
            this.groupBox_Kill.Location = new System.Drawing.Point(26, 27);
            this.groupBox_Kill.Name = "groupBox_Kill";
            this.groupBox_Kill.Size = new System.Drawing.Size(449, 74);
            this.groupBox_Kill.TabIndex = 12;
            this.groupBox_Kill.TabStop = false;
            // 
            // rb_Kill_Service
            // 
            this.rb_Kill_Service.AutoSize = true;
            this.rb_Kill_Service.Location = new System.Drawing.Point(111, 20);
            this.rb_Kill_Service.Name = "rb_Kill_Service";
            this.rb_Kill_Service.Size = new System.Drawing.Size(65, 16);
            this.rb_Kill_Service.TabIndex = 9;
            this.rb_Kill_Service.Text = "Service";
            this.rb_Kill_Service.UseVisualStyleBackColor = true;
            this.rb_Kill_Service.CheckedChanged += new System.EventHandler(this.rb_Kill_CheckedChanged);
            // 
            // txt_Kill_FileName
            // 
            this.txt_Kill_FileName.Location = new System.Drawing.Point(66, 39);
            this.txt_Kill_FileName.Name = "txt_Kill_FileName";
            this.txt_Kill_FileName.Size = new System.Drawing.Size(326, 21);
            this.txt_Kill_FileName.TabIndex = 11;
            this.txt_Kill_FileName.Text = "SOPServer2";
            // 
            // rb_Kill_Proc
            // 
            this.rb_Kill_Proc.AutoSize = true;
            this.rb_Kill_Proc.Checked = true;
            this.rb_Kill_Proc.Location = new System.Drawing.Point(13, 20);
            this.rb_Kill_Proc.Name = "rb_Kill_Proc";
            this.rb_Kill_Proc.Size = new System.Drawing.Size(70, 16);
            this.rb_Kill_Proc.TabIndex = 8;
            this.rb_Kill_Proc.TabStop = true;
            this.rb_Kill_Proc.Text = "Process";
            this.rb_Kill_Proc.UseVisualStyleBackColor = true;
            this.rb_Kill_Proc.CheckedChanged += new System.EventHandler(this.rb_Kill_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "파일명 : ";
            // 
            // chk_Start
            // 
            this.chk_Start.AutoSize = true;
            this.chk_Start.Checked = true;
            this.chk_Start.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Start.Location = new System.Drawing.Point(12, 235);
            this.chk_Start.Name = "chk_Start";
            this.chk_Start.Size = new System.Drawing.Size(49, 16);
            this.chk_Start.TabIndex = 2;
            this.chk_Start.Text = "Start";
            this.chk_Start.UseVisualStyleBackColor = true;
            this.chk_Start.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // chk_Upload
            // 
            this.chk_Upload.AutoSize = true;
            this.chk_Upload.Checked = true;
            this.chk_Upload.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Upload.Location = new System.Drawing.Point(12, 117);
            this.chk_Upload.Name = "chk_Upload";
            this.chk_Upload.Size = new System.Drawing.Size(63, 16);
            this.chk_Upload.TabIndex = 1;
            this.chk_Upload.Text = "Upload";
            this.chk_Upload.UseVisualStyleBackColor = true;
            this.chk_Upload.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // chk_Kill
            // 
            this.chk_Kill.AutoSize = true;
            this.chk_Kill.Checked = true;
            this.chk_Kill.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Kill.Location = new System.Drawing.Point(12, 12);
            this.chk_Kill.Name = "chk_Kill";
            this.chk_Kill.Size = new System.Drawing.Size(41, 16);
            this.chk_Kill.TabIndex = 0;
            this.chk_Kill.Text = "Kill";
            this.chk_Kill.UseVisualStyleBackColor = true;
            this.chk_Kill.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
            // 
            // btn_SendCommand
            // 
            this.btn_SendCommand.Location = new System.Drawing.Point(447, 12);
            this.btn_SendCommand.Name = "btn_SendCommand";
            this.btn_SendCommand.Size = new System.Drawing.Size(45, 23);
            this.btn_SendCommand.TabIndex = 7;
            this.btn_SendCommand.Text = "전송";
            this.btn_SendCommand.UseVisualStyleBackColor = true;
            this.btn_SendCommand.Click += new System.EventHandler(this.btn_SendCommand_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txt_SiteId);
            this.groupBox1.Controls.Add(this.label_isConnect);
            this.groupBox1.Controls.Add(this.btn_Connect);
            this.groupBox1.Location = new System.Drawing.Point(537, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 82);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DB Connect Info";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "Site ID : ";
            // 
            // txt_SiteId
            // 
            this.txt_SiteId.Location = new System.Drawing.Point(78, 28);
            this.txt_SiteId.Name = "txt_SiteId";
            this.txt_SiteId.Size = new System.Drawing.Size(52, 21);
            this.txt_SiteId.TabIndex = 10;
            this.txt_SiteId.Text = "1";
            // 
            // label_isConnect
            // 
            this.label_isConnect.AutoSize = true;
            this.label_isConnect.ForeColor = System.Drawing.Color.Red;
            this.label_isConnect.Location = new System.Drawing.Point(19, 60);
            this.label_isConnect.Name = "label_isConnect";
            this.label_isConnect.Size = new System.Drawing.Size(44, 12);
            this.label_isConnect.TabIndex = 24;
            this.label_isConnect.Text = "label13";
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(136, 27);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(75, 23);
            this.btn_Connect.TabIndex = 17;
            this.btn_Connect.Text = "연결";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // txt_GetProcName
            // 
            this.txt_GetProcName.Location = new System.Drawing.Point(75, 23);
            this.txt_GetProcName.Name = "txt_GetProcName";
            this.txt_GetProcName.Size = new System.Drawing.Size(354, 21);
            this.txt_GetProcName.TabIndex = 18;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1, 28);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 19;
            this.label12.Text = "프로세스명 : ";
            // 
            // chk_GetAllProc
            // 
            this.chk_GetAllProc.AutoSize = true;
            this.chk_GetAllProc.Checked = true;
            this.chk_GetAllProc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_GetAllProc.Location = new System.Drawing.Point(4, 5);
            this.chk_GetAllProc.Name = "chk_GetAllProc";
            this.chk_GetAllProc.Size = new System.Drawing.Size(48, 16);
            this.chk_GetAllProc.TabIndex = 20;
            this.chk_GetAllProc.Text = "전체";
            this.chk_GetAllProc.UseVisualStyleBackColor = true;
            this.chk_GetAllProc.CheckedChanged += new System.EventHandler(this.chk_GetAllProc_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "Download 경로 : ";
            // 
            // txt_Down_Path
            // 
            this.txt_Down_Path.Location = new System.Drawing.Point(103, 4);
            this.txt_Down_Path.Name = "txt_Down_Path";
            this.txt_Down_Path.Size = new System.Drawing.Size(326, 21);
            this.txt_Down_Path.TabIndex = 22;
            // 
            // txt_AgentPath
            // 
            this.txt_AgentPath.Location = new System.Drawing.Point(3, 3);
            this.txt_AgentPath.Name = "txt_AgentPath";
            this.txt_AgentPath.ReadOnly = true;
            this.txt_AgentPath.Size = new System.Drawing.Size(426, 21);
            this.txt_AgentPath.TabIndex = 25;
            // 
            // btn_AgentOpenFile
            // 
            this.btn_AgentOpenFile.Location = new System.Drawing.Point(435, 2);
            this.btn_AgentOpenFile.Name = "btn_AgentOpenFile";
            this.btn_AgentOpenFile.Size = new System.Drawing.Size(45, 23);
            this.btn_AgentOpenFile.TabIndex = 26;
            this.btn_AgentOpenFile.Text = "찾기";
            this.btn_AgentOpenFile.UseVisualStyleBackColor = true;
            this.btn_AgentOpenFile.Click += new System.EventHandler(this.btn_OpenFile_Click);
            // 
            // btn_DownloadLogFile
            // 
            this.btn_DownloadLogFile.Location = new System.Drawing.Point(669, 196);
            this.btn_DownloadLogFile.Name = "btn_DownloadLogFile";
            this.btn_DownloadLogFile.Size = new System.Drawing.Size(143, 23);
            this.btn_DownloadLogFile.TabIndex = 27;
            this.btn_DownloadLogFile.Text = "Log파일 보기";
            this.btn_DownloadLogFile.UseVisualStyleBackColor = true;
            this.btn_DownloadLogFile.Click += new System.EventHandler(this.btn_DownloadLogFile_Click);
            // 
            // txt_DownloadLocalPath
            // 
            this.txt_DownloadLocalPath.Location = new System.Drawing.Point(537, 121);
            this.txt_DownloadLocalPath.Name = "txt_DownloadLocalPath";
            this.txt_DownloadLocalPath.Size = new System.Drawing.Size(275, 21);
            this.txt_DownloadLocalPath.TabIndex = 29;
            this.txt_DownloadLocalPath.Text = "C:\\DownloadTemp";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(535, 105);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(201, 12);
            this.label15.TabIndex = 30;
            this.label15.Text = "1. (로컬) 다운로드 받을 로컬 경로 : ";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(535, 150);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(149, 12);
            this.label17.TabIndex = 34;
            this.label17.Text = "2. (서버) 로그 파일 경로 : ";
            // 
            // txt_LogFilePath
            // 
            this.txt_LogFilePath.Location = new System.Drawing.Point(538, 169);
            this.txt_LogFilePath.Name = "txt_LogFilePath";
            this.txt_LogFilePath.Size = new System.Drawing.Size(275, 21);
            this.txt_LogFilePath.TabIndex = 33;
            this.txt_LogFilePath.Text = "C:\\UnE\\Log\\Agent";
            // 
            // btn_SDMSUpdateOpenFile
            // 
            this.btn_SDMSUpdateOpenFile.Location = new System.Drawing.Point(435, 4);
            this.btn_SDMSUpdateOpenFile.Name = "btn_SDMSUpdateOpenFile";
            this.btn_SDMSUpdateOpenFile.Size = new System.Drawing.Size(45, 23);
            this.btn_SDMSUpdateOpenFile.TabIndex = 41;
            this.btn_SDMSUpdateOpenFile.Text = "찾기";
            this.btn_SDMSUpdateOpenFile.UseVisualStyleBackColor = true;
            this.btn_SDMSUpdateOpenFile.Click += new System.EventHandler(this.btn_SDMSUpdateOpenFile_Click);
            // 
            // txt_SDMSUpdate_LocalPath
            // 
            this.txt_SDMSUpdate_LocalPath.Location = new System.Drawing.Point(81, 5);
            this.txt_SDMSUpdate_LocalPath.Name = "txt_SDMSUpdate_LocalPath";
            this.txt_SDMSUpdate_LocalPath.ReadOnly = true;
            this.txt_SDMSUpdate_LocalPath.Size = new System.Drawing.Size(348, 21);
            this.txt_SDMSUpdate_LocalPath.TabIndex = 39;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(1, 11);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(84, 12);
            this.label19.TabIndex = 40;
            this.label19.Text = "Update 파일 : ";
            // 
            // cbCommand
            // 
            this.cbCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbCommand.FormattingEnabled = true;
            this.cbCommand.Location = new System.Drawing.Point(12, 12);
            this.cbCommand.Name = "cbCommand";
            this.cbCommand.Size = new System.Drawing.Size(429, 26);
            this.cbCommand.TabIndex = 44;
            this.cbCommand.SelectedIndexChanged += new System.EventHandler(this.cbCommand_SelectedIndexChanged);
            // 
            // pnAgentUpdate
            // 
            this.pnAgentUpdate.Controls.Add(this.btn_AgentOpenFile);
            this.pnAgentUpdate.Controls.Add(this.txt_AgentPath);
            this.pnAgentUpdate.Location = new System.Drawing.Point(12, 44);
            this.pnAgentUpdate.Name = "pnAgentUpdate";
            this.pnAgentUpdate.Size = new System.Drawing.Size(494, 27);
            this.pnAgentUpdate.TabIndex = 45;
            // 
            // pnGetProcList
            // 
            this.pnGetProcList.Controls.Add(this.txt_GetProcName);
            this.pnGetProcList.Controls.Add(this.label12);
            this.pnGetProcList.Controls.Add(this.chk_GetAllProc);
            this.pnGetProcList.Location = new System.Drawing.Point(12, 82);
            this.pnGetProcList.Name = "pnGetProcList";
            this.pnGetProcList.Size = new System.Drawing.Size(494, 49);
            this.pnGetProcList.TabIndex = 46;
            // 
            // pnGetFileList
            // 
            this.pnGetFileList.Controls.Add(this.label25);
            this.pnGetFileList.Controls.Add(this.txt_GetFileListPath);
            this.pnGetFileList.Controls.Add(this.label1);
            this.pnGetFileList.Location = new System.Drawing.Point(524, 300);
            this.pnGetFileList.Name = "pnGetFileList";
            this.pnGetFileList.Size = new System.Drawing.Size(494, 57);
            this.pnGetFileList.TabIndex = 47;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(3, 32);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(427, 12);
            this.label25.TabIndex = 5;
            this.label25.Text = ", 로 구분하여 여러개의 디렉토리를 확인 가능 / C:\\ 와 같은 루트 경로는 안됨";
            // 
            // pnDownload
            // 
            this.pnDownload.Controls.Add(this.lblDownloadLog);
            this.pnDownload.Controls.Add(this.label20);
            this.pnDownload.Controls.Add(this.label3);
            this.pnDownload.Controls.Add(this.txt_Down_Path);
            this.pnDownload.Location = new System.Drawing.Point(12, 134);
            this.pnDownload.Name = "pnDownload";
            this.pnDownload.Size = new System.Drawing.Size(494, 56);
            this.pnDownload.TabIndex = 48;
            // 
            // lblDownloadLog
            // 
            this.lblDownloadLog.AutoSize = true;
            this.lblDownloadLog.Location = new System.Drawing.Point(48, 36);
            this.lblDownloadLog.Name = "lblDownloadLog";
            this.lblDownloadLog.Size = new System.Drawing.Size(11, 12);
            this.lblDownloadLog.TabIndex = 38;
            this.lblDownloadLog.Text = "?";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(1, 36);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 12);
            this.label20.TabIndex = 37;
            this.label20.Text = "결과 : ";
            // 
            // pnSdmsUpdate
            // 
            this.pnSdmsUpdate.Controls.Add(this.btnSDMSUpdateNow);
            this.pnSdmsUpdate.Controls.Add(this.btnNewUpdateXML);
            this.pnSdmsUpdate.Controls.Add(this.btnSDMSUpdateShowXML);
            this.pnSdmsUpdate.Controls.Add(this.label7);
            this.pnSdmsUpdate.Controls.Add(this.txt_SDMSUpdate_LocalPath);
            this.pnSdmsUpdate.Controls.Add(this.label19);
            this.pnSdmsUpdate.Controls.Add(this.btn_SDMSUpdateOpenFile);
            this.pnSdmsUpdate.Location = new System.Drawing.Point(12, 196);
            this.pnSdmsUpdate.Name = "pnSdmsUpdate";
            this.pnSdmsUpdate.Size = new System.Drawing.Size(494, 88);
            this.pnSdmsUpdate.TabIndex = 49;
            // 
            // btnSDMSUpdateNow
            // 
            this.btnSDMSUpdateNow.Location = new System.Drawing.Point(39, 32);
            this.btnSDMSUpdateNow.Name = "btnSDMSUpdateNow";
            this.btnSDMSUpdateNow.Size = new System.Drawing.Size(143, 23);
            this.btnSDMSUpdateNow.TabIndex = 53;
            this.btnSDMSUpdateNow.Text = "SDMS 즉시 Update";
            this.btnSDMSUpdateNow.UseVisualStyleBackColor = true;
            this.btnSDMSUpdateNow.Click += new System.EventHandler(this.btnSDMSUpdateNow_Click);
            // 
            // btnSDMSUpdateShowXML
            // 
            this.btnSDMSUpdateShowXML.Location = new System.Drawing.Point(337, 33);
            this.btnSDMSUpdateShowXML.Name = "btnSDMSUpdateShowXML";
            this.btnSDMSUpdateShowXML.Size = new System.Drawing.Size(143, 23);
            this.btnSDMSUpdateShowXML.TabIndex = 52;
            this.btnSDMSUpdateShowXML.Text = "Update.xml 보기";
            this.btnSDMSUpdateShowXML.UseVisualStyleBackColor = true;
            this.btnSDMSUpdateShowXML.Click += new System.EventHandler(this.btnSDMSUpdateShowXML_Click);
            // 
            // pnFileCopy
            // 
            this.pnFileCopy.Controls.Add(this.label24);
            this.pnFileCopy.Controls.Add(this.label22);
            this.pnFileCopy.Controls.Add(this.chkFileCopyDelete);
            this.pnFileCopy.Controls.Add(this.txtFileCopyDestFileName);
            this.pnFileCopy.Controls.Add(this.label21);
            this.pnFileCopy.Controls.Add(this.txtFileCopySourceFileName);
            this.pnFileCopy.Controls.Add(this.label23);
            this.pnFileCopy.Location = new System.Drawing.Point(972, 10);
            this.pnFileCopy.Name = "pnFileCopy";
            this.pnFileCopy.Size = new System.Drawing.Size(494, 134);
            this.pnFileCopy.TabIndex = 51;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.Color.Red;
            this.label24.Location = new System.Drawing.Point(418, 110);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(65, 12);
            this.label24.TabIndex = 23;
            this.label24.Text = "파일명까지";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label22.Location = new System.Drawing.Point(5, 77);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(374, 48);
            this.label22.TabIndex = 22;
            this.label22.Text = "예)\r\n1. 폴더 ]  출발지 D:\\test  /  목적지 D:\\test1\r\n2. 파일 ]  출발지 D:\\test\\3.022.zip /  목적지 " +
    "D:\\test1\\3.022.zip\r\n";
            // 
            // chkFileCopyDelete
            // 
            this.chkFileCopyDelete.AutoSize = true;
            this.chkFileCopyDelete.Checked = true;
            this.chkFileCopyDelete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFileCopyDelete.Location = new System.Drawing.Point(7, 56);
            this.chkFileCopyDelete.Name = "chkFileCopyDelete";
            this.chkFileCopyDelete.Size = new System.Drawing.Size(172, 16);
            this.chkFileCopyDelete.TabIndex = 21;
            this.chkFileCopyDelete.Text = "복사 후 출발지 파일 지우기";
            this.chkFileCopyDelete.UseVisualStyleBackColor = true;
            // 
            // txtFileCopyDestFileName
            // 
            this.txtFileCopyDestFileName.Location = new System.Drawing.Point(52, 31);
            this.txtFileCopyDestFileName.Name = "txtFileCopyDestFileName";
            this.txtFileCopyDestFileName.Size = new System.Drawing.Size(377, 21);
            this.txtFileCopyDestFileName.TabIndex = 12;
            this.txtFileCopyDestFileName.Text = "D:\\test\\1";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(5, 37);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(41, 12);
            this.label21.TabIndex = 13;
            this.label21.Text = "목적지";
            // 
            // txtFileCopySourceFileName
            // 
            this.txtFileCopySourceFileName.Location = new System.Drawing.Point(52, 4);
            this.txtFileCopySourceFileName.Name = "txtFileCopySourceFileName";
            this.txtFileCopySourceFileName.Size = new System.Drawing.Size(377, 21);
            this.txtFileCopySourceFileName.TabIndex = 10;
            this.txtFileCopySourceFileName.Text = "D:\\test\\2\\1-1";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(5, 10);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(41, 12);
            this.label23.TabIndex = 11;
            this.label23.Text = "출발지";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(972, 150);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(480, 288);
            this.treeView1.TabIndex = 52;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // btnRefreshDirectory
            // 
            this.btnRefreshDirectory.Location = new System.Drawing.Point(972, 442);
            this.btnRefreshDirectory.Name = "btnRefreshDirectory";
            this.btnRefreshDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshDirectory.TabIndex = 53;
            this.btnRefreshDirectory.Text = "새로고침";
            this.btnRefreshDirectory.UseVisualStyleBackColor = true;
            this.btnRefreshDirectory.Click += new System.EventHandler(this.btnRefreshDirectory_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "file.jpg");
            // 
            // txtDirectoryPath
            // 
            this.txtDirectoryPath.Location = new System.Drawing.Point(1131, 442);
            this.txtDirectoryPath.Name = "txtDirectoryPath";
            this.txtDirectoryPath.ReadOnly = true;
            this.txtDirectoryPath.Size = new System.Drawing.Size(321, 21);
            this.txtDirectoryPath.TabIndex = 54;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(535, 228);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 12);
            this.label6.TabIndex = 56;
            this.label6.Text = "2. (서버) 업로드 경로 : ";
            // 
            // txt_UploadServerPath
            // 
            this.txt_UploadServerPath.Location = new System.Drawing.Point(538, 247);
            this.txt_UploadServerPath.Name = "txt_UploadServerPath";
            this.txt_UploadServerPath.Size = new System.Drawing.Size(275, 21);
            this.txt_UploadServerPath.TabIndex = 55;
            this.txt_UploadServerPath.Text = "C:\\UNE\\Upload";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(10, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(397, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "Update할 zip 파일에는 반드시 Update.xml 파일이 포함되어야만 합니다.";
            // 
            // btnNewUpdateXML
            // 
            this.btnNewUpdateXML.Location = new System.Drawing.Point(188, 32);
            this.btnNewUpdateXML.Name = "btnNewUpdateXML";
            this.btnNewUpdateXML.Size = new System.Drawing.Size(143, 23);
            this.btnNewUpdateXML.TabIndex = 52;
            this.btnNewUpdateXML.Text = "Update.xml 만들기";
            this.btnNewUpdateXML.UseVisualStyleBackColor = true;
            this.btnNewUpdateXML.Click += new System.EventHandler(this.btnNewUpdateXML_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1520, 513);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt_UploadServerPath);
            this.Controls.Add(this.txtDirectoryPath);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnRefreshDirectory);
            this.Controls.Add(this.pnFileCopy);
            this.Controls.Add(this.pnSdmsUpdate);
            this.Controls.Add(this.pnDownload);
            this.Controls.Add(this.pnGetFileList);
            this.Controls.Add(this.pnGetProcList);
            this.Controls.Add(this.pnAgentUpdate);
            this.Controls.Add(this.cbCommand);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txt_LogFilePath);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txt_DownloadLocalPath);
            this.Controls.Add(this.btn_DownloadLogFile);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_SendCommand);
            this.Controls.Add(this.pnUpdate);
            this.Name = "MainForm";
            this.Text = "Commander";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.pnUpdate.ResumeLayout(false);
            this.pnUpdate.PerformLayout();
            this.groupBox_Start.ResumeLayout(false);
            this.groupBox_Start.PerformLayout();
            this.groupBox_Down.ResumeLayout(false);
            this.groupBox_Down.PerformLayout();
            this.groupBox_Kill.ResumeLayout(false);
            this.groupBox_Kill.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnAgentUpdate.ResumeLayout(false);
            this.pnAgentUpdate.PerformLayout();
            this.pnGetProcList.ResumeLayout(false);
            this.pnGetProcList.PerformLayout();
            this.pnGetFileList.ResumeLayout(false);
            this.pnGetFileList.PerformLayout();
            this.pnDownload.ResumeLayout(false);
            this.pnDownload.PerformLayout();
            this.pnSdmsUpdate.ResumeLayout(false);
            this.pnSdmsUpdate.PerformLayout();
            this.pnFileCopy.ResumeLayout(false);
            this.pnFileCopy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txt_GetFileListPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Upload_LocalPath;
        private System.Windows.Forms.CheckBox chk_Start;
        private System.Windows.Forms.CheckBox chk_Upload;
        private System.Windows.Forms.CheckBox chk_Kill;
        private System.Windows.Forms.GroupBox groupBox_Kill;
        private System.Windows.Forms.TextBox txt_Kill_FileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rb_Kill_Service;
        private System.Windows.Forms.RadioButton rb_Kill_Proc;
        private System.Windows.Forms.GroupBox groupBox_Start;
        private System.Windows.Forms.RadioButton rb_Start_Service;
        private System.Windows.Forms.TextBox txt_Start_FileName;
        private System.Windows.Forms.RadioButton rb_Start_Proc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox_Down;
        private System.Windows.Forms.Button btn_SendCommand;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_SiteId;
        private System.Windows.Forms.TextBox txt_GetProcName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chk_GetAllProc;
        private System.Windows.Forms.Label label_isConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Down_Path;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_Upload_Path;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt_AgentPath;
        private System.Windows.Forms.Button btn_UploadOpenFile;
        private System.Windows.Forms.Button btn_AgentOpenFile;
        private System.Windows.Forms.Button btn_DownloadLogFile;
        private System.Windows.Forms.TextBox txt_DownloadLocalPath;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt_LogFilePath;
        private System.Windows.Forms.Button btn_SDMSUpdateOpenFile;
        private System.Windows.Forms.TextBox txt_SDMSUpdate_LocalPath;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cbCommand;
        private System.Windows.Forms.Panel pnAgentUpdate;
        private System.Windows.Forms.Panel pnGetProcList;
        private System.Windows.Forms.Panel pnGetFileList;
        private System.Windows.Forms.Panel pnDownload;
        private System.Windows.Forms.Panel pnSdmsUpdate;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label lblDownloadLog;
        private System.Windows.Forms.Panel pnFileCopy;
        private System.Windows.Forms.TextBox txtFileCopyDestFileName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtFileCopySourceFileName;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox chkFileCopyDelete;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnSDMSUpdateShowXML;
        private System.Windows.Forms.Button btnSDMSUpdateNow;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button btnRefreshDirectory;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox txtDirectoryPath;
        private System.Windows.Forms.Button btn_UploadOpenFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_UploadServerPath;
        private System.Windows.Forms.Button btnNewUpdateXML;
        private System.Windows.Forms.Label label7;
    }
}

