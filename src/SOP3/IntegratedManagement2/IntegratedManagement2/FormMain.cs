using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using DBUtility;
using System.IO;

namespace IntegratedManagement2
{
	public partial class FormMain : Form
	{
		private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

		public enum Mode { TRY_LOGIN = 0, REGIST_MEMBER, FIND_PASSWORD, CHANGE_PASSWORD, CHANGE_NICKNAME, SUCCESS_LOGIN, UNKNOWN };

		private int m_nInitWidth = 600;
		private int m_nInitHeight = 330;
		private bool m_bLeftMouseDown = false;
		private Point m_ptMove;

		private Mode m_modeCurrent = Mode.UNKNOWN;
		public Mode CurrentMode
		{
			get { return m_modeCurrent; }
		}

		private Mode m_modePrev = Mode.UNKNOWN;
		public Mode PrevMode
		{
			get { return m_modePrev; }
		}

        public bool SimulationMode
        {
            get { return checkBoxSimulationMode.Checked; }
        }

		private Dictionary<Mode, ArrayList> m_dicModeControls = new Dictionary<Mode, ArrayList>();

		private WebDBManager m_dbMgr = null;
		private LoginManager m_logInMgr = null;
		private ExecuteManager m_exeMgr = null;

		private string m_strNickNameTitle = "별명(선택사항)";
		private string m_strNickName = "";

		private bool m_isSetModeRadioControl = false;

		// SOP 생성기와 조직관리툴을 실행시킬수 있는 ID
		private string m_strAdminID = "";

        static private FormMain m_instance = null;
        static public FormMain Instance
        {
            get { return m_instance; }
        }

		private NetworkManager m_NetMgr = null;
		public NetworkManager NetManager
		{
			get { return m_NetMgr; }
		}

		public LoginManager LoginManager
		{
			get { return m_logInMgr; }
		}

        public IntegratedManagement2.ExecuteManager ExecuteManager
        {
            get { return m_exeMgr; }
        }

		private FormPreference m_SetupForm = null;
		public FormMain()
		{
            m_instance = this;

			InitializeComponent();
			
			m_SetupForm = new FormPreference(this);
			m_SetupForm.TopLevel = false;
			m_SetupForm.StartPosition = FormStartPosition.Manual;
			m_SetupForm.Parent = this;
			this.Controls.Add(m_SetupForm);

			m_dbMgr = new WebDBManager();
			m_NetMgr = new NetworkManager(m_dbMgr);
			m_logInMgr = new LoginManager(m_dbMgr, this);
			m_exeMgr = new ExecuteManager(this);	

			m_strAdminID = RegUtil.ReadRegValue("IntegratedManager", "admin_id");

#if VS2010
            checkBoxSimulationMode.Visible = false;
#endif
		}

		public void ReloadNetwork()
		{
            if (m_logInMgr.LoginState)
            {
                ProcessManager.Instance.AbortAllProcess();

                m_logInMgr.LogOut();

                SetLogout();
            }

            

            m_NetMgr.ReleaseThread();

			m_NetMgr = new NetworkManager(m_dbMgr);

            m_logInMgr = new LoginManager(m_dbMgr, this);

		}

		private void btnClose_Click(object sender, EventArgs e)
		{			
			this.Close();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			ProcessManager.Instance.InitProcess();

			InitButtons();
			InitSize();
			InitPosition();

			SetMode(Mode.TRY_LOGIN);

            try
            {
                //string szFileName = Application.StartupPath + "\\" + "UpdateOrg.exe";
                //if (File.Exists(szFileName))
                //{
                //    File.Delete(szFileName);
                //}
            }
            catch (Exception)
            {
            }

            // Server와 통신 접속이 아직 이루어지지 않았을 수 있으므로 CheckUpdate로 이동
            //ReadCurrentState();

            Thread t = new Thread(CheckUpdate);
            t.Start();		 
		}
        private bool m_bSilentExit = false;
        private bool m_bReservUpdate = false;
        private void CheckUpdate()
        {
            int nSleepCount = 0, nLimit = 10;

            while (!m_NetMgr.ClientProvider.IsConnected && nSleepCount++ < nLimit)
            {
                // Server와 접속할 때까지 기다린다.
                Thread.Sleep(1000);
            }

            ReadCurrentState();

            //bool bReservUpdate = false;
            Updater.AutoUpdater update = new Updater.AutoUpdater();
            while (!m_bExitThread)
            {
                if (m_bReservUpdate == true)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMessage form = new FormMessage();
                        if (form.ShowDialog() == DialogResult.OK)
                        {

                            FormMain.Instance.SaveCurrentState();

                            // need update? 
                            m_bExitThread = true;


                            //if (!ProcessManager.Instance.RunCheckProcess("UpdateOrg"))
                            {
                                ProcessManager.Instance.RunStartProcess("Updater", "");
                            }
                            m_bSilentExit = true;
                            Application.Exit();
                        }
                    });
                }
                // Get Time
                DateTime dtTime = DateTime.Now;
                if (dtTime.Hour >= 23 && dtTime.Hour < 24)
                {
                    CheckNUpdateSystem(update);
                    // Check Update
                    /*if (update.CheckUpdateXML())
                    {
                        FormMain.Instance.Invoke((MethodInvoker)delegate
                        {
                            FormMessage form = new FormMessage();
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                // need update? 
                                FormMain.Instance.SaveCurrentState();

                                m_bExitThread = true;
                                //if (!ProcessManager.Instance.RunCheckProcess("UpdateOrg"))
                                {
                                    ProcessManager.Instance.RunStartProcess("Updater", "");
                                }
                                m_bSilentExit = true;
                                Application.Exit();
                            }
                            else
                            {
                                m_bReservUpdate = true;
                            }
                        });
                    }*/
                }

                for (int i = 0; i < 3600; i++)
                {
                    Thread.Sleep(500);
                    if (m_bExitThread == true)
                        break;
                }
            }
        }

        public void CheckNUpdateSystem(Updater.AutoUpdater update)
        {
            if (update == null)
                update = new Updater.AutoUpdater();

            if (update.CheckUpdateXML())
            {
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMessage form = new FormMessage();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // need update? 
                        FormMain.Instance.SaveCurrentState();

                        m_bExitThread = true;
                        //if (!ProcessManager.Instance.RunCheckProcess("UpdateOrg"))
                        {
                            ProcessManager.Instance.RunStartProcess("Updater", "");
                        }
                        m_bSilentExit = true;
                        Application.Exit();
                    }
                    else
                    {
                        m_bReservUpdate = true;
                    }
                });
            }
        }

		private void InitPosition()
		{
			ArrayList arrLoginControls = new ArrayList();

			arrLoginControls.Add(labelID);
			arrLoginControls.Add(labelPassword);
			arrLoginControls.Add(textBoxID);
			arrLoginControls.Add(textBoxPassword);
			arrLoginControls.Add(btnLogin);
			arrLoginControls.Add(btnRegist);
			arrLoginControls.Add(btnFindPassword);

			InitPosition(groupBoxLogIn, arrLoginControls, Mode.TRY_LOGIN);

			ArrayList arrSuccessLoginControls = new ArrayList();

			arrSuccessLoginControls.Add(btnSOPManager);
			arrSuccessLoginControls.Add(labelSOPManager);
			arrSuccessLoginControls.Add(btnSOPSimulator);
			arrSuccessLoginControls.Add(labelSOPSimulator);
			arrSuccessLoginControls.Add(btnTeamManager);
			arrSuccessLoginControls.Add(labelTeamManager);
			arrSuccessLoginControls.Add(btnMessanger);
			arrSuccessLoginControls.Add(labelMessanger);
			arrSuccessLoginControls.Add(btnSDMS);
			arrSuccessLoginControls.Add(labelSDMS);
			arrSuccessLoginControls.Add(btnLogout);
			arrSuccessLoginControls.Add(btnChangePassword);

			btnSOPManager.Tag = ExecuteManager.APP_TYPE.SOP_MANAGER;
			btnSOPSimulator.Tag = ExecuteManager.APP_TYPE.SOP_SIMULATOR;
			btnTeamManager.Tag = ExecuteManager.APP_TYPE.TEAM_MANAGER;
			btnMessanger.Tag = ExecuteManager.APP_TYPE.SOP_MESSANGER;
			btnSDMS.Tag = ExecuteManager.APP_TYPE.SDMS;

			InitPosition(groupBoxSuccessLogin, arrSuccessLoginControls, Mode.SUCCESS_LOGIN);

			ArrayList arrRegisterControls = new ArrayList();

			arrRegisterControls.Add(labelMemberID);
			arrRegisterControls.Add(labelMemberName);
			arrRegisterControls.Add(labelConfirmPassword);
			arrRegisterControls.Add(textBoxMemberID);
			arrRegisterControls.Add(textBoxMemberName);
			arrRegisterControls.Add(textBoxConfirmPassword);
			arrRegisterControls.Add(btnRegistOK);
			arrRegisterControls.Add(btnRegistCancel);

			InitPosition(groupBoxRegister, arrRegisterControls, Mode.REGIST_MEMBER);

			ArrayList arrChangingPasswordControls = new ArrayList();

			arrChangingPasswordControls.Add(labelCurrentPassword);
			arrChangingPasswordControls.Add(labelChangingPassword);
			arrChangingPasswordControls.Add(labelConfirmChanging);
			arrChangingPasswordControls.Add(textBoxCurrentPassword);
			arrChangingPasswordControls.Add(textBoxChangingPassword);
			arrChangingPasswordControls.Add(textBoxConfirmChanging);
			arrChangingPasswordControls.Add(btnChanging);
			arrChangingPasswordControls.Add(btnCancelChanging);
			arrChangingPasswordControls.Add(radioChangePassword);
			arrChangingPasswordControls.Add(radioChangeNickName);

			InitPosition(groupBoxRegister, arrChangingPasswordControls, Mode.CHANGE_PASSWORD);

			ArrayList arrChangingNickNameControls = new ArrayList();

			arrChangingNickNameControls.Add(labelCurrentPassword);
			arrChangingNickNameControls.Add(labelChangingPassword);
			arrChangingNickNameControls.Add(textBoxCurrentPassword);
			arrChangingNickNameControls.Add(btnChanging);
			arrChangingNickNameControls.Add(btnCancelChanging);
			arrChangingNickNameControls.Add(radioChangePassword);
			arrChangingNickNameControls.Add(radioChangeNickName);

			InitPosition(groupBoxRegister, arrChangingNickNameControls, Mode.CHANGE_NICKNAME);

			ArrayList arrFindPasswordControls = new ArrayList();

			arrFindPasswordControls.Add(labelMemberID2);
			arrFindPasswordControls.Add(labelMemberName2);
			arrFindPasswordControls.Add(labelID2);
			arrFindPasswordControls.Add(textBoxMemberID2);
			arrFindPasswordControls.Add(textBoxMemberName2);
			arrFindPasswordControls.Add(textBoxID2);
			arrFindPasswordControls.Add(btnFindPasswordNext);
			arrFindPasswordControls.Add(btnFindPasswordCancel);
			arrFindPasswordControls.Add(labelFindPasswordDescription);

			InitPosition(groupBoxRegister, arrFindPasswordControls, Mode.FIND_PASSWORD);
		}

		private void InitPosition(Control ctrlPos, ArrayList arrControls, Mode mode)
		{
			ctrlPos.Visible = false;
			m_dicModeControls[mode] = arrControls;

			int nControlCount = arrControls.Count;
			if (nControlCount == 0)
				return;

			Control ctrlFirst = (Control)arrControls[0];

			int xMove = ctrlPos.Location.X - ctrlFirst.Location.X;
			int yMove = ctrlPos.Location.Y - ctrlFirst.Location.Y;

			foreach (Control ctrl in arrControls)
			{
				ctrl.Location = new Point(ctrl.Location.X + xMove, ctrl.Location.Y + yMove);
				ctrl.Visible = false;
			}
		}

		public void SetMode(Mode mode)
		{
			if (m_modeCurrent == mode)
				return;

			HideControls(m_modeCurrent);

			m_modePrev = m_modeCurrent;
			m_modeCurrent = mode;

			if (mode == Mode.CHANGE_NICKNAME)
			{
				labelCurrentPassword.Text = "변경될 별명";
				textBoxCurrentPassword.PasswordChar = '\0';

				labelChangingPassword.Text = "현재    별명      " + LoginManager.Instance.LoginUserNickName;

				m_isSetModeRadioControl = true;
				radioChangeNickName.Checked = true;
			}
			else if (mode == Mode.CHANGE_PASSWORD)
			{
				labelCurrentPassword.Text = "현재 비밀번호";
				textBoxCurrentPassword.PasswordChar = '*';

				labelChangingPassword.Text = "비  밀   번  호";

				m_isSetModeRadioControl = true;
				radioChangePassword.Checked = true;
			}
			else if (mode == Mode.SUCCESS_LOGIN)
			{
				if (LoginManager.LoginID == m_strAdminID)
				{
					btnSOPManager.Enabled = true;
					btnTeamManager.Enabled = true;
				}
				else
				{
					btnSOPManager.Enabled = false;
					btnTeamManager.Enabled = false;
				}

                checkBoxSimulationMode.Enabled = false;
			}
            else if (mode == Mode.TRY_LOGIN)
            {
                checkBoxSimulationMode.Enabled = true;
            }

			ShowControls(mode);

            if (mode == Mode.REGIST_MEMBER)
            {
                SetRegistControlMode(true);
                checkBoxSimulationMode.Enabled = false;
            }
            else if (mode == Mode.FIND_PASSWORD)
            {
                SetFindPasswordControlMode(true);
                checkBoxSimulationMode.Enabled = false;
            }

            ribbonButtonSetup.Visible = true;
		}

		private void SetFindPasswordControlMode(bool initMode)
		{
			labelFindPasswordDescription.Location = labelMemberID2.Location;

			if (initMode)
			{
				labelMemberName2.Text = "이     름";
				labelID2.Text = "아 이 디";
				btnFindPasswordNext.Text = "다음";

				textBoxMemberName2.PasswordChar = '\0';
				textBoxID2.PasswordChar = '\0';

				labelFindPasswordDescription.Visible = false;
				labelMemberID2.Visible = true;
				textBoxMemberID2.Visible = true;
			}
			else
			{
				labelMemberName2.Text = "비밀번호";
				labelID2.Text = "비밀번호 확인";
				btnFindPasswordNext.Text = "확인";

				textBoxMemberName2.PasswordChar = '*';
				textBoxID2.PasswordChar = '*';

				labelFindPasswordDescription.Visible = true;
				labelMemberID2.Visible = false;
				textBoxMemberID2.Visible = false;

				textBoxMemberName2.Text = "";
				textBoxID2.Text = "";

				textBoxMemberName2.Focus();
			}
		}

		private void SetRegistControlMode(bool initMode)
		{
			textBoxMemberID.Focus();

			if (initMode)
			{
				labelMemberID.Text = "사원번호";
				labelMemberName.Text = "이름";
				labelConfirmPassword.Text = m_strNickNameTitle;
				btnRegistOK.Text = "다음";

				textBoxMemberID.Text = "";
				textBoxMemberName.Text = "";

				textBoxMemberName.PasswordChar = '\0';

				textBoxConfirmPassword.PasswordChar = '\0';
				//labelConfirmPassword.Visible = false;
				//textBoxConfirmPassword.Visible = false;
			}
			else
			{
				labelMemberID.Text = "아 이 디";
				labelMemberName.Text = "비밀번호";
				labelConfirmPassword.Text = "비밀번호 확인";
				btnRegistOK.Text = "확인";

				textBoxMemberID.Text = "";
				textBoxMemberName.Text = "";
				textBoxConfirmPassword.Text = "";

				textBoxMemberName.PasswordChar = '*';
				textBoxConfirmPassword.PasswordChar = '*';

				labelConfirmPassword.Visible = true;
				textBoxConfirmPassword.Visible = true;
			}
		}

		private void HideControls(Mode mode)
		{
			if (!m_dicModeControls.ContainsKey(mode))
				return;

			ArrayList arrControls = m_dicModeControls[mode];

			foreach (Control ctrl in arrControls)
			{
				ctrl.Visible = false;
			}
		}

		private void ShowControls(Mode mode)
		{
			if (!m_dicModeControls.ContainsKey(mode))
				return;

			ArrayList arrControls = m_dicModeControls[mode];

			bool firstTextBox = true;
			Type type = typeof(TextBox);

			foreach (Control ctrl in arrControls)
			{
				ctrl.Visible = true;

				if (ctrl.GetType() == type)
				{
					((TextBox)ctrl).Text = "";

					if (firstTextBox)
					{
						ctrl.Focus();
						firstTextBox = false;
					}
				}
			}
		}

		private void InitButtons()
		{
			((RibbonButton)btnLogin).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
			((RibbonButton)btnRegist).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
			((RibbonButton)btnFindPassword).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
			((RibbonButton)btnSOPManager).NormalImage = global::IntegratedManagement2.Properties.Resources.sopmanager;
			((RibbonButton)btnSOPSimulator).NormalImage = global::IntegratedManagement2.Properties.Resources.sopsimulator;
			((RibbonButton)btnTeamManager).NormalImage = global::IntegratedManagement2.Properties.Resources.teammanager;
			((RibbonButton)btnMessanger).NormalImage = global::IntegratedManagement2.Properties.Resources.sopmessanger;
			((RibbonButton) btnSDMS).NormalImage = global::IntegratedManagement2.Properties.Resources.sdms;
			((RibbonButton)btnLogout).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
			((RibbonButton)btnChangePassword).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
			((RibbonButton)btnRegistOK).NormalImage = global::IntegratedManagement2.Properties.Resources.button;
			((RibbonButton)btnRegistCancel).NormalImage = global::IntegratedManagement2.Properties.Resources.button;

			//btnRegist.Size = new Size(135, 44);
			((RibbonButton)btnLogin).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnRegist).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnFindPassword).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnSOPManager).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnSOPSimulator).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnTeamManager).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnMessanger).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnSDMS).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnLogout).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnChangePassword).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnRegistOK).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
			((RibbonButton)btnRegistCancel).MouseOverBkgndImage = global::IntegratedManagement2.Properties.Resources.RibbonMouseOver_bkgnd;
					   
		}

		private void InitSize()
		{
			Point pt = this.Location;
			Rectangle rect = this.ClientRectangle;

			Point ptInit = new Point(pt.X + (rect.Width - m_nInitWidth) / 2, pt.Y + (rect.Height - m_nInitHeight) / 2);
			this.Location = ptInit;

			this.Size = new Size(m_nInitWidth, m_nInitHeight);
		}

		private void FormMain_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				m_bLeftMouseDown = true;
				m_ptMove = PointToScreen(new Point(e.X, e.Y));
			}
		}

		private void FormMain_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (m_bLeftMouseDown == true)
				{
					Point pt = PointToScreen(new Point(e.X, e.Y));
					int dx = pt.X - m_ptMove.X;
					int dy = pt.Y - m_ptMove.Y;
					if (!(dx == 0 && dy == 0))
					{
						Point ptCur = this.Location;
						this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
						m_ptMove.X += dx;
						m_ptMove.Y += dy;
					}
				}
			}
		}

		private void FormMain_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				m_bLeftMouseDown = false;
		}

		private void textBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				if (sender == textBoxID || sender == textBoxPassword)
					btnLogin_Click(null, null);
				else if (sender == textBoxMemberID || sender == textBoxMemberName || sender == textBoxConfirmPassword)
					btnRegistOK_Click(null, null);
				else if (sender == textBoxMemberID2 || sender == textBoxMemberName2 || sender == textBoxID2)
					btnFindPasswordNext_Click(null, null);
				else if (sender == textBoxCurrentPassword || sender == textBoxChangingPassword || sender == textBoxConfirmChanging)
					btnChanging_Click(null, null);
			}
		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			if (textBoxID.Text == "" || textBoxPassword.Text == "")
			{
				MessageBox.Show("아이디와 비밀번호를 입력하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			if (!m_logInMgr.LogIn(textBoxID.Text, textBoxPassword.Text))
			{
				MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ClearLoginTextBox();
				return;
			}
		}

		public void ClearLoginTextBox()
		{
			textBoxID.Text = "";
			textBoxPassword.Text = "";
		}


        private bool m_bExitThread = false;
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
            m_bExitThread = true;

            if (m_logInMgr.LoginState && m_bSilentExit == false)
			{
				if (DialogResult.No == MessageBox.Show("로그인되어 있는 모든 프로그램이 종료됩니다.", "종료 경고", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
				{
					e.Cancel = true;
					return;
				}
                //FormMain.Instance.SaveCurrentState();
			}

			ProcessManager.Instance.AbortAllProcess();

			m_logInMgr.LogOut();

			SetLogout();
			
			m_NetMgr.ReleaseThread();			
		}

		private void btnLogout_Click(object sender, EventArgs e)
		{
			if (DialogResult.No == MessageBox.Show("로그인되어 있는 모든 프로그램이 종료됩니다.", "종료 경고", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
			{
				return;
			}

            DBUtility.RegUtil.WriteRegValue("Update Info", "LastUser", "");
            DBUtility.RegUtil.WriteRegValue("Update Info", "LastEncr", "");

			if (!m_logInMgr.LogOut())
			{
				MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			
			SetLogout();
		}

		public void SetLogout()
		{
			SetMode(Mode.TRY_LOGIN);
			ProcessManager.Instance.AbortAllProcess();
		}


		private void btnApp_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			m_exeMgr.Run((ExecuteManager.APP_TYPE)btn.Tag);
		}

		private void btnRegist_Click(object sender, EventArgs e)
		{
			SetMode(Mode.REGIST_MEMBER);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (PrevMode != Mode.UNKNOWN)
				SetMode(PrevMode);
		}

		private void btnRegistOK_Click(object sender, EventArgs e)
		{
			if (labelConfirmPassword.Text == m_strNickNameTitle)
			{
				if (textBoxMemberID.Text.Length == 0)
				{
					MessageBox.Show("사원번호를 입력하세요");
					textBoxMemberID.Focus();
				}
				else if (textBoxMemberName.Text.Length == 0)
				{
					MessageBox.Show("이름을 입력하세요");
					textBoxMemberName.Focus();
				}
				else
				{
					string strGenUserID = "";
					int nCompanyMemberID = m_logInMgr.GetMemberID(textBoxMemberID.Text, textBoxMemberName.Text, ref strGenUserID);
					if (nCompanyMemberID == -2)
					{
						MessageBox.Show("삭제된 직원이거나 직원 정보가 잘못되었습니다.");
					}
					else if (nCompanyMemberID < 0)
					{
						MessageBox.Show("입력된 직원 정보가 잘못되었습니다.");
					}
					else if (nCompanyMemberID == 0)
					{
						MessageBox.Show("이미 회원가입이 되어 있습니다.");
					}
					else
					{
						m_strNickName = textBoxConfirmPassword.Text;
						SetRegistControlMode(false);
						labelMemberID.Tag = nCompanyMemberID;
					}
				}
			}
			else
			{
				if (textBoxMemberID.Text.Length == 0)
				{
					MessageBox.Show("아이디를 입력하세요");
					textBoxMemberID.Focus();
				}
				else if (textBoxMemberName.Text.Length == 0)
				{
					MessageBox.Show("비밀번호를 입력하세요");
					textBoxMemberName.Focus();
				}
				else if (textBoxConfirmPassword.Text.Length == 0)
				{
					MessageBox.Show("비밀번호를 한번더 입력하세요");
					textBoxConfirmPassword.Focus();
				}
				else
				{
					if (textBoxMemberName.Text != textBoxConfirmPassword.Text)
					{
						MessageBox.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요");
						textBoxConfirmPassword.Text = "";
						textBoxConfirmPassword.Focus();
					}
					else
					{
						int nCompanyMemberID = (int)labelMemberID.Tag;

						if (!m_logInMgr.JoinUser(nCompanyMemberID, textBoxMemberID.Text, textBoxMemberName.Text, m_strNickName))
						{
							MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
						
					}
				}
			}
		}

		public void FailRegisterUser(int nType)
		{
			if (nType == 0)
			{
				MessageBox.Show("이미 존재하는 아이디입니다.");
			}
			else if (nType == -1)
			{
				MessageBox.Show("삭제되거나 사용할 수 없는 사용자 아이디입니다.");
			}
			else if (nType < -1)
			{
				MessageBox.Show("회원가입에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요");
			}
		}

		public void SuccessRegisterUser()
		{
			MessageBox.Show("회원가입에 성공하였습니다.\r\n로그인 화면으로 이동합니다.");
			SetMode(Mode.TRY_LOGIN);

			textBoxID.Text = textBoxMemberID.Text;
			textBoxPassword.Text = "";
			textBoxPassword.Focus();
		}

		private void btnFindPassword_Click(object sender, EventArgs e)
		{
			SetMode(Mode.FIND_PASSWORD);
		}

		private void btnFindPasswordNext_Click(object sender, EventArgs e)
		{
			if (labelFindPasswordDescription.Visible == false)
			{
				if (textBoxMemberID2.Text.Length == 0)
				{
					MessageBox.Show("사원번호를 입력해주세요");
					textBoxMemberID2.Focus();
				}
				else if (textBoxMemberName2.Text.Length == 0)
				{
					MessageBox.Show("이름을 입력해주세요");
					textBoxMemberName2.Focus();
				}
				else if (textBoxID2.Text.Length == 0)
				{
					MessageBox.Show("아이디를 입력해주세요");
					textBoxID2.Focus();
				}
				else
				{
					string strGenUserID = "";
					int nCompanyMemberID = m_logInMgr.GetMemberID(textBoxMemberID2.Text, textBoxMemberName2.Text, ref strGenUserID);

					if (nCompanyMemberID < 0)
						MessageBox.Show("사원번호와 이름이 일치하지 않습니다.");
					else if (nCompanyMemberID > 0)
					{
						MessageBox.Show("회원가입이 되어있지 않습니다.\r\n회원가입을 진행하여 주십시오");
						SetMode(Mode.TRY_LOGIN);
					}
					else
					{
						if (textBoxID2.Text != strGenUserID)
							MessageBox.Show("입력된 직원정보와 아이디가 일치하지 않습니다.\r\n다시 확인하여 주십시오");
						else
						{
							SetFindPasswordControlMode(false);
							labelMemberID2.Tag = strGenUserID;
						}
					}
				}
			}
			else
			{
				if (textBoxMemberName2.Text.Length == 0)
				{
					MessageBox.Show("비밀번호를 입력하세요");
					textBoxMemberName2.Focus();
				}
				else if (textBoxID2.Text.Length == 0)
				{
					MessageBox.Show("비밀번호를 한번더 입력하세요");
					textBoxID2.Focus();
				}
				else
				{
					if (textBoxMemberName2.Text != textBoxID2.Text)
					{
						MessageBox.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요");
						textBoxID2.Text = "";
						textBoxID2.Focus();
					}
					else
					{
						if (!m_logInMgr.SetPassword((string)labelMemberID2.Tag, textBoxMemberName2.Text))
						{
							MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
				}
			}
		}

		public void SuccessChangePassword()
		{
			MessageBox.Show("비밀번호가 변경되었습니다.\r\n로그인 화면으로 이동합니다.");

			SetMode(Mode.TRY_LOGIN);

			textBoxID.Text = (string)labelMemberID2.Tag;
			textBoxPassword.Text = "";
			textBoxPassword.Focus();
		}

		public void FailChangePassword()
		{
			MessageBox.Show("비밀번호 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요");
		}

		public void SuccessChangeNickName()
		{
			MessageBox.Show("별명이 변경되었습니다.\r\n로그인 화면으로 이동합니다.");

			SetMode(Mode.TRY_LOGIN);

			textBoxID.Text = (string)labelMemberID2.Tag;
			textBoxPassword.Text = "";
			textBoxPassword.Focus();
		}

		public void FailChangeNickName()
		{
			MessageBox.Show("별명 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요");
		}

		private void btnChangePassword_Click(object sender, EventArgs e)
		{
			if (radioChangePassword.Checked)
				SetMode(Mode.CHANGE_PASSWORD);
			else
				SetMode(Mode.CHANGE_NICKNAME);

            ribbonButtonSetup.Visible = false;
		}

		private void btnChangeNickName_Click(object sender, EventArgs e)
		{
			SetMode(Mode.CHANGE_NICKNAME);
		}

		private void btnChanging_Click(object sender, EventArgs e)
		{
			if (m_modeCurrent == Mode.CHANGE_PASSWORD)
			{
				if (textBoxCurrentPassword.Text.Length == 0)
				{
					MessageBox.Show("현재 비밀번호를 입력하세요");
					textBoxCurrentPassword.Focus();
				}
				else if (textBoxChangingPassword.Text.Length == 0)
				{
					MessageBox.Show("변경할 비밀번호를 입력하세요");
					textBoxChangingPassword.Focus();
				}
				else if (textBoxConfirmChanging.Text.Length == 0)
				{
					MessageBox.Show("비밀번호를 한번더 입력하세요");
					textBoxConfirmChanging.Focus();
				}
				else
				{
					if (textBoxChangingPassword.Text != textBoxConfirmChanging.Text)
					{
						MessageBox.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요");
						textBoxConfirmChanging.Text = "";
						textBoxConfirmChanging.Focus();
					}
					else
					{

						if (!m_logInMgr.ChangePassword(textBoxCurrentPassword.Text, textBoxChangingPassword.Text))
						{
							MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
				}
			}
			else if (m_modeCurrent == Mode.CHANGE_NICKNAME)
			{
				if (!m_logInMgr.ChangeNickName(textBoxCurrentPassword.Text))
				{
					MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		public void SuccessChangePassword2()
		{
			MessageBox.Show("비밀번호가 변경되었습니다.\r\n이전 화면으로 이동합니다.");

			if (PrevMode != Mode.UNKNOWN)
				SetMode(PrevMode);
		}

		public void SuccessChangeNickName2()
		{
			MessageBox.Show("별명이 변경되었습니다.\r\n이전 화면으로 이동합니다.");

			if (PrevMode != Mode.UNKNOWN)
				SetMode(PrevMode);
		}

		private void btnMin_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

		private void radioChangePassword_CheckedChanged(object sender, EventArgs e)
		{
			if (radioChangePassword.Checked)
			{
				if (m_modeCurrent != Mode.CHANGE_PASSWORD && !m_isSetModeRadioControl)
				{
					Mode modePrev = m_modePrev;
					SetMode(Mode.CHANGE_PASSWORD);
					m_modePrev = modePrev;
				}
			}

			m_isSetModeRadioControl = false;
		}

		private void radioChangeNickName_CheckedChanged(object sender, EventArgs e)
		{
			if (radioChangeNickName.Checked)
			{
				if (m_modeCurrent != Mode.CHANGE_NICKNAME && !m_isSetModeRadioControl)
				{
					Mode modePrev = m_modePrev;
					SetMode(Mode.CHANGE_NICKNAME);
					m_modePrev = modePrev;
				}
			}

			m_isSetModeRadioControl = false;
		}
		
		private void ribbonButtonSetup_Click_1(object sender, EventArgs e)
		{
			m_SetupForm.Location = new Point(70, 30);
			m_SetupForm.BringToFront();
			m_SetupForm.InitDataLoad();
			m_SetupForm.Show();
		}

        private void button1_Click(object sender, EventArgs e)
        {
            //if (!ProcessManager.Instance.RunCheckProcess("UpdateOrg"))
            {
                ProcessManager.Instance.RunStartProcess("Updater.exe", "");
            }
            Application.Exit();
        }

        public void ReadCurrentState()
        {
            string szLastProcs = DBUtility.RegUtil.ReadRegValue("Update Info", "LastProc");
            string szExitUpdate = DBUtility.RegUtil.ReadRegValue("Update Info", "ExitOnUpdate");

            string szLastId = DBUtility.RegUtil.ReadRegValue("Update Info", "LastUser");
            string szLassPass = DBUtility.RegUtil.ReadRegValue("Update Info", "LastEncr");

            if (szExitUpdate == "1")
            {
                if (m_logInMgr.LogIn(szLastId, szLassPass, true))
                {
                    int nCount = 0;
                    while (m_modeCurrent != FormMain.Mode.SUCCESS_LOGIN)
                    {
                        Thread.Sleep(100);
                        nCount++;
                        if( nCount == 100)
                            break;
                    }

                    if (szLastProcs != null && szLastProcs != "")
                    {
                        string[] procs = szLastProcs.Split(',');
                        for (int i = 0; i < procs.Length; i++)
                        {
                            string strProc = procs[i];
                            strProc = strProc.Replace(":1", "");
                            m_exeMgr.Run(strProc);
                        }
                    }
                    DBUtility.RegUtil.WriteRegValue("Update Info", "LastProc", "");
                    DBUtility.RegUtil.WriteRegValue("Update Info", "ExitOnUpdate", "0");
                }
                else
                {
                    ConnectionLogEx.Instance.WriteLine("Auto Login Fail");

                }
            }

            
        }

        public void SaveCurrentState()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, SOPProcessInfo> pair in ProcessManager.Instance.ProcList)
            {
              
                SOPProcessInfo proc = (SOPProcessInfo)pair.Value;
                if (!proc.Exited)
                {
                    if (sb.Length != 0)
                        sb.Append(",");
                    sb.Append(proc.ProcessName);
                    sb.Append(":1");
                }               
            }
            DBUtility.RegUtil.WriteRegValue("Update Info", "LastProc", sb.ToString());
            DBUtility.RegUtil.WriteRegValue("Update Info", "ExitOnUpdate", "1");
        }
	}
}
