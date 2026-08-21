namespace MSMQTest
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
            this.btnSendSMS = new System.Windows.Forms.Button();
            this.btnGetAllMessage = new System.Windows.Forms.Button();
            this.btnCreateQueue = new System.Windows.Forms.Button();
            this.editMsgIter = new System.Windows.Forms.TextBox();
            this.editCallback = new System.Windows.Forms.TextBox();
            this.editMsg = new System.Windows.Forms.TextBox();
            this.cmbMysqlEncoding = new System.Windows.Forms.ComboBox();
            this.btnMysqlConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbStatusMysql = new System.Windows.Forms.Label();
            this.btnMysqlDisconnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.editMysqlServer = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFetchQueue = new System.Windows.Forms.Button();
            this.lbStatusQueue = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbMsgEncoding1 = new System.Windows.Forms.ComboBox();
            this.editMsgQueue = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSaveMsgToDB = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.editReciver = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnSaveRegistry = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbMsgEncoding2 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.Location = new System.Drawing.Point(51, 318);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Size = new System.Drawing.Size(100, 23);
            this.btnSendSMS.TabIndex = 0;
            this.btnSendSMS.Text = "MSMQ 보내기";
            this.btnSendSMS.UseVisualStyleBackColor = true;
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // btnGetAllMessage
            // 
            this.btnGetAllMessage.Location = new System.Drawing.Point(69, 124);
            this.btnGetAllMessage.Name = "btnGetAllMessage";
            this.btnGetAllMessage.Size = new System.Drawing.Size(75, 23);
            this.btnGetAllMessage.TabIndex = 1;
            this.btnGetAllMessage.Text = "가져오기";
            this.btnGetAllMessage.UseVisualStyleBackColor = true;
            this.btnGetAllMessage.Click += new System.EventHandler(this.btnGetAllMessage_Click);
            // 
            // btnCreateQueue
            // 
            this.btnCreateQueue.Location = new System.Drawing.Point(8, 124);
            this.btnCreateQueue.Name = "btnCreateQueue";
            this.btnCreateQueue.Size = new System.Drawing.Size(55, 23);
            this.btnCreateQueue.TabIndex = 2;
            this.btnCreateQueue.Text = "큐생성";
            this.btnCreateQueue.UseVisualStyleBackColor = true;
            this.btnCreateQueue.Click += new System.EventHandler(this.btnCreateQueue_Click);
            // 
            // editMsgIter
            // 
            this.editMsgIter.Location = new System.Drawing.Point(91, 279);
            this.editMsgIter.Name = "editMsgIter";
            this.editMsgIter.Size = new System.Drawing.Size(60, 21);
            this.editMsgIter.TabIndex = 3;
            this.editMsgIter.Text = "1";
            // 
            // editCallback
            // 
            this.editCallback.Location = new System.Drawing.Point(91, 26);
            this.editCallback.Name = "editCallback";
            this.editCallback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.editCallback.Size = new System.Drawing.Size(192, 21);
            this.editCallback.TabIndex = 4;
            this.editCallback.Text = "01052672290";
            // 
            // editMsg
            // 
            this.editMsg.Location = new System.Drawing.Point(91, 81);
            this.editMsg.Multiline = true;
            this.editMsg.Name = "editMsg";
            this.editMsg.Size = new System.Drawing.Size(192, 175);
            this.editMsg.TabIndex = 5;
            this.editMsg.Text = "[테스트]한글ABab1234";
            // 
            // cmbMysqlEncoding
            // 
            this.cmbMysqlEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMysqlEncoding.FormattingEnabled = true;
            this.cmbMysqlEncoding.Items.AddRange(new object[] {
            "KSC5601",
            "UTF-8",
            "ISO-8859-1"});
            this.cmbMysqlEncoding.Location = new System.Drawing.Point(66, 65);
            this.cmbMysqlEncoding.Name = "cmbMysqlEncoding";
            this.cmbMysqlEncoding.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbMysqlEncoding.Size = new System.Drawing.Size(150, 20);
            this.cmbMysqlEncoding.TabIndex = 6;
            this.cmbMysqlEncoding.SelectedIndexChanged += new System.EventHandler(this.cmbMysqlEncoding_SelectedIndexChanged);
            // 
            // btnMysqlConnect
            // 
            this.btnMysqlConnect.Location = new System.Drawing.Point(114, 102);
            this.btnMysqlConnect.Name = "btnMysqlConnect";
            this.btnMysqlConnect.Size = new System.Drawing.Size(98, 23);
            this.btnMysqlConnect.TabIndex = 7;
            this.btnMysqlConnect.Text = "접속하기";
            this.btnMysqlConnect.UseVisualStyleBackColor = true;
            this.btnMysqlConnect.Click += new System.EventHandler(this.btnMysqlConnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.lbStatusMysql);
            this.groupBox1.Controls.Add(this.btnMysqlDisconnect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.editMysqlServer);
            this.groupBox1.Controls.Add(this.cmbMysqlEncoding);
            this.groupBox1.Controls.Add(this.btnMysqlConnect);
            this.groupBox1.Location = new System.Drawing.Point(314, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 163);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMS 서버 접속 정보";
            // 
            // lbStatusMysql
            // 
            this.lbStatusMysql.AutoSize = true;
            this.lbStatusMysql.ForeColor = System.Drawing.Color.Red;
            this.lbStatusMysql.Location = new System.Drawing.Point(27, 140);
            this.lbStatusMysql.Name = "lbStatusMysql";
            this.lbStatusMysql.Size = new System.Drawing.Size(121, 12);
            this.lbStatusMysql.TabIndex = 13;
            this.lbStatusMysql.Text = "접속되지 않았습니다.";
            // 
            // btnMysqlDisconnect
            // 
            this.btnMysqlDisconnect.Enabled = false;
            this.btnMysqlDisconnect.Location = new System.Drawing.Point(24, 102);
            this.btnMysqlDisconnect.Name = "btnMysqlDisconnect";
            this.btnMysqlDisconnect.Size = new System.Drawing.Size(84, 23);
            this.btnMysqlDisconnect.TabIndex = 12;
            this.btnMysqlDisconnect.Text = "연결해제";
            this.btnMysqlDisconnect.UseVisualStyleBackColor = true;
            this.btnMysqlDisconnect.Click += new System.EventHandler(this.btnMysqlDisconnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "인코딩";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "서버";
            // 
            // editMysqlServer
            // 
            this.editMysqlServer.Location = new System.Drawing.Point(66, 31);
            this.editMysqlServer.Name = "editMysqlServer";
            this.editMysqlServer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.editMysqlServer.Size = new System.Drawing.Size(150, 21);
            this.editMysqlServer.TabIndex = 9;
            this.editMysqlServer.Text = "127.0.0.1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbMsgEncoding2);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.btnFetchQueue);
            this.groupBox2.Controls.Add(this.lbStatusQueue);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cmbMsgEncoding1);
            this.groupBox2.Controls.Add(this.editMsgQueue);
            this.groupBox2.Controls.Add(this.btnCreateQueue);
            this.groupBox2.Controls.Add(this.btnGetAllMessage);
            this.groupBox2.Location = new System.Drawing.Point(314, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 194);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Message Queue";
            // 
            // btnFetchQueue
            // 
            this.btnFetchQueue.Location = new System.Drawing.Point(150, 124);
            this.btnFetchQueue.Name = "btnFetchQueue";
            this.btnFetchQueue.Size = new System.Drawing.Size(75, 23);
            this.btnFetchQueue.TabIndex = 15;
            this.btnFetchQueue.Text = "큐감시";
            this.btnFetchQueue.UseVisualStyleBackColor = true;
            this.btnFetchQueue.Click += new System.EventHandler(this.btnFetchQueue_Click);
            // 
            // lbStatusQueue
            // 
            this.lbStatusQueue.AutoSize = true;
            this.lbStatusQueue.ForeColor = System.Drawing.Color.Red;
            this.lbStatusQueue.Location = new System.Drawing.Point(52, 163);
            this.lbStatusQueue.Name = "lbStatusQueue";
            this.lbStatusQueue.Size = new System.Drawing.Size(149, 12);
            this.lbStatusQueue.TabIndex = 14;
            this.lbStatusQueue.Text = "MQ 감시 해제 되었습니다.";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "인코딩";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "서버";
            // 
            // cmbMsgEncoding1
            // 
            this.cmbMsgEncoding1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMsgEncoding1.FormattingEnabled = true;
            this.cmbMsgEncoding1.Items.AddRange(new object[] {
            "KSC5601",
            "UTF-8",
            "ISO-8859-1"});
            this.cmbMsgEncoding1.Location = new System.Drawing.Point(66, 60);
            this.cmbMsgEncoding1.Name = "cmbMsgEncoding1";
            this.cmbMsgEncoding1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbMsgEncoding1.Size = new System.Drawing.Size(150, 20);
            this.cmbMsgEncoding1.TabIndex = 12;
            this.cmbMsgEncoding1.SelectedIndexChanged += new System.EventHandler(this.cmbMsgEncoding_SelectedIndexChanged);
            // 
            // editMsgQueue
            // 
            this.editMsgQueue.Location = new System.Drawing.Point(66, 27);
            this.editMsgQueue.Name = "editMsgQueue";
            this.editMsgQueue.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.editMsgQueue.Size = new System.Drawing.Size(150, 21);
            this.editMsgQueue.TabIndex = 9;
            this.editMsgQueue.Text = "192.168.0.195";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSaveMsgToDB);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.editReciver);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.editMsg);
            this.groupBox3.Controls.Add(this.editCallback);
            this.groupBox3.Controls.Add(this.editMsgIter);
            this.groupBox3.Controls.Add(this.btnSendSMS);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 363);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "문자 메세지";
            // 
            // btnSaveMsgToDB
            // 
            this.btnSaveMsgToDB.Location = new System.Drawing.Point(157, 318);
            this.btnSaveMsgToDB.Name = "btnSaveMsgToDB";
            this.btnSaveMsgToDB.Size = new System.Drawing.Size(100, 23);
            this.btnSaveMsgToDB.TabIndex = 18;
            this.btnSaveMsgToDB.Text = "DB에 보내기";
            this.btnSaveMsgToDB.UseVisualStyleBackColor = true;
            this.btnSaveMsgToDB.Click += new System.EventHandler(this.btnSaveMsgToDB_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(157, 282);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 17;
            this.label8.Text = "회";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 282);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "전송회수";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "전송 내용";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "받는 사람";
            // 
            // editReciver
            // 
            this.editReciver.Location = new System.Drawing.Point(91, 53);
            this.editReciver.Name = "editReciver";
            this.editReciver.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.editReciver.Size = new System.Drawing.Size(192, 21);
            this.editReciver.TabIndex = 13;
            this.editReciver.Text = "01043632290";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "보내는 사람";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(432, 395);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 27);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "종료하기";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSaveRegistry
            // 
            this.btnSaveRegistry.Location = new System.Drawing.Point(30, 395);
            this.btnSaveRegistry.Name = "btnSaveRegistry";
            this.btnSaveRegistry.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRegistry.TabIndex = 15;
            this.btnSaveRegistry.Text = "저장하기";
            this.btnSaveRegistry.UseVisualStyleBackColor = true;
            this.btnSaveRegistry.Click += new System.EventHandler(this.btnSaveRegistry_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 93);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 16;
            this.label10.Text = "인코딩";
            // 
            // cmbMsgEncoding2
            // 
            this.cmbMsgEncoding2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMsgEncoding2.FormattingEnabled = true;
            this.cmbMsgEncoding2.Items.AddRange(new object[] {
            "KSC5601",
            "UTF-8",
            "ISO-8859-1"});
            this.cmbMsgEncoding2.Location = new System.Drawing.Point(66, 90);
            this.cmbMsgEncoding2.Name = "cmbMsgEncoding2";
            this.cmbMsgEncoding2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cmbMsgEncoding2.Size = new System.Drawing.Size(150, 20);
            this.cmbMsgEncoding2.TabIndex = 17;
            this.cmbMsgEncoding2.SelectedIndexChanged += new System.EventHandler(this.cmbMsgEncoding2_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(171, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "문자셋";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 447);
            this.Controls.Add(this.btnSaveRegistry);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "KDNS SMS 테스트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSendSMS;
		private System.Windows.Forms.Button btnGetAllMessage;
		private System.Windows.Forms.Button btnCreateQueue;
		private System.Windows.Forms.TextBox editMsgIter;
        private System.Windows.Forms.TextBox editCallback;
        private System.Windows.Forms.TextBox editMsg;
        private System.Windows.Forms.ComboBox cmbMysqlEncoding;
        private System.Windows.Forms.Button btnMysqlConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox editMysqlServer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox editMsgQueue;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox editReciver;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbMsgEncoding1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnSaveRegistry;
        private System.Windows.Forms.Button btnMysqlDisconnect;
        private System.Windows.Forms.Button btnSaveMsgToDB;
        private System.Windows.Forms.Label lbStatusMysql;
        private System.Windows.Forms.Label lbStatusQueue;
        private System.Windows.Forms.Button btnFetchQueue;
        private System.Windows.Forms.ComboBox cmbMsgEncoding2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button1;
	}
}

