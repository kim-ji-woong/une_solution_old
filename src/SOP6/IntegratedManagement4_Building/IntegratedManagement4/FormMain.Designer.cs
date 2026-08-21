namespace IntegratedManagement4
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
            this.labelID = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelSDMS = new System.Windows.Forms.Label();
            this.labelSOPSimulator = new System.Windows.Forms.Label();
            this.labelMemberID = new System.Windows.Forms.Label();
            this.textBoxMemberID = new System.Windows.Forms.TextBox();
            this.labelMemberName = new System.Windows.Forms.Label();
            this.textBoxMemberName = new System.Windows.Forms.TextBox();
            this.labelConfirmPassword = new System.Windows.Forms.Label();
            this.textBoxConfirmPassword = new System.Windows.Forms.TextBox();
            this.labelCurrentPassword = new System.Windows.Forms.Label();
            this.textBoxCurrentPassword = new System.Windows.Forms.TextBox();
            this.labelChangingPassword = new System.Windows.Forms.Label();
            this.labelConfirmChanging = new System.Windows.Forms.Label();
            this.textBoxChangingPassword = new System.Windows.Forms.TextBox();
            this.textBoxConfirmChanging = new System.Windows.Forms.TextBox();
            this.labelMemberName2 = new System.Windows.Forms.Label();
            this.labelID2 = new System.Windows.Forms.Label();
            this.textBoxMemberName2 = new System.Windows.Forms.TextBox();
            this.textBoxID2 = new System.Windows.Forms.TextBox();
            this.labelFindPasswordDescription = new System.Windows.Forms.Label();
            this.radioChangePassword = new System.Windows.Forms.RadioButton();
            this.radioChangeNickName = new System.Windows.Forms.RadioButton();
            this.checkBoxSimulationMode = new System.Windows.Forms.CheckBox();
            this.checkBoxShowSensorMonitor = new System.Windows.Forms.CheckBox();
            this.timerSensorMonitor = new System.Windows.Forms.Timer(this.components);
            this.labelCurrVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.ckbSaveID = new System.Windows.Forms.CheckBox();
            this.ckbAutoLogin = new System.Windows.Forms.CheckBox();
            this.btnDownloadManual = new System.Windows.Forms.Button();
            this.btnDownloadVideo = new System.Windows.Forms.Button();
            this.btnDownloadPSMHandBook = new System.Windows.Forms.Button();
            this.btnShowInternalClients = new System.Windows.Forms.Button();
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.picAutoLogin = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ribbonButtonSetup = new UnE.GUI.RibbonButton();
            this.lblAutoLogin = new System.Windows.Forms.Label();
            this.lblSaveID = new System.Windows.Forms.Label();
            this.picSaveID = new System.Windows.Forms.PictureBox();
            this.btnRegist = new UnE.GUI.RibbonButton();
            this.btnLogin = new UnE.GUI.RibbonButton();
            this.btnFindPassword = new UnE.GUI.RibbonButton();
            this.pnlMemberAdd = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.eleLevel = new System.Windows.Forms.Integration.ElementHost();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAddNickName = new System.Windows.Forms.Label();
            this.txtAddNickName = new System.Windows.Forms.TextBox();
            this.btnRegistNext = new UnE.GUI.RibbonButton();
            this.btnRegistCancel = new UnE.GUI.RibbonButton();
            this.btnOption = new UnE.GUI.RibbonButton();
            this.pnlSuccessLogin = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.ribbonButtonSetup2 = new UnE.GUI.RibbonButton();
            this.btnLogout = new UnE.GUI.RibbonButton();
            this.btnChangePassword = new UnE.GUI.RibbonButton();
            this.btnSDMS = new UnE.GUI.RibbonButton();
            this.btnSOPSimulator = new UnE.GUI.RibbonButton();
            this.pnlChangeNickName = new System.Windows.Forms.Panel();
            this.btnFindPasswordNext = new UnE.GUI.RibbonButton();
            this.btnFindPasswordCancel = new UnE.GUI.RibbonButton();
            this.labelMemberID2 = new System.Windows.Forms.Label();
            this.textBoxMemberID2 = new System.Windows.Forms.TextBox();
            this.pnlChangePassword = new System.Windows.Forms.Panel();
            this.lblChiefChange = new System.Windows.Forms.Label();
            this.btnChangeChief = new UnE.GUI.RibbonButton();
            this.picChiefChange = new System.Windows.Forms.PictureBox();
            this.lblChangeNickName = new System.Windows.Forms.Label();
            this.picChangeNickName = new System.Windows.Forms.PictureBox();
            this.lblChangePassword = new System.Windows.Forms.Label();
            this.picChangePassword = new System.Windows.Forms.PictureBox();
            this.btnChanging = new UnE.GUI.RibbonButton();
            this.btnCancelChanging = new UnE.GUI.RibbonButton();
            this.lblNickName = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbtnBack = new IntegratedManagement4.RibbonButton();
            this.pnlMemberAdd2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRegistPrev = new UnE.GUI.RibbonButton();
            this.txtChief = new System.Windows.Forms.TextBox();
            this.btnRegistOK2 = new UnE.GUI.RibbonButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPhoeNumber = new System.Windows.Forms.TextBox();
            this.btnSetChief = new UnE.GUI.RibbonButton();
            this.rdoChiefChange = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.RibbonButton();
            this.btnMin = new UnE.GUI.RibbonButton();
            this.pnlLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSaveID)).BeginInit();
            this.pnlMemberAdd.SuspendLayout();
            this.pnlSuccessLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.pnlChangeNickName.SuspendLayout();
            this.pnlChangePassword.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChiefChange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChangeNickName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChangePassword)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlMemberAdd2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelID
            // 
            this.labelID.BackColor = System.Drawing.Color.Transparent;
            this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelID.ForeColor = System.Drawing.Color.White;
            this.labelID.Location = new System.Drawing.Point(63, 95);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(71, 21);
            this.labelID.TabIndex = 1;
            this.labelID.Text = "아 이 디";
            this.labelID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelPassword
            // 
            this.labelPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPassword.ForeColor = System.Drawing.Color.White;
            this.labelPassword.Location = new System.Drawing.Point(66, 124);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(71, 21);
            this.labelPassword.TabIndex = 1;
            this.labelPassword.Text = "비밀번호";
            this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxID
            // 
            this.textBoxID.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID.Location = new System.Drawing.Point(143, 90);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(340, 26);
            this.textBoxID.TabIndex = 0;
            this.textBoxID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPassword.Location = new System.Drawing.Point(143, 124);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(340, 26);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelSDMS
            // 
            this.labelSDMS.AutoSize = true;
            this.labelSDMS.BackColor = System.Drawing.Color.Transparent;
            this.labelSDMS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSDMS.ForeColor = System.Drawing.Color.White;
            this.labelSDMS.Location = new System.Drawing.Point(186, 171);
            this.labelSDMS.Name = "labelSDMS";
            this.labelSDMS.Size = new System.Drawing.Size(72, 16);
            this.labelSDMS.TabIndex = 7;
            this.labelSDMS.Text = "3D 모니터링";
            // 
            // labelSOPSimulator
            // 
            this.labelSOPSimulator.AutoSize = true;
            this.labelSOPSimulator.BackColor = System.Drawing.Color.Transparent;
            this.labelSOPSimulator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSOPSimulator.ForeColor = System.Drawing.Color.White;
            this.labelSOPSimulator.Location = new System.Drawing.Point(356, 171);
            this.labelSOPSimulator.Name = "labelSOPSimulator";
            this.labelSOPSimulator.Size = new System.Drawing.Size(94, 16);
            this.labelSOPSimulator.TabIndex = 7;
            this.labelSOPSimulator.Text = "SOP 시뮬레이터";
            // 
            // labelMemberID
            // 
            this.labelMemberID.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberID.ForeColor = System.Drawing.Color.White;
            this.labelMemberID.Location = new System.Drawing.Point(17, 58);
            this.labelMemberID.Name = "labelMemberID";
            this.labelMemberID.Size = new System.Drawing.Size(120, 21);
            this.labelMemberID.TabIndex = 1;
            this.labelMemberID.Text = "아이디";
            this.labelMemberID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxMemberID
            // 
            this.textBoxMemberID.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberID.Location = new System.Drawing.Point(143, 55);
            this.textBoxMemberID.Name = "textBoxMemberID";
            this.textBoxMemberID.Size = new System.Drawing.Size(340, 26);
            this.textBoxMemberID.TabIndex = 2;
            this.textBoxMemberID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelMemberName
            // 
            this.labelMemberName.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberName.ForeColor = System.Drawing.Color.White;
            this.labelMemberName.Location = new System.Drawing.Point(17, 120);
            this.labelMemberName.Name = "labelMemberName";
            this.labelMemberName.Size = new System.Drawing.Size(120, 21);
            this.labelMemberName.TabIndex = 1;
            this.labelMemberName.Text = "비밀번호";
            this.labelMemberName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxMemberName
            // 
            this.textBoxMemberName.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberName.Location = new System.Drawing.Point(143, 117);
            this.textBoxMemberName.Name = "textBoxMemberName";
            this.textBoxMemberName.Size = new System.Drawing.Size(340, 26);
            this.textBoxMemberName.TabIndex = 3;
            this.textBoxMemberName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelConfirmPassword
            // 
            this.labelConfirmPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmPassword.ForeColor = System.Drawing.Color.White;
            this.labelConfirmPassword.Location = new System.Drawing.Point(17, 151);
            this.labelConfirmPassword.Name = "labelConfirmPassword";
            this.labelConfirmPassword.Size = new System.Drawing.Size(120, 21);
            this.labelConfirmPassword.TabIndex = 1;
            this.labelConfirmPassword.Text = "비밀번호 확인";
            this.labelConfirmPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxConfirmPassword
            // 
            this.textBoxConfirmPassword.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxConfirmPassword.Location = new System.Drawing.Point(143, 148);
            this.textBoxConfirmPassword.Name = "textBoxConfirmPassword";
            this.textBoxConfirmPassword.PasswordChar = '*';
            this.textBoxConfirmPassword.Size = new System.Drawing.Size(340, 26);
            this.textBoxConfirmPassword.TabIndex = 4;
            this.textBoxConfirmPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelCurrentPassword
            // 
            this.labelCurrentPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrentPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrentPassword.ForeColor = System.Drawing.Color.White;
            this.labelCurrentPassword.Location = new System.Drawing.Point(20, 77);
            this.labelCurrentPassword.Margin = new System.Windows.Forms.Padding(0);
            this.labelCurrentPassword.Name = "labelCurrentPassword";
            this.labelCurrentPassword.Size = new System.Drawing.Size(120, 21);
            this.labelCurrentPassword.TabIndex = 1;
            this.labelCurrentPassword.Text = "현재 비밀번호";
            this.labelCurrentPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxCurrentPassword
            // 
            this.textBoxCurrentPassword.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrentPassword.Location = new System.Drawing.Point(143, 75);
            this.textBoxCurrentPassword.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxCurrentPassword.Name = "textBoxCurrentPassword";
            this.textBoxCurrentPassword.PasswordChar = '*';
            this.textBoxCurrentPassword.Size = new System.Drawing.Size(340, 26);
            this.textBoxCurrentPassword.TabIndex = 5;
            this.textBoxCurrentPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelChangingPassword
            // 
            this.labelChangingPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelChangingPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelChangingPassword.ForeColor = System.Drawing.Color.White;
            this.labelChangingPassword.Location = new System.Drawing.Point(20, 109);
            this.labelChangingPassword.Margin = new System.Windows.Forms.Padding(0);
            this.labelChangingPassword.Name = "labelChangingPassword";
            this.labelChangingPassword.Size = new System.Drawing.Size(120, 21);
            this.labelChangingPassword.TabIndex = 1;
            this.labelChangingPassword.Text = "현재    별명";
            this.labelChangingPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelConfirmChanging
            // 
            this.labelConfirmChanging.BackColor = System.Drawing.Color.Transparent;
            this.labelConfirmChanging.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelConfirmChanging.ForeColor = System.Drawing.Color.White;
            this.labelConfirmChanging.Location = new System.Drawing.Point(20, 142);
            this.labelConfirmChanging.Margin = new System.Windows.Forms.Padding(0);
            this.labelConfirmChanging.Name = "labelConfirmChanging";
            this.labelConfirmChanging.Size = new System.Drawing.Size(120, 21);
            this.labelConfirmChanging.TabIndex = 1;
            this.labelConfirmChanging.Text = "비밀번호 확인";
            this.labelConfirmChanging.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxChangingPassword
            // 
            this.textBoxChangingPassword.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxChangingPassword.Location = new System.Drawing.Point(143, 107);
            this.textBoxChangingPassword.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxChangingPassword.Name = "textBoxChangingPassword";
            this.textBoxChangingPassword.PasswordChar = '*';
            this.textBoxChangingPassword.Size = new System.Drawing.Size(340, 26);
            this.textBoxChangingPassword.TabIndex = 6;
            this.textBoxChangingPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxConfirmChanging
            // 
            this.textBoxConfirmChanging.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxConfirmChanging.Location = new System.Drawing.Point(143, 140);
            this.textBoxConfirmChanging.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxConfirmChanging.Name = "textBoxConfirmChanging";
            this.textBoxConfirmChanging.PasswordChar = '*';
            this.textBoxConfirmChanging.Size = new System.Drawing.Size(340, 26);
            this.textBoxConfirmChanging.TabIndex = 7;
            this.textBoxConfirmChanging.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelMemberName2
            // 
            this.labelMemberName2.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberName2.ForeColor = System.Drawing.Color.White;
            this.labelMemberName2.Location = new System.Drawing.Point(18, 110);
            this.labelMemberName2.Name = "labelMemberName2";
            this.labelMemberName2.Size = new System.Drawing.Size(120, 21);
            this.labelMemberName2.TabIndex = 1;
            this.labelMemberName2.Text = "코     드";
            this.labelMemberName2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelID2
            // 
            this.labelID2.BackColor = System.Drawing.Color.Transparent;
            this.labelID2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelID2.ForeColor = System.Drawing.Color.White;
            this.labelID2.Location = new System.Drawing.Point(18, 78);
            this.labelID2.Name = "labelID2";
            this.labelID2.Size = new System.Drawing.Size(120, 21);
            this.labelID2.TabIndex = 1;
            this.labelID2.Text = "아 이 디";
            this.labelID2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxMemberName2
            // 
            this.textBoxMemberName2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberName2.Location = new System.Drawing.Point(143, 107);
            this.textBoxMemberName2.Name = "textBoxMemberName2";
            this.textBoxMemberName2.Size = new System.Drawing.Size(340, 26);
            this.textBoxMemberName2.TabIndex = 9;
            this.textBoxMemberName2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // textBoxID2
            // 
            this.textBoxID2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID2.Location = new System.Drawing.Point(143, 75);
            this.textBoxID2.Name = "textBoxID2";
            this.textBoxID2.Size = new System.Drawing.Size(340, 26);
            this.textBoxID2.TabIndex = 10;
            this.textBoxID2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // labelFindPasswordDescription
            // 
            this.labelFindPasswordDescription.BackColor = System.Drawing.Color.Transparent;
            this.labelFindPasswordDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFindPasswordDescription.ForeColor = System.Drawing.Color.White;
            this.labelFindPasswordDescription.Location = new System.Drawing.Point(134, 46);
            this.labelFindPasswordDescription.Name = "labelFindPasswordDescription";
            this.labelFindPasswordDescription.Size = new System.Drawing.Size(301, 21);
            this.labelFindPasswordDescription.TabIndex = 1;
            this.labelFindPasswordDescription.Text = "새로운 비밀번호를 등록해 주십시오";
            this.labelFindPasswordDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // radioChangePassword
            // 
            this.radioChangePassword.AutoSize = true;
            this.radioChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.radioChangePassword.Checked = true;
            this.radioChangePassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.radioChangePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.radioChangePassword.Location = new System.Drawing.Point(1338, 842);
            this.radioChangePassword.Name = "radioChangePassword";
            this.radioChangePassword.Size = new System.Drawing.Size(101, 19);
            this.radioChangePassword.TabIndex = 20;
            this.radioChangePassword.TabStop = true;
            this.radioChangePassword.Text = "비밀번호 변경";
            this.radioChangePassword.UseVisualStyleBackColor = false;
            this.radioChangePassword.Visible = false;
            this.radioChangePassword.CheckedChanged += new System.EventHandler(this.radioChangePassword_CheckedChanged);
            // 
            // radioChangeNickName
            // 
            this.radioChangeNickName.AutoSize = true;
            this.radioChangeNickName.BackColor = System.Drawing.Color.Transparent;
            this.radioChangeNickName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.radioChangeNickName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.radioChangeNickName.Location = new System.Drawing.Point(1338, 867);
            this.radioChangeNickName.Name = "radioChangeNickName";
            this.radioChangeNickName.Size = new System.Drawing.Size(77, 19);
            this.radioChangeNickName.TabIndex = 20;
            this.radioChangeNickName.Text = "별명 변경";
            this.radioChangeNickName.UseVisualStyleBackColor = false;
            this.radioChangeNickName.Visible = false;
            this.radioChangeNickName.CheckedChanged += new System.EventHandler(this.radioChangeNickName_CheckedChanged);
            // 
            // checkBoxSimulationMode
            // 
            this.checkBoxSimulationMode.AutoSize = true;
            this.checkBoxSimulationMode.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxSimulationMode.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.checkBoxSimulationMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.checkBoxSimulationMode.Location = new System.Drawing.Point(92, 130);
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
            this.checkBoxShowSensorMonitor.Location = new System.Drawing.Point(92, 153);
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
            // labelCurrVersion
            // 
            this.labelCurrVersion.AutoSize = true;
            this.labelCurrVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelCurrVersion.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelCurrVersion.Location = new System.Drawing.Point(19, 304);
            this.labelCurrVersion.Name = "labelCurrVersion";
            this.labelCurrVersion.Size = new System.Drawing.Size(64, 13);
            this.labelCurrVersion.TabIndex = 25;
            this.labelCurrVersion.Text = "Ver.  1.0.0.0";
            // 
            // labelCopyright
            // 
            this.labelCopyright.BackColor = System.Drawing.Color.Transparent;
            this.labelCopyright.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCopyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.labelCopyright.Location = new System.Drawing.Point(15, 304);
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
            this.ckbSaveID.Location = new System.Drawing.Point(216, 322);
            this.ckbSaveID.Name = "ckbSaveID";
            this.ckbSaveID.Size = new System.Drawing.Size(167, 19);
            this.ckbSaveID.TabIndex = 25;
            this.ckbSaveID.Text = "아이디/비밀번호 저장하기";
            this.ckbSaveID.UseVisualStyleBackColor = false;
            this.ckbSaveID.Visible = false;
            this.ckbSaveID.CheckedChanged += new System.EventHandler(this.ckbSaveID_CheckedChanged);
            // 
            // ckbAutoLogin
            // 
            this.ckbAutoLogin.AutoSize = true;
            this.ckbAutoLogin.BackColor = System.Drawing.Color.Transparent;
            this.ckbAutoLogin.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ckbAutoLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.ckbAutoLogin.Location = new System.Drawing.Point(216, 345);
            this.ckbAutoLogin.Name = "ckbAutoLogin";
            this.ckbAutoLogin.Size = new System.Drawing.Size(90, 19);
            this.ckbAutoLogin.TabIndex = 26;
            this.ckbAutoLogin.Text = "자동 로그인";
            this.ckbAutoLogin.UseVisualStyleBackColor = false;
            this.ckbAutoLogin.Visible = false;
            this.ckbAutoLogin.CheckedChanged += new System.EventHandler(this.ckbAutoLogin_CheckedChanged);
            // 
            // btnDownloadManual
            // 
            this.btnDownloadManual.Location = new System.Drawing.Point(92, 16);
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
            this.btnDownloadVideo.Location = new System.Drawing.Point(92, 45);
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
            this.btnDownloadPSMHandBook.Location = new System.Drawing.Point(92, 74);
            this.btnDownloadPSMHandBook.Name = "btnDownloadPSMHandBook";
            this.btnDownloadPSMHandBook.Size = new System.Drawing.Size(104, 23);
            this.btnDownloadPSMHandBook.TabIndex = 27;
            this.btnDownloadPSMHandBook.Text = "PSM 핸드북";
            this.btnDownloadPSMHandBook.UseVisualStyleBackColor = true;
            this.btnDownloadPSMHandBook.Visible = false;
            this.btnDownloadPSMHandBook.Click += new System.EventHandler(this.btnDownloadPSMHandBook_Click);
            // 
            // btnShowInternalClients
            // 
            this.btnShowInternalClients.Location = new System.Drawing.Point(92, 101);
            this.btnShowInternalClients.Name = "btnShowInternalClients";
            this.btnShowInternalClients.Size = new System.Drawing.Size(104, 23);
            this.btnShowInternalClients.TabIndex = 28;
            this.btnShowInternalClients.Text = "접속된 Client 확인";
            this.btnShowInternalClients.UseVisualStyleBackColor = true;
            this.btnShowInternalClients.Visible = false;
            this.btnShowInternalClients.Click += new System.EventHandler(this.btnShowInternalClients_Click);
            // 
            // pnlLogin
            // 
            this.pnlLogin.BackColor = System.Drawing.Color.Transparent;
            this.pnlLogin.Controls.Add(this.picAutoLogin);
            this.pnlLogin.Controls.Add(this.pictureBox1);
            this.pnlLogin.Controls.Add(this.ribbonButtonSetup);
            this.pnlLogin.Controls.Add(this.lblAutoLogin);
            this.pnlLogin.Controls.Add(this.lblSaveID);
            this.pnlLogin.Controls.Add(this.picSaveID);
            this.pnlLogin.Controls.Add(this.textBoxID);
            this.pnlLogin.Controls.Add(this.textBoxPassword);
            this.pnlLogin.Controls.Add(this.btnRegist);
            this.pnlLogin.Controls.Add(this.btnLogin);
            this.pnlLogin.Controls.Add(this.btnFindPassword);
            this.pnlLogin.Controls.Add(this.labelID);
            this.pnlLogin.Controls.Add(this.labelPassword);
            this.pnlLogin.Controls.Add(this.ckbSaveID);
            this.pnlLogin.Controls.Add(this.ckbAutoLogin);
            this.pnlLogin.Location = new System.Drawing.Point(0, 30);
            this.pnlLogin.Name = "pnlLogin";
            this.pnlLogin.Size = new System.Drawing.Size(600, 265);
            this.pnlLogin.TabIndex = 42;
            // 
            // picAutoLogin
            // 
            this.picAutoLogin.BackColor = System.Drawing.Color.Transparent;
            this.picAutoLogin.BackgroundImage = global::IntegratedManagement4.Properties.Resources.@__COMMON_ckb_disable;
            this.picAutoLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picAutoLogin.Location = new System.Drawing.Point(498, 130);
            this.picAutoLogin.Margin = new System.Windows.Forms.Padding(0);
            this.picAutoLogin.Name = "picAutoLogin";
            this.picAutoLogin.Size = new System.Drawing.Size(15, 15);
            this.picAutoLogin.TabIndex = 56;
            this.picAutoLogin.TabStop = false;
            this.picAutoLogin.Click += new System.EventHandler(this.AutoLogin_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Image = global::IntegratedManagement4.Properties.Resources.title_parc1;
            this.pictureBox1.Location = new System.Drawing.Point(200, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(204, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 56;
            this.pictureBox1.TabStop = false;
            // 
            // ribbonButtonSetup
            // 
            this.ribbonButtonSetup.CheckButton = false;
            this.ribbonButtonSetup.CheckedBkgndImage = null;
            this.ribbonButtonSetup.CheckedImage = null;
            this.ribbonButtonSetup.CheckedMouseOver = null;
            this.ribbonButtonSetup.ClickedBackgroundImage = null;
            this.ribbonButtonSetup.ClickedImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetup.ClickedImage")));
            this.ribbonButtonSetup.CustomImageRect = new System.Drawing.Rectangle(0, 0, 44, 44);
            this.ribbonButtonSetup.DisabledBkgndImage = null;
            this.ribbonButtonSetup.DisabledImage = null;
            this.ribbonButtonSetup.ForeColorChecked = System.Drawing.Color.White;
            this.ribbonButtonSetup.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.ribbonButtonSetup.ForeColorDisabled = System.Drawing.Color.White;
            this.ribbonButtonSetup.ForeColorMouseOver = System.Drawing.Color.White;
            this.ribbonButtonSetup.ForeColorsByTypeUse = false;
            this.ribbonButtonSetup.ID = -1;
            this.ribbonButtonSetup.InitButtonWidth = 45;
            this.ribbonButtonSetup.IsChecked = false;
            this.ribbonButtonSetup.Location = new System.Drawing.Point(520, 309);
            this.ribbonButtonSetup.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ribbonButtonSetup.MouseOverBkgndImage = null;
            this.ribbonButtonSetup.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetup.MouseOverImage")));
            this.ribbonButtonSetup.Name = "ribbonButtonSetup";
            this.ribbonButtonSetup.NormalImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetup.NormalImage")));
            this.ribbonButtonSetup.Owner = null;
            this.ribbonButtonSetup.Size = new System.Drawing.Size(45, 45);
            this.ribbonButtonSetup.TabIndex = 37;
            this.ribbonButtonSetup.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButtonSetup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButtonSetup.ToolTipText = "";
            this.ribbonButtonSetup.UseCustomImageRect = true;
            this.ribbonButtonSetup.UseTextLocation = false;
            this.ribbonButtonSetup.UseVisualStyleBackColor = true;
            this.ribbonButtonSetup.Visible = false;
            this.ribbonButtonSetup.Click += new System.EventHandler(this.ribbonButtonSetup_Click_1);
            // 
            // lblAutoLogin
            // 
            this.lblAutoLogin.AutoSize = true;
            this.lblAutoLogin.BackColor = System.Drawing.Color.Transparent;
            this.lblAutoLogin.Font = new System.Drawing.Font("굴림", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAutoLogin.ForeColor = System.Drawing.Color.White;
            this.lblAutoLogin.Location = new System.Drawing.Point(518, 132);
            this.lblAutoLogin.Name = "lblAutoLogin";
            this.lblAutoLogin.Size = new System.Drawing.Size(76, 13);
            this.lblAutoLogin.TabIndex = 57;
            this.lblAutoLogin.Text = "자동 로그인";
            this.lblAutoLogin.Click += new System.EventHandler(this.AutoLogin_Click);
            // 
            // lblSaveID
            // 
            this.lblSaveID.AutoSize = true;
            this.lblSaveID.BackColor = System.Drawing.Color.Transparent;
            this.lblSaveID.Font = new System.Drawing.Font("굴림", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSaveID.ForeColor = System.Drawing.Color.White;
            this.lblSaveID.Location = new System.Drawing.Point(518, 109);
            this.lblSaveID.Name = "lblSaveID";
            this.lblSaveID.Size = new System.Drawing.Size(76, 13);
            this.lblSaveID.TabIndex = 55;
            this.lblSaveID.Text = "ID/PW 저장";
            this.lblSaveID.Click += new System.EventHandler(this.SaveID_Click);
            // 
            // picSaveID
            // 
            this.picSaveID.BackColor = System.Drawing.Color.Transparent;
            this.picSaveID.BackgroundImage = global::IntegratedManagement4.Properties.Resources.@__COMMON_ckb_disable;
            this.picSaveID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picSaveID.Location = new System.Drawing.Point(498, 107);
            this.picSaveID.Margin = new System.Windows.Forms.Padding(0);
            this.picSaveID.Name = "picSaveID";
            this.picSaveID.Size = new System.Drawing.Size(15, 15);
            this.picSaveID.TabIndex = 54;
            this.picSaveID.TabStop = false;
            this.picSaveID.Click += new System.EventHandler(this.SaveID_Click);
            // 
            // btnRegist
            // 
            this.btnRegist.BackColor = System.Drawing.Color.Transparent;
            this.btnRegist.CheckButton = false;
            this.btnRegist.CheckedBkgndImage = null;
            this.btnRegist.CheckedImage = null;
            this.btnRegist.CheckedMouseOver = null;
            this.btnRegist.ClickedBackgroundImage = null;
            this.btnRegist.ClickedImage = global::IntegratedManagement4.Properties.Resources.regist_click;
            this.btnRegist.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnRegist.DisabledBkgndImage = null;
            this.btnRegist.DisabledImage = null;
            this.btnRegist.ForeColorChecked = System.Drawing.Color.White;
            this.btnRegist.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnRegist.ForeColorDisabled = System.Drawing.Color.White;
            this.btnRegist.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnRegist.ForeColorsByTypeUse = false;
            this.btnRegist.ID = -1;
            this.btnRegist.InitButtonWidth = 165;
            this.btnRegist.IsChecked = false;
            this.btnRegist.Location = new System.Drawing.Point(143, 212);
            this.btnRegist.Margin = new System.Windows.Forms.Padding(0);
            this.btnRegist.MouseOverBkgndImage = null;
            this.btnRegist.MouseOverImage = global::IntegratedManagement4.Properties.Resources.regist_hover;
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.NormalImage = global::IntegratedManagement4.Properties.Resources.regist_normal;
            this.btnRegist.Owner = null;
            this.btnRegist.Size = new System.Drawing.Size(165, 45);
            this.btnRegist.TabIndex = 35;
            this.btnRegist.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRegist.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRegist.ToolTipText = "";
            this.btnRegist.UseCustomImageRect = true;
            this.btnRegist.UseTextLocation = false;
            this.btnRegist.UseVisualStyleBackColor = false;
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.CheckButton = false;
            this.btnLogin.CheckedBkgndImage = null;
            this.btnLogin.CheckedImage = null;
            this.btnLogin.CheckedMouseOver = null;
            this.btnLogin.ClickedBackgroundImage = null;
            this.btnLogin.ClickedImage = global::IntegratedManagement4.Properties.Resources.login_click;
            this.btnLogin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 340, 45);
            this.btnLogin.DisabledBkgndImage = null;
            this.btnLogin.DisabledImage = null;
            this.btnLogin.ForeColorChecked = System.Drawing.Color.White;
            this.btnLogin.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnLogin.ForeColorDisabled = System.Drawing.Color.White;
            this.btnLogin.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnLogin.ForeColorsByTypeUse = false;
            this.btnLogin.ID = -1;
            this.btnLogin.InitButtonWidth = 340;
            this.btnLogin.IsChecked = false;
            this.btnLogin.Location = new System.Drawing.Point(143, 160);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(0);
            this.btnLogin.MouseOverBkgndImage = null;
            this.btnLogin.MouseOverImage = global::IntegratedManagement4.Properties.Resources.login_hover;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.NormalImage = global::IntegratedManagement4.Properties.Resources.login_normal;
            this.btnLogin.Owner = null;
            this.btnLogin.Size = new System.Drawing.Size(340, 45);
            this.btnLogin.TabIndex = 34;
            this.btnLogin.TextLocation = new System.Drawing.Point(0, 0);
            this.btnLogin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnLogin.ToolTipText = "";
            this.btnLogin.UseCustomImageRect = true;
            this.btnLogin.UseTextLocation = false;
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnFindPassword
            // 
            this.btnFindPassword.BackColor = System.Drawing.Color.Transparent;
            this.btnFindPassword.CheckButton = false;
            this.btnFindPassword.CheckedBkgndImage = null;
            this.btnFindPassword.CheckedImage = null;
            this.btnFindPassword.CheckedMouseOver = null;
            this.btnFindPassword.ClickedBackgroundImage = null;
            this.btnFindPassword.ClickedImage = global::IntegratedManagement4.Properties.Resources.findPassword_click;
            this.btnFindPassword.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnFindPassword.DisabledBkgndImage = null;
            this.btnFindPassword.DisabledImage = null;
            this.btnFindPassword.ForeColorChecked = System.Drawing.Color.White;
            this.btnFindPassword.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnFindPassword.ForeColorDisabled = System.Drawing.Color.White;
            this.btnFindPassword.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnFindPassword.ForeColorsByTypeUse = false;
            this.btnFindPassword.ID = -1;
            this.btnFindPassword.InitButtonWidth = 165;
            this.btnFindPassword.IsChecked = false;
            this.btnFindPassword.Location = new System.Drawing.Point(318, 212);
            this.btnFindPassword.Margin = new System.Windows.Forms.Padding(0);
            this.btnFindPassword.MouseOverBkgndImage = null;
            this.btnFindPassword.MouseOverImage = global::IntegratedManagement4.Properties.Resources.findPassword_hover;
            this.btnFindPassword.Name = "btnFindPassword";
            this.btnFindPassword.NormalImage = global::IntegratedManagement4.Properties.Resources.findPassword_normal;
            this.btnFindPassword.Owner = null;
            this.btnFindPassword.Size = new System.Drawing.Size(165, 45);
            this.btnFindPassword.TabIndex = 36;
            this.btnFindPassword.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFindPassword.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFindPassword.ToolTipText = "";
            this.btnFindPassword.UseCustomImageRect = true;
            this.btnFindPassword.UseTextLocation = false;
            this.btnFindPassword.UseVisualStyleBackColor = false;
            this.btnFindPassword.Click += new System.EventHandler(this.btnFindPassword_Click);
            // 
            // pnlMemberAdd
            // 
            this.pnlMemberAdd.BackColor = System.Drawing.Color.Transparent;
            this.pnlMemberAdd.Controls.Add(this.label1);
            this.pnlMemberAdd.Controls.Add(this.textBoxCode);
            this.pnlMemberAdd.Controls.Add(this.eleLevel);
            this.pnlMemberAdd.Controls.Add(this.label6);
            this.pnlMemberAdd.Controls.Add(this.label5);
            this.pnlMemberAdd.Controls.Add(this.lblAddNickName);
            this.pnlMemberAdd.Controls.Add(this.txtAddNickName);
            this.pnlMemberAdd.Controls.Add(this.btnRegistNext);
            this.pnlMemberAdd.Controls.Add(this.labelMemberID);
            this.pnlMemberAdd.Controls.Add(this.btnRegistCancel);
            this.pnlMemberAdd.Controls.Add(this.textBoxMemberID);
            this.pnlMemberAdd.Controls.Add(this.labelMemberName);
            this.pnlMemberAdd.Controls.Add(this.btnOption);
            this.pnlMemberAdd.Controls.Add(this.labelConfirmPassword);
            this.pnlMemberAdd.Controls.Add(this.textBoxMemberName);
            this.pnlMemberAdd.Controls.Add(this.textBoxConfirmPassword);
            this.pnlMemberAdd.Location = new System.Drawing.Point(671, 9);
            this.pnlMemberAdd.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMemberAdd.Name = "pnlMemberAdd";
            this.pnlMemberAdd.Size = new System.Drawing.Size(600, 265);
            this.pnlMemberAdd.TabIndex = 43;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(17, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 21);
            this.label1.TabIndex = 59;
            this.label1.Text = "코  드";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxCode
            // 
            this.textBoxCode.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCode.Location = new System.Drawing.Point(143, 179);
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(340, 26);
            this.textBoxCode.TabIndex = 60;
            // 
            // eleLevel
            // 
            this.eleLevel.BackColor = System.Drawing.Color.White;
            this.eleLevel.Location = new System.Drawing.Point(143, 87);
            this.eleLevel.Name = "eleLevel";
            this.eleLevel.Size = new System.Drawing.Size(340, 26);
            this.eleLevel.TabIndex = 58;
            this.eleLevel.Text = "elementHost1";
            this.eleLevel.Child = null;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(17, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 21);
            this.label6.TabIndex = 45;
            this.label6.Text = "계정 등급";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(230, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 37);
            this.label5.TabIndex = 44;
            this.label5.Text = "회원가입";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAddNickName
            // 
            this.lblAddNickName.BackColor = System.Drawing.Color.Transparent;
            this.lblAddNickName.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAddNickName.ForeColor = System.Drawing.Color.White;
            this.lblAddNickName.Location = new System.Drawing.Point(56, 314);
            this.lblAddNickName.Name = "lblAddNickName";
            this.lblAddNickName.Size = new System.Drawing.Size(120, 21);
            this.lblAddNickName.TabIndex = 42;
            this.lblAddNickName.Text = "별명";
            this.lblAddNickName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAddNickName.Visible = false;
            // 
            // txtAddNickName
            // 
            this.txtAddNickName.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtAddNickName.Location = new System.Drawing.Point(182, 311);
            this.txtAddNickName.Name = "txtAddNickName";
            this.txtAddNickName.Size = new System.Drawing.Size(400, 26);
            this.txtAddNickName.TabIndex = 43;
            this.txtAddNickName.Visible = false;
            // 
            // btnRegistNext
            // 
            this.btnRegistNext.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistNext.CheckButton = false;
            this.btnRegistNext.CheckedBkgndImage = null;
            this.btnRegistNext.CheckedImage = null;
            this.btnRegistNext.CheckedMouseOver = null;
            this.btnRegistNext.ClickedBackgroundImage = null;
            this.btnRegistNext.ClickedImage = global::IntegratedManagement4.Properties.Resources.ok_click;
            this.btnRegistNext.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnRegistNext.DisabledBkgndImage = null;
            this.btnRegistNext.DisabledImage = null;
            this.btnRegistNext.ForeColorChecked = System.Drawing.Color.White;
            this.btnRegistNext.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnRegistNext.ForeColorDisabled = System.Drawing.Color.White;
            this.btnRegistNext.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnRegistNext.ForeColorsByTypeUse = false;
            this.btnRegistNext.ID = -1;
            this.btnRegistNext.InitButtonWidth = 165;
            this.btnRegistNext.IsChecked = false;
            this.btnRegistNext.Location = new System.Drawing.Point(143, 211);
            this.btnRegistNext.Margin = new System.Windows.Forms.Padding(0);
            this.btnRegistNext.MouseOverBkgndImage = null;
            this.btnRegistNext.MouseOverImage = global::IntegratedManagement4.Properties.Resources.ok_hover;
            this.btnRegistNext.Name = "btnRegistNext";
            this.btnRegistNext.NormalImage = global::IntegratedManagement4.Properties.Resources.ok_normal;
            this.btnRegistNext.Owner = null;
            this.btnRegistNext.Size = new System.Drawing.Size(165, 45);
            this.btnRegistNext.TabIndex = 40;
            this.btnRegistNext.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRegistNext.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRegistNext.ToolTipText = "";
            this.btnRegistNext.UseCustomImageRect = true;
            this.btnRegistNext.UseTextLocation = false;
            this.btnRegistNext.UseVisualStyleBackColor = false;
            this.btnRegistNext.Click += new System.EventHandler(this.btnRegNext);
            // 
            // btnRegistCancel
            // 
            this.btnRegistCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistCancel.CheckButton = false;
            this.btnRegistCancel.CheckedBkgndImage = null;
            this.btnRegistCancel.CheckedImage = null;
            this.btnRegistCancel.CheckedMouseOver = null;
            this.btnRegistCancel.ClickedBackgroundImage = null;
            this.btnRegistCancel.ClickedImage = global::IntegratedManagement4.Properties.Resources.cancle_click;
            this.btnRegistCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnRegistCancel.DisabledBkgndImage = null;
            this.btnRegistCancel.DisabledImage = null;
            this.btnRegistCancel.ForeColorChecked = System.Drawing.Color.White;
            this.btnRegistCancel.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnRegistCancel.ForeColorDisabled = System.Drawing.Color.White;
            this.btnRegistCancel.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnRegistCancel.ForeColorsByTypeUse = false;
            this.btnRegistCancel.ID = -1;
            this.btnRegistCancel.InitButtonWidth = 165;
            this.btnRegistCancel.IsChecked = false;
            this.btnRegistCancel.Location = new System.Drawing.Point(320, 211);
            this.btnRegistCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnRegistCancel.MouseOverBkgndImage = null;
            this.btnRegistCancel.MouseOverImage = global::IntegratedManagement4.Properties.Resources.cancle_hover;
            this.btnRegistCancel.Name = "btnRegistCancel";
            this.btnRegistCancel.NormalImage = global::IntegratedManagement4.Properties.Resources.cancle_normal;
            this.btnRegistCancel.Owner = null;
            this.btnRegistCancel.Size = new System.Drawing.Size(165, 45);
            this.btnRegistCancel.TabIndex = 41;
            this.btnRegistCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRegistCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRegistCancel.ToolTipText = "";
            this.btnRegistCancel.UseCustomImageRect = true;
            this.btnRegistCancel.UseTextLocation = false;
            this.btnRegistCancel.UseVisualStyleBackColor = false;
            this.btnRegistCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOption
            // 
            this.btnOption.BackColor = System.Drawing.Color.Transparent;
            this.btnOption.CheckButton = false;
            this.btnOption.CheckedBkgndImage = null;
            this.btnOption.CheckedImage = null;
            this.btnOption.CheckedMouseOver = null;
            this.btnOption.ClickedBackgroundImage = null;
            this.btnOption.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnOptionClick;
            this.btnOption.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 55);
            this.btnOption.DisabledBkgndImage = null;
            this.btnOption.DisabledImage = null;
            this.btnOption.ForeColorChecked = System.Drawing.Color.White;
            this.btnOption.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnOption.ForeColorDisabled = System.Drawing.Color.White;
            this.btnOption.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnOption.ForeColorsByTypeUse = false;
            this.btnOption.ID = -1;
            this.btnOption.InitButtonWidth = 120;
            this.btnOption.IsChecked = false;
            this.btnOption.Location = new System.Drawing.Point(12, 286);
            this.btnOption.Margin = new System.Windows.Forms.Padding(0);
            this.btnOption.MouseOverBkgndImage = null;
            this.btnOption.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnOptionClick;
            this.btnOption.Name = "btnOption";
            this.btnOption.NormalImage = global::IntegratedManagement4.Properties.Resources.btnOption;
            this.btnOption.Owner = null;
            this.btnOption.Size = new System.Drawing.Size(120, 55);
            this.btnOption.TabIndex = 39;
            this.btnOption.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOption.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOption.ToolTipText = "";
            this.btnOption.UseCustomImageRect = true;
            this.btnOption.UseTextLocation = false;
            this.btnOption.UseVisualStyleBackColor = false;
            this.btnOption.Visible = false;
            this.btnOption.Click += new System.EventHandler(this.btnOption_Click);
            // 
            // pnlSuccessLogin
            // 
            this.pnlSuccessLogin.BackColor = System.Drawing.Color.Transparent;
            this.pnlSuccessLogin.Controls.Add(this.pictureBox2);
            this.pnlSuccessLogin.Controls.Add(this.ribbonButtonSetup2);
            this.pnlSuccessLogin.Controls.Add(this.btnLogout);
            this.pnlSuccessLogin.Controls.Add(this.btnChangePassword);
            this.pnlSuccessLogin.Controls.Add(this.btnSDMS);
            this.pnlSuccessLogin.Controls.Add(this.btnSOPSimulator);
            this.pnlSuccessLogin.Controls.Add(this.labelSDMS);
            this.pnlSuccessLogin.Controls.Add(this.labelSOPSimulator);
            this.pnlSuccessLogin.Location = new System.Drawing.Point(671, 277);
            this.pnlSuccessLogin.Name = "pnlSuccessLogin";
            this.pnlSuccessLogin.Size = new System.Drawing.Size(600, 250);
            this.pnlSuccessLogin.TabIndex = 44;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox2.Image = global::IntegratedManagement4.Properties.Resources.title_parc1;
            this.pictureBox2.Location = new System.Drawing.Point(200, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(204, 71);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 58;
            this.pictureBox2.TabStop = false;
            // 
            // ribbonButtonSetup2
            // 
            this.ribbonButtonSetup2.CheckButton = false;
            this.ribbonButtonSetup2.CheckedBkgndImage = null;
            this.ribbonButtonSetup2.CheckedImage = null;
            this.ribbonButtonSetup2.CheckedMouseOver = null;
            this.ribbonButtonSetup2.ClickedBackgroundImage = null;
            this.ribbonButtonSetup2.ClickedImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetup2.ClickedImage")));
            this.ribbonButtonSetup2.CustomImageRect = new System.Drawing.Rectangle(0, 0, 44, 44);
            this.ribbonButtonSetup2.DisabledBkgndImage = null;
            this.ribbonButtonSetup2.DisabledImage = null;
            this.ribbonButtonSetup2.ForeColorChecked = System.Drawing.Color.White;
            this.ribbonButtonSetup2.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.ribbonButtonSetup2.ForeColorDisabled = System.Drawing.Color.White;
            this.ribbonButtonSetup2.ForeColorMouseOver = System.Drawing.Color.White;
            this.ribbonButtonSetup2.ForeColorsByTypeUse = false;
            this.ribbonButtonSetup2.ID = -1;
            this.ribbonButtonSetup2.InitButtonWidth = 45;
            this.ribbonButtonSetup2.IsChecked = false;
            this.ribbonButtonSetup2.Location = new System.Drawing.Point(543, 199);
            this.ribbonButtonSetup2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ribbonButtonSetup2.MouseOverBkgndImage = null;
            this.ribbonButtonSetup2.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetup2.MouseOverImage")));
            this.ribbonButtonSetup2.Name = "ribbonButtonSetup2";
            this.ribbonButtonSetup2.NormalImage = ((System.Drawing.Image)(resources.GetObject("ribbonButtonSetup2.NormalImage")));
            this.ribbonButtonSetup2.Owner = null;
            this.ribbonButtonSetup2.Size = new System.Drawing.Size(45, 45);
            this.ribbonButtonSetup2.TabIndex = 58;
            this.ribbonButtonSetup2.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButtonSetup2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButtonSetup2.ToolTipText = "";
            this.ribbonButtonSetup2.UseCustomImageRect = true;
            this.ribbonButtonSetup2.UseTextLocation = false;
            this.ribbonButtonSetup2.UseVisualStyleBackColor = true;
            this.ribbonButtonSetup2.Visible = false;
            this.ribbonButtonSetup2.Click += new System.EventHandler(this.ribbonButtonSetup_Click_1);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.Transparent;
            this.btnLogout.CheckButton = false;
            this.btnLogout.CheckedBkgndImage = null;
            this.btnLogout.CheckedImage = null;
            this.btnLogout.CheckedMouseOver = null;
            this.btnLogout.ClickedBackgroundImage = null;
            this.btnLogout.ClickedImage = global::IntegratedManagement4.Properties.Resources.logout_click;
            this.btnLogout.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnLogout.DisabledBkgndImage = null;
            this.btnLogout.DisabledImage = null;
            this.btnLogout.ForeColor = System.Drawing.Color.Black;
            this.btnLogout.ForeColorChecked = System.Drawing.Color.White;
            this.btnLogout.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnLogout.ForeColorDisabled = System.Drawing.Color.White;
            this.btnLogout.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnLogout.ForeColorsByTypeUse = false;
            this.btnLogout.ID = -1;
            this.btnLogout.InitButtonWidth = 165;
            this.btnLogout.IsChecked = false;
            this.btnLogout.Location = new System.Drawing.Point(142, 200);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(0);
            this.btnLogout.MouseOverBkgndImage = null;
            this.btnLogout.MouseOverImage = global::IntegratedManagement4.Properties.Resources.logout_hover;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.NormalImage = global::IntegratedManagement4.Properties.Resources.logout_normal;
            this.btnLogout.Owner = null;
            this.btnLogout.Size = new System.Drawing.Size(165, 45);
            this.btnLogout.TabIndex = 47;
            this.btnLogout.TextLocation = new System.Drawing.Point(0, 13);
            this.btnLogout.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnLogout.ToolTipText = "";
            this.btnLogout.UseCustomImageRect = true;
            this.btnLogout.UseTextLocation = true;
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.btnChangePassword.CheckButton = false;
            this.btnChangePassword.CheckedBkgndImage = null;
            this.btnChangePassword.CheckedImage = null;
            this.btnChangePassword.CheckedMouseOver = null;
            this.btnChangePassword.ClickedBackgroundImage = null;
            this.btnChangePassword.ClickedImage = global::IntegratedManagement4.Properties.Resources.account_click;
            this.btnChangePassword.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnChangePassword.DisabledBkgndImage = null;
            this.btnChangePassword.DisabledImage = null;
            this.btnChangePassword.ForeColorChecked = System.Drawing.Color.White;
            this.btnChangePassword.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnChangePassword.ForeColorDisabled = System.Drawing.Color.White;
            this.btnChangePassword.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnChangePassword.ForeColorsByTypeUse = false;
            this.btnChangePassword.ID = -1;
            this.btnChangePassword.InitButtonWidth = 165;
            this.btnChangePassword.IsChecked = false;
            this.btnChangePassword.Location = new System.Drawing.Point(319, 200);
            this.btnChangePassword.Margin = new System.Windows.Forms.Padding(0);
            this.btnChangePassword.MouseOverBkgndImage = null;
            this.btnChangePassword.MouseOverImage = global::IntegratedManagement4.Properties.Resources.account_hover;
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.NormalImage = global::IntegratedManagement4.Properties.Resources.account_normal;
            this.btnChangePassword.Owner = null;
            this.btnChangePassword.Size = new System.Drawing.Size(165, 45);
            this.btnChangePassword.TabIndex = 43;
            this.btnChangePassword.TextLocation = new System.Drawing.Point(0, 0);
            this.btnChangePassword.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnChangePassword.ToolTipText = "";
            this.btnChangePassword.UseCustomImageRect = true;
            this.btnChangePassword.UseTextLocation = false;
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnSDMS
            // 
            this.btnSDMS.BackColor = System.Drawing.Color.Transparent;
            this.btnSDMS.CheckButton = false;
            this.btnSDMS.CheckedBkgndImage = null;
            this.btnSDMS.CheckedImage = null;
            this.btnSDMS.CheckedMouseOver = null;
            this.btnSDMS.ClickedBackgroundImage = null;
            this.btnSDMS.ClickedImage = global::IntegratedManagement4.Properties.Resources._3D_Click;
            this.btnSDMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 79, 79);
            this.btnSDMS.DisabledBkgndImage = null;
            this.btnSDMS.DisabledImage = null;
            this.btnSDMS.ForeColor = System.Drawing.Color.Black;
            this.btnSDMS.ForeColorChecked = System.Drawing.Color.White;
            this.btnSDMS.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSDMS.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSDMS.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSDMS.ForeColorsByTypeUse = false;
            this.btnSDMS.ID = -1;
            this.btnSDMS.InitButtonWidth = 79;
            this.btnSDMS.IsChecked = false;
            this.btnSDMS.Location = new System.Drawing.Point(183, 84);
            this.btnSDMS.Margin = new System.Windows.Forms.Padding(0);
            this.btnSDMS.MouseOverBkgndImage = null;
            this.btnSDMS.MouseOverImage = global::IntegratedManagement4.Properties.Resources._3D_Hover;
            this.btnSDMS.Name = "btnSDMS";
            this.btnSDMS.NormalImage = global::IntegratedManagement4.Properties.Resources._3D_Normal;
            this.btnSDMS.Owner = null;
            this.btnSDMS.Size = new System.Drawing.Size(79, 79);
            this.btnSDMS.TabIndex = 48;
            this.btnSDMS.TextLocation = new System.Drawing.Point(0, 13);
            this.btnSDMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSDMS.ToolTipText = "";
            this.btnSDMS.UseCustomImageRect = true;
            this.btnSDMS.UseTextLocation = true;
            this.btnSDMS.UseVisualStyleBackColor = false;
            this.btnSDMS.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // btnSOPSimulator
            // 
            this.btnSOPSimulator.BackColor = System.Drawing.Color.Transparent;
            this.btnSOPSimulator.CheckButton = false;
            this.btnSOPSimulator.CheckedBkgndImage = null;
            this.btnSOPSimulator.CheckedImage = null;
            this.btnSOPSimulator.CheckedMouseOver = null;
            this.btnSOPSimulator.ClickedBackgroundImage = null;
            this.btnSOPSimulator.ClickedImage = global::IntegratedManagement4.Properties.Resources.e_sop_Click;
            this.btnSOPSimulator.CustomImageRect = new System.Drawing.Rectangle(0, 0, 79, 79);
            this.btnSOPSimulator.DisabledBkgndImage = null;
            this.btnSOPSimulator.DisabledImage = null;
            this.btnSOPSimulator.ForeColor = System.Drawing.Color.Black;
            this.btnSOPSimulator.ForeColorChecked = System.Drawing.Color.White;
            this.btnSOPSimulator.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSOPSimulator.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSOPSimulator.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSOPSimulator.ForeColorsByTypeUse = false;
            this.btnSOPSimulator.ID = -1;
            this.btnSOPSimulator.InitButtonWidth = 79;
            this.btnSOPSimulator.IsChecked = false;
            this.btnSOPSimulator.Location = new System.Drawing.Point(359, 84);
            this.btnSOPSimulator.Margin = new System.Windows.Forms.Padding(0);
            this.btnSOPSimulator.MouseOverBkgndImage = null;
            this.btnSOPSimulator.MouseOverImage = global::IntegratedManagement4.Properties.Resources.e_sop_Hover;
            this.btnSOPSimulator.Name = "btnSOPSimulator";
            this.btnSOPSimulator.NormalImage = global::IntegratedManagement4.Properties.Resources.e_sop_Normal;
            this.btnSOPSimulator.Owner = null;
            this.btnSOPSimulator.Size = new System.Drawing.Size(79, 79);
            this.btnSOPSimulator.TabIndex = 49;
            this.btnSOPSimulator.TextLocation = new System.Drawing.Point(0, 13);
            this.btnSOPSimulator.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSOPSimulator.ToolTipText = "";
            this.btnSOPSimulator.UseCustomImageRect = true;
            this.btnSOPSimulator.UseTextLocation = true;
            this.btnSOPSimulator.UseVisualStyleBackColor = false;
            this.btnSOPSimulator.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // pnlChangeNickName
            // 
            this.pnlChangeNickName.BackColor = System.Drawing.Color.Transparent;
            this.pnlChangeNickName.Controls.Add(this.btnFindPasswordNext);
            this.pnlChangeNickName.Controls.Add(this.btnFindPasswordCancel);
            this.pnlChangeNickName.Controls.Add(this.labelMemberID2);
            this.pnlChangeNickName.Controls.Add(this.labelMemberName2);
            this.pnlChangeNickName.Controls.Add(this.labelID2);
            this.pnlChangeNickName.Controls.Add(this.textBoxMemberName2);
            this.pnlChangeNickName.Controls.Add(this.textBoxID2);
            this.pnlChangeNickName.Controls.Add(this.labelFindPasswordDescription);
            this.pnlChangeNickName.Controls.Add(this.textBoxMemberID2);
            this.pnlChangeNickName.Location = new System.Drawing.Point(671, 536);
            this.pnlChangeNickName.Name = "pnlChangeNickName";
            this.pnlChangeNickName.Size = new System.Drawing.Size(600, 265);
            this.pnlChangeNickName.TabIndex = 45;
            // 
            // btnFindPasswordNext
            // 
            this.btnFindPasswordNext.BackColor = System.Drawing.Color.Transparent;
            this.btnFindPasswordNext.CheckButton = false;
            this.btnFindPasswordNext.CheckedBkgndImage = null;
            this.btnFindPasswordNext.CheckedImage = null;
            this.btnFindPasswordNext.CheckedMouseOver = null;
            this.btnFindPasswordNext.ClickedBackgroundImage = null;
            this.btnFindPasswordNext.ClickedImage = global::IntegratedManagement4.Properties.Resources.next_click;
            this.btnFindPasswordNext.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnFindPasswordNext.DisabledBkgndImage = null;
            this.btnFindPasswordNext.DisabledImage = null;
            this.btnFindPasswordNext.ForeColor = System.Drawing.Color.Black;
            this.btnFindPasswordNext.ForeColorChecked = System.Drawing.Color.White;
            this.btnFindPasswordNext.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnFindPasswordNext.ForeColorDisabled = System.Drawing.Color.White;
            this.btnFindPasswordNext.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnFindPasswordNext.ForeColorsByTypeUse = false;
            this.btnFindPasswordNext.ID = -1;
            this.btnFindPasswordNext.InitButtonWidth = 165;
            this.btnFindPasswordNext.IsChecked = false;
            this.btnFindPasswordNext.Location = new System.Drawing.Point(143, 148);
            this.btnFindPasswordNext.Margin = new System.Windows.Forms.Padding(0);
            this.btnFindPasswordNext.MouseOverBkgndImage = null;
            this.btnFindPasswordNext.MouseOverImage = global::IntegratedManagement4.Properties.Resources.next_hover;
            this.btnFindPasswordNext.Name = "btnFindPasswordNext";
            this.btnFindPasswordNext.NormalImage = global::IntegratedManagement4.Properties.Resources.next_normal;
            this.btnFindPasswordNext.Owner = null;
            this.btnFindPasswordNext.Size = new System.Drawing.Size(165, 45);
            this.btnFindPasswordNext.TabIndex = 48;
            this.btnFindPasswordNext.TextLocation = new System.Drawing.Point(0, 13);
            this.btnFindPasswordNext.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFindPasswordNext.ToolTipText = "";
            this.btnFindPasswordNext.UseCustomImageRect = true;
            this.btnFindPasswordNext.UseTextLocation = false;
            this.btnFindPasswordNext.UseVisualStyleBackColor = false;
            this.btnFindPasswordNext.Click += new System.EventHandler(this.btnFindPasswordNext_Click);
            // 
            // btnFindPasswordCancel
            // 
            this.btnFindPasswordCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnFindPasswordCancel.CheckButton = false;
            this.btnFindPasswordCancel.CheckedBkgndImage = null;
            this.btnFindPasswordCancel.CheckedImage = null;
            this.btnFindPasswordCancel.CheckedMouseOver = null;
            this.btnFindPasswordCancel.ClickedBackgroundImage = null;
            this.btnFindPasswordCancel.ClickedImage = global::IntegratedManagement4.Properties.Resources.cancle_click;
            this.btnFindPasswordCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnFindPasswordCancel.DisabledBkgndImage = null;
            this.btnFindPasswordCancel.DisabledImage = null;
            this.btnFindPasswordCancel.ForeColorChecked = System.Drawing.Color.White;
            this.btnFindPasswordCancel.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnFindPasswordCancel.ForeColorDisabled = System.Drawing.Color.White;
            this.btnFindPasswordCancel.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnFindPasswordCancel.ForeColorsByTypeUse = false;
            this.btnFindPasswordCancel.ID = -1;
            this.btnFindPasswordCancel.InitButtonWidth = 165;
            this.btnFindPasswordCancel.IsChecked = false;
            this.btnFindPasswordCancel.Location = new System.Drawing.Point(319, 148);
            this.btnFindPasswordCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnFindPasswordCancel.MouseOverBkgndImage = null;
            this.btnFindPasswordCancel.MouseOverImage = global::IntegratedManagement4.Properties.Resources.cancle_hover;
            this.btnFindPasswordCancel.Name = "btnFindPasswordCancel";
            this.btnFindPasswordCancel.NormalImage = global::IntegratedManagement4.Properties.Resources.cancle_normal;
            this.btnFindPasswordCancel.Owner = null;
            this.btnFindPasswordCancel.Size = new System.Drawing.Size(165, 45);
            this.btnFindPasswordCancel.TabIndex = 47;
            this.btnFindPasswordCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFindPasswordCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFindPasswordCancel.ToolTipText = "";
            this.btnFindPasswordCancel.UseCustomImageRect = true;
            this.btnFindPasswordCancel.UseTextLocation = false;
            this.btnFindPasswordCancel.UseVisualStyleBackColor = false;
            this.btnFindPasswordCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelMemberID2
            // 
            this.labelMemberID2.BackColor = System.Drawing.Color.Transparent;
            this.labelMemberID2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMemberID2.ForeColor = System.Drawing.Color.White;
            this.labelMemberID2.Location = new System.Drawing.Point(18, 47);
            this.labelMemberID2.Name = "labelMemberID2";
            this.labelMemberID2.Size = new System.Drawing.Size(120, 21);
            this.labelMemberID2.TabIndex = 1;
            this.labelMemberID2.Text = "사원번호";
            this.labelMemberID2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelMemberID2.Visible = false;
            // 
            // textBoxMemberID2
            // 
            this.textBoxMemberID2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemberID2.Location = new System.Drawing.Point(143, 44);
            this.textBoxMemberID2.Name = "textBoxMemberID2";
            this.textBoxMemberID2.Size = new System.Drawing.Size(239, 26);
            this.textBoxMemberID2.TabIndex = 8;
            this.textBoxMemberID2.Visible = false;
            this.textBoxMemberID2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // pnlChangePassword
            // 
            this.pnlChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.pnlChangePassword.Controls.Add(this.lblChiefChange);
            this.pnlChangePassword.Controls.Add(this.btnChangeChief);
            this.pnlChangePassword.Controls.Add(this.picChiefChange);
            this.pnlChangePassword.Controls.Add(this.lblChangeNickName);
            this.pnlChangePassword.Controls.Add(this.picChangeNickName);
            this.pnlChangePassword.Controls.Add(this.lblChangePassword);
            this.pnlChangePassword.Controls.Add(this.picChangePassword);
            this.pnlChangePassword.Controls.Add(this.labelChangingPassword);
            this.pnlChangePassword.Controls.Add(this.btnChanging);
            this.pnlChangePassword.Controls.Add(this.btnCancelChanging);
            this.pnlChangePassword.Controls.Add(this.labelCurrentPassword);
            this.pnlChangePassword.Controls.Add(this.textBoxCurrentPassword);
            this.pnlChangePassword.Controls.Add(this.labelConfirmChanging);
            this.pnlChangePassword.Controls.Add(this.textBoxConfirmChanging);
            this.pnlChangePassword.Controls.Add(this.textBoxChangingPassword);
            this.pnlChangePassword.Controls.Add(this.lblNickName);
            this.pnlChangePassword.Location = new System.Drawing.Point(1304, 536);
            this.pnlChangePassword.Name = "pnlChangePassword";
            this.pnlChangePassword.Size = new System.Drawing.Size(600, 265);
            this.pnlChangePassword.TabIndex = 46;
            // 
            // lblChiefChange
            // 
            this.lblChiefChange.AutoSize = true;
            this.lblChiefChange.BackColor = System.Drawing.Color.Transparent;
            this.lblChiefChange.Font = new System.Drawing.Font("굴림", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblChiefChange.ForeColor = System.Drawing.Color.White;
            this.lblChiefChange.Location = new System.Drawing.Point(32, 306);
            this.lblChiefChange.Name = "lblChiefChange";
            this.lblChiefChange.Size = new System.Drawing.Size(82, 13);
            this.lblChiefChange.TabIndex = 64;
            this.lblChiefChange.Text = "책임자 변경";
            this.lblChiefChange.Visible = false;
            this.lblChiefChange.Click += new System.EventHandler(this.ChiefChange_Click);
            // 
            // btnChangeChief
            // 
            this.btnChangeChief.BackColor = System.Drawing.Color.Transparent;
            this.btnChangeChief.CheckButton = false;
            this.btnChangeChief.CheckedBkgndImage = null;
            this.btnChangeChief.CheckedImage = null;
            this.btnChangeChief.CheckedMouseOver = null;
            this.btnChangeChief.ClickedBackgroundImage = null;
            this.btnChangeChief.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnChangeChiefClick;
            this.btnChangeChief.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 55);
            this.btnChangeChief.DisabledBkgndImage = null;
            this.btnChangeChief.DisabledImage = null;
            this.btnChangeChief.ForeColorChecked = System.Drawing.Color.White;
            this.btnChangeChief.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnChangeChief.ForeColorDisabled = System.Drawing.Color.White;
            this.btnChangeChief.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnChangeChief.ForeColorsByTypeUse = false;
            this.btnChangeChief.ID = -1;
            this.btnChangeChief.InitButtonWidth = 120;
            this.btnChangeChief.IsChecked = false;
            this.btnChangeChief.Location = new System.Drawing.Point(451, 275);
            this.btnChangeChief.Margin = new System.Windows.Forms.Padding(0);
            this.btnChangeChief.MouseOverBkgndImage = null;
            this.btnChangeChief.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnChangeChiefClick;
            this.btnChangeChief.Name = "btnChangeChief";
            this.btnChangeChief.NormalImage = global::IntegratedManagement4.Properties.Resources.btnChangeChief;
            this.btnChangeChief.Owner = null;
            this.btnChangeChief.Size = new System.Drawing.Size(120, 55);
            this.btnChangeChief.TabIndex = 65;
            this.btnChangeChief.TextLocation = new System.Drawing.Point(0, 0);
            this.btnChangeChief.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnChangeChief.ToolTipText = "";
            this.btnChangeChief.UseCustomImageRect = true;
            this.btnChangeChief.UseTextLocation = false;
            this.btnChangeChief.UseVisualStyleBackColor = false;
            this.btnChangeChief.Visible = false;
            this.btnChangeChief.Click += new System.EventHandler(this.btnChangeChief_Click);
            // 
            // picChiefChange
            // 
            this.picChiefChange.BackColor = System.Drawing.Color.Transparent;
            this.picChiefChange.BackgroundImage = global::IntegratedManagement4.Properties.Resources.@__SOPEDIT_Disable2;
            this.picChiefChange.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picChiefChange.Location = new System.Drawing.Point(10, 303);
            this.picChiefChange.Margin = new System.Windows.Forms.Padding(0);
            this.picChiefChange.Name = "picChiefChange";
            this.picChiefChange.Size = new System.Drawing.Size(20, 20);
            this.picChiefChange.TabIndex = 63;
            this.picChiefChange.TabStop = false;
            this.picChiefChange.Visible = false;
            this.picChiefChange.Click += new System.EventHandler(this.ChiefChange_Click);
            // 
            // lblChangeNickName
            // 
            this.lblChangeNickName.AutoSize = true;
            this.lblChangeNickName.BackColor = System.Drawing.Color.Transparent;
            this.lblChangeNickName.Font = new System.Drawing.Font("굴림", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblChangeNickName.ForeColor = System.Drawing.Color.White;
            this.lblChangeNickName.Location = new System.Drawing.Point(32, 284);
            this.lblChangeNickName.Name = "lblChangeNickName";
            this.lblChangeNickName.Size = new System.Drawing.Size(68, 13);
            this.lblChangeNickName.TabIndex = 61;
            this.lblChangeNickName.Text = "별명 변경";
            this.lblChangeNickName.Visible = false;
            this.lblChangeNickName.Click += new System.EventHandler(this.ChangeNickName_Click);
            // 
            // picChangeNickName
            // 
            this.picChangeNickName.BackColor = System.Drawing.Color.Transparent;
            this.picChangeNickName.BackgroundImage = global::IntegratedManagement4.Properties.Resources.@__SOPEDIT_Disable2;
            this.picChangeNickName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picChangeNickName.Location = new System.Drawing.Point(10, 281);
            this.picChangeNickName.Margin = new System.Windows.Forms.Padding(0);
            this.picChangeNickName.Name = "picChangeNickName";
            this.picChangeNickName.Size = new System.Drawing.Size(20, 20);
            this.picChangeNickName.TabIndex = 60;
            this.picChangeNickName.TabStop = false;
            this.picChangeNickName.Visible = false;
            this.picChangeNickName.Click += new System.EventHandler(this.ChangeNickName_Click);
            // 
            // lblChangePassword
            // 
            this.lblChangePassword.AutoSize = true;
            this.lblChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.lblChangePassword.Font = new System.Drawing.Font("굴림", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblChangePassword.ForeColor = System.Drawing.Color.White;
            this.lblChangePassword.Location = new System.Drawing.Point(32, 262);
            this.lblChangePassword.Name = "lblChangePassword";
            this.lblChangePassword.Size = new System.Drawing.Size(96, 13);
            this.lblChangePassword.TabIndex = 59;
            this.lblChangePassword.Text = "비밀번호 변경";
            this.lblChangePassword.Visible = false;
            this.lblChangePassword.Click += new System.EventHandler(this.ChangePassword_Click);
            // 
            // picChangePassword
            // 
            this.picChangePassword.BackColor = System.Drawing.Color.Transparent;
            this.picChangePassword.BackgroundImage = global::IntegratedManagement4.Properties.Resources.@__SOPEDIT_Disable2;
            this.picChangePassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picChangePassword.Location = new System.Drawing.Point(10, 259);
            this.picChangePassword.Margin = new System.Windows.Forms.Padding(0);
            this.picChangePassword.Name = "picChangePassword";
            this.picChangePassword.Size = new System.Drawing.Size(20, 20);
            this.picChangePassword.TabIndex = 58;
            this.picChangePassword.TabStop = false;
            this.picChangePassword.Visible = false;
            this.picChangePassword.Click += new System.EventHandler(this.ChangePassword_Click);
            // 
            // btnChanging
            // 
            this.btnChanging.BackColor = System.Drawing.Color.Transparent;
            this.btnChanging.CheckButton = false;
            this.btnChanging.CheckedBkgndImage = null;
            this.btnChanging.CheckedImage = null;
            this.btnChanging.CheckedMouseOver = null;
            this.btnChanging.ClickedBackgroundImage = null;
            this.btnChanging.ClickedImage = global::IntegratedManagement4.Properties.Resources.change_click;
            this.btnChanging.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnChanging.DisabledBkgndImage = null;
            this.btnChanging.DisabledImage = null;
            this.btnChanging.ForeColor = System.Drawing.Color.Black;
            this.btnChanging.ForeColorChecked = System.Drawing.Color.White;
            this.btnChanging.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnChanging.ForeColorDisabled = System.Drawing.Color.White;
            this.btnChanging.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnChanging.ForeColorsByTypeUse = false;
            this.btnChanging.ID = -1;
            this.btnChanging.InitButtonWidth = 165;
            this.btnChanging.IsChecked = false;
            this.btnChanging.Location = new System.Drawing.Point(143, 180);
            this.btnChanging.Margin = new System.Windows.Forms.Padding(0);
            this.btnChanging.MouseOverBkgndImage = null;
            this.btnChanging.MouseOverImage = global::IntegratedManagement4.Properties.Resources.change_hover;
            this.btnChanging.Name = "btnChanging";
            this.btnChanging.NormalImage = global::IntegratedManagement4.Properties.Resources.change_normal;
            this.btnChanging.Owner = null;
            this.btnChanging.Size = new System.Drawing.Size(165, 45);
            this.btnChanging.TabIndex = 49;
            this.btnChanging.TextLocation = new System.Drawing.Point(0, 13);
            this.btnChanging.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnChanging.ToolTipText = "";
            this.btnChanging.UseCustomImageRect = true;
            this.btnChanging.UseTextLocation = false;
            this.btnChanging.UseVisualStyleBackColor = false;
            this.btnChanging.Click += new System.EventHandler(this.btnChanging_Click);
            // 
            // btnCancelChanging
            // 
            this.btnCancelChanging.BackColor = System.Drawing.Color.Transparent;
            this.btnCancelChanging.CheckButton = false;
            this.btnCancelChanging.CheckedBkgndImage = null;
            this.btnCancelChanging.CheckedImage = null;
            this.btnCancelChanging.CheckedMouseOver = null;
            this.btnCancelChanging.ClickedBackgroundImage = null;
            this.btnCancelChanging.ClickedImage = global::IntegratedManagement4.Properties.Resources.cancle_click;
            this.btnCancelChanging.CustomImageRect = new System.Drawing.Rectangle(0, 0, 165, 45);
            this.btnCancelChanging.DisabledBkgndImage = null;
            this.btnCancelChanging.DisabledImage = null;
            this.btnCancelChanging.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancelChanging.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancelChanging.ForeColorDisabled = System.Drawing.Color.White;
            this.btnCancelChanging.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancelChanging.ForeColorsByTypeUse = false;
            this.btnCancelChanging.ID = -1;
            this.btnCancelChanging.InitButtonWidth = 165;
            this.btnCancelChanging.IsChecked = false;
            this.btnCancelChanging.Location = new System.Drawing.Point(318, 180);
            this.btnCancelChanging.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancelChanging.MouseOverBkgndImage = null;
            this.btnCancelChanging.MouseOverImage = global::IntegratedManagement4.Properties.Resources.cancle_hover;
            this.btnCancelChanging.Name = "btnCancelChanging";
            this.btnCancelChanging.NormalImage = global::IntegratedManagement4.Properties.Resources.cancle_normal;
            this.btnCancelChanging.Owner = null;
            this.btnCancelChanging.Size = new System.Drawing.Size(165, 45);
            this.btnCancelChanging.TabIndex = 47;
            this.btnCancelChanging.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancelChanging.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancelChanging.ToolTipText = "";
            this.btnCancelChanging.UseCustomImageRect = true;
            this.btnCancelChanging.UseTextLocation = false;
            this.btnCancelChanging.UseVisualStyleBackColor = false;
            this.btnCancelChanging.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblNickName
            // 
            this.lblNickName.BackColor = System.Drawing.Color.Transparent;
            this.lblNickName.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNickName.ForeColor = System.Drawing.Color.White;
            this.lblNickName.Location = new System.Drawing.Point(141, 110);
            this.lblNickName.Margin = new System.Windows.Forms.Padding(0);
            this.lblNickName.Name = "lblNickName";
            this.lblNickName.Size = new System.Drawing.Size(227, 21);
            this.lblNickName.TabIndex = 62;
            this.lblNickName.Text = "닉네임";
            this.lblNickName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNickName.Visible = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.rbtnBack);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(107, 228);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(108, 50);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // rbtnBack
            // 
            this.rbtnBack.BackColor = System.Drawing.Color.Transparent;
            this.rbtnBack.BackgroundImage = global::IntegratedManagement4.Properties.Resources.back;
            this.rbtnBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnBack.CheckedBkgndImage = null;
            this.rbtnBack.CheckedImage = null;
            this.rbtnBack.IsChecked = false;
            this.rbtnBack.Location = new System.Drawing.Point(3, 5);
            this.rbtnBack.Margin = new System.Windows.Forms.Padding(3, 5, 0, 0);
            this.rbtnBack.MouseOverBkgndImage = null;
            this.rbtnBack.Name = "rbtnBack";
            this.rbtnBack.NormalImage = null;
            this.rbtnBack.Owner = null;
            this.rbtnBack.Size = new System.Drawing.Size(40, 40);
            this.rbtnBack.TabIndex = 22;
            this.rbtnBack.UseVisualStyleBackColor = false;
            this.rbtnBack.Click += new System.EventHandler(this.rbtnBack_Click);
            // 
            // pnlMemberAdd2
            // 
            this.pnlMemberAdd2.BackColor = System.Drawing.Color.Transparent;
            this.pnlMemberAdd2.Controls.Add(this.label3);
            this.pnlMemberAdd2.Controls.Add(this.btnRegistPrev);
            this.pnlMemberAdd2.Controls.Add(this.txtChief);
            this.pnlMemberAdd2.Controls.Add(this.btnRegistOK2);
            this.pnlMemberAdd2.Controls.Add(this.label4);
            this.pnlMemberAdd2.Controls.Add(this.txtPhoeNumber);
            this.pnlMemberAdd2.Controls.Add(this.btnSetChief);
            this.pnlMemberAdd2.Location = new System.Drawing.Point(1345, 15);
            this.pnlMemberAdd2.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMemberAdd2.Name = "pnlMemberAdd2";
            this.pnlMemberAdd2.Size = new System.Drawing.Size(600, 200);
            this.pnlMemberAdd2.TabIndex = 44;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(54, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "책임자";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRegistPrev
            // 
            this.btnRegistPrev.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistPrev.CheckButton = false;
            this.btnRegistPrev.CheckedBkgndImage = null;
            this.btnRegistPrev.CheckedImage = null;
            this.btnRegistPrev.CheckedMouseOver = null;
            this.btnRegistPrev.ClickedBackgroundImage = null;
            this.btnRegistPrev.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnPrevClick;
            this.btnRegistPrev.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.btnRegistPrev.DisabledBkgndImage = null;
            this.btnRegistPrev.DisabledImage = null;
            this.btnRegistPrev.ForeColorChecked = System.Drawing.Color.White;
            this.btnRegistPrev.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnRegistPrev.ForeColorDisabled = System.Drawing.Color.White;
            this.btnRegistPrev.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnRegistPrev.ForeColorsByTypeUse = false;
            this.btnRegistPrev.ID = -1;
            this.btnRegistPrev.InitButtonWidth = 115;
            this.btnRegistPrev.IsChecked = false;
            this.btnRegistPrev.Location = new System.Drawing.Point(286, 143);
            this.btnRegistPrev.Margin = new System.Windows.Forms.Padding(0);
            this.btnRegistPrev.MouseOverBkgndImage = null;
            this.btnRegistPrev.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnPrevClick;
            this.btnRegistPrev.Name = "btnRegistPrev";
            this.btnRegistPrev.NormalImage = global::IntegratedManagement4.Properties.Resources.btnPrev;
            this.btnRegistPrev.Owner = null;
            this.btnRegistPrev.Size = new System.Drawing.Size(115, 45);
            this.btnRegistPrev.TabIndex = 41;
            this.btnRegistPrev.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRegistPrev.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRegistPrev.ToolTipText = "";
            this.btnRegistPrev.UseCustomImageRect = true;
            this.btnRegistPrev.UseTextLocation = false;
            this.btnRegistPrev.UseVisualStyleBackColor = false;
            this.btnRegistPrev.Click += new System.EventHandler(this.btnRegistPrev_Click);
            // 
            // txtChief
            // 
            this.txtChief.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtChief.Location = new System.Drawing.Point(179, 6);
            this.txtChief.Name = "txtChief";
            this.txtChief.ReadOnly = true;
            this.txtChief.Size = new System.Drawing.Size(228, 26);
            this.txtChief.TabIndex = 2;
            // 
            // btnRegistOK2
            // 
            this.btnRegistOK2.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistOK2.CheckButton = false;
            this.btnRegistOK2.CheckedBkgndImage = null;
            this.btnRegistOK2.CheckedImage = null;
            this.btnRegistOK2.CheckedMouseOver = null;
            this.btnRegistOK2.ClickedBackgroundImage = null;
            this.btnRegistOK2.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnOKClick;
            this.btnRegistOK2.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.btnRegistOK2.DisabledBkgndImage = null;
            this.btnRegistOK2.DisabledImage = null;
            this.btnRegistOK2.ForeColorChecked = System.Drawing.Color.White;
            this.btnRegistOK2.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnRegistOK2.ForeColorDisabled = System.Drawing.Color.White;
            this.btnRegistOK2.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnRegistOK2.ForeColorsByTypeUse = false;
            this.btnRegistOK2.ID = -1;
            this.btnRegistOK2.InitButtonWidth = 115;
            this.btnRegistOK2.IsChecked = false;
            this.btnRegistOK2.Location = new System.Drawing.Point(171, 143);
            this.btnRegistOK2.Margin = new System.Windows.Forms.Padding(0);
            this.btnRegistOK2.MouseOverBkgndImage = null;
            this.btnRegistOK2.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnOKClick;
            this.btnRegistOK2.Name = "btnRegistOK2";
            this.btnRegistOK2.NormalImage = global::IntegratedManagement4.Properties.Resources.btnOK;
            this.btnRegistOK2.Owner = null;
            this.btnRegistOK2.Size = new System.Drawing.Size(115, 45);
            this.btnRegistOK2.TabIndex = 40;
            this.btnRegistOK2.TextLocation = new System.Drawing.Point(0, 0);
            this.btnRegistOK2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnRegistOK2.ToolTipText = "";
            this.btnRegistOK2.UseCustomImageRect = true;
            this.btnRegistOK2.UseTextLocation = false;
            this.btnRegistOK2.UseVisualStyleBackColor = false;
            this.btnRegistOK2.Click += new System.EventHandler(this.btnRegistOK_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(54, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 21);
            this.label4.TabIndex = 1;
            this.label4.Text = "전화번호";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhoeNumber
            // 
            this.txtPhoeNumber.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtPhoeNumber.Location = new System.Drawing.Point(179, 39);
            this.txtPhoeNumber.Name = "txtPhoeNumber";
            this.txtPhoeNumber.ReadOnly = true;
            this.txtPhoeNumber.Size = new System.Drawing.Size(228, 26);
            this.txtPhoeNumber.TabIndex = 3;
            // 
            // btnSetChief
            // 
            this.btnSetChief.BackColor = System.Drawing.Color.Transparent;
            this.btnSetChief.CheckButton = false;
            this.btnSetChief.CheckedBkgndImage = null;
            this.btnSetChief.CheckedImage = null;
            this.btnSetChief.CheckedMouseOver = null;
            this.btnSetChief.ClickedBackgroundImage = null;
            this.btnSetChief.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnSetChiefClick;
            this.btnSetChief.CustomImageRect = new System.Drawing.Rectangle(0, 0, 120, 55);
            this.btnSetChief.DisabledBkgndImage = null;
            this.btnSetChief.DisabledImage = null;
            this.btnSetChief.ForeColorChecked = System.Drawing.Color.White;
            this.btnSetChief.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSetChief.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSetChief.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSetChief.ForeColorsByTypeUse = false;
            this.btnSetChief.ID = -1;
            this.btnSetChief.InitButtonWidth = 120;
            this.btnSetChief.IsChecked = false;
            this.btnSetChief.Location = new System.Drawing.Point(410, 3);
            this.btnSetChief.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetChief.MouseOverBkgndImage = null;
            this.btnSetChief.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnSetChiefClick;
            this.btnSetChief.Name = "btnSetChief";
            this.btnSetChief.NormalImage = global::IntegratedManagement4.Properties.Resources.btnSetChief;
            this.btnSetChief.Owner = null;
            this.btnSetChief.Size = new System.Drawing.Size(120, 55);
            this.btnSetChief.TabIndex = 38;
            this.btnSetChief.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSetChief.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSetChief.ToolTipText = "";
            this.btnSetChief.UseCustomImageRect = true;
            this.btnSetChief.UseTextLocation = false;
            this.btnSetChief.UseVisualStyleBackColor = false;
            this.btnSetChief.Click += new System.EventHandler(this.btnSetChief_Click);
            // 
            // rdoChiefChange
            // 
            this.rdoChiefChange.AutoSize = true;
            this.rdoChiefChange.BackColor = System.Drawing.Color.Transparent;
            this.rdoChiefChange.Checked = true;
            this.rdoChiefChange.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.rdoChiefChange.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.rdoChiefChange.Location = new System.Drawing.Point(1338, 817);
            this.rdoChiefChange.Name = "rdoChiefChange";
            this.rdoChiefChange.Size = new System.Drawing.Size(89, 19);
            this.rdoChiefChange.TabIndex = 54;
            this.rdoChiefChange.TabStop = true;
            this.rdoChiefChange.Text = "담당자 변경";
            this.rdoChiefChange.UseVisualStyleBackColor = false;
            this.rdoChiefChange.Visible = false;
            this.rdoChiefChange.CheckedChanged += new System.EventHandler(this.rdoChiefChange_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDownloadManual);
            this.panel1.Controls.Add(this.checkBoxSimulationMode);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.checkBoxShowSensorMonitor);
            this.panel1.Controls.Add(this.btnDownloadVideo);
            this.panel1.Controls.Add(this.btnDownloadPSMHandBook);
            this.panel1.Controls.Add(this.btnShowInternalClients);
            this.panel1.Location = new System.Drawing.Point(1600, 224);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 290);
            this.panel1.TabIndex = 55;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.CheckButton = false;
            this.btnClose.CheckedBkgndImage = null;
            this.btnClose.CheckedImage = null;
            this.btnClose.CheckedMouseOver = null;
            this.btnClose.ClickedBackgroundImage = null;
            this.btnClose.ClickedImage = global::IntegratedManagement4.Properties.Resources.WindowClose_Click;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 20, 20);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.ForeColorChecked = System.Drawing.Color.White;
            this.btnClose.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnClose.ForeColorDisabled = System.Drawing.Color.White;
            this.btnClose.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnClose.ForeColorsByTypeUse = false;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 20;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(574, 6);
            this.btnClose.MouseOverBkgndImage = null;
            this.btnClose.MouseOverImage = global::IntegratedManagement4.Properties.Resources.WindowClose_Click;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::IntegratedManagement4.Properties.Resources.WindowClose;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 53;
            this.btnClose.TextLocation = new System.Drawing.Point(0, 0);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnClose.ToolTipText = "";
            this.btnClose.UseCustomImageRect = true;
            this.btnClose.UseTextLocation = false;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMin
            // 
            this.btnMin.BackColor = System.Drawing.Color.Transparent;
            this.btnMin.CheckButton = false;
            this.btnMin.CheckedBkgndImage = null;
            this.btnMin.CheckedImage = null;
            this.btnMin.CheckedMouseOver = null;
            this.btnMin.ClickedBackgroundImage = null;
            this.btnMin.ClickedImage = global::IntegratedManagement4.Properties.Resources.WindowHide_Click;
            this.btnMin.CustomImageRect = new System.Drawing.Rectangle(0, 0, 20, 20);
            this.btnMin.DisabledBkgndImage = null;
            this.btnMin.DisabledImage = null;
            this.btnMin.ForeColorChecked = System.Drawing.Color.White;
            this.btnMin.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnMin.ForeColorDisabled = System.Drawing.Color.White;
            this.btnMin.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnMin.ForeColorsByTypeUse = false;
            this.btnMin.ID = -1;
            this.btnMin.InitButtonWidth = 20;
            this.btnMin.IsChecked = false;
            this.btnMin.Location = new System.Drawing.Point(546, 6);
            this.btnMin.MouseOverBkgndImage = null;
            this.btnMin.MouseOverImage = global::IntegratedManagement4.Properties.Resources.WindowHide_Click;
            this.btnMin.Name = "btnMin";
            this.btnMin.NormalImage = global::IntegratedManagement4.Properties.Resources.WindowHide;
            this.btnMin.Owner = null;
            this.btnMin.Size = new System.Drawing.Size(20, 20);
            this.btnMin.TabIndex = 52;
            this.btnMin.TextLocation = new System.Drawing.Point(0, 0);
            this.btnMin.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnMin.ToolTipText = "";
            this.btnMin.UseCustomImageRect = true;
            this.btnMin.UseTextLocation = false;
            this.btnMin.UseVisualStyleBackColor = false;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.BackgroundImage = global::IntegratedManagement4.Properties.Resources.background_parc1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1834, 900);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rdoChiefChange);
            this.Controls.Add(this.pnlMemberAdd2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.pnlChangeNickName);
            this.Controls.Add(this.pnlMemberAdd);
            this.Controls.Add(this.radioChangePassword);
            this.Controls.Add(this.labelCurrVersion);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.radioChangeNickName);
            this.Controls.Add(this.pnlChangePassword);
            this.Controls.Add(this.pnlLogin);
            this.Controls.Add(this.pnlSuccessLogin);
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
            this.Move += new System.EventHandler(this.FormMain_Move);
            this.pnlLogin.ResumeLayout(false);
            this.pnlLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSaveID)).EndInit();
            this.pnlMemberAdd.ResumeLayout(false);
            this.pnlMemberAdd.PerformLayout();
            this.pnlSuccessLogin.ResumeLayout(false);
            this.pnlSuccessLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.pnlChangeNickName.ResumeLayout(false);
            this.pnlChangeNickName.PerformLayout();
            this.pnlChangePassword.ResumeLayout(false);
            this.pnlChangePassword.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChiefChange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChangeNickName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChangePassword)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlMemberAdd2.ResumeLayout(false);
            this.pnlMemberAdd2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelSDMS;
        private System.Windows.Forms.Label labelSOPSimulator;
        private System.Windows.Forms.Label labelMemberID;
        private System.Windows.Forms.TextBox textBoxMemberID;
        private System.Windows.Forms.Label labelMemberName;
        private System.Windows.Forms.TextBox textBoxMemberName;
        private System.Windows.Forms.Label labelConfirmPassword;
        private System.Windows.Forms.TextBox textBoxConfirmPassword;
        private System.Windows.Forms.Label labelCurrentPassword;
        private System.Windows.Forms.TextBox textBoxCurrentPassword;
        private System.Windows.Forms.Label labelChangingPassword;
        private System.Windows.Forms.Label labelConfirmChanging;
        private System.Windows.Forms.TextBox textBoxChangingPassword;
        private System.Windows.Forms.TextBox textBoxConfirmChanging;
        private System.Windows.Forms.Label labelMemberName2;
        private System.Windows.Forms.Label labelID2;
        private System.Windows.Forms.TextBox textBoxMemberName2;
        private System.Windows.Forms.TextBox textBoxID2;
        private System.Windows.Forms.Label labelFindPasswordDescription;
        private System.Windows.Forms.RadioButton radioChangePassword;
        private System.Windows.Forms.RadioButton radioChangeNickName;
        private System.Windows.Forms.CheckBox checkBoxSimulationMode;
        private System.Windows.Forms.CheckBox checkBoxShowSensorMonitor;
        private System.Windows.Forms.Timer timerSensorMonitor;
        private RibbonButton rbtnBack;
        private System.Windows.Forms.Label labelCurrVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.CheckBox ckbSaveID;
        private System.Windows.Forms.CheckBox ckbAutoLogin;
        private System.Windows.Forms.Button btnDownloadManual;
        private System.Windows.Forms.Button btnDownloadVideo;
        private System.Windows.Forms.Button btnDownloadPSMHandBook;
        private System.Windows.Forms.Button btnShowInternalClients;
        private UnE.GUI.RibbonButton btnLogin;
        private UnE.GUI.RibbonButton btnRegist;
        private UnE.GUI.RibbonButton btnFindPassword;
        private UnE.GUI.RibbonButton ribbonButtonSetup;
        private UnE.GUI.RibbonButton btnSetChief;
        private UnE.GUI.RibbonButton btnOption;
        private UnE.GUI.RibbonButton btnRegistOK2;
        private UnE.GUI.RibbonButton btnRegistCancel;
        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.Panel pnlMemberAdd;
        private System.Windows.Forms.Panel pnlSuccessLogin;
        private System.Windows.Forms.Panel pnlChangeNickName;
        private System.Windows.Forms.Panel pnlChangePassword;
        private UnE.GUI.RibbonButton btnLogout;
        private UnE.GUI.RibbonButton btnChangePassword;
        private UnE.GUI.RibbonButton btnFindPasswordNext;
        private UnE.GUI.RibbonButton btnFindPasswordCancel;
        private UnE.GUI.RibbonButton btnChanging;
        private UnE.GUI.RibbonButton btnCancelChanging;
        private UnE.GUI.RibbonButton btnSDMS;
        private UnE.GUI.RibbonButton btnSOPSimulator;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private UnE.GUI.RibbonButton btnMin;
        private UnE.GUI.RibbonButton btnClose;
        private System.Windows.Forms.PictureBox picSaveID;
        private System.Windows.Forms.Label lblSaveID;
        private System.Windows.Forms.Label lblAutoLogin;
        private System.Windows.Forms.PictureBox picAutoLogin;
        private System.Windows.Forms.Label lblChangePassword;
        private System.Windows.Forms.PictureBox picChangePassword;
        private System.Windows.Forms.Label lblChangeNickName;
        private System.Windows.Forms.PictureBox picChangeNickName;
        private System.Windows.Forms.Label lblNickName;
        private System.Windows.Forms.Panel pnlMemberAdd2;
        private System.Windows.Forms.Label label3;
        private UnE.GUI.RibbonButton btnRegistPrev;
        private UnE.GUI.RibbonButton btnRegistNext;
        private System.Windows.Forms.TextBox txtChief;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPhoeNumber;
        private System.Windows.Forms.Label lblAddNickName;
        private System.Windows.Forms.TextBox txtAddNickName;
        private System.Windows.Forms.Label lblChiefChange;
        private System.Windows.Forms.PictureBox picChiefChange;
        private System.Windows.Forms.RadioButton rdoChiefChange;
        private UnE.GUI.RibbonButton btnChangeChief;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.RibbonButton ribbonButtonSetup2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Integration.ElementHost eleLevel;
        private System.Windows.Forms.Label labelMemberID2;
        private System.Windows.Forms.TextBox textBoxMemberID2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCode;
    }
}

