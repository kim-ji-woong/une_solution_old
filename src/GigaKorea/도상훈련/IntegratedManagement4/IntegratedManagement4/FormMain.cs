using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using DBUtility2;
using System.IO;
using IntegratedManagement4.PopupDialog;

namespace IntegratedManagement4
{
    public partial class FormMain : Form
	{
        private UEWpfControl.WpfComboBox m_cbLevel = null;

        private class EtcButton
        {
            private UnE.GUI.RibbonButton m_btn = new UnE.GUI.RibbonButton();
            private Label m_label = new Label();
            private FormMain m_frm = null;


            public bool Visible
            {
                get { return m_btn.Visible; }
                set { m_btn.Visible = m_label.Visible = value; }
            }

            public Label Label
            {
                get { return m_label; }
            }

            public UnE.GUI.RibbonButton Button
            {
                get { return m_btn; }
            }

            public EtcButton()
            {
            }

            public EtcButton(FormMain frm, Bitmap btnImage, Bitmap btnOverImage, string strText, UnE.GUI.RibbonButton btnLocation, Label labelLocation, int nLabelWidth, int nLabelMove, ExecuteManager.APP_TYPE appType)
            {
                m_btn.Margin = new Padding(0, 0, 0, 0);
                m_btn.BackColor = System.Drawing.Color.Transparent;

                m_btn.NormalImage = btnImage;
                m_btn.MouseOverImage = btnOverImage;
                m_btn.ClickedImage = btnOverImage;                
                                
                m_btn.Size = new System.Drawing.Size(68, 68);

                m_btn.UseCustomImageRect = true;
                m_btn.CustomImageRect = new Rectangle(0, 0, 68, 68);

                m_btn.Location = btnLocation.Location;
                m_btn.Tag = appType;
                m_btn.Click += new System.EventHandler(frm.btnApp_Click);

                m_label.Location = new Point(labelLocation.Location.X + nLabelMove, labelLocation.Location.Y);
                m_label.Text = strText;
                m_label.ForeColor = labelLocation.ForeColor;
                m_label.BackColor = labelLocation.BackColor;
                m_label.Font = labelLocation.Font;
                m_label.Size = new Size(nLabelWidth, m_label.Size.Height);

                frm.Controls.Add(m_btn);
                frm.Controls.Add(m_label);

                m_btn.Show();
                m_label.Show();

                Visible = false;
            }
        }

		private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

		public enum Mode { TRY_LOGIN = 0, REGIST_MEMBER, FIND_PASSWORD, CHANGE_PASSWORD, CHANGE_NICKNAME, SUCCESS_LOGIN, CHANGE_CHIEF, UNKNOWN };

		private int m_nInitWidth = 463;
		private int m_nInitHeight = 295;
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

		private Dictionary<Mode, ArrayList> m_dicModeControls = new Dictionary<Mode, ArrayList>();

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }
        private WebDBManager m_dbMainMgr = null;

        private LoginManager m_logInMgr = null;
		private ExecuteManager m_exeMgr = null;

		private string m_strNickNameTitle = "별명(선택사항)";
		private string m_strNickName = "";

		private bool m_isSetModeRadioControl = false;

		// SOP 생성기와 조직관리툴을 실행시킬수 있는 ID
		private string m_strAdminID = "";

        // SOP 버전
        private string m_strSOPVersion = string.Empty;

        private List<EtcButton> m_etcButtons = new List<EtcButton>();

        private bool m_needLogin = false;

        static private FormMain m_instance = null;
        static public FormMain Instance
        {
            get { return m_instance; }
        }

		private NetworkWebManager m_NetMgr = null;
		public NetworkWebManager NetManager
		{
			get { return m_NetMgr; }
		}

        private NetworkServer m_netServer = null;
        public NetworkServer NetworkServer
        {
            get { return m_netServer; }
        }

		public LoginManager LoginManager
		{
			get { return m_logInMgr; }
		}

        public IntegratedManagement4.ExecuteManager ExecuteManager
        {
            get { return m_exeMgr; }
        }

		private FormPreference m_SetupForm = null;
        public FormPreference SetupForm
        {
            get { return m_SetupForm; }
        }

        private bool m_isClosing = false;
        public bool Closing
        {
            get { return m_isClosing; }
        }

        private int m_nSensorMonitorProcessID = 0;
        private bool m_ignoreSensorMonitorChanged = false;

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private Chief m_Chief = null;        

        private UnE.GUI.RibbonButton m_btnFirst = null, m_btnSecond = null;
        private Label m_labelFirst = null, m_labelSecond = null;

        public bool SDMSwithSOPSimulator
        {
            get;
            set;
        }

        public Color ColCustomBlack
        {
            get
            {
                return Color.FromArgb(43, 43, 43);
            }
        }

        public Color ColCustomOrange
        {
            get
            {
                return Color.FromArgb(245, 168, 44);
            }
        }

        private String m_memberID = "";
        public String MemberID
        {
            get
            {
                return m_memberID;
            }
            set
            {
                m_memberID = value;
            }
        }
        
        private String m_memberName = "";
        public string MemberName
        {
            get
            {
                return m_memberName;
            }
            set
            {
                m_memberName = value;
            }
        }

        #region 해상도 별 윈도우 비율 정리

        //개발 환경 윈도우 해상도
        private double DevWindowWidth = 1920;
        private double DevWindowHeight = 1040;

        private double WinBoundsWidth = 1920;
        private double WinBoundsHeight = 1040;

        private double CurWinBoundsWidth = 1920;
        private double CurWinBoundsHeight = 1040;

        public double WindowWidthRate = 1d;
        public double WindowHeightRate = 1d;

        public void GetWindowRate()
        {
            /*CurWinBoundsWidth = Screen.FromControl(this).WorkingArea.Width;
            CurWinBoundsHeight = Screen.FromControl(this).WorkingArea.Height;

            WindowWidthRate = Math.Round(CurWinBoundsWidth / WinBoundsWidth, 1);
            WindowHeightRate = Math.Round(CurWinBoundsHeight / WinBoundsHeight, 1);

            if (WindowWidthRate > 2)
                WindowWidthRate = 2;
            if (WindowHeightRate > 2)
                WindowHeightRate = 2;

            if (WindowWidthRate != 1 || WindowHeightRate != 1)
            {
                WinBoundsWidth = CurWinBoundsWidth;
                WinBoundsHeight = CurWinBoundsHeight;

                event_WinRateChanged();
            }*/
        }

        public double[] GetCurWindowRate()
        {
            double WindowWidthRate = Math.Round(WinBoundsWidth / DevWindowWidth, 1);
            double WindowHeightRate = Math.Round(WinBoundsHeight / DevWindowHeight, 1);

            if (WindowWidthRate > 2)
                WindowWidthRate = 2;
            if (WindowHeightRate > 2)
                WindowHeightRate = 2;

            return new double[] { WindowWidthRate, WindowHeightRate };
        }

        public delegate void WindowRateChanged();
        public event WindowRateChanged event_WinRateChanged;

        public void UpdateWindowRate(Control ctl, double pWindowRateWidth, double pWindowRateHeight, String pFontFamily = "굴림")
        {
            if (ctl is UnE.GUI.RibbonButton || ctl.GetType().Name == "RibbonButton")
            {
                #region RibbonButton
                ((UnE.GUI.RibbonButton)ctl).CustomImageRect = new Rectangle(0, 0, (int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                ((UnE.GUI.RibbonButton)ctl).InitButtonWidth = ((UnE.GUI.RibbonButton)ctl).CustomImageRect.Width;
                ((UnE.GUI.RibbonButton)ctl).Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));

                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);

                ((UnE.GUI.RibbonButton)ctl).TextLocation = new Point((int)(((UnE.GUI.RibbonButton)ctl).TextLocation.X * pWindowRateWidth), (int)(((UnE.GUI.RibbonButton)ctl).TextLocation.Y * pWindowRateHeight));
                #endregion
            }
            else if (ctl is Form || ctl.GetType().Name == "Form")
            {
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is Button || ctl.GetType().Name == "Button")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is Label || ctl.GetType().Name == "Label")
            {
                #region Label
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);

                if (((Label)ctl).AutoSize == false)
                {
                    ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                }
                #endregion
            }
            else if (ctl is TextBox || ctl.GetType().Name == "TextBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is PictureBox || ctl.GetType().Name == "PictureBox")
            {
                ctl.Size = new System.Drawing.Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is GroupBox || ctl.GetType().Name == "GroupBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is Panel || ctl.GetType().Name == "Panel")
            {
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is FlowLayoutPanel || ctl.GetType().Name == "FlowLayoutPanel")
            {
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is DataGridView || ctl.GetType().Name == "DataGridView")
            {
                #region DataGridView
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));

                DataGridView dgv = ctl as DataGridView;
                fLabelFontSize = dgv.AlternatingRowsDefaultCellStyle.Font.Size * pWindowRateWidth;
                dgv.AlternatingRowsDefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.Font.Style);

                fLabelFontSize = dgv.DefaultCellStyle.Font.Size * pWindowRateWidth;
                dgv.DefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.Font.Style);

                fLabelFontSize = dgv.RowsDefaultCellStyle.Font.Size * pWindowRateWidth;

                dgv.RowsDefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.Font.Style);

                if (dgv.ColumnCount > 0)
                {
                    for (Int32 index = 0; index < dgv.ColumnCount; index++)
                    {
                        dgv.Columns[index].Width = (int)(dgv.Columns[index].Width * pWindowRateWidth);
                    }
                }

                dgv.ColumnHeadersHeight = (int)(dgv.ColumnHeadersHeight * pWindowRateHeight);
                dgv.RowTemplate.Height = (int)(dgv.RowTemplate.Height * pWindowRateHeight);

                #endregion
            }
            else if (ctl is TreeView || ctl.GetType().Name == "TreeView")
            {
                #region TreeView
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ((TreeView)ctl).Indent = (int)((float)((TreeView)ctl).Indent * pWindowRateHeight);
                #endregion
            }
            else if (ctl is RichTextBox || ctl.GetType().Name == "RichTextBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is ComboBox || ctl.GetType().Name == "ComboBox")
            {
                #region ComboBox
                ComboBox cbo = (ComboBox)ctl;
                float fLabelFontSize = (int)(cbo.Font.Size * pWindowRateWidth);
                cbo.Font = new Font(pFontFamily, fLabelFontSize, ctl.Font.Style);
                cbo.Size = new Size((int)(cbo.Size.Width * pWindowRateWidth), (int)(cbo.Size.Height * pWindowRateHeight));
                #endregion
            }
            else if (ctl is CheckBox || ctl.GetType().Name == "CheckBox")
            {
                float fLabelFontSize = (int)(ctl.Font.Size * pWindowRateWidth);
                ctl.Font = new Font(pFontFamily, fLabelFontSize, ctl.Font.Style);
            }
            else if (ctl is ContextMenuStrip || ctl.GetType().Name == "ContextMenuStrip")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
            }
            else
            {
                return;
            }

            ctl.Location = new Point((int)(ctl.Location.X * pWindowRateWidth), (int)(ctl.Location.Y * pWindowRateHeight));
        }

        #endregion

		public FormMain()
		{
            m_instance = this;

            m_nSiteID = LoadSiteID();
            m_dbMgr = new WebDBManager(m_nSiteID);

            int nMainSiteID;
            // main site id 읽기
            if (ReadConfig("MainSiteID", out nMainSiteID) == false)
                nMainSiteID = 300;

            m_dbMainMgr = new WebDBManager(nMainSiteID);

            /*string szSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");
            if(!int.TryParse(szSiteID, out m_nSiteID))
            {
                m_nSiteID = -1;
                UnE.Utility.UMessageBox.Show(this, "대상 Site를 지정할 수 없습니다. INI파일의 SiteID를 확인하십시요.", "설정오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();                    
            }*/

            InitializeComponent();

            UnE.Utility.UMessageBoxRibbon.Font = new System.Drawing.Font(Program.prgFont, 12f, FontStyle.Bold);
            UnE.Utility.UMessageBoxRibbon.FrameColor = ColCustomBlack;
            UnE.Utility.UMessageBoxRibbon.TitleColor = ColCustomOrange;
            UnE.Utility.UMessageBoxRibbon.BackColor = ColCustomBlack;
            UnE.Utility.UMessageBoxRibbon.ForeColor = Color.White;
            UnE.Utility.UMessageBoxRibbon.CloseButtonImage = global::IntegratedManagement4.Properties.Resources.Close_40_40_Default;
            UnE.Utility.UMessageBoxRibbon.CloseButtonOverImage = global::IntegratedManagement4.Properties.Resources.Close_40_40_Click;

            pnlMemberAdd.Location = pnlLogin.Location;
            pnlMemberAdd2.Location = pnlLogin.Location;
            pnlSuccessLogin.Location = pnlLogin.Location;
            pnlChangeNickName.Location = pnlLogin.Location;
            pnlChangePassword.Location = pnlLogin.Location;

			m_SetupForm = new FormPreference(this);
			m_SetupForm.TopLevel = false;
			m_SetupForm.StartPosition = FormStartPosition.Manual;
			m_SetupForm.Parent = this;
			this.Controls.Add(m_SetupForm);
			
            m_NetMgr = new NetworkWebManager(m_dbMgr);
            m_logInMgr = new LoginManager(m_dbMgr, this);
            
            m_exeMgr = new ExecuteManager(this);
            m_netServer = new NetworkServer(InternalMessage.GetInternalServerPort(m_dbMgr, m_nSiteID));
            SDMSwithSOPSimulator = CheckSDMSwithSOPSimulator();
            m_netServer.NetworkServerLoad();

            m_strAdminID = RegUtil.ReadRegValue("IntegratedManager", "admin_id", m_nSiteID);

            ReadAssemplyInfo();

            UpdateRadioButtonImage();

            event_WinRateChanged += FormMain_event_WinRateChanged;

#if DEBUGGING
            btnShowInternalClients.Visible = true;
#endif
		}

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        void FormMain_event_WinRateChanged()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = FormMain.Instance.WindowWidthRate;
            double WindowRateHeight = FormMain.Instance.WindowHeightRate;


            m_SetupForm.WindowRateWidth = WindowRateWidth;
            m_SetupForm.WindowRateHeight = WindowRateHeight;

            m_SetupForm.WinRateChanged();

            UnE.Utility.UMessageBoxRibbon.WindowRateWidth = dWindowRate[0];
            UnE.Utility.UMessageBoxRibbon.WindowRateHeight = dWindowRate[1];


            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            foreach (Control ctl in this.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in pnlLogin.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in pnlSuccessLogin.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in pnlMemberAdd.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in pnlChangeNickName.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in pnlChangePassword.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }

            foreach (Control ctl in pnlMemberAdd2.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }            
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);                
            }

            FormMain.Instance.UpdateWindowRate(pctl, WindowRateWidth, WindowRateHeight);
        }

        // SDMS와 SOP Simulator를 동시에 사용하는가?
        private bool CheckSDMSwithSOPSimulator()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'SDMSwithSOPSimulator' and SiteID = " + m_nSiteID;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return true;

            string strValue = WebDBManager.GetStringField(arrResult[0].ToString());

            if (strValue == null)
                return true;

            strValue = strValue.Trim();

            if (strValue == "0" || string.Compare(strValue, "false", true) == 0)
                return false;

            return true;
        }

        public int LoadSiteID()
        {
            Utility ini = new Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        private void ReadAssemplyInfo()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

            //m_strSOPVersion = asm.GetName().Version.ToString();
            //labelCurrVersion.Text = String.Format("Ver.  {0}", m_strSOPVersion);

            object[] arrAsm = asm.GetCustomAttributes(false);
            foreach (object assm in arrAsm)
            {
                if (assm.GetType() == typeof(System.Reflection.AssemblyCopyrightAttribute))
                {
                    System.Reflection.AssemblyCopyrightAttribute assAttr = assm as System.Reflection.AssemblyCopyrightAttribute;
                    labelCopyright.Text = assAttr.Copyright.Replace("&", "&&");

                    break;
                }
            }



            string strSOPVersion = RegUtil.ReadRegValue("Update Info", "Current", m_nSiteID);

            if (String.IsNullOrWhiteSpace(strSOPVersion))
            {
                string[] arrStr = { "0", "0", "0", "0" };
                int nPos = 0;

                foreach (string str in strSOPVersion.Split('.'))
                {
                    if (nPos > 3)
                        break;

                    if (str.Length > 1)
                    {
                        foreach (char chr in str.ToArray())
                        {
                            arrStr[nPos++] = chr.ToString();

                            if (nPos > 3)
                                break;
                        }
                    }
                    else
                    {
                        arrStr[nPos++] = str;
                    }
                }

                m_strSOPVersion = String.Join(".", arrStr);
            }
            else
            {
                m_strSOPVersion = strSOPVersion;   
            }

            if (m_strSOPVersion != null && m_strSOPVersion.Length > 0)
            {
                labelCurrVersion.Text = String.Format("Ver. {0}", m_strSOPVersion);

                labelCurrVersion.Visible = true;
            }
            else
                labelCurrVersion.Visible = false;
        }

        private void AddEtcButtons(int nEtcButtonCount)
        {
            EtcButton btnMessenger = //new EtcButton(this, global::IntegratedManagement4.Properties.Resources.sopmessanger, "메시지 전송", m_btnFirst, m_labelFirst, 70, 0, IntegratedManagement4.ExecuteManager.APP_TYPE.SOP_MESSANGER);
                new EtcButton(
                    this,
                    global::IntegratedManagement4.Properties.Resources.sopmessanger,
                    global::IntegratedManagement4.Properties.Resources.sopmessanger,
                    "메세지 전송",m_btnFirst, m_labelFirst, 0, 0, IntegratedManagement4.ExecuteManager.APP_TYPE.SOP_MESSANGER
                    );

            EtcButton btnWeather = //new EtcButton(this, global::IntegratedManagement4.Properties.Resources.weather, "기후입력", m_btnSecond, m_labelSecond, 65, 18, IntegratedManagement4.ExecuteManager.APP_TYPE.SOP_WEATHER);
                new EtcButton(
                    this,
                     global::IntegratedManagement4.Properties.Resources.weather,
                      global::IntegratedManagement4.Properties.Resources.weather,
                      "기후입력", m_btnSecond, m_labelSecond, 0, 0, IntegratedManagement4.ExecuteManager.APP_TYPE.SOP_WEATHER
                    );


            // 기타 버튼이 하나밖에 없으면 그냥 [메시지 전송]툴을 기타 자리에 놓는다.
            /*
            if (nEtcButtonCount == 1)
            {
                ArrayList arrControls = m_dicModeControls[Mode.SUCCESS_LOGIN];

                arrControls.Remove(btnEtc);
                arrControls.Remove(labelEtc);
                Set3ButtonPositions(btnEtc, labelEtc, arrControls);
                return;

                int nIndex1 = arrControls.IndexOf(btnEtc);
                int nIndex2 = arrControls.IndexOf(labelEtc);

                if (nIndex1 < 0 || nIndex2 < 0)
                    m_etcButtons.Add(btnMessenger);
                else
                {
                    arrControls.Remove(btnEtc);
                    arrControls.Remove(labelEtc);

                    if (nIndex1 < nIndex2)
                    {
                        arrControls.Insert(nIndex1, btnMessenger.Button);
                        arrControls.Insert(nIndex2, btnMessenger.Label);
                    }
                    else
                    {
                        arrControls.Insert(nIndex2, btnMessenger.Label);
                        arrControls.Insert(nIndex1, btnMessenger.Button);
                    }

                    btnMessenger.Button.Location = btnEtc.Location;
                    btnMessenger.Label.Location = new Point(labelEtc.Location.X - 15, labelEtc.Location.Y);

                    btnEtc.Visible = labelEtc.Visible = false;
                }
            }
            else
            {
                m_etcButtons.Add(btnMessenger);
            }
            */
            
            m_etcButtons.Add(btnWeather);
        }

        private void Set3ButtonPositions(Button btn4th, Label label4th, ArrayList arrControls)
        {
            Control ctrl1stLabel = (Control)arrControls[1];
            Control ctrl1stButton = (Control)arrControls[0];
            Control ctrl2ndLabel = (Control)arrControls[3];
            Control ctrl2ndButton = (Control)arrControls[2];
            Control ctrl3rdLabel = (Control)arrControls[5];
            Control ctrl3rdButton = (Control)arrControls[4];

            Point pt4thBtn = btn4th.Location;
            Point pt3rdBtn = ctrl3rdButton.Location;
            Point pt3rdLabel = ctrl3rdLabel.Location;
            Point pt2ndBtn = ctrl2ndButton.Location;
            Point pt2ndLabel = ctrl2ndLabel.Location;

            ctrl3rdButton.Location = pt4thBtn;
            ctrl3rdLabel.Location = new Point(ctrl3rdButton.Location.X - (pt3rdBtn.X - pt3rdLabel.X), ctrl3rdLabel.Location.Y);
            ctrl2ndButton.Location = new Point((ctrl1stButton.Location.X + pt4thBtn.X) / 2, ctrl2ndButton.Location.Y);
            ctrl2ndLabel.Location = new Point(ctrl2ndButton.Location.X - (pt2ndBtn.X - pt2ndLabel.X), ctrl2ndLabel.Location.Y);
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

			m_NetMgr = new NetworkWebManager(m_dbMgr);

            m_logInMgr = new LoginManager(m_dbMgr, this);

		}

		private void btnClose_Click(object sender, EventArgs e)
		{			
			this.Close();
		}

        private int GetEtcButtonCount()
        {
            // 하나는 메시지 전송툴
            int nEtcButtonCount = 1;

            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'ShowWeatherInfo' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return 0;

            if (arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

                if (strValue == "1")
                    nEtcButtonCount++;
            }

            return nEtcButtonCount;
        }

		private void FormMain_Load(object sender, EventArgs e)
		{
			ProcessManager.Instance.InitProcess();

            int nEtcButtonCount = GetEtcButtonCount();

			InitButtons();
			InitSize();
			InitPosition();
            AddEtcButtons(nEtcButtonCount);

            SetMode(Mode.TRY_LOGIN);

            Thread t = new Thread(CheckUpdate);
            t.Start();
		}
        private bool m_bSilentExit = false;
        private bool m_bReservUpdate = false;
        
        private void CheckUpdate()
        {
            int nSleepCount = 0, nLimit = 10;

            while (!m_NetMgr.IsConnected && nSleepCount++ < nLimit)
            {
                // Server와 접속할 때까지 기다린다.
                Thread.Sleep(1000);
            }
            
            if (!m_bExitThread)
                ReadCurrentState();

            while (!m_bExitThread)
            {
                Updater.AutoUpdater update = new Updater.AutoUpdater();
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
                }

                for (int i = 0; i < 3600; i++)
                {
                    Thread.Sleep(500);
                    if (m_bExitThread == true)
                        break;
                }
            }
        }

        public void CheckNUpdateSystem(Updater.AutoUpdater update, bool bForceRestart = false)
        {
            if (update == null)
                update = new Updater.AutoUpdater();

            bool bNeedUpdate = update.CheckUpdateXML();

            try
            {
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    if (bNeedUpdate == true)
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
                            return;
                        }
                        else
                        {
                            m_bReservUpdate = true;
                        }
                    }

                    if (bForceRestart == true)
                    {
                        FormMain.Instance.SaveCurrentState();
                        m_bSilentExit = true;
                        {
                            Thread t = new Thread(RunUpdateThread);
                            t.Start();

                        }

                        Application.Exit();
                    }
                });
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.StackTrace);
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
            }
        }

        public void RunUpdateThread()
        {
            Thread.Sleep(3000);
            ProcessManager.Instance.RunStartProcess("Updater", "");
        }

        private void InitPosition()
		{
            //ribbonButtonSetup.Location = new Point(ribbonButtonSetup.Location.X + 450, ribbonButtonSetup.Location.Y);
            //rbtnBack.Location = new Point(ribbonButtonSetup.Location.X - rbtnBack.Size.Width - 5, ribbonButtonSetup.Location.Y);
            rbtnBack.Visible = false;

            // Button들 배열 순서를 바꾼다.
            List<ExecuteManager.APP_TYPE> changingList = RearrangeButtons();

			ArrayList arrLoginControls = new ArrayList();

            //arrLoginControls.Add(labelID);
            //arrLoginControls.Add(labelPassword);
            //arrLoginControls.Add(textBoxID);
            //arrLoginControls.Add(textBoxPassword);
            //arrLoginControls.Add(ckbSaveID);
            //arrLoginControls.Add(ckbAutoLogin);
            //arrLoginControls.Add(btnLogin);
            //arrLoginControls.Add(btnRegist);
            //arrLoginControls.Add(btnFindPassword);

            arrLoginControls.Add(pnlLogin);
			InitPosition(null, arrLoginControls, Mode.TRY_LOGIN);

			ArrayList arrSuccessLoginControls = new ArrayList();

            // 버튼이 4개로 줄어듦에 따라 위치를 X좌표 +100 적용.
            foreach (ExecuteManager.APP_TYPE type in changingList)
            {
                UnE.GUI.RibbonButton btn;
                Label label;

                if (!GetButtonSet(type, out btn, out label))
                    continue;

                arrSuccessLoginControls.Add(btn);
                arrSuccessLoginControls.Add(label);
            }

			/*arrSuccessLoginControls.Add(btnSOPManager);
			arrSuccessLoginControls.Add(labelSOPManager);
			arrSuccessLoginControls.Add(btnSOPSimulator);
			arrSuccessLoginControls.Add(labelSOPSimulator);
			arrSuccessLoginControls.Add(btnTeamManager);
			arrSuccessLoginControls.Add(labelTeamManager);
            arrSuccessLoginControls.Add(btnEtc);
            arrSuccessLoginControls.Add(labelEtc);
			arrSuccessLoginControls.Add(btnSDMS);
			arrSuccessLoginControls.Add(labelSDMS);*/

            arrSuccessLoginControls.Add(btnLogout);
            //arrSuccessLoginControls.Add(btnChangePassword);

            arrSuccessLoginControls.Add(pnlSuccessLogin);

			btnTeamMangaer.Tag = ExecuteManager.APP_TYPE.TEAM_MANAGER; 
            btnViewer.Tag = ExecuteManager.APP_TYPE.VIEWER;
            btnSOPSimulator.Tag = ExecuteManager.APP_TYPE.SOP_SIMULATOR;
            btnSOPManager.Tag = ExecuteManager.APP_TYPE.SOP_MANAGER;



            InitPosition(null, arrSuccessLoginControls, Mode.SUCCESS_LOGIN);

			ArrayList arrRegisterControls = new ArrayList();

            arrRegisterControls.Add(labelMemberID);
            arrRegisterControls.Add(labelMemberName);
            arrRegisterControls.Add(labelConfirmPassword);
            //arrRegisterControls.Add(btnSetChief);
            //arrRegisterControls.Add(btnOption);
            arrRegisterControls.Add(textBoxMemberID);
            arrRegisterControls.Add(textBoxMemberName);
            arrRegisterControls.Add(textBoxConfirmPassword);
            //arrRegisterControls.Add(labelChief);
            arrRegisterControls.Add(btnRegistOK2);
            arrRegisterControls.Add(btnRegistCancel);

            arrRegisterControls.Add(pnlMemberAdd);
            arrRegisterControls.Add(pnlMemberAdd2);            

			InitPosition(null, arrRegisterControls, Mode.REGIST_MEMBER);

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
            arrChangingPasswordControls.Add(rdoChiefChange);

            arrChangingPasswordControls.Add(pnlChangePassword);
			InitPosition(null, arrChangingPasswordControls, Mode.CHANGE_PASSWORD);

			ArrayList arrChangingNickNameControls = new ArrayList();

            arrChangingNickNameControls.Add(labelCurrentPassword);
            arrChangingNickNameControls.Add(labelChangingPassword);
            arrChangingNickNameControls.Add(textBoxCurrentPassword);
            arrChangingNickNameControls.Add(btnChanging);
            arrChangingNickNameControls.Add(btnCancelChanging);
            arrChangingNickNameControls.Add(radioChangePassword);
            arrChangingNickNameControls.Add(radioChangeNickName);
            arrChangingPasswordControls.Add(rdoChiefChange);
            arrChangingNickNameControls.Add(lblNickName);

            arrChangingNickNameControls.Add(pnlChangePassword);
			InitPosition(null, arrChangingNickNameControls, Mode.CHANGE_NICKNAME);

            //
            ArrayList arrChangingChiefControls = new ArrayList();

            arrChangingChiefControls.Add(labelCurrentPassword);
            arrChangingChiefControls.Add(labelChangingPassword);
            arrChangingChiefControls.Add(textBoxCurrentPassword);
            arrChangingChiefControls.Add(textBoxChangingPassword);
            
            arrChangingChiefControls.Add(btnChangeChief);

            arrChangingChiefControls.Add(btnChanging);
            arrChangingChiefControls.Add(btnCancelChanging);
            arrChangingChiefControls.Add(radioChangePassword);
            arrChangingChiefControls.Add(radioChangeNickName);
            arrChangingChiefControls.Add(rdoChiefChange);

            arrChangingChiefControls.Add(pnlChangePassword);
            InitPosition(null, arrChangingChiefControls, Mode.CHANGE_CHIEF);
            //

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


            arrFindPasswordControls.Add(pnlChangeNickName);
			InitPosition(null, arrFindPasswordControls, Mode.FIND_PASSWORD);
		}

		private void InitPosition(Control ctrlPos, ArrayList arrControls, Mode mode)
		{
			//ctrlPos.Visible = false;
			m_dicModeControls[mode] = arrControls;

			int nControlCount = arrControls.Count;
			if (nControlCount == 0)
				return;

			//Control ctrlFirst = (Control)arrControls[0];

            //int xMove = ctrlPos.Location.X - ctrlFirst.Location.X;
            //int yMove = ctrlPos.Location.Y - ctrlFirst.Location.Y;

			foreach (Control ctrl in arrControls)
			{
				//ctrl.Location = new Point(ctrl.Location.X + xMove, ctrl.Location.Y + yMove);
				ctrl.Visible = false;
			}
		}

		public void SetMode(Mode mode,Object obj = null)
		{
			if (m_modeCurrent == mode)
				return;

			HideControls(m_modeCurrent);

			m_modePrev = m_modeCurrent;
			m_modeCurrent = mode;

            checkBoxShowSensorMonitor.Visible = false;            

			if (mode == Mode.CHANGE_NICKNAME)
			{
				labelCurrentPassword.Text = "변경될 별명";
                textBoxCurrentPassword.ReadOnly = false;
				textBoxCurrentPassword.PasswordChar = '\0';

                labelChangingPassword.Text = "현재    별명";// +LoginManager.Instance.LoginUserNickName;
                lblNickName.Text = LoginManager.Instance.LoginUserNickName;
                                
				m_isSetModeRadioControl = true;
				radioChangeNickName.Checked = true;
			}
			else if (mode == Mode.CHANGE_PASSWORD)
			{
				labelCurrentPassword.Text = "현재 비밀번호";
                textBoxCurrentPassword.ReadOnly = false;
				textBoxCurrentPassword.PasswordChar = '*';

				labelChangingPassword.Text = "비  밀   번  호";
                textBoxChangingPassword.ReadOnly = false;
                textBoxChangingPassword.PasswordChar = '*';
                
				m_isSetModeRadioControl = true;
				radioChangePassword.Checked = true;
			}                
            else if(mode == Mode.CHANGE_CHIEF)
            {
                labelCurrentPassword.Text = "책임자";
                textBoxCurrentPassword.ReadOnly = true;
                textBoxCurrentPassword.PasswordChar = '\0';

                labelChangingPassword.Text = "전화번호";
                textBoxChangingPassword.ReadOnly = true;
                textBoxChangingPassword.PasswordChar = '\0';

                m_isSetModeRadioControl = true;
                rdoChiefChange.Checked = true;
            }
			else if (mode == Mode.SUCCESS_LOGIN)
			{
                if (obj is Chief)
                {
                    m_Chief = obj as Chief;
                    OfficePhoneNumber oo = new OfficePhoneNumber(m_Chief.CallerPhoneNumber, false);
                    m_Chief.CallerPhoneNumber = oo.Number;
                }

                // 모든 ID가 항상 사용할 수 있도록 한다.
				//if (LoginManager.LoginID == m_strAdminID)
				{
					btnTeamMangaer.Enabled = true;
					//btnTeamManager.Enabled = true;
				}
				/*else
				{
					btnSOPManager.Enabled = false;
					btnTeamManager.Enabled = false;
				}*/

                checkBoxSimulationMode.Enabled = false;
                checkBoxShowSensorMonitor.Visible = false;

                /*if (SimulationMode)
                {
                    if (IsAliveSensorMonitor())
                        SensorMonitorState(true);
                    else
                        SensorMonitorState(false);

                    timerSensorMonitor.Start();
                }


                if (SimulationMode && !SOPHiddenServer.HiddenServer.Instance.IsRunning)
                {
                    // 최초 SiteID를 지정해 준다.
                    SOPHiddenServer.HiddenServer.Instance.SiteID = m_nSiteID;

                    ProgressMessageBox.Show();
                    BroadcastWatcher.Instance.Start();
                    SOPHiddenServer.HiddenServer.Instance.Start(SimulationDBManager.DBFilePath, SimulationDBManager.DBPassword);
                }
                else if (!SimulationMode && SOPHiddenServer.HiddenServer.Instance.IsRunning)
                {
                    // 종료시 SiteID를 저장해준다.
                    SOPHiddenServer.HiddenServer.Instance.SiteID = m_nSiteID;

                    BroadcastWatcher.Instance.Stop();
                    SOPHiddenServer.HiddenServer.Instance.Stop();
                }*/
			}
            else if (mode == Mode.TRY_LOGIN)
            {
                checkBoxSimulationMode.Enabled = true;
            }

			ShowControls(mode);

            if (mode == Mode.REGIST_MEMBER)
            {
                // 기존
                //SetRegistControlMode(true);
                //checkBoxSimulationMode.Enabled = false;

                SetRegistControlMode(false);
            }
            else if (mode == Mode.FIND_PASSWORD)
            {
                SetFindPasswordControlMode(true);
                checkBoxSimulationMode.Enabled = false;
            }
            else if(mode == Mode.CHANGE_CHIEF)
            {
                if (m_Chief != null)
                {
                    textBoxCurrentPassword.Text = m_Chief.DisplayText;
                    OfficePhoneNumber oo = new OfficePhoneNumber(m_Chief.CallerPhoneNumber, false);
                    m_Chief.CallerPhoneNumber = oo.Number;
                    textBoxChangingPassword.Text = m_Chief.CallerPhoneNumber;
                }
            }

            /*if (!SimulationMode)*/
                //ribbonButtonSetup.Visible = true;

            foreach (EtcButton btn in m_etcButtons)
            {
                btn.Visible = false;
            }

            rbtnBack.Visible = false;
		}

		private void SetFindPasswordControlMode(bool initMode)
		{
            labelFindPasswordDescription.Location = new Point(labelFindPasswordDescription.Location.X, labelMemberID2.Location.Y);

			if (initMode)
			{
				labelMemberName2.Text = "코     드";
				labelID2.Text = "아 이 디";				
                //btnFindPasswordNext.NormalImage = global::IntegratedManagement4.Properties.Resources.btnNext;
                //btnFindPasswordNext.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnNextClick;
                //btnFindPasswordNext.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnNextClick;

				textBoxMemberName2.PasswordChar = '\0';
				textBoxID2.PasswordChar = '\0';

				labelFindPasswordDescription.Visible = false;
                labelMemberID2.Visible = false;
                textBoxMemberID2.Visible = false;
                //labelMemberID2.Visible = true;
                //textBoxMemberID2.Visible = true;
            }
			else
			{
                labelID2.Text = "비밀번호";
                labelMemberName2.Text = "비밀번호 확인";
                btnFindPasswordNext.NormalImage = global::IntegratedManagement4.Properties.Resources.ok_normal;
                btnFindPasswordNext.MouseOverImage = global::IntegratedManagement4.Properties.Resources.ok_hover;
                btnFindPasswordNext.ClickedImage = global::IntegratedManagement4.Properties.Resources.ok_click;



                textBoxMemberName2.PasswordChar = '*';
				textBoxID2.PasswordChar = '*';

				labelFindPasswordDescription.Visible = true;
                labelMemberID2.Visible = false;
                textBoxMemberID2.Visible = false;
                labelMemberName2.Visible = true;
                textBoxMemberName2.Visible = true;

                textBoxMemberName2.Text = "";
				textBoxID2.Text = "";

                textBoxID2.Focus();
			}
		}

		private void SetRegistControlMode(bool initMode)
		{
			textBoxMemberID.Focus();

			if (initMode)
			{
                //labelChief.Text = "";
                //btnSetChief.Visible = labelChief.Visible = true;

				labelMemberID.Text = "사원번호";
				labelMemberName.Text = "이름";
				labelConfirmPassword.Text = m_strNickNameTitle;

                btnRegistNext.NormalImage = global::IntegratedManagement4.Properties.Resources.btnNext;
                btnRegistNext.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnNextClick;
                btnRegistNext.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnNextClick;

				textBoxMemberID.Text = "";
				textBoxMemberName.Text = "";

				textBoxMemberName.PasswordChar = '\0';

				textBoxConfirmPassword.PasswordChar = '\0';
				//labelConfirmPassword.Visible = false;
				//textBoxConfirmPassword.Visible = false;
			}
			else
			{
                // 기존
                //btnSetChief.Visible = labelChief.Visible = false;

                //labelChief.Visible = false;
                btnSetChief.Visible = true;
                //btnOption.Visible = true;

				//labelMemberID.Text = "아 이 디";
				//labelMemberName.Text = "비밀번호";
				//labelConfirmPassword.Text = "비밀번호 확인";
                //btnRegistNext.NormalImage = global::IntegratedManagement4.Properties.Resources.btnNext;
                //btnRegistNext.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnNextClick;
                //btnRegistNext.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnNextClick;

				textBoxMemberID.Text = "";
				textBoxMemberName.Text = "";
				textBoxConfirmPassword.Text = "";
                textBoxNickName.Text = "";

                txtChief.Text = "";
                txtPhoeNumber.Text = "";

				textBoxMemberName.PasswordChar = '*';
				textBoxConfirmPassword.PasswordChar = '*';

				labelConfirmPassword.Visible = true;
				textBoxConfirmPassword.Visible = true;

                pnlMemberAdd.BringToFront();
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
                /*if (SimulationMode && ctrl == btnChangePassword)
                    ctrl.Visible = false;
                else*/
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

        private bool GetButtonSet(ExecuteManager.APP_TYPE type, out UnE.GUI.RibbonButton btn, out Label label)
        {
            if (type == IntegratedManagement4.ExecuteManager.APP_TYPE.SOP_MANAGER)
            {
                btn = btnTeamMangaer;
                label = labelTeamMangaer;
            }
            /*else if (type == IntegratedManagement4.ExecuteManager.APP_TYPE.SOP_SIMULATOR)
            {
                btn = btnSOPSimulator;
                label = labelSOPSimulator;
            }*/
            /*else if (type == IntegratedManagement4.ExecuteManager.APP_TYPE.TEAM_MANAGER)
            {
                btn = btnTeamManager;
                label = labelTeamManager;
            }*/
            /*else if (type == IntegratedManagement4.ExecuteManager.APP_TYPE.ETC)
            {
                btn = btnEtc;
                label = labelEtc;
            }*/
            else if (type == IntegratedManagement4.ExecuteManager.APP_TYPE.SDMS)
            {
                btn = btnViewer;
                label = labelSOPSimulator;
            }
            else
            {
                btn = null;
                label = null;
                return false;
            }

            return true;
        }

        // Button들 배열 순서를 바꾼다.
        private List<ExecuteManager.APP_TYPE> RearrangeButtons()
        {
            List<ExecuteManager.APP_TYPE> originList = new List<ExecuteManager.APP_TYPE>();
            originList.Add(ExecuteManager.APP_TYPE.SOP_MANAGER);
            originList.Add(ExecuteManager.APP_TYPE.SDMS);
            //originList.Add(ExecuteManager.APP_TYPE.SOP_SIMULATOR);
            originList.Add(ExecuteManager.APP_TYPE.TEAM_MANAGER);
            originList.Add(ExecuteManager.APP_TYPE.ETC);

            List<ExecuteManager.APP_TYPE> changingList = new List<ExecuteManager.APP_TYPE>();
            changingList.Add(ExecuteManager.APP_TYPE.SDMS);
            //changingList.Add(ExecuteManager.APP_TYPE.SOP_SIMULATOR);
            changingList.Add(ExecuteManager.APP_TYPE.TEAM_MANAGER);
            changingList.Add(ExecuteManager.APP_TYPE.SOP_MANAGER);
            changingList.Add(ExecuteManager.APP_TYPE.ETC);

            RearrangeButtons(originList, changingList);
            return changingList;
        }

        private void RearrangeButtons(List<ExecuteManager.APP_TYPE> originList, List<ExecuteManager.APP_TYPE> changingList)
        {
            int nOriginCount = originList.Count;
            int nChangingCount = changingList.Count;

            UnE.GUI.RibbonButton btn, btn2;
            Label label, label2;

            if (nOriginCount >= 2)
            {
                if (!GetButtonSet(originList[0], out btn, out label))
                    return;
                if (!GetButtonSet(originList[1], out btn2, out label2))
                    return;

                m_btnFirst = btn;
                m_btnSecond = btn2;
                m_labelFirst = label;
                m_labelSecond = label2;
            }

            if (nOriginCount != nChangingCount)
                return;

            List<Point> originPoints = new List<Point>();

            for (int i=0;i<nOriginCount;i++)
            {
                if (!GetButtonSet(originList[i], out btn, out label))
                    return;

                if (!GetButtonSet(changingList[i], out btn2, out label2))
                    return;

                originPoints.Add(btn.Location);
            }

            for (int i=0;i<nOriginCount;i++)
            {
                Point ptButton = originPoints[i];

                if (!GetButtonSet(changingList[i], out btn2, out label2))
                    return;

                int nLabelPos = btn2.Location.X - label2.Location.X;
                btn2.Location = ptButton;
                label2.Location = new Point(btn2.Location.X - nLabelPos, label2.Location.Y);
            }

            GetButtonSet(changingList[0], out btn, out label);
            GetButtonSet(changingList[1], out btn2, out label2);

            m_btnFirst = btn;
            m_btnSecond = btn2;
            m_labelFirst = label;
            m_labelSecond = label2;
        }

		private void InitButtons()
		{
            //((RibbonButton)btnLogin).NormalImage = global::IntegratedManagement4.Properties.Resources.button;
            //((RibbonButton)btnRegist).NormalImage = global::IntegratedManagement4.Properties.Resources.button;
            //((RibbonButton)btnFindPassword).NormalImage = global::IntegratedManagement4.Properties.Resources.button;
            //((RibbonButton)btnSOPManager).NormalImage = global::IntegratedManagement4.Properties.Resources.sopmanager2;
            //((RibbonButton)btnSOPSimulator).NormalImage = global::IntegratedManagement4.Properties.Resources.sopsimulator;
            //((RibbonButton)btnTeamManager).NormalImage = global::IntegratedManagement4.Properties.Resources.teammanager;
            //((RibbonButton)btnEtc).NormalImage = global::IntegratedManagement4.Properties.Resources.etc;
            //((RibbonButton)btnSDMS).NormalImage = global::IntegratedManagement4.Properties.Resources.sdms;
            //((RibbonButton)btnLogout).NormalImage = global::IntegratedManagement4.Properties.Resources.button;
            //((RibbonButton)btnChangePassword).NormalImage = global::IntegratedManagement4.Properties.Resources.button;
            //((RibbonButton)btnRegistOK).NormalImage = global::IntegratedManagement4.Properties.Resources.button;
            //((RibbonButton)btnRegistCancel).NormalImage = global::IntegratedManagement4.Properties.Resources.button;

            ////btnRegist.Size = new Size(135, 44);
            //((RibbonButton)btnLogin).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnRegist).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnFindPassword).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnSOPManager).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnSOPSimulator).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnTeamManager).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnEtc).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnSDMS).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnLogout).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnChangePassword).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnRegistOK).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
            //((RibbonButton)btnRegistCancel).MouseOverBkgndImage = global::IntegratedManagement4.Properties.Resources.RibbonMouseOver_bkgnd;
					   
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
        //private ServerRestarting restartPop = null;
		private void btnLogin_Click(object sender, EventArgs e)
		{
            btnLogin.Enabled = false;

			if (textBoxID.Text == "" || textBoxPassword.Text == "")
			{
                //MessageBox.Show("아이디와 비밀번호를 입력하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //UnE.Utility.UMessageBoxRibbon.Show("아이디와 비밀번호를 입력하세요.", "로그인 경고",   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                FormMessageBox msgBox = new FormMessageBox("아이디와 비밀번호를 입력하세요.", "로그인 경고", MessageBoxButtons.OK);
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.ShowDialog();
                
                return;
			}

			if (!m_logInMgr.LogIn(textBoxID.Text, textBoxPassword.Text))
			{
                //UnE.Utility.UMessageBoxRibbon.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ClearLoginTextBox();
            }
            else
            {
                if (ckbSaveID.Checked == true)
                {
                    string strEncrypt = AES256Cipher.AES_encrypt(textBoxPassword.Text, key);
                    RegUtil.WriteRegValue("IntegratedManager", "LastUser", textBoxID.Text, m_nSiteID);
                    RegUtil.WriteRegValue("IntegratedManager", "LastEncr", strEncrypt, m_nSiteID);
                }
                else
                {
                    RegUtil.WriteRegValue("IntegratedManager", "LastUser", "", m_nSiteID);
                    RegUtil.WriteRegValue("IntegratedManager", "LastEncr", "", m_nSiteID);
                }

                // 로그인 성공 시 
                //string strNickName = LoginManager.Instance.LoginUserNickName;
                // DB 조회하여 닉네임 넣기
                string strNickName = SearchNickName(textBoxID.Text);

                Utility ini = new Utility();
                string strSiteID = ini.setinivalue("Server Connection Info", "userNickName", strNickName);
            }

            btnLogin.Enabled = true;
		}

        private string SearchNickName(string strUserID)
        {
            string strRet = "";
            string strSQL = "select NickName from SOPGenUser where UserID = '" + strUserID + "'";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            Dictionary<int, string> dicLevel = new Dictionary<int, string>();

            if (arrResult == null || arrResult.Count == 0)
                return strRet;

            for (int i = 0; arrResult.Count > i; i++)
            {
                strRet = WebDBManager.GetStringField(arrResult[i]);
            }

            return strRet;
        }

        public void ClearLoginTextBox()
		{
			textBoxID.Text = "";
			textBoxPassword.Text = "";
		}

        private bool m_bExitThread = false;
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
            if (m_logInMgr.LoginState && m_bSilentExit == false)
			{
                //DialogResult result = 
                //UnE.Utility.UMessageBoxRibbon.Show("로그인되어 있는 모든 프로그램이 종료됩니다.", "종료 경고", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                FormMessageBox msgBox = new FormMessageBox("로그인되어 있는 모든 프로그램이 종료됩니다.", "종료 경고", MessageBoxButtons.YesNo);
                msgBox.StartPosition = FormStartPosition.CenterParent;

                DialogResult result = msgBox.ShowDialog();

                if (result == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}
                //FormMain.Instance.SaveCurrentState();
			}

            m_isClosing = true;
            m_bExitThread = true;

            m_netServer.NetworkServerClosing();
			ProcessManager.Instance.AbortAllProcess();

            /*if (SimulationMode)
            {
                KillSensorMonitor();

                if (m_logInMgr.LoginState)
                    timerSensorMonitor.Stop();

                ProcessManager.Instance.AbortAllProcess();
            }
            else*/
            {
                m_logInMgr.LogOut();

                SetLogout();

                m_NetMgr.ReleaseThread();
            }

            /*if (SOPHiddenServer.HiddenServer.Instance.IsRunning)
                SOPHiddenServer.HiddenServer.Instance.Stop();*/
		}

		private void btnLogout_Click(object sender, EventArgs e)
		{

            //DialogResult result = UnE.Utility.UMessageBoxRibbon.Show("로그인되어 있는 모든 프로그램이 종료됩니다.", "종료 경고", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            FormMessageBox msgBox = new FormMessageBox("로그인되어 있는 모든 프로그램이 종료됩니다.", "종료 경고", MessageBoxButtons.YesNo);
            msgBox.StartPosition = FormStartPosition.CenterParent;

            DialogResult result =  msgBox.ShowDialog();

            if (result == DialogResult.No)
			{
				return;
			}

            RegUtil.WriteRegValue("Update Info", "LastUser", "", m_nSiteID);
            RegUtil.WriteRegValue("Update Info", "LastEncr", "", m_nSiteID);

			if (!m_logInMgr.LogOut())
			{
				//MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //UnE.Utility.UMessageBoxRibbon.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                msgBox = new FormMessageBox("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK);
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.ShowDialog();
            }

            m_Chief = null;
			SetLogout();

            if(ckbSaveID.Checked == true)
            {
                string szLastId2 = RegUtil.ReadRegValue("IntegratedManager", "LastUser", m_nSiteID);
                string szLassPass2 = RegUtil.ReadRegValue("IntegratedManager", "LastEncr", m_nSiteID);

                string szText = AES256Cipher.AES_decrypt(szLassPass2, key);
                textBoxID.Text = szLastId2;
                textBoxPassword.Text = szText;
            }

            btnLogin.Focus();
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

            /*
            if (btn == btnSOPSimulator)
            {
                btnSOPSimulator.IsChecked = !btnSOPSimulator.IsChecked;
                btnSOPSimulator.Refresh();
            }
            */
		}

		private void btnRegist_Click(object sender, EventArgs e)
		{
            m_Chief = null;
			SetMode(Mode.REGIST_MEMBER);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{            
			if (PrevMode != Mode.UNKNOWN)
				SetMode(PrevMode);
		}

        private void btnRegistPrev_Click(object sender, EventArgs e)
        {
            pnlMemberAdd.Visible = true;
            pnlMemberAdd2.Visible = false;
        }

		private void btnRegistOK_Click(object sender, EventArgs e)
        {
            try
            {
                #region 주석
                //if (labelConfirmPassword.Text == m_strNickNameTitle)
                //{
                //    if (m_Chief == null)
                //    {
                //        btnSetChief.Focus();
                //        throw new ApplicationException("책임자를 설정하세요.");
                //    }
                //    //if (textBoxMemberID.Text.Length == 0)
                //    //{
                //    //    MessageBox.Show("사원번호를 입력하세요");
                //    //    textBoxMemberID.Focus();
                //    //}
                //    //else 
                //    //    if (textBoxMemberName.Text.Length == 0)
                //    //{
                //    //    MessageBox.Show("이름을 입력하세요");
                //    //    textBoxMemberName.Focus();
                //    //}
                //    //else
                //    {
                //        if (textBoxMemberID.Text.Length > 0)
                //        {
                //            string strGenUserID = "";
                //            int nCompanyMemberID = m_logInMgr.GetMemberID(textBoxMemberID.Text, textBoxMemberName.Text, ref strGenUserID);
                //            if (nCompanyMemberID == -2)
                //            {
                //                throw new ApplicationException("삭제된 직원이거나 직원 정보가 잘못되었습니다.");
                //            }
                //            else if (nCompanyMemberID < 0)
                //            {
                //                throw new ApplicationException("입력된 직원 정보가 잘못되었습니다.");
                //            }
                //            else if (nCompanyMemberID == 0)
                //            {
                //                throw new ApplicationException("이미 회원가입이 되어 있습니다.");
                //            }
                //            else
                //            {
                //                m_strNickName = textBoxConfirmPassword.Text;
                //                SetRegistControlMode(false);
                //                labelMemberID.Tag = nCompanyMemberID;
                //            }
                //        }
                //        else
                //        {
                //            m_strNickName = textBoxConfirmPassword.Text;
                //            SetRegistControlMode(false);
                //            //labelMemberID.Tag = nCompanyMemberID;
                //        }
                //    }
                //}
                //else
                #endregion

                if (m_Chief == null || m_Chief.DisplayText.Length == 0 || m_Chief.CallerPhoneNumber.Length == 0)
                    throw new ApplicationException("책임자가 설정되지 않았습니다.");

                // -1 : Company Member 선택 안함 
                int nCompanyMemberID = -1;
                if (labelMemberID.Tag != null)
                    nCompanyMemberID = (int)labelMemberID.Tag;

                if (!m_logInMgr.JoinUser(nCompanyMemberID, textBoxMemberID.Text, textBoxMemberName.Text, m_strNickName, m_Chief))
                {
                    throw new ApplicationException("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.");
                }
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBoxRibbon.Show(ex.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //MessageBox.Show(ex.Message);
            }
            m_Chief = null;
        }

		public void FailRegisterUser(int nType)
		{
			if (nType == 0)
			{
				//MessageBox.Show("이미 존재하는 아이디입니다.");
                //UnE.Utility.UMessageBoxRibbon.Show("이미 존재하는 아이디입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormMessageBox msgBox = new FormMessageBox("이미 존재하는 아이디입니다.", "알림", MessageBoxButtons.OK);
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.ShowDialog();
            }
			else if (nType == -1)
			{
				//MessageBox.Show("삭제되거나 사용할 수 없는 사용자 아이디입니다.");
                //UnE.Utility.UMessageBoxRibbon.Show("삭제되거나 사용할 수 없는 사용자 아이디입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormMessageBox msgBox = new FormMessageBox("삭제되거나 사용할 수 없는 사용자 아이디입니다.", "알림", MessageBoxButtons.OK);
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.ShowDialog();
            }
			else if (nType < -1)
			{
				//MessageBox.Show("회원가입에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요");
                //UnE.Utility.UMessageBoxRibbon.Show("회원가입에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormMessageBox msgBox = new FormMessageBox("회원가입에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요.", "알림", MessageBoxButtons.OK);
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.ShowDialog();
            }
		}

        public void FailChangeSOPGenUserCommander(LoginManager.CommanderErrorType nType)
        {
            if (nType == LoginManager.CommanderErrorType.FAIL_DELETE_DAY)
            {                
                UnE.Utility.UMessageBoxRibbon.Show("기존 주간 책임자 삭제 실패.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (nType == LoginManager.CommanderErrorType.FAIL_INSERT_DAY)
            {              
                UnE.Utility.UMessageBoxRibbon.Show("주간 책임자 추가 실패.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (nType == LoginManager.CommanderErrorType.FAIL_UPDATE_DAY)
            {                
                UnE.Utility.UMessageBoxRibbon.Show("주간 책임자 갱신 실패.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (nType == LoginManager.CommanderErrorType.FAIL_DELETE_NIGHT)
            {
                UnE.Utility.UMessageBoxRibbon.Show("기존 야간 책임자 삭제 실패.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (nType == LoginManager.CommanderErrorType.FAIL_INSERT_DAY)
            {
                UnE.Utility.UMessageBoxRibbon.Show("야간 책임자 추가 실패.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (nType == LoginManager.CommanderErrorType.FAIL_UPDATE_DAY)
            {
                UnE.Utility.UMessageBoxRibbon.Show("야간 책임자 갱신 실패.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

		public void SuccessRegisterUser()
		{
			//MessageBox.Show("회원가입에 성공하였습니다.\r\n로그인 화면으로 이동합니다.");
            //UnE.Utility.UMessageBoxRibbon.Show("회원가입에 성공하였습니다.\r\n로그인 화면으로 이동합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FormMessageBox msgBox = new FormMessageBox("회원가입에 성공하였습니다.\r\n로그인 화면으로 이동합니다.", "알림", MessageBoxButtons.OK);
            msgBox.StartPosition = FormStartPosition.CenterParent;
            msgBox.ShowDialog();

            SetMode(Mode.TRY_LOGIN);

			textBoxID.Text = textBoxMemberID.Text;
			textBoxPassword.Text = "";
			textBoxPassword.Focus();
		}

        public void SuccessChangeSOPGenUserCommander()
        {
            SetMode(Mode.SUCCESS_LOGIN);

            textBoxCurrentPassword.Text = "";
            textBoxChangingPassword.Text = "";

            UnE.Utility.UMessageBoxRibbon.Show("책임자 변경에 성공했습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

		private void btnFindPassword_Click(object sender, EventArgs e)
		{
			SetMode(Mode.FIND_PASSWORD);
		}

		private void btnFindPasswordNext_Click(object sender, EventArgs e)
		{
			if (labelFindPasswordDescription.Visible == false)
			{
                /*if (textBoxMemberID2.Text.Length == 0)
				{
					//MessageBox.Show("사원번호를 입력해주세요");
                    UnE.Utility.UMessageBoxRibbon.Show("사원번호를 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
					textBoxMemberID2.Focus();
				}
				else if (textBoxMemberName2.Text.Length == 0)
				{
					//MessageBox.Show("이름을 입력해주세요");
                    UnE.Utility.UMessageBoxRibbon.Show("이름을 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
					textBoxMemberName2.Focus();
				}*/
                if (textBoxID2.Text.Length == 0)
				{
					//MessageBox.Show("아이디를 입력해주세요");
                    //UnE.Utility.UMessageBoxRibbon.Show("아이디를 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormMessageBox msgBox = new FormMessageBox("아이디를 입력해주세요", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxID2.Focus();
				}
                else if (textBoxMemberName2.Text.Length == 0)
                {
                    textBoxMemberName2.Focus();

                    FormMessageBox msgBox = new FormMessageBox("코드를 입력하세요.", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxMemberName2.Focus();
                }
                else
				{
                    bool bChk = false;
                    bool bChkCode = false;

					string strGenUserID = textBoxID2.Text;
                    //int nCompanyMemberID = m_logInMgr.GetMemberID(textBoxMemberID2.Text, textBoxMemberName2.Text, ref strGenUserID);
                    bChk = m_logInMgr.CheckID(textBoxID2.Text);
                    bChkCode = m_logInMgr.CheckJoinCode(textBoxMemberName2.Text);

                    bChk = (bChk == true && bChkCode == true);
                    /*if (nCompanyMemberID < 0)
                    {
                        //MessageBox.Show("사원번호와 이름이 일치하지 않습니다.");
                        UnE.Utility.UMessageBoxRibbon.Show("사원번호와 이름이 일치하지 않습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }						
					else if (nCompanyMemberID > 0)
					{
						//MessageBox.Show("회원가입이 되어있지 않습니다.\r\n회원가입을 진행하여 주십시오");
                        UnE.Utility.UMessageBoxRibbon.Show("회원가입이 되어있지 않습니다.\r\n회원가입을 진행하여 주십시오", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

						SetMode(Mode.TRY_LOGIN);
					}*/
                    if (bChk == false)
                    {
                        //MessageBox.Show("입력된 직원정보와 아이디가 일치하지 않습니다.\r\n다시 확인하여 주십시오");
                        //UnE.Utility.UMessageBoxRibbon.Show("입력된 직원정보와 아이디가 일치하지 않습니다.\r\n다시 확인하여 주십시오", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FormMessageBox msgBox = new FormMessageBox("입력된 아이디 또는 인증코드가 일치하지 않습니다.\r\n다시 확인하여 주십시오", "알림", MessageBoxButtons.OK);
                        msgBox.StartPosition = FormStartPosition.CenterParent;
                        msgBox.ShowDialog();
                    }
					else
					{
						SetFindPasswordControlMode(false);
						labelMemberID2.Tag = strGenUserID;
					}
				}
			}
			else
			{
				if (textBoxID2.Text.Length == 0) 
                {
                    //MessageBox.Show("비밀번호를 입력하세요");
                    //UnE.Utility.UMessageBoxRibbon.Show("비밀번호를 입력하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormMessageBox msgBox = new FormMessageBox("비밀번호를 입력하세요", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxID2.Focus();
				}
				else if (textBoxMemberName2.Text.Length == 0)
				{
					//MessageBox.Show("비밀번호를 한번더 입력하세요");
                    //UnE.Utility.UMessageBoxRibbon.Show("비밀번호를 한번더 입력하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormMessageBox msgBox = new FormMessageBox("비밀번호를 한번더 입력하세요", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxMemberName2.Focus();
				}
				else
				{
					if (textBoxMemberName2.Text != textBoxID2.Text)
					{
                        //MessageBox.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요");
                        //UnE.Utility.UMessageBoxRibbon.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FormMessageBox msgBox = new FormMessageBox("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK);
                        msgBox.StartPosition = FormStartPosition.CenterParent;
                        msgBox.ShowDialog();

                        textBoxMemberName2.Text = "";
                        textBoxMemberName2.Focus();
					}
					else
					{
						if (!m_logInMgr.SetPassword((string)labelMemberID2.Tag, textBoxID2.Text))
						{
							//MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //UnE.Utility.UMessageBoxRibbon.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            FormMessageBox msgBox = new FormMessageBox("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK);
                            msgBox.StartPosition = FormStartPosition.CenterParent;
                            msgBox.ShowDialog();
                        }
              
					}
				}
			}
		}

		public void SuccessChangePassword()
		{
			//MessageBox.Show("비밀번호가 변경되었습니다.\r\n로그인 화면으로 이동합니다.");
            //UnE.Utility.UMessageBoxRibbon.Show("비밀번호가 변경되었습니다.\r\n로그인 화면으로 이동합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FormMessageBox msgBox = new FormMessageBox("비밀번호가 변경되었습니다.\r\n로그인 화면으로 이동합니다.", "알림", MessageBoxButtons.OK);
            msgBox.StartPosition = FormStartPosition.CenterParent;
            msgBox.ShowDialog();

            SetMode(Mode.TRY_LOGIN);

			textBoxID.Text = (string)labelMemberID2.Tag;
			textBoxPassword.Text = "";
			textBoxPassword.Focus();
		}

		public void FailChangePassword()
		{
			//MessageBox.Show("비밀번호 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요");
            //UnE.Utility.UMessageBoxRibbon.Show("비밀번호 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FormMessageBox msgBox = new FormMessageBox("비밀번호 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요", "알림", MessageBoxButtons.OK);
            msgBox.StartPosition = FormStartPosition.CenterParent;
            msgBox.ShowDialog();
        }

		public void SuccessChangeNickName()
		{
			//MessageBox.Show("별명이 변경되었습니다.\r\n로그인 화면으로 이동합니다.");
            UnE.Utility.UMessageBoxRibbon.Show("별명이 변경되었습니다.\r\n로그인 화면으로 이동합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

			SetMode(Mode.TRY_LOGIN);

			textBoxID.Text = (string)labelMemberID2.Tag;
			textBoxPassword.Text = "";
			textBoxPassword.Focus();
		}

		public void FailChangeNickName()
		{
			//MessageBox.Show("별명 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요");
            UnE.Utility.UMessageBoxRibbon.Show("별명 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnChangePassword_Click(object sender, EventArgs e)
		{
			if (radioChangePassword.Checked)
				SetMode(Mode.CHANGE_PASSWORD);
			else
				SetMode(Mode.CHANGE_NICKNAME);

            ribbonButtonSetup.Visible = false;
		}

		private void btnChanging_Click(object sender, EventArgs e)
		{
			if (m_modeCurrent == Mode.CHANGE_PASSWORD)
			{
				if (textBoxCurrentPassword.Text.Length == 0)
				{
                    //MessageBox.Show("현재 비밀번호를 입력하세요");
                    //UnE.Utility.UMessageBoxRibbon.Show("현재 비밀번호를 입력하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FormMessageBox msgBox = new FormMessageBox("현재 비밀번호를 입력하세요", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxCurrentPassword.Focus();
				}
				else if (textBoxChangingPassword.Text.Length == 0)
				{
                    //MessageBox.Show("변경할 비밀번호를 입력하세요");
                    //UnE.Utility.UMessageBoxRibbon.Show("변경할 비밀번호를 입력하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FormMessageBox msgBox = new FormMessageBox("변경할 비밀번호를 입력하세요", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxChangingPassword.Focus();
				}
				else if (textBoxConfirmChanging.Text.Length == 0)
				{
                    //MessageBox.Show("비밀번호를 한번더 입력하세요");
                    //UnE.Utility.UMessageBoxRibbon.Show("비밀번호를 한번더 입력하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FormMessageBox msgBox = new FormMessageBox("비밀번호를 한번더 입력하세요", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    textBoxConfirmChanging.Focus();
				}
				else
				{
					if (textBoxChangingPassword.Text != textBoxConfirmChanging.Text)
					{
						//MessageBox.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요");
                        //UnE.Utility.UMessageBoxRibbon.Show("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        FormMessageBox msgBox = new FormMessageBox("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요", "알림", MessageBoxButtons.OK);
                        msgBox.StartPosition = FormStartPosition.CenterParent;
                        msgBox.ShowDialog();

                        textBoxConfirmChanging.Text = "";
						textBoxConfirmChanging.Focus();
					}
					else
					{

						if (!m_logInMgr.ChangePassword(textBoxCurrentPassword.Text, textBoxChangingPassword.Text))
						{
                            //MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //UnE.Utility.UMessageBoxRibbon.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            FormMessageBox msgBox = new FormMessageBox("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK);
                            msgBox.StartPosition = FormStartPosition.CenterParent;
                            msgBox.ShowDialog();
                        }
					}
				}
			}
			else if (m_modeCurrent == Mode.CHANGE_NICKNAME)
			{
				if (!m_logInMgr.ChangeNickName(textBoxCurrentPassword.Text))
				{
                    //MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //UnE.Utility.UMessageBoxRibbon.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FormMessageBox msgBox = new FormMessageBox("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();
                }
			}
            else if (m_modeCurrent == Mode.CHANGE_CHIEF)
            {
                if (m_Chief == null || (m_Chief.DayLight_Day == false && m_Chief.DayLight_Night == false))
                {
                    UnE.Utility.UMessageBoxRibbon.Show("책임자 정보가 없습니다", "확인", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (!m_logInMgr.ChangeSOPGenCommander(m_Chief))
                {
                    //MessageBox.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //UnE.Utility.UMessageBoxRibbon.Show("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FormMessageBox msgBox = new FormMessageBox("SOP서버가 연결되어 있지 않습니다.\n서버 실행 상태를 확인하세요.", "로그인 경고", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();
                }
            }
		}

		public void SuccessChangePassword2()
		{
			//MessageBox.Show("비밀번호가 변경되었습니다.\r\n이전 화면으로 이동합니다.");
            //UnE.Utility.UMessageBoxRibbon.Show("비밀번호가 변경되었습니다.\r\n이전 화면으로 이동합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FormMessageBox msgBox = new FormMessageBox("비밀번호가 변경되었습니다.\r\n이전 화면으로 이동합니다.", "알림", MessageBoxButtons.OK);
            msgBox.StartPosition = FormStartPosition.CenterParent;
            msgBox.ShowDialog();

            if (PrevMode != Mode.UNKNOWN)
				SetMode(PrevMode);
		}

		public void SuccessChangeNickName2()
		{
			//MessageBox.Show("별명이 변경되었습니다.\r\n이전 화면으로 이동합니다.");
            UnE.Utility.UMessageBoxRibbon.Show("별명이 변경되었습니다.\r\n이전 화면으로 이동합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

			if (PrevMode != Mode.UNKNOWN)
				SetMode(PrevMode);
		}

		private void btnMin_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

        private void UpdateRadioButtonImage()
        {
            if (radioChangePassword.Checked)
            {
                picChangePassword.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
                picChangeNickName.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picChiefChange.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
            }
            else if (radioChangeNickName.Checked)
            {
                picChangePassword.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picChangeNickName.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
                picChiefChange.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
            }
            else if(rdoChiefChange.Checked)
            {
                picChangePassword.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picChangeNickName.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picChiefChange.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
            }
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

                    ribbonButtonSetup.Visible = false;
                }
            }
            
            UpdateRadioButtonImage();
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

                    ribbonButtonSetup.Visible = false;
                }
            }

            UpdateRadioButtonImage();
			m_isSetModeRadioControl = false;
		}

        private void rdoChiefChange_CheckedChanged(object sender, EventArgs e)
        {
            //rdoChiefChange
            if (rdoChiefChange.Checked)
            {
                if (m_modeCurrent != Mode.CHANGE_CHIEF && !m_isSetModeRadioControl)
                {
                    Mode modePrev = m_modePrev;
                    SetMode(Mode.CHANGE_CHIEF);
                    m_modePrev = modePrev;

                    ribbonButtonSetup.Visible = false;
                }
            }

            UpdateRadioButtonImage();
            m_isSetModeRadioControl = false;
        }
		
		private void ribbonButtonSetup_Click_1(object sender, EventArgs e)
		{
			m_SetupForm.Location = new Point(0, 10);
			m_SetupForm.BringToFront();

            btnMin.BringToFront();
            btnClose.BringToFront();
            
            labelCopyright.BringToFront();
            labelCurrVersion.BringToFront();

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
            string szLastProcs = RegUtil.ReadRegValue("Update Info", "LastProc", m_nSiteID);
            string szExitUpdate = RegUtil.ReadRegValue("Update Info", "ExitOnUpdate", m_nSiteID);

            string szLastId = RegUtil.ReadRegValue("Update Info", "LastUser", m_nSiteID);
            string szLassPass = RegUtil.ReadRegValue("Update Info", "LastEncr", m_nSiteID);

            // 저장된
            string szSaveID = RegUtil.ReadRegValue("IntegratedManager", "SaveID", m_nSiteID);
            if (szSaveID == "1")
            {
                string szLastId2 = RegUtil.ReadRegValue("IntegratedManager", "LastUser", m_nSiteID);
                string szLassPass2 = RegUtil.ReadRegValue("IntegratedManager", "LastEncr", m_nSiteID);

                if (szLastId2 != null && szLastId2 != "" && szLassPass2 != null && szLassPass2 != "")
                {
                    string szText = AES256Cipher.AES_decrypt(szLassPass2, key);
                    string szAutoLogin = RegUtil.ReadRegValue("IntegratedManager", "AutoLogin", m_nSiteID);

                    if (FormMain.Instance == null || FormMain.Instance.IsDisposed == true)
                        return;

                    FormMain.Instance.Invoke((MethodInvoker)delegate()
                    {
                        if (szAutoLogin == "1")
                            ckbAutoLogin.Checked = true;
                        ckbSaveID.Checked = true;
                        textBoxID.Text = szLastId2;
                        textBoxPassword.Text = szText;
                    });
                }                              
            }


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
                    RegUtil.WriteRegValue("Update Info", "LastProc", "", m_nSiteID);
                    RegUtil.WriteRegValue("Update Info", "ExitOnUpdate", "0", m_nSiteID);
                }
                else
                {
                    ConnectionLogEx.Instance.WriteLine("Auto Login Fail");

                }
            }
            else
            {
                string szAutoLogin = RegUtil.ReadRegValue("IntegratedManager", "AutoLogin", m_nSiteID);
                string szLastId2 = RegUtil.ReadRegValue("IntegratedManager", "LastUser", m_nSiteID);
                string szLassPass2 = RegUtil.ReadRegValue("IntegratedManager", "LastEncr", m_nSiteID);
                if (szAutoLogin == "1" && szLastId2 != null && szLastId2 != "" && szLassPass2 != null && szLassPass2 != "")
                {
                    if (m_logInMgr.LogIn(szLastId2, szLassPass2, true))
                    {
                        int nCount = 0;
                        while (m_modeCurrent != FormMain.Mode.SUCCESS_LOGIN)
                        {
                            Thread.Sleep(100);
                            nCount++;
                            if (nCount == 100)
                                break;
                        }
                    }
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
                    // libCCTV, CCTVewer는 상태를 기록하지 않는다.
                    if (proc.ProcessName.StartsWith("SOPSimulator"))
                    {
                        if (sb.Length != 0)
                            sb.Append(",");
                        sb.Append(proc.ProcessName);
                        sb.Append(":1");
                    }
                }
            }
            RegUtil.WriteRegValue("Update Info", "LastProc", sb.ToString(), m_nSiteID);
            RegUtil.WriteRegValue("Update Info", "ExitOnUpdate", "1", m_nSiteID);
        }

        private bool IsAliveSensorMonitor()
        {
            if (m_nSensorMonitorProcessID == 0)
                return false;

            try
            {
                if (System.Diagnostics.Process.GetProcessById(m_nSensorMonitorProcessID) == null)
                {
                    m_nSensorMonitorProcessID = 0;
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                m_nSensorMonitorProcessID = 0;
                return false;
            }

            return true;
        }

        private void SensorMonitorState(bool checkedState)
        {
            m_ignoreSensorMonitorChanged = true;
            checkBoxShowSensorMonitor.Checked = checkedState;
            m_ignoreSensorMonitorChanged = false;
        }

        private void checkBoxShowSensorMonitor_CheckedChanged(object sender, EventArgs e)
        {
            /*if (m_ignoreSensorMonitorChanged)
            {
                m_ignoreSensorMonitorChanged = false;
                return;
            }

            if (checkBoxShowSensorMonitor.Checked)
            {
                if (IsAliveSensorMonitor())
                    return;

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = Application.StartupPath + "\\SensorMonitor.exe";
                startInfo.Arguments = "127.0.0.1 \"수신반 모니터(연습용모드)\"";

                try
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);

                    if (process == null)
                    {
                        SensorMonitorState(false);
                        return;
                    }
                    else
                    {
                        m_nSensorMonitorProcessID = process.Id;

                        // SensorMonitor가 로딩될때까지 다시 클릭할 수 없도록 한다.
                        checkBoxShowSensorMonitor.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex);
                    m_nSensorMonitorProcessID = 0;
                    SensorMonitorState(false);
                }
            }
            else
            {
                if (!IsAliveSensorMonitor())
                    return;

                if (!KillSensorMonitor())
                {
                    SensorMonitorState(true);
                }
            }*/
        }

        private bool IsVisibleSensorMonitor()
        {
            if (m_nSensorMonitorProcessID == 0)
                return false;

            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(m_nSensorMonitorProcessID);

                if (process == null)
                    return false;

                return process.MainWindowHandle.ToInt32() > 0;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return false;
        }

        private void timerSensorMonitor_Tick(object sender, EventArgs e)
        {
            if (!checkBoxShowSensorMonitor.Enabled)
            {
                if (IsVisibleSensorMonitor())
                    checkBoxShowSensorMonitor.Enabled = true;
            }
            else
            {
                if (checkBoxShowSensorMonitor.Checked)
                {
                    if (!IsVisibleSensorMonitor())
                        SensorMonitorState(false);
                }
            }
        }

        public void ShowEtcButtons()
        {
            ArrayList arrControls = m_dicModeControls[Mode.SUCCESS_LOGIN];

            foreach (Control ctrl in arrControls)
            {
                if (ctrl is RibbonButton || ctrl is Label)
                    ctrl.Visible = false;
            }

            //btnSOPManager.Visible = btnSOPSimulator.Visible = btnTeamManager.Visible = btnEtc.Visible = btnSDMS.Visible = false;
            //labelSOPManager.Visible = labelSOPSimulator.Visible = labelTeamManager.Visible = labelEtc.Visible = labelSDMS.Visible = false;

            foreach (EtcButton btn in m_etcButtons)
            {
                btn.Visible = true;
            }

            rbtnBack.Visible = true;
        }

        public void HideEtcButtons()
        {
            ArrayList arrControls = m_dicModeControls[Mode.SUCCESS_LOGIN];

            foreach (Control ctrl in arrControls)
            {
                if (ctrl is RibbonButton || ctrl is Label)
                    ctrl.Visible = true;
            }

            //btnSOPManager.Visible = btnSOPSimulator.Visible = btnTeamManager.Visible = btnEtc.Visible = btnSDMS.Visible = true;
            //labelSOPManager.Visible = labelSOPSimulator.Visible = labelTeamManager.Visible = labelEtc.Visible = labelSDMS.Visible = true;

            foreach (EtcButton btn in m_etcButtons)
            {
                btn.Visible = false;
            }

            rbtnBack.Visible = false;
        }

        private EtcButton GetEtcButton(ExecuteManager.APP_TYPE appType)
        {
            foreach (EtcButton btn in m_etcButtons)
            {
                UnE.GUI.RibbonButton rbtn = btn.Button;

                if (rbtn == null)
                    continue;

                if (rbtn.Tag == null)
                    continue;

                ExecuteManager.APP_TYPE type = (ExecuteManager.APP_TYPE)rbtn.Tag;

                if (type == appType)
                    return btn;
            }

            return null;
        }

        private void rbtnBack_Click(object sender, EventArgs e)
        {
            HideEtcButtons();
        }

        private void ckbSaveID_CheckedChanged(object sender, EventArgs e)
        {
            if(ckbSaveID.Checked == false)
            {
                picSaveID.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_disable;

                RegUtil.WriteRegValue("IntegratedManager", "SaveID", "0", m_nSiteID);
                ckbAutoLogin.Checked = false;
            }
            else
            {
                picSaveID.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_enable;

                RegUtil.WriteRegValue("IntegratedManager", "SaveID", "1", m_nSiteID);
            }
        }

        private void ckbAutoLogin_CheckedChanged(object sender, EventArgs e)
        {
            if(ckbAutoLogin.Checked == true)
            {
                picAutoLogin.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_enable;

                RegUtil.WriteRegValue("IntegratedManager", "AutoLogin", "1", m_nSiteID);
            }
            else
            {
                picAutoLogin.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_disable;

                RegUtil.WriteRegValue("IntegratedManager", "AutoLogin", "0", m_nSiteID);
            }
        }

        private void btnDownloadManual_Click(object sender, EventArgs e)
        {
            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='SystemManualPath' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strFileName = WebDBManager.GetStringField(arrResult[0]);

                if (strFileName != null && strFileName != "")
                {
                    int nIndex = strFileName.LastIndexOf('.');
                    string strExt = "", strName = strFileName;

                    if (nIndex >= 0)
                    {
                        strExt = strFileName.Substring(nIndex);
                        strName = strFileName.Substring(0, nIndex);
                    }

                    string szURL = String.Format("{0}{1}", FormMain.Instance.DBManager.WebServerURL.Replace("/SOP", "/Doc/"), strFileName);

                    SaveFileDialog dlg = new SaveFileDialog();

                    dlg.Filter = string.Format("Manual File (*{0})|*{0}", strExt);
                    string defaultName = strName;
                    dlg.FileName = defaultName;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string szPath = dlg.FileName;

                        try
                        {
                            System.Net.WebClient client = new System.Net.WebClient();
                            client.DownloadFile(szURL, szPath);

                            if (File.Exists(szPath) == true)
                                OpenPDF(1, szPath);
                        }
                        catch (Exception ex)
                        {
                            // 파일이 이미 열려있음.
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private void btnDownloadVideo_Click(object sender, EventArgs e)
        {
            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='VideoManualPath' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strFileName = WebDBManager.GetStringField(arrResult[0]);

                if (strFileName != null && strFileName != "")
                {
                    int nIndex = strFileName.LastIndexOf('.');
                    string strExt = "", strName = strFileName;

                    if (nIndex >= 0)
                    {
                        strExt = strFileName.Substring(nIndex);
                        strName = strFileName.Substring(0, nIndex);
                    }

                    string szURL = String.Format("{0}{1}", FormMain.Instance.DBManager.WebServerURL.Replace("/SOP", "/Doc/"), strFileName);

                    SaveFileDialog dlg = new SaveFileDialog();

                    dlg.Filter = string.Format("동영상 File (*{0})|*{0}", strExt);
                    string defaultName = strName;
                    dlg.FileName = defaultName;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string szPath = dlg.FileName;

                        try
                        {
                            System.Net.WebClient client = new System.Net.WebClient();
                            client.DownloadFile(szURL, szPath);

                            if (File.Exists(szPath) == true)
                            {
                                // Open Windows Media Player
                                //System.Diagnostics.Process.Start("wmplayer.exe");
                                // Play Video
                                System.Diagnostics.Process.Start(szPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            // 파일이 이미 열려있음.
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private void btnDownloadPSMHandBook_Click(object sender, EventArgs e)
        {
            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='PSMHandBookPath' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strFileName = WebDBManager.GetStringField(arrResult[0]);

                if (strFileName != null && strFileName != "")
                {
                    int nIndex = strFileName.LastIndexOf('.');
                    string strExt = "", strName = strFileName;

                    if (nIndex >= 0)
                    {
                        strExt = strFileName.Substring(nIndex);
                        strName = strFileName.Substring(0, nIndex);
                    }

                    string szURL = String.Format("{0}{1}", FormMain.Instance.DBManager.WebServerURL.Replace("/SOP", "/Doc/"), strFileName);

                    SaveFileDialog dlg = new SaveFileDialog();

                    dlg.Filter = string.Format("HandBook File (*{0})|*{0}", strExt);
                    string defaultName = strName;
                    dlg.FileName = defaultName;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string szPath = dlg.FileName;

                        try
                        {
                            System.Net.WebClient client = new System.Net.WebClient();
                            client.DownloadFile(szURL, szPath);

                            if (File.Exists(szPath) == true)
                                OpenPDF(1, szPath);
                        }
                        catch (Exception ex)
                        {
                            // 파일이 이미 열려있음.
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        public static void OpenPDF(int nPageNumber, string strPath)
        {
            string strAcrobatPath = "";
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            if (nPageNumber > 0 && GetRegistry(ref strAcrobatPath))
            {
                startInfo.Arguments = string.Format("/A \"page={0}&zoom=100\" \"{1}\"", nPageNumber, strPath);
                startInfo.FileName = strAcrobatPath;
            }
            else
                startInfo.FileName = strPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = startInfo;
            process.Start();
        }

        public static bool GetRegistry(ref string strAcrobatPath)
        {
            const string AcrobatRoot = @"Applications\AcroRD32.exe";

            Microsoft.Win32.RegistryKey R = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(AcrobatRoot);

            if (R == null)
                return false;

            if (strAcrobatPath != null && strAcrobatPath.Length > 0)
                return true;

            strAcrobatPath = "";

            Microsoft.Win32.RegistryKey shell = R.OpenSubKey("shell");

            if (shell == null)
                return false;

            Microsoft.Win32.RegistryKey read = shell.OpenSubKey("Read");

            if (read == null)
                return false;

            Microsoft.Win32.RegistryKey command = read.OpenSubKey("command");

            if (command == null)
                return false;

            string[] names = command.GetValueNames();

            if (names == null || names.Count() == 0)
                return false;

            object value = command.GetValue(names[0]);

            if (value == null)
                return false;

            string strValue = value.ToString();

            int nIndex1 = strValue.IndexOf('\"');

            if (nIndex1 < 0)
                return false;

            int nIndex2 = strValue.IndexOf('\"', nIndex1 + 1);

            if (nIndex2 < 0)
                return false;

            string strPath = strValue.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strAcrobatPath = strPath;

            return true;
        }

        private void btnShowInternalClients_Click(object sender, EventArgs e)
        {
            FormClients frm = new FormClients(this.NetworkServer.ServiceProvider.LockObject);
            frm.Show(this);
        }

        private void btnSetChief_Click(object sender, EventArgs e)
        {
            SetChief frm = new SetChief(m_dbMgr, m_Chief);
            double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            frm.WindowRateWidth = dWindowRate[0];
            frm.WindowRateHeight = dWindowRate[1];
            frm.UpdateControl();

            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                m_Chief = frm.Chief;                
                this.txtChief.Text = m_Chief.DisplayText;
                this.txtPhoeNumber.Text = m_Chief.CallerPhoneNumber;
            }
            else
            {
                m_Chief = null;
                this.txtChief.Text = "";
                this.txtPhoeNumber.Text = "";
            }
        }

        private void btnOption_Click(object sender, EventArgs e)
        {
            SetOption frm = new SetOption(m_logInMgr, MemberID, MemberName);
            double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            frm.WindowRateWidth = dWindowRate[0];
            frm.WindowRateHeight = dWindowRate[1];
            frm.UpdateControl();

            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                labelMemberID.Tag = frm.ComapnyMember;
                MemberID = frm.MemberID;
                MemberName = frm.MemberName;
            }
            else
            {
                labelMemberID.Tag = null;
                MemberID = "";
                MemberName = "";
            }
        }

        private void SaveID_Click(object sender, EventArgs e)
        {
            ckbSaveID.Checked = !ckbSaveID.Checked;
        }

        private void AutoLogin_Click(object sender, EventArgs e)
        {
            ckbAutoLogin.Checked = !ckbAutoLogin.Checked;
        }

        private void ChangeNickName_Click(object sender, EventArgs e)
        {
            radioChangeNickName.Checked = !radioChangeNickName.Checked;            
        }

        private void ChangePassword_Click(object sender, EventArgs e)
        {
            radioChangePassword.Checked = !radioChangePassword.Checked;
        }

        private void ChiefChange_Click(object sender, EventArgs e)
        {
            rdoChiefChange.Checked = !rdoChiefChange.Checked;
        }

        private void btnRegNext(object sender, EventArgs e)
        {
            try
            {
                if (textBoxMemberID.Text.Length == 0)
                {
                    textBoxMemberID.Focus();
                   
                    FormMessageBox msgBox = new FormMessageBox("아이디를 입력하세요.", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    return;
                }
                else if (textBoxNickName.Text.Length == 0)
                {
                    textBoxNickName.Focus();
                    FormMessageBox msgBox = new FormMessageBox("닉네임을 입력하세요.", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    return;
                }
                else if (textBoxMemberName.Text.Length == 0)
                {
                    textBoxMemberName.Focus();
                    //throw new ApplicationException("비밀번호를 입력하세요");

                    FormMessageBox msgBox = new FormMessageBox("비밀번호를 입력하세요.", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    return;
                }
                else if (textBoxConfirmPassword.Text.Length == 0)
                {
                    textBoxConfirmPassword.Focus();
                    //throw new ApplicationException("비밀번호를 한번더 입력하세요");

                    FormMessageBox msgBox = new FormMessageBox("비밀번호를 한번더 입력하세요.", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    return;
                }
                if (textBoxMemberName.Text != textBoxConfirmPassword.Text)
                {
                    textBoxConfirmPassword.Text = "";
                    textBoxConfirmPassword.Focus();
                    //throw new ApplicationException("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요");

                    FormMessageBox msgBox = new FormMessageBox("비밀번호 입력이 일치하지 않습니다.\r\n대소문자 구별에 유의하신후 다시 한번 비밀번호를 입력해 주세요.", "알림", MessageBoxButtons.OK);
                    msgBox.StartPosition = FormStartPosition.CenterParent;
                    msgBox.ShowDialog();

                    return;
                }

                // -1 : Company Member 선택 안함 
                int nCompanyMemberID = -1;
                if (labelMemberID.Tag != null)
                    nCompanyMemberID = (int)labelMemberID.Tag;

                if (m_logInMgr.JoinUser(nCompanyMemberID, textBoxMemberID.Text, textBoxMemberName.Text, textBoxNickName.Text))
                {
                    // LinkMember 추가
                    InsertLinkMember(textBoxMemberID.Text, textBoxNickName.Text);
                }

            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBoxRibbon.Show(ex.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            m_Chief = null;
        }

        private void btnChangeChief_Click(object sender, EventArgs e)
        {
            SetChief frm = new SetChief(m_dbMgr, m_Chief);
            frm.StartPosition = FormStartPosition.CenterParent;
            double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            frm.WindowRateWidth = dWindowRate[0];
            frm.WindowRateHeight = dWindowRate[1];
            frm.UpdateControl();

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                m_Chief = frm.Chief;
                this.textBoxCurrentPassword.Text = m_Chief.DisplayText;
                this.textBoxChangingPassword.Text = m_Chief.CallerPhoneNumber;
            }
            else
            {
                //m_Chief = null;
                //this.textBoxCurrentPassword.Text = "";
                //this.textBoxChangingPassword.Text = "";
            }
        }

        private void FormMain_Move(object sender, EventArgs e)
        {
            GetWindowRate();
        }

        public void OnConnected()
        {
            Reconnect();
        }

        private bool socketIsValid = false;
        public void OnMakeItself()
        {
            socketIsValid = true;
            Reconnect();
        }

        private void Reconnect()
        {            
            if (m_needLogin && socketIsValid)
            {
                m_needLogin = false;

                System.Diagnostics.Trace.WriteLine("OnConnected");

                this.Invoke((MethodInvoker)delegate
                {
                    btnLogin.Enabled = false;
                    btnLogin_Click(null, null);
                    m_needLogin = false;
                    btnLogin.Enabled = true;
                });
            }
        }


        private Dictionary<int, string> GetGenLevel()
        {
            string strSQL = "select ID, LevelName from SOPGenLevel";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            Dictionary<int, string> dicLevel = new Dictionary<int, string>();

            if (arrResult == null || arrResult.Count == 0)
                return null;

            for (int i = 0; arrResult.Count > i; i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1]);

                dicLevel.Add(nID, strLevelName);
            }

            return dicLevel;
        }

        private bool InsertLinkMember(string strUserID, string strNickName)
        {
            string strSQL = "insert into LinkMember (UserID, NickName) values ('" + strUserID + "', '" + strNickName + "')";
            ArrayList arrResult = m_dbMainMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }
    }
}
