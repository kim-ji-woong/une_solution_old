namespace CCTV_WESP_Sample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUserID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPW = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboChannels = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboFR = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboResolution = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnGetServerTime = new System.Windows.Forms.Button();
            this.btnPlayVideoEx = new System.Windows.Forms.Button();
            this.btnPlayVideo = new System.Windows.Forms.Button();
            this.btnStopVideo = new System.Windows.Forms.Button();
            this.btnStartRecording = new System.Windows.Forms.Button();
            this.btnEndRecording = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelServerTime = new System.Windows.Forms.Label();
            this.axWESPMonitorCtrl1 = new AxWESPMONITORLib.AxWESPMonitorCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.axWESPMonitorCtrl1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(593, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(677, 44);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(123, 20);
            this.textBoxIP.TabIndex = 2;
            this.textBoxIP.Text = "192.168.30.50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(593, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(677, 73);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(123, 20);
            this.textBoxPort.TabIndex = 2;
            this.textBoxPort.Text = "80";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(593, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "User ID";
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.Location = new System.Drawing.Point(677, 102);
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(123, 20);
            this.textBoxUserID.TabIndex = 2;
            this.textBoxUserID.Text = "admin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(593, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Password";
            // 
            // textBoxPW
            // 
            this.textBoxPW.Location = new System.Drawing.Point(677, 132);
            this.textBoxPW.Name = "textBoxPW";
            this.textBoxPW.PasswordChar = '*';
            this.textBoxPW.Size = new System.Drawing.Size(123, 20);
            this.textBoxPW.TabIndex = 2;
            this.textBoxPW.Text = "Gw@nggyo03";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(593, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Channel";
            // 
            // cboChannels
            // 
            this.cboChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChannels.FormattingEnabled = true;
            this.cboChannels.Items.AddRange(new object[] {
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
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cboChannels.Location = new System.Drawing.Point(677, 191);
            this.cboChannels.Name = "cboChannels";
            this.cboChannels.Size = new System.Drawing.Size(123, 21);
            this.cboChannels.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(593, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Frame Rate";
            // 
            // cboFR
            // 
            this.cboFR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFR.FormattingEnabled = true;
            this.cboFR.Items.AddRange(new object[] {
            "30",
            "15",
            "10",
            "5",
            "1",
            "Snapshot"});
            this.cboFR.Location = new System.Drawing.Point(677, 220);
            this.cboFR.Name = "cboFR";
            this.cboFR.Size = new System.Drawing.Size(123, 21);
            this.cboFR.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(593, 251);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Resolution";
            // 
            // cboResolution
            // 
            this.cboResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboResolution.FormattingEnabled = true;
            this.cboResolution.Items.AddRange(new object[] {
            "Lowest",
            "Low",
            "Normal",
            "High",
            "Highest"});
            this.cboResolution.Location = new System.Drawing.Point(677, 247);
            this.cboResolution.Name = "cboResolution";
            this.cboResolution.Size = new System.Drawing.Size(123, 21);
            this.cboResolution.TabIndex = 3;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(10, 366);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(64, 25);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "접속";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(80, 366);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(64, 25);
            this.btnDisconnect.TabIndex = 4;
            this.btnDisconnect.Text = "접속해제";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // btnGetServerTime
            // 
            this.btnGetServerTime.Location = new System.Drawing.Point(10, 398);
            this.btnGetServerTime.Name = "btnGetServerTime";
            this.btnGetServerTime.Size = new System.Drawing.Size(134, 25);
            this.btnGetServerTime.TabIndex = 4;
            this.btnGetServerTime.Text = "서버시간 받아오기";
            this.btnGetServerTime.UseVisualStyleBackColor = true;
            // 
            // btnPlayVideoEx
            // 
            this.btnPlayVideoEx.Location = new System.Drawing.Point(10, 429);
            this.btnPlayVideoEx.Name = "btnPlayVideoEx";
            this.btnPlayVideoEx.Size = new System.Drawing.Size(134, 25);
            this.btnPlayVideoEx.TabIndex = 4;
            this.btnPlayVideoEx.Text = "Play VideoEx";
            this.btnPlayVideoEx.UseVisualStyleBackColor = true;
            // 
            // btnPlayVideo
            // 
            this.btnPlayVideo.Location = new System.Drawing.Point(193, 366);
            this.btnPlayVideo.Name = "btnPlayVideo";
            this.btnPlayVideo.Size = new System.Drawing.Size(64, 25);
            this.btnPlayVideo.TabIndex = 4;
            this.btnPlayVideo.Text = "Play Video";
            this.btnPlayVideo.UseVisualStyleBackColor = true;
            this.btnPlayVideo.Click += new System.EventHandler(this.btnPlayVideo_Click);
            // 
            // btnStopVideo
            // 
            this.btnStopVideo.Location = new System.Drawing.Point(262, 366);
            this.btnStopVideo.Name = "btnStopVideo";
            this.btnStopVideo.Size = new System.Drawing.Size(64, 25);
            this.btnStopVideo.TabIndex = 4;
            this.btnStopVideo.Text = "Stop Video";
            this.btnStopVideo.UseVisualStyleBackColor = true;
            // 
            // btnStartRecording
            // 
            this.btnStartRecording.Location = new System.Drawing.Point(193, 398);
            this.btnStartRecording.Name = "btnStartRecording";
            this.btnStartRecording.Size = new System.Drawing.Size(64, 25);
            this.btnStartRecording.TabIndex = 4;
            this.btnStartRecording.Text = "녹화시작";
            this.btnStartRecording.UseVisualStyleBackColor = true;
            // 
            // btnEndRecording
            // 
            this.btnEndRecording.Location = new System.Drawing.Point(262, 398);
            this.btnEndRecording.Name = "btnEndRecording";
            this.btnEndRecording.Size = new System.Drawing.Size(64, 25);
            this.btnEndRecording.TabIndex = 4;
            this.btnEndRecording.Text = "녹화종료";
            this.btnEndRecording.UseVisualStyleBackColor = true;
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(193, 429);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(64, 25);
            this.btnSaveImage.TabIndex = 4;
            this.btnSaveImage.Text = "화면저장";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(494, 376);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(51, 13);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "상태정보";
            // 
            // labelServerTime
            // 
            this.labelServerTime.AutoSize = true;
            this.labelServerTime.Location = new System.Drawing.Point(494, 404);
            this.labelServerTime.Name = "labelServerTime";
            this.labelServerTime.Size = new System.Drawing.Size(51, 13);
            this.labelServerTime.TabIndex = 5;
            this.labelServerTime.Text = "서버시간";
            // 
            // axWESPMonitorCtrl1
            // 
            this.axWESPMonitorCtrl1.Enabled = true;
            this.axWESPMonitorCtrl1.Location = new System.Drawing.Point(12, 12);
            this.axWESPMonitorCtrl1.Name = "axWESPMonitorCtrl1";
            this.axWESPMonitorCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWESPMonitorCtrl1.OcxState")));
            this.axWESPMonitorCtrl1.Size = new System.Drawing.Size(491, 297);
            this.axWESPMonitorCtrl1.TabIndex = 0;
            this.axWESPMonitorCtrl1.AckReceived += new AxWESPMONITORLib._IWESPMonitorCtrlEvents_AckReceivedEventHandler(this.axWESPMonitorCtrl1_AckReceived);
            this.axWESPMonitorCtrl1.ErrorReceived += new AxWESPMONITORLib._IWESPMonitorCtrlEvents_ErrorReceivedEventHandler(this.axWESPMonitorCtrl1_ErrorReceived);
            this.axWESPMonitorCtrl1.ServerTimeReceived += new AxWESPMONITORLib._IWESPMonitorCtrlEvents_ServerTimeReceivedEventHandler(this.axWESPMonitorCtrl1_ServerTimeReceived);
            this.axWESPMonitorCtrl1.SizeChanged += new System.EventHandler(this.axWESPMonitorCtrl1_SizeChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 473);
            this.Controls.Add(this.labelServerTime);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnPlayVideoEx);
            this.Controls.Add(this.btnGetServerTime);
            this.Controls.Add(this.btnStopVideo);
            this.Controls.Add(this.btnEndRecording);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnStartRecording);
            this.Controls.Add(this.btnPlayVideo);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cboResolution);
            this.Controls.Add(this.cboFR);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboChannels);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxPW);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxUserID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.axWESPMonitorCtrl1);
            this.Name = "FormMain";
            this.Text = "Play Video";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axWESPMonitorCtrl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxWESPMONITORLib.AxWESPMonitorCtrl axWESPMonitorCtrl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxUserID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPW;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboChannels;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboFR;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboResolution;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnGetServerTime;
        private System.Windows.Forms.Button btnPlayVideoEx;
        private System.Windows.Forms.Button btnPlayVideo;
        private System.Windows.Forms.Button btnStopVideo;
        private System.Windows.Forms.Button btnStartRecording;
        private System.Windows.Forms.Button btnEndRecording;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelServerTime;
    }
}

