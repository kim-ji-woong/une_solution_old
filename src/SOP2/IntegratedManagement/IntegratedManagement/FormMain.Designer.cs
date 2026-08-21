namespace IntegratedManagement
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.panelLogin = new System.Windows.Forms.Panel();
			this.btnPasswordCheck = new System.Windows.Forms.Button();
			this.btnJoin = new System.Windows.Forms.Button();
			this.btnLogin = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.textBoxID = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnManager = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnLogout = new System.Windows.Forms.Button();
			this.btnPassChange = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.btn_SDMS = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.btn_MessageSend = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnTeamManager = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnMonitoring = new System.Windows.Forms.Button();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.lblCaption = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.axSkinFramework = new AxXtremeSkinFramework.AxSkinFramework();
			this.panelJoin = new System.Windows.Forms.Panel();
			this.textBoxPassword_J = new System.Windows.Forms.TextBox();
			this.textBoxPPassword_J = new System.Windows.Forms.TextBox();
			this.textBoxName_J = new System.Windows.Forms.TextBox();
			this.textBoxID_J = new System.Windows.Forms.TextBox();
			this.btnCancel_J = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.btnOK_J = new System.Windows.Forms.Button();
			this.panelCheck = new System.Windows.Forms.Panel();
			this.btnCancel_P = new System.Windows.Forms.Button();
			this.btnOK_P = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textBoxName_C = new System.Windows.Forms.TextBox();
			this.textBoxID_C = new System.Windows.Forms.TextBox();
			this.panelPassChange = new System.Windows.Forms.Panel();
			this.label15 = new System.Windows.Forms.Label();
			this.textBoxCheckPPass_c = new System.Windows.Forms.TextBox();
			this.btnCancel_C = new System.Windows.Forms.Button();
			this.btnOK_C = new System.Windows.Forms.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.textBoxCheckPass_c = new System.Windows.Forms.TextBox();
			this.textBoxPass_c = new System.Windows.Forms.TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.panelLogin.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).BeginInit();
			this.panelJoin.SuspendLayout();
			this.panelCheck.SuspendLayout();
			this.panelPassChange.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelLogin
			// 
			this.panelLogin.BackColor = System.Drawing.Color.Transparent;
			this.panelLogin.Controls.Add(this.btnPasswordCheck);
			this.panelLogin.Controls.Add(this.btnJoin);
			this.panelLogin.Controls.Add(this.btnLogin);
			this.panelLogin.Controls.Add(this.label2);
			this.panelLogin.Controls.Add(this.label1);
			this.panelLogin.Controls.Add(this.textBoxPassword);
			this.panelLogin.Controls.Add(this.textBoxID);
			this.panelLogin.Location = new System.Drawing.Point(279, 109);
			this.panelLogin.Name = "panelLogin";
			this.panelLogin.Size = new System.Drawing.Size(273, 115);
			this.panelLogin.TabIndex = 1;
			// 
			// btnPasswordCheck
			// 
			this.btnPasswordCheck.Location = new System.Drawing.Point(152, 91);
			this.btnPasswordCheck.Name = "btnPasswordCheck";
			this.btnPasswordCheck.Size = new System.Drawing.Size(108, 21);
			this.btnPasswordCheck.TabIndex = 6;
			this.btnPasswordCheck.Text = "비밀번호 찾기";
			this.btnPasswordCheck.UseVisualStyleBackColor = true;
			this.btnPasswordCheck.Click += new System.EventHandler(this.btnPasswordCheck_Click);
			// 
			// btnJoin
			// 
			this.btnJoin.Location = new System.Drawing.Point(79, 91);
			this.btnJoin.Name = "btnJoin";
			this.btnJoin.Size = new System.Drawing.Size(67, 21);
			this.btnJoin.TabIndex = 5;
			this.btnJoin.Text = "회원가입";
			this.btnJoin.UseVisualStyleBackColor = true;
			this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
			// 
			// btnLogin
			// 
			this.btnLogin.Location = new System.Drawing.Point(185, 33);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(75, 48);
			this.btnLogin.TabIndex = 4;
			this.btnLogin.Text = "로그인";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "비밀번호 :";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "아 이 디  :";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(79, 60);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '*';
			this.textBoxPassword.Size = new System.Drawing.Size(100, 21);
			this.textBoxPassword.TabIndex = 3;
			this.textBoxPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPassword_KeyPress);
			// 
			// textBoxID
			// 
			this.textBoxID.Location = new System.Drawing.Point(79, 33);
			this.textBoxID.Name = "textBoxID";
			this.textBoxID.Size = new System.Drawing.Size(100, 21);
			this.textBoxID.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.btnManager);
			this.groupBox1.Location = new System.Drawing.Point(9, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(130, 143);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label3.Location = new System.Drawing.Point(2, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(133, 23);
			this.label3.TabIndex = 3;
			this.label3.Text = "SOP 생성기";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnManager
			// 
			this.btnManager.BackgroundImage = global::IntegratedManagement.Properties.Resources.SOPManager_64;
			this.btnManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnManager.Location = new System.Drawing.Point(29, 26);
			this.btnManager.Name = "btnManager";
			this.btnManager.Size = new System.Drawing.Size(75, 75);
			this.btnManager.TabIndex = 2;
			this.btnManager.UseVisualStyleBackColor = true;
			this.btnManager.Click += new System.EventHandler(this.btnManager_Click);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.Transparent;
			this.panel2.Controls.Add(this.btnLogout);
			this.panel2.Controls.Add(this.btnPassChange);
			this.panel2.Controls.Add(this.groupBox5);
			this.panel2.Controls.Add(this.groupBox4);
			this.panel2.Controls.Add(this.groupBox3);
			this.panel2.Controls.Add(this.groupBox2);
			this.panel2.Controls.Add(this.groupBox1);
			this.panel2.Location = new System.Drawing.Point(14, 80);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(699, 178);
			this.panel2.TabIndex = 1;
			// 
			// btnLogout
			// 
			this.btnLogout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnLogout.Location = new System.Drawing.Point(423, 151);
			this.btnLogout.Name = "btnLogout";
			this.btnLogout.Size = new System.Drawing.Size(134, 22);
			this.btnLogout.TabIndex = 26;
			this.btnLogout.Text = "로그아웃";
			this.btnLogout.UseVisualStyleBackColor = true;
			this.btnLogout.Visible = false;
			this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
			// 
			// btnPassChange
			// 
			this.btnPassChange.Location = new System.Drawing.Point(563, 151);
			this.btnPassChange.Name = "btnPassChange";
			this.btnPassChange.Size = new System.Drawing.Size(130, 24);
			this.btnPassChange.TabIndex = 6;
			this.btnPassChange.Text = "비밀번호 변경";
			this.btnPassChange.UseVisualStyleBackColor = true;
			this.btnPassChange.Click += new System.EventHandler(this.btnPassChange_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.label16);
			this.groupBox5.Controls.Add(this.btn_SDMS);
			this.groupBox5.Location = new System.Drawing.Point(561, 3);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(130, 143);
			this.groupBox5.TabIndex = 5;
			this.groupBox5.TabStop = false;
			// 
			// label16
			// 
			this.label16.BackColor = System.Drawing.Color.Transparent;
			this.label16.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label16.Location = new System.Drawing.Point(2, 112);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(133, 23);
			this.label16.TabIndex = 3;
			this.label16.Text = "자동 화재탐지 시스템";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btn_SDMS
			// 
			this.btn_SDMS.BackgroundImage = global::IntegratedManagement.Properties.Resources.SDMS_Red_64;
			this.btn_SDMS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_SDMS.Location = new System.Drawing.Point(29, 26);
			this.btn_SDMS.Name = "btn_SDMS";
			this.btn_SDMS.Size = new System.Drawing.Size(75, 75);
			this.btn_SDMS.TabIndex = 7;
			this.btn_SDMS.UseVisualStyleBackColor = true;
			this.btn_SDMS.Click += new System.EventHandler(this.btn_SDMS_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.label6);
			this.groupBox4.Controls.Add(this.btn_MessageSend);
			this.groupBox4.Location = new System.Drawing.Point(425, 3);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(130, 143);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.Transparent;
			this.label6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label6.Location = new System.Drawing.Point(2, 112);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(133, 23);
			this.label6.TabIndex = 3;
			this.label6.Text = "메세지 전송";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btn_MessageSend
			// 
			this.btn_MessageSend.BackgroundImage = global::IntegratedManagement.Properties.Resources.Message_64;
			this.btn_MessageSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_MessageSend.Location = new System.Drawing.Point(29, 26);
			this.btn_MessageSend.Name = "btn_MessageSend";
			this.btn_MessageSend.Size = new System.Drawing.Size(75, 75);
			this.btn_MessageSend.TabIndex = 7;
			this.btn_MessageSend.UseVisualStyleBackColor = true;
			this.btn_MessageSend.Click += new System.EventHandler(this.btn_MessageSend_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.btnTeamManager);
			this.groupBox3.Location = new System.Drawing.Point(285, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(133, 143);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label5.Location = new System.Drawing.Point(2, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(133, 23);
			this.label5.TabIndex = 3;
			this.label5.Text = "SOP 조직관리 툴";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnTeamManager
			// 
			this.btnTeamManager.BackgroundImage = global::IntegratedManagement.Properties.Resources.TeamManagement_64;
			this.btnTeamManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnTeamManager.Location = new System.Drawing.Point(29, 26);
			this.btnTeamManager.Name = "btnTeamManager";
			this.btnTeamManager.Size = new System.Drawing.Size(75, 75);
			this.btnTeamManager.TabIndex = 2;
			this.btnTeamManager.UseVisualStyleBackColor = true;
			this.btnTeamManager.Click += new System.EventHandler(this.btnTeamManager_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.btnMonitoring);
			this.groupBox2.Location = new System.Drawing.Point(143, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(137, 143);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label4.Location = new System.Drawing.Point(2, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(133, 23);
			this.label4.TabIndex = 3;
			this.label4.Text = "SOP 모니터링시스템";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnMonitoring
			// 
			this.btnMonitoring.BackgroundImage = global::IntegratedManagement.Properties.Resources.Monitoring_64;
			this.btnMonitoring.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnMonitoring.Location = new System.Drawing.Point(29, 26);
			this.btnMonitoring.Name = "btnMonitoring";
			this.btnMonitoring.Size = new System.Drawing.Size(75, 75);
			this.btnMonitoring.TabIndex = 2;
			this.btnMonitoring.UseVisualStyleBackColor = true;
			this.btnMonitoring.Click += new System.EventHandler(this.btnMonitoring_Click);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Text = "notifyIcon1";
			this.notifyIcon1.Visible = true;
			// 
			// lblCaption
			// 
			this.lblCaption.BackColor = System.Drawing.Color.Transparent;
			this.lblCaption.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
			this.lblCaption.Location = new System.Drawing.Point(23, 21);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblCaption.Size = new System.Drawing.Size(459, 35);
			this.lblCaption.TabIndex = 0;
			this.lblCaption.Text = "통합관리 시스템";
			this.lblCaption.Visible = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.Image = global::IntegratedManagement.Properties.Resources.pictureBox1_Image;
			this.pictureBox1.Location = new System.Drawing.Point(14, 57);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(439, 17);
			this.pictureBox1.TabIndex = 23;
			this.pictureBox1.TabStop = false;
			// 
			// axSkinFramework
			// 
			this.axSkinFramework.Enabled = true;
			this.axSkinFramework.Location = new System.Drawing.Point(9, 10);
			this.axSkinFramework.Name = "axSkinFramework";
			this.axSkinFramework.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework.OcxState")));
			this.axSkinFramework.Size = new System.Drawing.Size(24, 24);
			this.axSkinFramework.TabIndex = 24;
			// 
			// panelJoin
			// 
			this.panelJoin.BackColor = System.Drawing.Color.Transparent;
			this.panelJoin.Controls.Add(this.textBoxPassword_J);
			this.panelJoin.Controls.Add(this.textBoxPPassword_J);
			this.panelJoin.Controls.Add(this.textBoxName_J);
			this.panelJoin.Controls.Add(this.textBoxID_J);
			this.panelJoin.Controls.Add(this.btnCancel_J);
			this.panelJoin.Controls.Add(this.label10);
			this.panelJoin.Controls.Add(this.label9);
			this.panelJoin.Controls.Add(this.label8);
			this.panelJoin.Controls.Add(this.label7);
			this.panelJoin.Controls.Add(this.btnOK_J);
			this.panelJoin.Location = new System.Drawing.Point(144, 100);
			this.panelJoin.Name = "panelJoin";
			this.panelJoin.Size = new System.Drawing.Size(421, 167);
			this.panelJoin.TabIndex = 25;
			// 
			// textBoxPassword_J
			// 
			this.textBoxPassword_J.Location = new System.Drawing.Point(120, 96);
			this.textBoxPassword_J.Name = "textBoxPassword_J";
			this.textBoxPassword_J.PasswordChar = '*';
			this.textBoxPassword_J.Size = new System.Drawing.Size(160, 21);
			this.textBoxPassword_J.TabIndex = 7;
			// 
			// textBoxPPassword_J
			// 
			this.textBoxPPassword_J.Location = new System.Drawing.Point(120, 136);
			this.textBoxPPassword_J.Name = "textBoxPPassword_J";
			this.textBoxPPassword_J.PasswordChar = '*';
			this.textBoxPPassword_J.Size = new System.Drawing.Size(160, 21);
			this.textBoxPPassword_J.TabIndex = 8;
			// 
			// textBoxName_J
			// 
			this.textBoxName_J.Location = new System.Drawing.Point(120, 56);
			this.textBoxName_J.Name = "textBoxName_J";
			this.textBoxName_J.Size = new System.Drawing.Size(160, 21);
			this.textBoxName_J.TabIndex = 6;
			// 
			// textBoxID_J
			// 
			this.textBoxID_J.Location = new System.Drawing.Point(120, 16);
			this.textBoxID_J.Name = "textBoxID_J";
			this.textBoxID_J.Size = new System.Drawing.Size(160, 21);
			this.textBoxID_J.TabIndex = 5;
			// 
			// btnCancel_J
			// 
			this.btnCancel_J.Location = new System.Drawing.Point(307, 49);
			this.btnCancel_J.Name = "btnCancel_J";
			this.btnCancel_J.Size = new System.Drawing.Size(75, 23);
			this.btnCancel_J.TabIndex = 10;
			this.btnCancel_J.Text = "취  소";
			this.btnCancel_J.UseVisualStyleBackColor = true;
			this.btnCancel_J.Click += new System.EventHandler(this.btnCancel_J_Click);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(18, 60);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(89, 12);
			this.label10.TabIndex = 2;
			this.label10.Text = "이             름 :";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(18, 140);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(89, 12);
			this.label9.TabIndex = 4;
			this.label9.Text = "비밀번호 확인 :";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(18, 100);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(89, 12);
			this.label8.TabIndex = 3;
			this.label8.Text = "비  밀   번  호 :";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(18, 20);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 12);
			this.label7.TabIndex = 1;
			this.label7.Text = "사  원   번  호 :";
			// 
			// btnOK_J
			// 
			this.btnOK_J.Location = new System.Drawing.Point(307, 16);
			this.btnOK_J.Name = "btnOK_J";
			this.btnOK_J.Size = new System.Drawing.Size(75, 23);
			this.btnOK_J.TabIndex = 9;
			this.btnOK_J.Text = "확  인";
			this.btnOK_J.UseVisualStyleBackColor = true;
			this.btnOK_J.Click += new System.EventHandler(this.btnOK_J_Click);
			// 
			// panelCheck
			// 
			this.panelCheck.BackColor = System.Drawing.Color.Transparent;
			this.panelCheck.Controls.Add(this.btnCancel_P);
			this.panelCheck.Controls.Add(this.btnOK_P);
			this.panelCheck.Controls.Add(this.label11);
			this.panelCheck.Controls.Add(this.label12);
			this.panelCheck.Controls.Add(this.textBoxName_C);
			this.panelCheck.Controls.Add(this.textBoxID_C);
			this.panelCheck.Location = new System.Drawing.Point(266, 127);
			this.panelCheck.Name = "panelCheck";
			this.panelCheck.Size = new System.Drawing.Size(273, 115);
			this.panelCheck.TabIndex = 7;
			// 
			// btnCancel_P
			// 
			this.btnCancel_P.Location = new System.Drawing.Point(185, 59);
			this.btnCancel_P.Name = "btnCancel_P";
			this.btnCancel_P.Size = new System.Drawing.Size(80, 21);
			this.btnCancel_P.TabIndex = 6;
			this.btnCancel_P.Text = "취  소";
			this.btnCancel_P.UseVisualStyleBackColor = true;
			this.btnCancel_P.Click += new System.EventHandler(this.btnCancel_P_Click);
			// 
			// btnOK_P
			// 
			this.btnOK_P.Location = new System.Drawing.Point(185, 33);
			this.btnOK_P.Name = "btnOK_P";
			this.btnOK_P.Size = new System.Drawing.Size(80, 21);
			this.btnOK_P.TabIndex = 5;
			this.btnOK_P.Text = "찾  기";
			this.btnOK_P.UseVisualStyleBackColor = true;
			this.btnOK_P.Click += new System.EventHandler(this.btnOK_P_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(8, 65);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(69, 12);
			this.label11.TabIndex = 2;
			this.label11.Text = "이       름 : ";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(8, 37);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(65, 12);
			this.label12.TabIndex = 1;
			this.label12.Text = "사원 번호 :";
			// 
			// textBoxName_C
			// 
			this.textBoxName_C.Location = new System.Drawing.Point(79, 60);
			this.textBoxName_C.Name = "textBoxName_C";
			this.textBoxName_C.Size = new System.Drawing.Size(100, 21);
			this.textBoxName_C.TabIndex = 4;
			// 
			// textBoxID_C
			// 
			this.textBoxID_C.Location = new System.Drawing.Point(79, 33);
			this.textBoxID_C.Name = "textBoxID_C";
			this.textBoxID_C.Size = new System.Drawing.Size(100, 21);
			this.textBoxID_C.TabIndex = 3;
			// 
			// panelPassChange
			// 
			this.panelPassChange.BackColor = System.Drawing.Color.Transparent;
			this.panelPassChange.Controls.Add(this.label15);
			this.panelPassChange.Controls.Add(this.textBoxCheckPPass_c);
			this.panelPassChange.Controls.Add(this.btnCancel_C);
			this.panelPassChange.Controls.Add(this.btnOK_C);
			this.panelPassChange.Controls.Add(this.label13);
			this.panelPassChange.Controls.Add(this.label14);
			this.panelPassChange.Controls.Add(this.textBoxCheckPass_c);
			this.panelPassChange.Controls.Add(this.textBoxPass_c);
			this.panelPassChange.Location = new System.Drawing.Point(217, 112);
			this.panelPassChange.Name = "panelPassChange";
			this.panelPassChange.Size = new System.Drawing.Size(351, 131);
			this.panelPassChange.TabIndex = 8;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(8, 91);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(93, 12);
			this.label15.TabIndex = 3;
			this.label15.Text = "비밀번호 확인 : ";
			// 
			// textBoxCheckPPass_c
			// 
			this.textBoxCheckPPass_c.Location = new System.Drawing.Point(110, 88);
			this.textBoxCheckPPass_c.Name = "textBoxCheckPPass_c";
			this.textBoxCheckPPass_c.PasswordChar = '*';
			this.textBoxCheckPPass_c.Size = new System.Drawing.Size(100, 21);
			this.textBoxCheckPPass_c.TabIndex = 6;
			// 
			// btnCancel_C
			// 
			this.btnCancel_C.Location = new System.Drawing.Point(237, 59);
			this.btnCancel_C.Name = "btnCancel_C";
			this.btnCancel_C.Size = new System.Drawing.Size(80, 21);
			this.btnCancel_C.TabIndex = 8;
			this.btnCancel_C.Text = "취  소";
			this.btnCancel_C.UseVisualStyleBackColor = true;
			this.btnCancel_C.Click += new System.EventHandler(this.btnCancel_C_Click);
			// 
			// btnOK_C
			// 
			this.btnOK_C.Location = new System.Drawing.Point(237, 33);
			this.btnOK_C.Name = "btnOK_C";
			this.btnOK_C.Size = new System.Drawing.Size(80, 21);
			this.btnOK_C.TabIndex = 7;
			this.btnOK_C.Text = "바꾸기";
			this.btnOK_C.UseVisualStyleBackColor = true;
			this.btnOK_C.Click += new System.EventHandler(this.btnOK_C_Click);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(8, 64);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(93, 12);
			this.label13.TabIndex = 2;
			this.label13.Text = "비  밀  번  호  : ";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(8, 37);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(89, 12);
			this.label14.TabIndex = 1;
			this.label14.Text = "현재 비밀번호 :";
			// 
			// textBoxCheckPass_c
			// 
			this.textBoxCheckPass_c.Location = new System.Drawing.Point(110, 61);
			this.textBoxCheckPass_c.Name = "textBoxCheckPass_c";
			this.textBoxCheckPass_c.PasswordChar = '*';
			this.textBoxCheckPass_c.Size = new System.Drawing.Size(100, 21);
			this.textBoxCheckPass_c.TabIndex = 5;
			// 
			// textBoxPass_c
			// 
			this.textBoxPass_c.Location = new System.Drawing.Point(110, 34);
			this.textBoxPass_c.Name = "textBoxPass_c";
			this.textBoxPass_c.PasswordChar = '*';
			this.textBoxPass_c.Size = new System.Drawing.Size(100, 21);
			this.textBoxPass_c.TabIndex = 4;
			// 
			// timer1
			// 
			this.timer1.Interval = 10000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImage = global::IntegratedManagement.Properties.Resources.background_1;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(725, 320);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panelPassChange);
			this.Controls.Add(this.panelCheck);
			this.Controls.Add(this.panelJoin);
			this.Controls.Add(this.panelLogin);
			this.Controls.Add(this.axSkinFramework);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.lblCaption);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "통합관리 시스템";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.panelLogin.ResumeLayout(false);
			this.panelLogin.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.axSkinFramework)).EndInit();
			this.panelJoin.ResumeLayout(false);
			this.panelJoin.PerformLayout();
			this.panelCheck.ResumeLayout(false);
			this.panelCheck.PerformLayout();
			this.panelPassChange.ResumeLayout(false);
			this.panelPassChange.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnManager;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnTeamManager;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnMonitoring;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.PictureBox pictureBox1;
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Panel panelJoin;
        private System.Windows.Forms.Button btnOK_J;
        private System.Windows.Forms.TextBox textBoxPassword_J;
        private System.Windows.Forms.TextBox textBoxPPassword_J;
        private System.Windows.Forms.TextBox textBoxName_J;
        private System.Windows.Forms.TextBox textBoxID_J;
        private System.Windows.Forms.Button btnCancel_J;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnPasswordCheck;
        private System.Windows.Forms.Panel panelCheck;
        private System.Windows.Forms.Button btnCancel_P;
        private System.Windows.Forms.Button btnOK_P;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxName_C;
        private System.Windows.Forms.TextBox textBoxID_C;
        private System.Windows.Forms.Button btnPassChange;
        private System.Windows.Forms.Panel panelPassChange;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxCheckPPass_c;
        private System.Windows.Forms.Button btnCancel_C;
        private System.Windows.Forms.Button btnOK_C;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxCheckPass_c;
        private System.Windows.Forms.TextBox textBoxPass_c;
        private System.Windows.Forms.Button btn_MessageSend;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btn_SDMS;
    }
}

