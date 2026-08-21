namespace IntegratedManagement2
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
            this.btnClose = new System.Windows.Forms.Button();
            this.labelID = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.groupBoxLogIn = new System.Windows.Forms.GroupBox();
            this.labelSOPManager = new System.Windows.Forms.Label();
            this.labelSOPSimulator = new System.Windows.Forms.Label();
            this.labelTeamManager = new System.Windows.Forms.Label();
            this.labelSDMS = new System.Windows.Forms.Label();
            this.groupBoxSuccessLogin = new System.Windows.Forms.GroupBox();
            this.labelMemberID = new System.Windows.Forms.Label();
            this.textBoxMemberID = new System.Windows.Forms.TextBox();
            this.labelMemberName = new System.Windows.Forms.Label();
            this.textBoxMemberName = new System.Windows.Forms.TextBox();
            this.labelConfirmPassword = new System.Windows.Forms.Label();
            this.textBoxConfirmPassword = new System.Windows.Forms.TextBox();
            this.groupBoxRegister = new System.Windows.Forms.GroupBox();
            this.labelCurrentPassword = new System.Windows.Forms.Label();
            this.textBoxCurrentPassword = new System.Windows.Forms.TextBox();
            this.labelChangingPassword = new System.Windows.Forms.Label();
            this.labelConfirmChanging = new System.Windows.Forms.Label();
            this.textBoxChangingPassword = new System.Windows.Forms.TextBox();
            this.textBoxConfirmChanging = new System.Windows.Forms.TextBox();
            this.labelMemberID2 = new System.Windows.Forms.Label();
            this.textBoxMemberID2 = new System.Windows.Forms.TextBox();
            this.labelMemberName2 = new System.Windows.Forms.Label();
            this.labelID2 = new System.Windows.Forms.Label();
            this.textBoxMemberName2 = new System.Windows.Forms.TextBox();
            this.textBoxID2 = new System.Windows.Forms.TextBox();
            this.labelFindPasswordDescription = new System.Windows.Forms.Label();
            this.btnMin = new System.Windows.Forms.Button();
            this.radioChangePassword = new System.Windows.Forms.RadioButton();
            this.radioChangeNickName = new System.Windows.Forms.RadioButton();
            this.checkBoxSimulationMode = new System.Windows.Forms.CheckBox();
            this.checkBoxShowSensorMonitor = new System.Windows.Forms.CheckBox();
            this.timerSensorMonitor = new System.Windows.Forms.Timer(this.components);
            this.labelChief = new System.Windows.Forms.Label();
            this.btnSetChief = new System.Windows.Forms.Button();
            this.labelCurrVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.ckbSaveID = new System.Windows.Forms.CheckBox();
            this.ckbAutoLogin = new System.Windows.Forms.CheckBox();
            this.btnDownloadManual = new System.Windows.Forms.Button();
            this.btnDownloadVideo = new System.Windows.Forms.Button();
            this.btnDownloadPSMHandBook = new System.Windows.Forms.Button();
            this.labelTrainingEva = new System.Windows.Forms.Label();
            this.btnOption = new System.Windows.Forms.Button();
            this.btnTrainingEva = new IntegratedManagement2.RibbonButton();
            this.rbtnBack = new IntegratedManagement2.RibbonButton();
            this.ribbonButtonSetup = new IntegratedManagement2.RibbonButton();
            this.btnSDMS = new IntegratedManagement2.RibbonButton();
            this.btnTeamManager = new IntegratedManagement2.RibbonButton();
            this.btnSOPSimulator = new IntegratedManagement2.RibbonButton();
            this.btnSOPManager = new IntegratedManagement2.RibbonButton();
            this.btnFindPassword = new IntegratedManagement2.RibbonButton();
            this.btnChangePassword = new IntegratedManagement2.RibbonButton();
            this.btnFindPasswordCancel = new IntegratedManagement2.RibbonButton();
            this.btnCancelChanging = new IntegratedManagement2.RibbonButton();
            this.btnRegistCancel = new IntegratedManagement2.RibbonButton();
            this.btnFindPasswordNext = new IntegratedManagement2.RibbonButton();
            this.btnChanging = new IntegratedManagement2.RibbonButton();
            this.btnRegistOK = new IntegratedManagement2.RibbonButton();
            this.btnLogout = new IntegratedManagement2.RibbonButton();
            this.btnRegist = new IntegratedManagement2.RibbonButton();
            this.btnLogin = new IntegratedManagement2.RibbonButton();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = global::IntegratedManagement2.Properties.Resources.CloseWindow_Normal;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClose.Location = new System.Drawing.Point(568, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 24);
            this.btnClose.TabIndex = 11;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.BackColor = System.Drawing.Color.Transparent;
            this.labelID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelID.Location = new System.Drawing.Point(12, 546);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(64, 20);
            this.labelID.TabIndex = 1;
            this.labelID.Text = "아 이 디";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelPassword.Location = new System.Drawing.Point(12, 573);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(69, 20);
            this.labelPassword.TabIndex = 1;
            this.labelPassword.Text = "비밀번호";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(90, 549);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(105, 21);
            this.textBoxID.TabIndex = 0;
            this.textBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(90, 574);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // groupBoxLogIn
            // 
            this.groupBoxLogIn.Location = new System.Drawing.Point(157, 154);
            this.groupBoxLogIn.Name = "groupBoxLogIn";
            this.groupBoxLogIn.Size = new System.Drawing.Size(28, 23);
            this.groupBoxLogIn.TabIndex = 5;
            this.groupBoxLogIn.TabStop = false;
            this.groupBoxLogIn.Text = "로그인 UI 위치";
            // 
            // labelSOPManager
            // 
            this.labelSOPManager.AutoSize = true;
            this.labelSOPManager.BackColor = System.Drawing.Color.Transparent;
            this.labelSOPManager.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSOPManager.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelSOPManager.Location = new System.Drawing.Point(353, 442);
            this.labelSOPManager.Name = "labelSOPManager";
            this.labelSOPManager.Size = new System.Drawing.Size(65, 13);
            this.labelSOPManager.TabIndex = 7;
            this.labelSOPManager.Text = "SOP 생성기";
            // 
            // labelSOPSimulator
            // 
            this.labelSOPSimulator.AutoSize = true;
            this.labelSOPSimulator.BackColor = System.Drawing.Color.Transparent;
            this.labelSOPSimulator.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSOPSimulator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelSOPSimulator.Location = new System.Drawing.Point(517, 442);
            this.labelSOPSimulator.Name = "labelSOPSimulator";
            this.labelSOPSimulator.Size = new System.Drawing.Size(87, 13);
            this.labelSOPSimulator.TabIndex = 7;
            this.labelSOPSimulator.Text = "SOP 시뮬레이션";
            this.labelSOPSimulator.Visible = false;
            // 
            // labelTeamManager
            // 
            this.labelTeamManager.AutoSize = true;
            this.labelTeamManager.BackColor = System.Drawing.Color.Transparent;
            this.labelTeamManager.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTeamManager.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelTeamManager.Location = new System.Drawing.Point(219, 442);
            this.labelTeamManager.Name = "labelTeamManager";
            this.labelTeamManager.Size = new System.Drawing.Size(62, 13);
            this.labelTeamManager.TabIndex = 7;
            this.labelTeamManager.Text = "조직관리툴";
            // 
            // labelSDMS
            // 
            this.labelSDMS.AutoSize = true;
            this.labelSDMS.BackColor = System.Drawing.Color.Transparent;
            this.labelSDMS.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSDMS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelSDMS.Location = new System.Drawing.Point(90, 442);
            this.labelSDMS.Name = "labelSDMS";
            this.labelSDMS.Size = new System.Drawing.Size(51, 13);
            this.labelSDMS.TabIndex = 7;
            this.labelSDMS.Text = "재난관리";
            // 
            // groupBoxSuccessLogin
            // 
            this.groupBoxSuccessLogin.Location = new System.Drawing.Point(125, 156);
            this.groupBoxSuccessLogin.Name = "groupBoxSuccessLogin";
            this.groupBoxSuccessLogin.Size = new System.Drawing.Size(45, 38);
            this.groupBoxSuccessLogin.TabIndex = 8;
            this.groupBoxSuccessLogin.TabStop = false;
            this.groupBoxSuccessLogin.Text = "로그인 성공 UI 위치";
            // 
            // labelMemberID
            // 
            this.labelMemberID.AutoSize = true;
            this.labelMemberID.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelMemberID.Location = new System.Drawing.Point(394, 543);
            this.labelMemberID.Name = "labelMemberID";
            this.labelMemberID.Size = new System.Drawing.Size(69, 20);
            this.labelMemberID.TabIndex = 1;
            this.labelMemberID.Text = "사원번호";
            // 
            // textBoxMemberID
            // 
            this.textBoxMemberID.Location = new System.Drawing.Point(511, 546);
            this.textBoxMemberID.Name = "textBoxMemberID";
            this.textBoxMemberID.Size = new System.Drawing.Size(105, 21);
            this.textBoxMemberID.TabIndex = 2;
            this.textBoxMemberID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelMemberName
            // 
            this.labelMemberName.AutoSize = true;
            this.labelMemberName.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberName.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelMemberName.Location = new System.Drawing.Point(394, 568);
            this.labelMemberName.Name = "labelMemberName";
            this.labelMemberName.Size = new System.Drawing.Size(39, 20);
            this.labelMemberName.TabIndex = 1;
            this.labelMemberName.Text = "이름";
            // 
            // textBoxMemberName
            // 
            this.textBoxMemberName.Location = new System.Drawing.Point(511, 571);
            this.textBoxMemberName.Name = "textBoxMemberName";
            this.textBoxMemberName.Size = new System.Drawing.Size(105, 21);
            this.textBoxMemberName.TabIndex = 3;
            this.textBoxMemberName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelConfirmPassword
            // 
            this.labelConfirmPassword.AutoSize = true;
            this.labelConfirmPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelConfirmPassword.Location = new System.Drawing.Point(394, 593);
            this.labelConfirmPassword.Name = "labelConfirmPassword";
            this.labelConfirmPassword.Size = new System.Drawing.Size(104, 20);
            this.labelConfirmPassword.TabIndex = 1;
            this.labelConfirmPassword.Text = "비밀번호 확인";
            // 
            // textBoxConfirmPassword
            // 
            this.textBoxConfirmPassword.Location = new System.Drawing.Point(511, 596);
            this.textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            this.textBoxConfirmPassword.PasswordChar = '*';
            this.textBoxConfirmPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxConfirmPassword.TabIndex = 4;
            this.textBoxConfirmPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // groupBoxRegister
            // 
            this.groupBoxRegister.Location = new System.Drawing.Point(188, 155);
            this.groupBoxRegister.Name = "groupBoxRegister";
            this.groupBoxRegister.Size = new System.Drawing.Size(29, 28);
            this.groupBoxRegister.TabIndex = 8;
            this.groupBoxRegister.TabStop = false;
            this.groupBoxRegister.Text = "회원가입 UI";
            // 
            // labelCurrentPassword
            // 
            this.labelCurrentPassword.AutoSize = true;
            this.labelCurrentPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrentPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelCurrentPassword.Location = new System.Drawing.Point(820, 518);
            this.labelCurrentPassword.Name = "labelCurrentPassword";
            this.labelCurrentPassword.Size = new System.Drawing.Size(104, 20);
            this.labelCurrentPassword.TabIndex = 1;
            this.labelCurrentPassword.Text = "현재 비밀번호";
            // 
            // textBoxCurrentPassword
            // 
            this.textBoxCurrentPassword.Location = new System.Drawing.Point(937, 521);
            this.textBoxCurrentPassword.Name = "textBoxCurrentPassword";
            this.textBoxCurrentPassword.PasswordChar = '*';
            this.textBoxCurrentPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxCurrentPassword.TabIndex = 5;
            this.textBoxCurrentPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelChangingPassword
            // 
            this.labelChangingPassword.AutoSize = true;
            this.labelChangingPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelChangingPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelChangingPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelChangingPassword.Location = new System.Drawing.Point(820, 543);
            this.labelChangingPassword.Name = "labelChangingPassword";
            this.labelChangingPassword.Size = new System.Drawing.Size(104, 20);
            this.labelChangingPassword.TabIndex = 1;
            this.labelChangingPassword.Text = "비  밀   번  호";
            // 
            // labelConfirmChanging
            // 
            this.labelConfirmChanging.AutoSize = true;
            this.labelConfirmChanging.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmChanging.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmChanging.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelConfirmChanging.Location = new System.Drawing.Point(820, 568);
            this.labelConfirmChanging.Name = "labelConfirmChanging";
            this.labelConfirmChanging.Size = new System.Drawing.Size(104, 20);
            this.labelConfirmChanging.TabIndex = 1;
            this.labelConfirmChanging.Text = "비밀번호 확인";
            // 
            // textBoxChangingPassword
            // 
            this.textBoxChangingPassword.Location = new System.Drawing.Point(937, 546);
            this.textBoxChangingPassword.Name = "textBoxChangingPassword";
            this.textBoxChangingPassword.PasswordChar = '*';
            this.textBoxChangingPassword.Size = new System.Drawing.Size(105, 21);
            this.textBoxChangingPassword.TabIndex = 6;
            this.textBoxChangingPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxConfirmChanging
            // 
            this.textBoxConfirmChanging.Location = new System.Drawing.Point(937, 571);
            this.textBoxConfirmChanging.Name = "textBoxConfirmChanging";
            this.textBoxConfirmChanging.PasswordChar = '*';
            this.textBoxConfirmChanging.Size = new System.Drawing.Size(105, 21);
            this.textBoxConfirmChanging.TabIndex = 7;
            this.textBoxConfirmChanging.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelMemberID2
            // 
            this.labelMemberID2.AutoSize = true;
            this.labelMemberID2.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberID2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberID2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelMemberID2.Location = new System.Drawing.Point(754, 270);
            this.labelMemberID2.Name = "labelMemberID2";
            this.labelMemberID2.Size = new System.Drawing.Size(69, 20);
            this.labelMemberID2.TabIndex = 1;
            this.labelMemberID2.Text = "사원번호";
            // 
            // textBoxMemberID2
            // 
            this.textBoxMemberID2.Location = new System.Drawing.Point(871, 273);
            this.textBoxMemberID2.Name = "textBoxMemberID2";
            this.textBoxMemberID2.Size = new System.Drawing.Size(105, 21);
            this.textBoxMemberID2.TabIndex = 8;
            this.textBoxMemberID2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelMemberName2
            // 
            this.labelMemberName2.AutoSize = true;
            this.labelMemberName2.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberName2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberName2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelMemberName2.Location = new System.Drawing.Point(754, 295);
            this.labelMemberName2.Name = "labelMemberName2";
            this.labelMemberName2.Size = new System.Drawing.Size(64, 20);
            this.labelMemberName2.TabIndex = 1;
            this.labelMemberName2.Text = "이     름";
            // 
            // labelID2
            // 
            this.labelID2.AutoSize = true;
            this.labelID2.BackColor = System.Drawing.Color.Transparent;
            this.labelID2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelID2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelID2.Location = new System.Drawing.Point(754, 320);
            this.labelID2.Name = "labelID2";
            this.labelID2.Size = new System.Drawing.Size(64, 20);
            this.labelID2.TabIndex = 1;
            this.labelID2.Text = "아 이 디";
            // 
            // textBoxMemberName2
            // 
            this.textBoxMemberName2.Location = new System.Drawing.Point(871, 298);
            this.textBoxMemberName2.Name = "textBoxMemberName2";
            this.textBoxMemberName2.Size = new System.Drawing.Size(105, 21);
            this.textBoxMemberName2.TabIndex = 9;
            this.textBoxMemberName2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxID2
            // 
            this.textBoxID2.Location = new System.Drawing.Point(871, 323);
            this.textBoxID2.Name = "textBoxID2";
            this.textBoxID2.Size = new System.Drawing.Size(105, 21);
            this.textBoxID2.TabIndex = 10;
            this.textBoxID2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelFindPasswordDescription
            // 
            this.labelFindPasswordDescription.AutoSize = true;
            this.labelFindPasswordDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelFindPasswordDescription.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFindPasswordDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelFindPasswordDescription.Location = new System.Drawing.Point(754, 245);
            this.labelFindPasswordDescription.Name = "labelFindPasswordDescription";
            this.labelFindPasswordDescription.Size = new System.Drawing.Size(253, 20);
            this.labelFindPasswordDescription.TabIndex = 1;
            this.labelFindPasswordDescription.Text = "새로운 비밀번호를 등록해 주십시오.";
            // 
            // btnMin
            // 
            this.btnMin.BackgroundImage = global::IntegratedManagement2.Properties.Resources.HideWindow_Normal;
            this.btnMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnMin.Location = new System.Drawing.Point(537, 2);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(32, 24);
            this.btnMin.TabIndex = 19;
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // radioChangePassword
            // 
            this.radioChangePassword.AutoSize = true;
            this.radioChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.radioChangePassword.Checked = true;
            this.radioChangePassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.radioChangePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.radioChangePassword.Location = new System.Drawing.Point(685, 626);
            this.radioChangePassword.Name = "radioChangePassword";
            this.radioChangePassword.Size = new System.Drawing.Size(101, 19);
            this.radioChangePassword.TabIndex = 20;
            this.radioChangePassword.TabStop = true;
            this.radioChangePassword.Text = "비밀번호 변경";
            this.radioChangePassword.UseVisualStyleBackColor = false;
            this.radioChangePassword.CheckedChanged += new System.EventHandler(this.radioChangePassword_CheckedChanged);
            // 
            // radioChangeNickName
            // 
            this.radioChangeNickName.AutoSize = true;
            this.radioChangeNickName.BackColor = System.Drawing.Color.Transparent;
            this.radioChangeNickName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.radioChangeNickName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.radioChangeNickName.Location = new System.Drawing.Point(685, 651);
            this.radioChangeNickName.Name = "radioChangeNickName";
            this.radioChangeNickName.Size = new System.Drawing.Size(77, 19);
            this.radioChangeNickName.TabIndex = 20;
            this.radioChangeNickName.Text = "별명 변경";
            this.radioChangeNickName.UseVisualStyleBackColor = false;
            this.radioChangeNickName.CheckedChanged += new System.EventHandler(this.radioChangeNickName_CheckedChanged);
            // 
            // checkBoxSimulationMode
            // 
            this.checkBoxSimulationMode.AutoSize = true;
            this.checkBoxSimulationMode.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxSimulationMode.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.checkBoxSimulationMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.checkBoxSimulationMode.Location = new System.Drawing.Point(465, 259);
            this.checkBoxSimulationMode.Name = "checkBoxSimulationMode";
            this.checkBoxSimulationMode.Size = new System.Drawing.Size(88, 24);
            this.checkBoxSimulationMode.TabIndex = 23;
            this.checkBoxSimulationMode.Text = "연습모드";
            this.checkBoxSimulationMode.UseVisualStyleBackColor = false;
            this.checkBoxSimulationMode.Visible = false;
            // 
            // checkBoxShowSensorMonitor
            // 
            this.checkBoxShowSensorMonitor.AutoSize = true;
            this.checkBoxShowSensorMonitor.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxShowSensorMonitor.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.checkBoxShowSensorMonitor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.checkBoxShowSensorMonitor.Location = new System.Drawing.Point(465, 282);
            this.checkBoxShowSensorMonitor.Name = "checkBoxShowSensorMonitor";
            this.checkBoxShowSensorMonitor.Size = new System.Drawing.Size(123, 24);
            this.checkBoxShowSensorMonitor.TabIndex = 23;
            this.checkBoxShowSensorMonitor.Text = "연습용 수신반";
            this.checkBoxShowSensorMonitor.UseVisualStyleBackColor = false;
            this.checkBoxShowSensorMonitor.Visible = false;
            this.checkBoxShowSensorMonitor.CheckedChanged += new System.EventHandler(this.checkBoxShowSensorMonitor_CheckedChanged);
            // 
            // timerSensorMonitor
            // 
            this.timerSensorMonitor.Interval = 1000;
            this.timerSensorMonitor.Tick += new System.EventHandler(this.timerSensorMonitor_Tick);
            // 
            // labelChief
            // 
            this.labelChief.AutoSize = true;
            this.labelChief.BackColor = System.Drawing.Color.Transparent;
            this.labelChief.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelChief.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelChief.Location = new System.Drawing.Point(514, 625);
            this.labelChief.Name = "labelChief";
            this.labelChief.Size = new System.Drawing.Size(104, 20);
            this.labelChief.TabIndex = 1;
            this.labelChief.Text = "계정별 책임자";
            this.labelChief.Visible = false;
            // 
            // btnSetChief
            // 
            this.btnSetChief.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSetChief.Location = new System.Drawing.Point(398, 624);
            this.btnSetChief.Name = "btnSetChief";
            this.btnSetChief.Size = new System.Drawing.Size(100, 23);
            this.btnSetChief.TabIndex = 24;
            this.btnSetChief.Text = "책임자 설정";
            this.btnSetChief.UseVisualStyleBackColor = true;
            this.btnSetChief.Click += new System.EventHandler(this.btnSetChief_Click);
            // 
            // labelCurrVersion
            // 
            this.labelCurrVersion.AutoSize = true;
            this.labelCurrVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrVersion.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelCurrVersion.Location = new System.Drawing.Point(4, 316);
            this.labelCurrVersion.Name = "labelCurrVersion";
            this.labelCurrVersion.Size = new System.Drawing.Size(64, 13);
            this.labelCurrVersion.TabIndex = 25;
            this.labelCurrVersion.Text = "Ver.  1.0.0.0";
            // 
            // labelCopyright
            // 
            this.labelCopyright.BackColor = System.Drawing.Color.Transparent;
            this.labelCopyright.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCopyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelCopyright.Location = new System.Drawing.Point(0, 316);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(598, 13);
            this.labelCopyright.TabIndex = 26;
            this.labelCopyright.Text = "Copyright @ U&&E";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ckbSaveID
            // 
            this.ckbSaveID.AutoSize = true;
            this.ckbSaveID.BackColor = System.Drawing.Color.Transparent;
            this.ckbSaveID.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbSaveID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.ckbSaveID.Location = new System.Drawing.Point(53, 607);
            this.ckbSaveID.Name = "ckbSaveID";
            this.ckbSaveID.Size = new System.Drawing.Size(167, 19);
            this.ckbSaveID.TabIndex = 25;
            this.ckbSaveID.Text = "아이디/비밀번호 저장하기";
            this.ckbSaveID.UseVisualStyleBackColor = false;
            this.ckbSaveID.CheckedChanged += new System.EventHandler(this.ckbSaveID_CheckedChanged);
            // 
            // ckbAutoLogin
            // 
            this.ckbAutoLogin.AutoSize = true;
            this.ckbAutoLogin.BackColor = System.Drawing.Color.Transparent;
            this.ckbAutoLogin.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbAutoLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.ckbAutoLogin.Location = new System.Drawing.Point(53, 628);
            this.ckbAutoLogin.Name = "ckbAutoLogin";
            this.ckbAutoLogin.Size = new System.Drawing.Size(90, 19);
            this.ckbAutoLogin.TabIndex = 26;
            this.ckbAutoLogin.Text = "자동 로그인";
            this.ckbAutoLogin.UseVisualStyleBackColor = false;
            this.ckbAutoLogin.CheckedChanged += new System.EventHandler(this.ckbAutoLogin_CheckedChanged);
            // 
            // btnDownloadManual
            // 
            this.btnDownloadManual.Location = new System.Drawing.Point(16, 12);
            this.btnDownloadManual.Name = "btnDownloadManual";
            this.btnDownloadManual.Size = new System.Drawing.Size(104, 23);
            this.btnDownloadManual.TabIndex = 27;
            this.btnDownloadManual.Text = "사용자 매뉴얼";
            this.btnDownloadManual.UseVisualStyleBackColor = true;
            this.btnDownloadManual.Visible = false;
            this.btnDownloadManual.Click += new System.EventHandler(this.btnDownloadManual_Click);
            // 
            // btnDownloadVideo
            // 
            this.btnDownloadVideo.Location = new System.Drawing.Point(16, 41);
            this.btnDownloadVideo.Name = "btnDownloadVideo";
            this.btnDownloadVideo.Size = new System.Drawing.Size(104, 23);
            this.btnDownloadVideo.TabIndex = 27;
            this.btnDownloadVideo.Text = "동영상 교육자료";
            this.btnDownloadVideo.UseVisualStyleBackColor = true;
            this.btnDownloadVideo.Visible = false;
            this.btnDownloadVideo.Click += new System.EventHandler(this.btnDownloadVideo_Click);
            // 
            // btnDownloadPSMHandBook
            // 
            this.btnDownloadPSMHandBook.Location = new System.Drawing.Point(16, 70);
            this.btnDownloadPSMHandBook.Name = "btnDownloadPSMHandBook";
            this.btnDownloadPSMHandBook.Size = new System.Drawing.Size(104, 23);
            this.btnDownloadPSMHandBook.TabIndex = 27;
            this.btnDownloadPSMHandBook.Text = "PSM 핸드북";
            this.btnDownloadPSMHandBook.UseVisualStyleBackColor = true;
            this.btnDownloadPSMHandBook.Visible = false;
            this.btnDownloadPSMHandBook.Click += new System.EventHandler(this.btnDownloadPSMHandBook_Click);
            // 
            // labelTrainingEva
            // 
            this.labelTrainingEva.AutoSize = true;
            this.labelTrainingEva.BackColor = System.Drawing.Color.Transparent;
            this.labelTrainingEva.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTrainingEva.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelTrainingEva.Location = new System.Drawing.Point(443, 442);
            this.labelTrainingEva.Name = "labelTrainingEva";
            this.labelTrainingEva.Size = new System.Drawing.Size(55, 13);
            this.labelTrainingEva.TabIndex = 29;
            this.labelTrainingEva.Text = "훈련 평가";
            this.labelTrainingEva.Visible = false;
            // 
            // btnOption
            // 
            this.btnOption.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOption.Location = new System.Drawing.Point(511, 624);
            this.btnOption.Name = "btnOption";
            this.btnOption.Size = new System.Drawing.Size(105, 23);
            this.btnOption.TabIndex = 30;
            this.btnOption.Text = "선택사항";
            this.btnOption.UseVisualStyleBackColor = true;
            this.btnOption.Click += new System.EventHandler(this.btnOption_Click);
            // 
            // btnTrainingEva
            // 
            this.btnTrainingEva.BackColor = System.Drawing.Color.Transparent;
            this.btnTrainingEva.BackgroundImage = global::IntegratedManagement2.Properties.Resources.sopsimulator;
            this.btnTrainingEva.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnTrainingEva.CheckedBkgndImage = null;
            this.btnTrainingEva.CheckedImage = null;
            this.btnTrainingEva.IsChecked = false;
            this.btnTrainingEva.Location = new System.Drawing.Point(437, 368);
            this.btnTrainingEva.MouseOverBkgndImage = null;
            this.btnTrainingEva.Name = "btnTrainingEva";
            this.btnTrainingEva.NormalImage = null;
            this.btnTrainingEva.Owner = null;
            this.btnTrainingEva.Size = new System.Drawing.Size(68, 68);
            this.btnTrainingEva.TabIndex = 28;
            this.btnTrainingEva.UseVisualStyleBackColor = false;
            this.btnTrainingEva.Visible = false;
            this.btnTrainingEva.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // rbtnBack
            // 
            this.rbtnBack.BackColor = System.Drawing.Color.Transparent;
            this.rbtnBack.BackgroundImage = global::IntegratedManagement2.Properties.Resources.back;
            this.rbtnBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnBack.CheckedBkgndImage = null;
            this.rbtnBack.CheckedImage = null;
            this.rbtnBack.IsChecked = false;
            this.rbtnBack.Location = new System.Drawing.Point(30, 258);
            this.rbtnBack.MouseOverBkgndImage = null;
            this.rbtnBack.Name = "rbtnBack";
            this.rbtnBack.NormalImage = null;
            this.rbtnBack.Owner = null;
            this.rbtnBack.Size = new System.Drawing.Size(40, 40);
            this.rbtnBack.TabIndex = 22;
            this.rbtnBack.UseVisualStyleBackColor = false;
            this.rbtnBack.Click += new System.EventHandler(this.rbtnBack_Click);
            // 
            // ribbonButtonSetup
            // 
            this.ribbonButtonSetup.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButtonSetup.BackgroundImage = global::IntegratedManagement2.Properties.Resources.sopmanager;
            this.ribbonButtonSetup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ribbonButtonSetup.CheckedBkgndImage = null;
            this.ribbonButtonSetup.CheckedImage = null;
            this.ribbonButtonSetup.IsChecked = false;
            this.ribbonButtonSetup.Location = new System.Drawing.Point(72, 258);
            this.ribbonButtonSetup.MouseOverBkgndImage = null;
            this.ribbonButtonSetup.Name = "ribbonButtonSetup";
            this.ribbonButtonSetup.NormalImage = null;
            this.ribbonButtonSetup.Owner = null;
            this.ribbonButtonSetup.Size = new System.Drawing.Size(40, 40);
            this.ribbonButtonSetup.TabIndex = 22;
            this.ribbonButtonSetup.UseVisualStyleBackColor = false;
            this.ribbonButtonSetup.Click += new System.EventHandler(this.ribbonButtonSetup_Click_1);
            // 
            // btnSDMS
            // 
            this.btnSDMS.BackColor = System.Drawing.Color.Transparent;
            this.btnSDMS.BackgroundImage = global::IntegratedManagement2.Properties.Resources.sdms;
            this.btnSDMS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSDMS.CheckedBkgndImage = null;
            this.btnSDMS.CheckedImage = null;
            this.btnSDMS.IsChecked = false;
            this.btnSDMS.Location = new System.Drawing.Point(81, 368);
            this.btnSDMS.MouseOverBkgndImage = null;
            this.btnSDMS.Name = "btnSDMS";
            this.btnSDMS.NormalImage = null;
            this.btnSDMS.Owner = null;
            this.btnSDMS.Size = new System.Drawing.Size(68, 68);
            this.btnSDMS.TabIndex = 6;
            this.btnSDMS.UseVisualStyleBackColor = false;
            this.btnSDMS.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // btnTeamManager
            // 
            this.btnTeamManager.BackColor = System.Drawing.Color.Transparent;
            this.btnTeamManager.BackgroundImage = global::IntegratedManagement2.Properties.Resources.teammanager;
            this.btnTeamManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnTeamManager.CheckedBkgndImage = null;
            this.btnTeamManager.CheckedImage = null;
            this.btnTeamManager.IsChecked = false;
            this.btnTeamManager.Location = new System.Drawing.Point(216, 368);
            this.btnTeamManager.MouseOverBkgndImage = null;
            this.btnTeamManager.Name = "btnTeamManager";
            this.btnTeamManager.NormalImage = null;
            this.btnTeamManager.Owner = null;
            this.btnTeamManager.Size = new System.Drawing.Size(68, 68);
            this.btnTeamManager.TabIndex = 6;
            this.btnTeamManager.UseVisualStyleBackColor = false;
            this.btnTeamManager.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // btnSOPSimulator
            // 
            this.btnSOPSimulator.BackColor = System.Drawing.Color.Transparent;
            this.btnSOPSimulator.BackgroundImage = global::IntegratedManagement2.Properties.Resources.sopsimulator;
            this.btnSOPSimulator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSOPSimulator.CheckedBkgndImage = null;
            this.btnSOPSimulator.CheckedImage = null;
            this.btnSOPSimulator.IsChecked = false;
            this.btnSOPSimulator.Location = new System.Drawing.Point(526, 368);
            this.btnSOPSimulator.MouseOverBkgndImage = null;
            this.btnSOPSimulator.Name = "btnSOPSimulator";
            this.btnSOPSimulator.NormalImage = null;
            this.btnSOPSimulator.Owner = null;
            this.btnSOPSimulator.Size = new System.Drawing.Size(68, 68);
            this.btnSOPSimulator.TabIndex = 6;
            this.btnSOPSimulator.UseVisualStyleBackColor = false;
            this.btnSOPSimulator.Visible = false;
            this.btnSOPSimulator.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // btnSOPManager
            // 
            this.btnSOPManager.BackColor = System.Drawing.Color.Transparent;
            this.btnSOPManager.BackgroundImage = global::IntegratedManagement2.Properties.Resources.sopmanager;
            this.btnSOPManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSOPManager.CheckedBkgndImage = null;
            this.btnSOPManager.CheckedImage = null;
            this.btnSOPManager.IsChecked = false;
            this.btnSOPManager.Location = new System.Drawing.Point(351, 368);
            this.btnSOPManager.MouseOverBkgndImage = null;
            this.btnSOPManager.Name = "btnSOPManager";
            this.btnSOPManager.NormalImage = null;
            this.btnSOPManager.Owner = null;
            this.btnSOPManager.Size = new System.Drawing.Size(68, 68);
            this.btnSOPManager.TabIndex = 6;
            this.btnSOPManager.UseVisualStyleBackColor = false;
            this.btnSOPManager.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // btnFindPassword
            // 
            this.btnFindPassword.BackColor = System.Drawing.Color.Transparent;
            this.btnFindPassword.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnFindPassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnFindPassword.CheckedBkgndImage = null;
            this.btnFindPassword.CheckedImage = null;
            this.btnFindPassword.IsChecked = false;
            this.btnFindPassword.Location = new System.Drawing.Point(157, 660);
            this.btnFindPassword.MouseOverBkgndImage = null;
            this.btnFindPassword.Name = "btnFindPassword";
            this.btnFindPassword.NormalImage = null;
            this.btnFindPassword.Owner = null;
            this.btnFindPassword.Size = new System.Drawing.Size(135, 38);
            this.btnFindPassword.TabIndex = 18;
            this.btnFindPassword.Text = "비밀번호 찾기";
            this.btnFindPassword.UseVisualStyleBackColor = false;
            this.btnFindPassword.Click += new System.EventHandler(this.btnFindPassword_Click);
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.btnChangePassword.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnChangePassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnChangePassword.CheckedBkgndImage = null;
            this.btnChangePassword.CheckedImage = null;
            this.btnChangePassword.IsChecked = false;
            this.btnChangePassword.Location = new System.Drawing.Point(263, 472);
            this.btnChangePassword.MouseOverBkgndImage = null;
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.NormalImage = null;
            this.btnChangePassword.Owner = null;
            this.btnChangePassword.Size = new System.Drawing.Size(135, 44);
            this.btnChangePassword.TabIndex = 4;
            this.btnChangePassword.Text = "계정 관리";
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnFindPasswordCancel
            // 
            this.btnFindPasswordCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnFindPasswordCancel.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnFindPasswordCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnFindPasswordCancel.CheckedBkgndImage = null;
            this.btnFindPasswordCancel.CheckedImage = null;
            this.btnFindPasswordCancel.IsChecked = false;
            this.btnFindPasswordCancel.Location = new System.Drawing.Point(871, 380);
            this.btnFindPasswordCancel.MouseOverBkgndImage = null;
            this.btnFindPasswordCancel.Name = "btnFindPasswordCancel";
            this.btnFindPasswordCancel.NormalImage = null;
            this.btnFindPasswordCancel.Owner = null;
            this.btnFindPasswordCancel.Size = new System.Drawing.Size(105, 44);
            this.btnFindPasswordCancel.TabIndex = 12;
            this.btnFindPasswordCancel.Text = "취소";
            this.btnFindPasswordCancel.UseVisualStyleBackColor = false;
            this.btnFindPasswordCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCancelChanging
            // 
            this.btnCancelChanging.BackColor = System.Drawing.Color.Transparent;
            this.btnCancelChanging.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnCancelChanging.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCancelChanging.CheckedBkgndImage = null;
            this.btnCancelChanging.CheckedImage = null;
            this.btnCancelChanging.IsChecked = false;
            this.btnCancelChanging.Location = new System.Drawing.Point(937, 628);
            this.btnCancelChanging.MouseOverBkgndImage = null;
            this.btnCancelChanging.Name = "btnCancelChanging";
            this.btnCancelChanging.NormalImage = null;
            this.btnCancelChanging.Owner = null;
            this.btnCancelChanging.Size = new System.Drawing.Size(105, 44);
            this.btnCancelChanging.TabIndex = 14;
            this.btnCancelChanging.Text = "취소";
            this.btnCancelChanging.UseVisualStyleBackColor = false;
            this.btnCancelChanging.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRegistCancel
            // 
            this.btnRegistCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistCancel.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnRegistCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegistCancel.CheckedBkgndImage = null;
            this.btnRegistCancel.CheckedImage = null;
            this.btnRegistCancel.IsChecked = false;
            this.btnRegistCancel.Location = new System.Drawing.Point(511, 653);
            this.btnRegistCancel.MouseOverBkgndImage = null;
            this.btnRegistCancel.Name = "btnRegistCancel";
            this.btnRegistCancel.NormalImage = null;
            this.btnRegistCancel.Owner = null;
            this.btnRegistCancel.Size = new System.Drawing.Size(105, 44);
            this.btnRegistCancel.TabIndex = 16;
            this.btnRegistCancel.Text = "취소";
            this.btnRegistCancel.UseVisualStyleBackColor = false;
            this.btnRegistCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFindPasswordNext
            // 
            this.btnFindPasswordNext.BackColor = System.Drawing.Color.Transparent;
            this.btnFindPasswordNext.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnFindPasswordNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnFindPasswordNext.CheckedBkgndImage = null;
            this.btnFindPasswordNext.CheckedImage = null;
            this.btnFindPasswordNext.IsChecked = false;
            this.btnFindPasswordNext.Location = new System.Drawing.Point(758, 380);
            this.btnFindPasswordNext.MouseOverBkgndImage = null;
            this.btnFindPasswordNext.Name = "btnFindPasswordNext";
            this.btnFindPasswordNext.NormalImage = null;
            this.btnFindPasswordNext.Owner = null;
            this.btnFindPasswordNext.Size = new System.Drawing.Size(105, 44);
            this.btnFindPasswordNext.TabIndex = 11;
            this.btnFindPasswordNext.Text = "다음";
            this.btnFindPasswordNext.UseVisualStyleBackColor = false;
            this.btnFindPasswordNext.Click += new System.EventHandler(this.btnFindPasswordNext_Click);
            // 
            // btnChanging
            // 
            this.btnChanging.BackColor = System.Drawing.Color.Transparent;
            this.btnChanging.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnChanging.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnChanging.CheckedBkgndImage = null;
            this.btnChanging.CheckedImage = null;
            this.btnChanging.IsChecked = false;
            this.btnChanging.Location = new System.Drawing.Point(824, 628);
            this.btnChanging.MouseOverBkgndImage = null;
            this.btnChanging.Name = "btnChanging";
            this.btnChanging.NormalImage = null;
            this.btnChanging.Owner = null;
            this.btnChanging.Size = new System.Drawing.Size(105, 44);
            this.btnChanging.TabIndex = 13;
            this.btnChanging.Text = "바꾸기";
            this.btnChanging.UseVisualStyleBackColor = false;
            this.btnChanging.Click += new System.EventHandler(this.btnChanging_Click);
            // 
            // btnRegistOK
            // 
            this.btnRegistOK.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistOK.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnRegistOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegistOK.CheckedBkgndImage = null;
            this.btnRegistOK.CheckedImage = null;
            this.btnRegistOK.IsChecked = false;
            this.btnRegistOK.Location = new System.Drawing.Point(398, 653);
            this.btnRegistOK.MouseOverBkgndImage = null;
            this.btnRegistOK.Name = "btnRegistOK";
            this.btnRegistOK.NormalImage = null;
            this.btnRegistOK.Owner = null;
            this.btnRegistOK.Size = new System.Drawing.Size(105, 44);
            this.btnRegistOK.TabIndex = 15;
            this.btnRegistOK.Text = "다음";
            this.btnRegistOK.UseVisualStyleBackColor = false;
            this.btnRegistOK.Click += new System.EventHandler(this.btnRegistOK_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.Transparent;
            this.btnLogout.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnLogout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLogout.CheckedBkgndImage = null;
            this.btnLogout.CheckedImage = null;
            this.btnLogout.IsChecked = false;
            this.btnLogout.Location = new System.Drawing.Point(100, 472);
            this.btnLogout.MouseOverBkgndImage = null;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.NormalImage = null;
            this.btnLogout.Owner = null;
            this.btnLogout.Size = new System.Drawing.Size(135, 44);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "로그아웃";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnRegist
            // 
            this.btnRegist.BackColor = System.Drawing.Color.Transparent;
            this.btnRegist.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnRegist.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegist.CheckedBkgndImage = null;
            this.btnRegist.CheckedImage = null;
            this.btnRegist.IsChecked = false;
            this.btnRegist.Location = new System.Drawing.Point(16, 660);
            this.btnRegist.MouseOverBkgndImage = null;
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.NormalImage = null;
            this.btnRegist.Owner = null;
            this.btnRegist.Size = new System.Drawing.Size(135, 38);
            this.btnRegist.TabIndex = 17;
            this.btnRegist.Text = "회원가입";
            this.btnRegist.UseVisualStyleBackColor = false;
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLogin.CheckedBkgndImage = null;
            this.btnLogin.CheckedImage = null;
            this.btnLogin.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.btnLogin.IsChecked = false;
            this.btnLogin.Location = new System.Drawing.Point(209, 548);
            this.btnLogin.MouseOverBkgndImage = null;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.NormalImage = null;
            this.btnLogin.Owner = null;
            this.btnLogin.Size = new System.Drawing.Size(83, 49);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::IntegratedManagement2.Properties.Resources.background_v3;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1108, 718);
            this.Controls.Add(this.btnOption);
            this.Controls.Add(this.labelTrainingEva);
            this.Controls.Add(this.btnTrainingEva);
            this.Controls.Add(this.btnDownloadPSMHandBook);
            this.Controls.Add(this.btnDownloadVideo);
            this.Controls.Add(this.btnDownloadManual);
            this.Controls.Add(this.labelCurrVersion);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.ckbAutoLogin);
            this.Controls.Add(this.ckbSaveID);
            this.Controls.Add(this.btnSetChief);
            this.Controls.Add(this.checkBoxShowSensorMonitor);
            this.Controls.Add(this.checkBoxSimulationMode);
            this.Controls.Add(this.rbtnBack);
            this.Controls.Add(this.ribbonButtonSetup);
            this.Controls.Add(this.radioChangeNickName);
            this.Controls.Add(this.radioChangePassword);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.groupBoxRegister);
            this.Controls.Add(this.groupBoxSuccessLogin);
            this.Controls.Add(this.labelSDMS);
            this.Controls.Add(this.labelTeamManager);
            this.Controls.Add(this.labelSOPSimulator);
            this.Controls.Add(this.labelSOPManager);
            this.Controls.Add(this.btnSDMS);
            this.Controls.Add(this.btnTeamManager);
            this.Controls.Add(this.btnSOPSimulator);
            this.Controls.Add(this.btnSOPManager);
            this.Controls.Add(this.groupBoxLogIn);
            this.Controls.Add(this.btnFindPassword);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.btnFindPasswordCancel);
            this.Controls.Add(this.btnCancelChanging);
            this.Controls.Add(this.btnRegistCancel);
            this.Controls.Add(this.btnFindPasswordNext);
            this.Controls.Add(this.btnChanging);
            this.Controls.Add(this.btnRegistOK);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnRegist);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxID2);
            this.Controls.Add(this.textBoxConfirmChanging);
            this.Controls.Add(this.textBoxConfirmPassword);
            this.Controls.Add(this.textBoxMemberName2);
            this.Controls.Add(this.textBoxChangingPassword);
            this.Controls.Add(this.textBoxMemberName);
            this.Controls.Add(this.labelID2);
            this.Controls.Add(this.labelChief);
            this.Controls.Add(this.labelConfirmChanging);
            this.Controls.Add(this.labelConfirmPassword);
            this.Controls.Add(this.labelMemberName2);
            this.Controls.Add(this.labelChangingPassword);
            this.Controls.Add(this.labelMemberName);
            this.Controls.Add(this.textBoxMemberID2);
            this.Controls.Add(this.textBoxCurrentPassword);
            this.Controls.Add(this.textBoxMemberID);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.labelFindPasswordDescription);
            this.Controls.Add(this.labelMemberID2);
            this.Controls.Add(this.labelCurrentPassword);
            this.Controls.Add(this.labelMemberID);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelID);
            this.Controls.Add(this.btnClose);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "통합관리시스템";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxPassword;
        private RibbonButton btnLogin;
        private RibbonButton btnRegist;
        private RibbonButton btnFindPassword;
        private System.Windows.Forms.GroupBox groupBoxLogIn;
        private RibbonButton btnSOPManager;
        private System.Windows.Forms.Label labelSOPManager;
        private RibbonButton btnSOPSimulator;
        private System.Windows.Forms.Label labelSOPSimulator;
        private RibbonButton btnTeamManager;
        private System.Windows.Forms.Label labelTeamManager;
        private RibbonButton btnSDMS;
        private System.Windows.Forms.Label labelSDMS;
        private RibbonButton btnLogout;
        private RibbonButton btnChangePassword;
        private System.Windows.Forms.GroupBox groupBoxSuccessLogin;
        private System.Windows.Forms.Label labelMemberID;
        private System.Windows.Forms.TextBox textBoxMemberID;
        private System.Windows.Forms.Label labelMemberName;
        private System.Windows.Forms.TextBox textBoxMemberName;
        private System.Windows.Forms.Label labelConfirmPassword;
        private System.Windows.Forms.TextBox textBoxConfirmPassword;
        private RibbonButton btnRegistOK;
        private RibbonButton btnRegistCancel;
        private System.Windows.Forms.GroupBox groupBoxRegister;
        private System.Windows.Forms.Label labelCurrentPassword;
        private System.Windows.Forms.TextBox textBoxCurrentPassword;
        private System.Windows.Forms.Label labelChangingPassword;
        private System.Windows.Forms.Label labelConfirmChanging;
        private System.Windows.Forms.TextBox textBoxChangingPassword;
        private System.Windows.Forms.TextBox textBoxConfirmChanging;
        private RibbonButton btnChanging;
        private RibbonButton btnCancelChanging;
        private System.Windows.Forms.Label labelMemberID2;
        private System.Windows.Forms.TextBox textBoxMemberID2;
        private System.Windows.Forms.Label labelMemberName2;
        private System.Windows.Forms.Label labelID2;
        private System.Windows.Forms.TextBox textBoxMemberName2;
        private System.Windows.Forms.TextBox textBoxID2;
		private RibbonButton btnFindPasswordNext;
        private RibbonButton btnFindPasswordCancel;
        private System.Windows.Forms.Label labelFindPasswordDescription;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.RadioButton radioChangePassword;
        private System.Windows.Forms.RadioButton radioChangeNickName;
        private RibbonButton ribbonButtonSetup;
        private System.Windows.Forms.CheckBox checkBoxSimulationMode;
        private System.Windows.Forms.CheckBox checkBoxShowSensorMonitor;
        private System.Windows.Forms.Timer timerSensorMonitor;
        private RibbonButton rbtnBack;
        private System.Windows.Forms.Label labelChief;
        private System.Windows.Forms.Button btnSetChief;
        private System.Windows.Forms.Label labelCurrVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.CheckBox ckbSaveID;
        private System.Windows.Forms.CheckBox ckbAutoLogin;
        private System.Windows.Forms.Button btnDownloadManual;
        private System.Windows.Forms.Button btnDownloadVideo;
        private System.Windows.Forms.Button btnDownloadPSMHandBook;
        private RibbonButton btnTrainingEva;
        private System.Windows.Forms.Label labelTrainingEva;
        private System.Windows.Forms.Button btnOption;
    }
}

