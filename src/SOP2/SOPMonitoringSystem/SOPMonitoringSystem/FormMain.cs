using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using UnE.Geometry;

namespace SOPMonitoringSystem
{
    using Process;
    using Sections;

    using System.Runtime.InteropServices;
    using System.IO;
    delegate long mciSendStringDelegate(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

    public partial class FormMain : Form, SOPDisasterSystem.ISOPInfo
    {
        private SOPLog m_logFile = new SOPLog();
        public SOPLog LogFile
        {
            get { return m_logFile; }
        }

        private SOPManager m_sopMgr = null;
        private static IntPtr instanceHandle = IntPtr.Zero;
        private MP3Player m_player = new MP3Player();

        private bool m_CloseThread = false;

        public bool CloseThread
        {
            get { return m_CloseThread; }
            set { m_CloseThread = value; }
        }

        private XtremeCommandBars.CommandBarPopup ControlFile;
        private XtremeCommandBars.RibbonBackstageTab m_ControlOption;
        private XtremeCommandBars.RibbonTab TabWrite = null;
        private XtremeCommandBars.RibbonGroup GroupAnnounce = null;
        private XtremeCommandBars.RibbonGroup GroupView = null;
        private XtremeCommandBars.RibbonGroup GroupControl = null;
        private XtremeCommandBars.CommandBarControl ctrlReal = null;    // 실제모드인가?
        private XtremeCommandBars.CommandBarControl ctrlVirtual = null;      // 모의훈련인가?
        private XtremeCommandBars.CommandBarControl ctrlReg = null;     // 등록 버전인가?
        private XtremeCommandBars.CommandBarControl ctrlNonReg = null; // 미등록 버전인가?
        private XtremeCommandBars.CommandBarControl ctrlWeekday = null; // 평일 버전인가?
        private XtremeCommandBars.CommandBarControl ctrlWeekend = null; // 야간 버전인가?
        private XtremeCommandBars.CommandBarControl ctrlPlay = null;
        private XtremeCommandBars.CommandBarControl ctrlCancel = null;
        private XtremeCommandBars.CommandBarControl ctrCurrent = null;
        private XtremeCommandBars.CommandBarControl ctrScaletoFit = null;
        private XtremeCommandBars.CommandBarControl ctrlControl = null;
        private XtremeCommandBars.CommandBarControl ctrlControlReturn = null;
        private XtremeCommandBars.CommandBarControl ctrlMonitoring = null;
        private XtremeCommandBars.CommandBarControl ctrlControlRequest = null;

        private XtremeCommandBars.CommandBarControl ctrPlayAnn = null, ctrPauseAnn = null, ctrStopAnn = null, ctrCount = null;

        private PageBackstageHome m_pageHome;
        private PageBackstageOption m_pageOption;

        protected string m_strSkinFolder;
        private bool m_isFirst = false;
        private bool m_isOpen = false;
        public bool isOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }
        private WebDBManager m_dbMgr = null;

        private string m_strVersion = "V1.0";
        private bool m_isReadVersion = false;

        private FormStatus m_frmStatus = null;
        private FormRealTimeInfo m_frmReal = null;
        private SOPDisasterSystem.FormMain m_frmMain2 = null;
        public SOPDisasterSystem.FormMain FrmMain2
        {
            get { return m_frmMain2; }
            set { m_frmMain2 = value; }
        }

        private FormMissionStatus m_frmMain3 = null;
        public FormMissionStatus FrmMain3
        {
            get { return m_frmMain3; }
        }

        private PopupProgressReport m_frmReport = null;

        private Sections.WorkFlow m_currentWork = null;
        public Sections.WorkFlow CurrentWork
        {
            get { return m_currentWork; }
        }
        private bool DoorBellChecked = true; // 초기 DoorBellChecked

        private Sections.Section m_currentSection = null;
        public Sections.Section CurrentSection
        {
            get { return m_currentSection; }
            set { m_currentSection = value; }
        }
        
        private int m_nChangeUserID;
        public int ChangeUserID
        {
            get { return m_nChangeUserID; }
            set { m_nChangeUserID = value; }
        }

        private SDMS.NetworkManager m_netMgr = null;

        public SDMS.NetworkManager NetworkManager
        {
            get { return m_netMgr; }
        }

        //////////////////////////////////////////////////////////////////////////
        static public FormMain Instance;

        //private int m_nSirenCount = 0;
        private int m_nSOPGenUserID = -1;
        private int m_nSOPGenUserLevel = 1;

        private string m_strSOPGenUserRealName = "";
        
        private ArrayList m_arrConnectedUser = new ArrayList();
        private Thread DBWrite = null;
        private bool m_isStop = false; // 쓰레드를 중지하고자 할때 true

        // 제어권 요청으로 인하여 WorkerThread가 일시 정지된 상태인가?
        private bool m_isWorkingTempThread = false;

        private bool m_smsOn = false;
        // 협력업체들에게도 SOP 시작 및 종료시 문자메시지를 보낼것인가?
        private bool m_smsExternalCompanyMemberOn = false;

        private bool m_useEzSMS = false;
        public bool UseEzSMS
        {
            get { return m_useEzSMS; }
            set { m_useEzSMS = value; }
        }

        private bool m_useBroadcast = false;
        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        private bool m_showMissionText = false;
        public bool ShowMissionText
        {
            get { return m_showMissionText; }
            set { m_showMissionText = value; }
        }

        // Workflow가 Event와 함께 시작되었는지 여부
        private bool bStartWorkflowEvent = false;

        // 제어권 요청을 하였는지 여부
        private bool m_bRequestControl = false;

        // 제어권 요청창
        private PopupRequestProgress m_frmRequestProgress = null;

        // 제어권 요청 리스트
        PopupRequestControl m_frmRequestControl = null;

        // 현재 실행중인 SOP의 실행임무들에 대한 상세 옵션
        private Dictionary<Sections.MissionItem, MissionItemInfo> m_dicMissionInfo = new Dictionary<MissionItem, MissionItemInfo>();

        public void SetMissionInfo(Sections.MissionItem item, MissionItemInfo info)
        {
            m_dicMissionInfo[item] = info;
        }

        public MissionItemInfo GetMissionInfo(Sections.MissionItem item)
        {
            if (m_dicMissionInfo.ContainsKey(item))
                return m_dicMissionInfo[item];

            return null;
        }

        // CompanyMember 휴대폰 암호화
        /*private void Test()
        {
            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
            string strNum = AES256Cipher.AES_encrypt("01025893257", key);
            System.IO.StreamReader reader = new StreamReader("c:/UnE/CompanyMember.txt", Encoding.UTF8);
            System.IO.StreamWriter writer = new StreamWriter("c:/UnE/query.sql", false, Encoding.UTF8);

            int nID = 1;

            while (!reader.EndOfStream)
            {
                string strPhoneNumber = reader.ReadLine();
                string strEncrypt = AES256Cipher.AES_encrypt(strPhoneNumber, key);

                string strSQL = string.Format("Update sop3.dbo.CompanyMember set PhoneNumber = '{0}' where id = {1};", strEncrypt, nID++);
                writer.WriteLine(strSQL);

                //string strDecrypt = AES256Cipher.AES_decrypt(strEncrypt, key);
            }

            reader.Close();
            writer.Close();
        }*/

        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName)
        {
            m_nSOPGenUserID = nSOPGenUserID;
            m_strSOPGenUserRealName = strSOPGenUserRealName;

            InitializeComponent();

            Instance = this;
            m_strSkinFolder = StylesPath();
            SkinLoad();

            m_dbMgr = new WebDBManager(this);
            m_sopMgr = new SOPManager(m_dbMgr);
            m_netMgr = SDMS.NetworkManager.Instance;
                       
            //////////////////////////////////////////////////////////////////////////
            ProcessManager pProcessManager = ProcessManager.Instance;
            WorkFlowManager pWorkflowManager = WorkFlowManager.Instance;
            TabPageManager pPageManager = TabPageManager.Instance;

            TTSManager pTtsManager = TTSManager.Instance;
            pTtsManager.DBMgr = m_dbMgr;

            instanceHandle = this.Handle;
        }

        private void LinkSOPDisasterSystem()
        {
            if (m_frmMain2 != null)
                m_frmMain2.GetSituation().SetSOPInfo(this);
        }

        public Point GetMonitoringPosition()
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            if (sc.Length == 0)
            {
                return new Point(0, 0);
            }

            string szNum = nMonitoring.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nMonitoring)
            {
                return sc[nIdx].Bounds.Location;
            }
            return new Point(0, 0);
        }
        
        private bool SetMonitorForm(Form form, int nDisplay)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (form == null)
                return false;

            
            if (sc.Length == 0)
            {
                return false;
            }

            string szNum = nDisplay.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nDisplay)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);
                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Maximized;
            }
            return true;
        }


        private int nMonitoring = 1;
        private int nDisaster = 2;
        private int nMission = 3;

        private void FormMain_Load(object sender, EventArgs e)
        {
            string strSMSOn = DBManager.LoadIni("sms_on", "Server Connection Info");
            m_smsOn = strSMSOn == "1";

            string strezSMSOn = DBManager.LoadIni("ez_sms_on", "Server Connection Info");
            m_useEzSMS = strezSMSOn == "1";

            string strSMSExternalCompanyMemberOn = DBManager.LoadIni("sms_externalCompanyMember_on", "Server Connection Info");
            m_smsExternalCompanyMemberOn = strSMSExternalCompanyMemberOn == "1";

            string strBroadcast = DBManager.LoadIni("broadcast_on", "Server Connection Info");
            m_useBroadcast = strBroadcast == "1";

            string strMIssionText = DBManager.LoadIni("show_mission_text", "Server Connection Info");
            m_showMissionText = strMIssionText == "1";

            LoadIcons();
            CreateRibbonBar();
            
            //////////////////////////////////////////////////////////////////////////
            // 모니터 출력을 지정
            m_frmMain2 = new SOPDisasterSystem.FormMain(this);
            m_frmReport = new PopupProgressReport(this);

            string szMonitoring = DBManager.LoadIni("MonitoringSystem", "Monitor Info");
            try
            {
                nMonitoring = int.Parse(szMonitoring);
            }
            catch (System.Exception)
            {            	
            }
            
            string szDisaster = DBManager.LoadIni("DisasterSystem", "Monitor Info");
            try
            {
                nDisaster = int.Parse(szDisaster);
            }
            catch (System.Exception)
            {
            }

            string szMission = DBManager.LoadIni("MissionList", "Monitor Info");
            if (szMission == null || szMission.Equals(""))
            {
                szMission = "-1";
            }
            try
            {
                nMission = int.Parse(szMission);
            }
            catch (System.Exception)
            {
            }
            
            SetMonitorForm(this, nMonitoring);
            this.WindowState = FormWindowState.Maximized;

            SetMonitorForm(m_frmMain2, nDisaster);            
            m_frmMain2.Show(); 
          
            ReportInfo();
            LinkSOPDisasterSystem();


            m_frmMain3 = new FormMissionStatus();
            
            if (nMission != -1)
            {
                SetMonitorForm(m_frmMain3, nMission);
                m_frmMain3.ShowMaximize();
                m_frmMain3.Show();

                //showMonitor3();
            }           

            m_nSOPGenUserLevel = DBManager.GetGenUserLevel(m_nSOPGenUserID);

            bool isRegular = ctrlReg.Checked;     // 등록 모드인가?
            bool isNormal = ctrlWeekday.Checked;  // 평일 버전인가?

            m_sopMgr.Load(isRegular, isNormal);
            History.HistoryManager.Instance.LoadActionStepHistory(m_dbMgr);

            ctrlNonReg.Enabled = false;
            ctrlControlReturn.Visible = false;
            ctrlControlRequest.Visible = true;

            // Default는 훈련 모드
            ctrlReal.Checked = false;
            ctrlVirtual.Checked = true;

            // 초기 제어권 없음으로 설정
            SetControl(false);

            // Docking Bar 설정
            int left, top, right, bottom;
            axCommandBars.GetClientRect(out left, out top, out right, out bottom);
            panelMain.SetBounds(left, top, right - left, bottom - top);

            m_pageOption.mPreviewBox.OriginSize = m_pageHome.tabControl.Size;
            m_pageOption.mPreviewBox.ThumbnailBackColor = m_pageHome.tabControl.BackColor;
            m_pageOption.mPreviewBox.TargetContorl = m_pageHome.tabControl;

            CreateStatusForm();
            
            this.Text += " " + GetAppVersion();
            FormMain.Instance.GetPageHome().panel.Visible = false;
            FormMain.Instance.GetPageHome().SetBackgroundImage(false); 
            TTSManager.Instance.UseBroadcast = m_useBroadcast;

        }

        private void CreateStatusForm()
        {
            int left, top, right, bottom;
            GroupView.GetRect(out left, out top, out right, out bottom);

            //int nTabCount = this.GetPageHome().TabControls.TabCount;            
			//int nTabHeight = 22;
            //if (nTabCount > 0)
            //{
            //    int nHeight1 = this.GetPageHome().TabControls.Height;
            //    int nHeight2 = this.GetPageHome().TabControls.TabPages[0].Height;
            //    nTabHeight = nHeight1 - nHeight2;
            //}

            FormStatus frm = new FormStatus(this);
            m_frmStatus = frm;
            //frm.Location = new Point(right + 1-95, top - nTabHeight - 4)
            frm.Location = new Point(right + 1-95, top - 3);
            m_frmStatus.RealMode(ctrlReal.Checked);
            frm.Show();

            FormRealTimeInfo frm2 = new FormRealTimeInfo(this);
            m_frmReal = frm2;
            //frm2.Location = new Point(right + 1 + frm.Width - 95, top - nTabHeight - 4);
            frm2.Location = new Point(right + 2 + frm.Width - 95, top - 3);
            frm2.Show();
        }

        public int high = 0;

        private void FormMain_Activated(object sender, EventArgs e)
        {
            if (!m_isFirst)
            {
                m_pageHome.Location = new Point(0, 0);
                m_pageHome.Dock = DockStyle.Fill;
                m_pageHome.TopLevel = false;
                m_pageHome.Parent = this;
                panelMain.Controls.Add(m_pageHome);
                m_pageHome.Show();
                m_isFirst = true;

                if (m_sopMgr.IsOpened)
                {
                    if (LoadSOP())
                    {
                        StartWriteDB(); //사용자 접속 정보 DB에 쓰기

                        // 기존에 실행되고 있던 SOP를 불러온다.
                        LoadHistory();

                        SectionTabPage page = (SectionTabPage)GetPageHome().TabControls.SelectedTab;
                        int nCount = GetPageHome().TabControls.Controls.Count;
                        if (page != null)
                        {
                            int nActionStepID = page.ActionStepID;

                            if (nActionStepID != 0)
                            {
                                m_isOpen = true;
                                FormMain.Instance.GetPageHome().panel.Visible = true;
                                FormMain.Instance.GetPageHome().SetBackgroundImage(true);

                                ctrlReal.Checked = !page.VirtualMode;
                                ActionStepInfo info = FormMain.Instance.SOPManager.GetActionStepInfo(nActionStepID);
                                BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
                                TreeNode node = tree.FindActionStepNode(nActionStepID);
                                tree.PrevSelectedDisasterID = info.DisasterID;
                                //tree.SelectSop(node);

                                bool isRealMode = true;
                                int nCurrentActionStepID = ReadCurrentActionStep(ref isRealMode);

                               // if (HasControl == true)
                                {
                                    if (nCurrentActionStepID >= 0)
                                        GetPageHome().GetDockScenario().SelectedGridRow(nCurrentActionStepID, isRealMode);
                                    else
                                    {
                                        // 마지막 시나리오 선택
                                        GetPageHome().GetDockScenario().SetSelectedGridRow();
                                    }
                                }

                                

                                page.ReSizePanel();
                            }
                        }
                    }
                    LoadCompanyMember();
                }
            }
        }

        public int ReadCurrentActionStep(ref bool isRealMode)
        {
            string strSQL = "select ActionStepID, RealMode from CurrentActionStep where id = 1";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count <= 1)
                return -1;

            int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);

            return nActionStepID;
        }

        void showMonitor2()
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            m_frmMain2.StartPosition = FormStartPosition.Manual;
            if (sc.Length >= 2)
            {
                this.Location = sc[0].Bounds.Location;
                m_frmMain2.Location = sc[1].Bounds.Location;
                m_frmMain2.Size = new Size(1600 , 1400);
            }
            else
            {
                m_frmMain2.Location = sc[0].Bounds.Location;
                m_frmMain2.Size = new Size(1600, 1400);
            }            
            m_frmMain2.Show();
        }

        void showMonitor3()
        {
            m_frmMain3.Title = "";

            Screen[] sc;
            sc = Screen.AllScreens;
            m_frmMain3.StartPosition = FormStartPosition.Manual;

            if (sc.Length >= 3)
            {
                this.Location = sc[0].Bounds.Location;
                m_frmMain3.Location = sc[2].Bounds.Location;
                m_frmMain3.Size = new Size(1600, 1400);
            }
            else
            {
                m_frmMain3.Location = sc[sc.Length - 1].Bounds.Location;
                m_frmMain3.Size = new Size(1600, 1400);
            }

            // 전체 화면
            m_frmMain3.ShowMaximize();
            m_frmMain3.Show();
        }

        void ReportInfo()
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            m_frmReport.StartPosition = FormStartPosition.Manual;
            if (sc.Length == 2)
            {
                this.Location = sc[0].Bounds.Location;
                Point pt = sc[1].Bounds.Location;
                pt.X = sc[1].Bounds.Location.X + 265;
                pt.Y = sc[1].Bounds.Location.Y + 87;
                m_frmReport.Location = pt;
            }
            else
            {
                m_frmReport.Location = sc[0].Bounds.Location;
            }

            //m_frmReport.Show();
        }

        private bool LoadSOP()
        {
            BarLevelTree tree = m_pageHome.GetDockScenario().GetBarLevelTree();
            return tree.Load(m_sopMgr, ctrlReg.Checked, ctrlWeekday.Checked);
        }

        private bool LoadHistory()
        {
            return m_pageHome.GetDockScenario().LoadHistory(m_dbMgr, m_sopMgr);
        }

        private void StartWriteDB()
        {
            DBWrite = new Thread(new ThreadStart(WorkerThreadMethod));
            DBWrite.IsBackground = false;
            DBWrite.Start();
            Thread.Sleep(500);
        }

        private void StopWriteDB()
        {
            try
            {
                if (DBWrite != null && DBWrite.IsAlive)
                {
                    m_isWorkingTempThread = false;
                    m_isStop = true;
                    DBWrite.Join(500);
                    DBWrite.Abort();

                    DBWrite = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
            }
        }

        private bool LoadCompanyMember()
        {
            DockingRightPersonnel personnel = m_pageHome.GetDockPersonnel();
            return personnel.Load(m_sopMgr);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_frmReal != null)
                m_frmReal.StopTimer();
            if (m_frmMain2 != null && m_frmMain2.Visible == true)
            {
                m_frmMain2.Visible = false;
                m_frmMain2.Invoke((MethodInvoker)delegate
                {
                    m_frmMain2.Close();
                    m_frmMain2.Dispose();
                });                
            }

            if (m_frmReport != null)
                m_frmReport.Dispose();
        }

        private void SkinLoad()
        {
            axSkinFramework.LoadSkin(m_strSkinFolder + "Office2010.cjstyles", "Normalblue.ini");
            axSkinFramework.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BACKGROUND);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");
            return strExePath + "\\Styles\\";
        }
		
		public static Bitmap GetImageByName(string imageName)
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("neutral");
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			string resourceName = "SOPMonitoringSystem.Properties.Resources";
			var rm = new System.Resources.ResourceManager(resourceName, asm);
			return (Bitmap)rm.GetObject(imageName);
		}

		private void AddBitmapFormRes(string name, object id)
		{
			string szName = name.Replace("-", "_");
			Bitmap bImage = GetImageByName(szName);
			axCommandBars.Icons.AddBitmap(bImage.GetHbitmap().ToInt32(), id, XtremeCommandBars.XTPImageState.xtpImageNormal, true);
		}

        private void LoadIcons()
        {
            axCommandBars.Options.UseSharedImageList = false;

			AddBitmapFormRes("BackstageIcons", new int[] { ID.ID_FILE_NEWSOP, ID.ID_FILE_SAVE, ID.ID_FILE_SAVE_AS, ID.ID_FILE_OPEN, ID.ID_FILE_CLOSE, ID.ID_FILE_OPTIONS, ID.ID_APP_EXIT });
			AddBitmapFormRes("RibbonRunIcons", new int[] { ID.ID_RUN_PLAY, ID.ID_RUN_CANCEL, ID.ID_RUN_COMPLETE });
			AddBitmapFormRes("RibbonLargeIcons", new int[] { ID.ID_RUN_FRONT, ID.ID_ANNOUNCE_PLAY, ID.ID_RUN_FRONT, ID.ID_ANNOUNCE_PAUSE, ID.ID_ANNOUNCE_STOP, ID.ID_ANNOUNCE_COUNT });
			AddBitmapFormRes("current_scale_to_fit",  new int[] { ID.ID_VIEW_CURRENT, ID.ID_VIEW_SCALETOFIT });
			AddBitmapFormRes("RibbonControls", new int[] { ID.ID_CONTROL_CONTROL, ID.ID_CONTROL_RETURN, ID.ID_CONTROL_MONITORING, ID.ID_CONTROL_REQUEST });

            XtremeCommandBars.ToolTipContext ToolTipContext = null;
            ToolTipContext = axCommandBars.ToolTipContext;
            ToolTipContext.Style = XtremeCommandBars.XTPToolTipStyle.xtpToolTipResource;
            ToolTipContext.ShowTitleAndDescription(true, XtremeCommandBars.XTPToolTipIcon.xtpToolTipIconNone);
            ToolTipContext.SetMargin(2, 2, 2, 2);
            ToolTipContext.MaxTipWidth = 180;
        }

        private void CreateRibbonBar()
        {
            XtremeCommandBars.RibbonGroup GroupRun = null;

            XtremeCommandBars.RibbonBar RibbonBar = null;
            RibbonBar = axCommandBars.AddRibbonBar("The Ribbon");
            RibbonBar.EnableDocking(XtremeCommandBars.XTPToolBarFlags.xtpFlagStretched);

            ControlFile = RibbonBar.AddSystemButton();
            ControlFile.IconId = ID.ID_SYSTEM_ICON;
            ControlFile.Caption = "옵션";
            ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonCaption;

            CreateBackstageView();

            TabWrite = RibbonBar.InsertTab(0, "실행");
            TabWrite.Id = ID.ID_TAB_WRITE;

            XtremeCommandBars.CommandBarControl ctrl = null;

            GroupControl = TabWrite.Groups.AddGroup("컨트롤", ID.ID_GROUP_CONTROL);
            ctrlControl = GroupControl.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_CONTROL_CONTROL, "제어 ", false, false);
            ctrlControlReturn = GroupControl.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_CONTROL_RETURN, "제어권 반납", false, false);
            ctrlMonitoring = GroupControl.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_CONTROL_MONITORING, "모니터링", false, false);
            ctrlControlRequest = GroupControl.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_CONTROL_REQUEST, "제어권 요청", false, false);

            GroupRun = TabWrite.Groups.AddGroup("실행", ID.ID_GROUP_LEVEL);
            //Control = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_RUN_FRONT, "&이전단계", false, false);
            ctrlPlay = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_RUN_PLAY, "시작", false, false);
            ctrlCancel = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_RUN_CANCEL, "실행취소", false, false);
            ctrlCancel.Enabled = false;

            ctrlReal = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlRadioButton, ID.ID_RUN_REAL, "실제모드", false, false);
            ctrlVirtual = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlRadioButton, ID.ID_RUN_VIRTUAL, "모의훈련모드", false, false);

            GroupRun = TabWrite.Groups.AddGroup("모드", ID.ID_GROUP_LEVEL);
            ctrlReg = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlRadioButton, ID.ID_MODE_REGISTER, "등록모드", false, false);
            ctrlNonReg = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlRadioButton, ID.ID_MODE_NONREGISTER, "미등록모드", false, false);
            ctrl = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlButton, 0, "&", false, false);
            ctrlWeekday = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlRadioButton, ID.ID_MODE_WEEKDAY, "평일", false, false);
            ctrlWeekend = GroupRun.Add(XtremeCommandBars.XTPControlType.xtpControlRadioButton, ID.ID_MODE_WEEKEND, "야간 및 휴일", false, false);

            GroupAnnounce = TabWrite.Groups.AddGroup("안내방송", ID.ID_GROUP_ANNOUNCE);
            ctrPlayAnn = GroupAnnounce.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_ANNOUNCE_PLAY, "시작", false, false);
            ctrPauseAnn = GroupAnnounce.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_ANNOUNCE_PAUSE, "일시정지", false, false);
            ctrStopAnn = GroupAnnounce.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_ANNOUNCE_STOP, "정지", false, false);
            ctrCount = GroupAnnounce.Add(XtremeCommandBars.XTPControlType.xtpControlLabel, ID.ID_ANNOUNCE_COUNT, "", false, false);

            GroupView = TabWrite.Groups.AddGroup("보기", ID.ID_GROUP_VIEW);
            ctrCurrent = GroupView.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_VIEW_CURRENT, "Current", false, false);
            ctrScaletoFit = GroupView.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_VIEW_SCALETOFIT, "Scale to Fit", false, false);

            RibbonBar.ShowQuickAccess = false;
            RibbonBar.ShowCaptionAlways = false;

            ctrlReal.Checked = true;
            ctrlReg.Checked = true;
            ctrlWeekday.Checked = true;
        }

        private void CreateBackstageView()
        {
            XtremeCommandBars.RibbonBar RibbonBar;
            RibbonBar = (XtremeCommandBars.RibbonBar)axCommandBars.ActiveMenuBar;

            XtremeCommandBars.RibbonBackstageView BackstageView;
            BackstageView = (XtremeCommandBars.RibbonBackstageView)axCommandBars.CreateCommandBar("CXTPRibbonBackstageView");

            RibbonBar.AddSystemButton().CommandBar = (XtremeCommandBars.CommandBar)BackstageView;

            // 파일 탭 메뉴
            if (m_pageHome == null)
                m_pageHome = new PageBackstageHome();
            if (m_pageOption == null)
                m_pageOption = new PageBackstageOption();

            GetPageOption().DeleteOptionChange += m_pageHome.DeleteOptionChanged;
            m_ControlOption = BackstageView.AddTab(ID.ID_FILE_OPTIONS, "옵션", m_pageOption.Handle.ToInt32());
            BackstageView.AddCommand(ID.ID_APP_EXIT, "끝내기");
            m_ControlOption.DefaultItem = true;
        }

        private XtremeCommandBars.RibbonBar RibbonBar()
        {
            return (XtremeCommandBars.RibbonBar)axCommandBars.ActiveMenuBar;
        }
        
        //////////////////////////////////////////////////////////////////////////
        // ADD , RUN , STOP WORKFLOW 
        private void AddWorkflow(TabPage page)
        {
            int tabId = -1;
            ArrayList arSections = new ArrayList();
            foreach (Control control in page.Controls)
            {
                if (control.GetType() == typeof(Sections.PanelSectionEx))
                {
                    Sections.PanelSectionEx pane = (Sections.PanelSectionEx)control;
                    tabId = pane.ActionStepID;
                    arSections.AddRange(pane.Sections);
                }
            }
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;

            SectionTabPage tabPage = (SectionTabPage)page;
            bool bTabPage = !tabPage.VirtualMode;
            tabPage.UseWaterMark = GetPageOption().GetVirtualMode();
            tabPage.VirtualMode = !FormMain.Instance.IsReal;
            manager.Add(tabPage.ActionStepID, arSections, !tabPage.VirtualMode);
        }

        public int GetTabActionStepID(TabPage tabPage)
        {
            foreach (Control control in tabPage.Controls)
            {
                if (control.GetType() == typeof(Sections.PanelSectionEx))
                {
                    Sections.PanelSectionEx pane = (Sections.PanelSectionEx)control;
                    return pane.ActionStepID;
                }
            }
            return -1;
        }
       
        private void RunWorkflowAsync(object sender, ProcessEventArgs ex)
        {
            if (sender == null)
            {
                return;
            }
            WorkFlowStartNotifyProcess start = (WorkFlowStartNotifyProcess)sender;
            if (start == null || start.Popup.DialogResult == DialogResult.Cancel)
            {
                bStartWorkflowEvent = false;
                return;
            }

            if (start == null || start.PopupOption.DialogResult == DialogResult.Cancel)
            {
                bStartWorkflowEvent = false;
                return;
            }
            
            bool bSendSMS = start.UseSMS;

            if (GetPageHome().TabControls.SelectedTab == null)
            {
                bStartWorkflowEvent = false;
                return;
            }

            WorkFlow work = RunWorkflow();
			
            if (work != null)
            {
                work.BeginEndEventSendSMS = bSendSMS;
                work.SOPName = start.SOPName;
                if (start.HasPosition)
                {
                    work.Position = start.PositionName;
                    work.HasPosition = true;
                    work.LastPosition = start.Popup.LastPoistion;
					SetCurrentWorkflow(work);
                }
                else
                {
                    work.Position = "";
                    work.HasPosition = false;
                    work.LastPosition = null;
                }
            }
            bStartWorkflowEvent = false;
        }

        public Sections.WorkFlow GetCurrentWorkflow()
        {
            return m_currentWork;
        }

        public void SetCurrentWorkflow(Sections.WorkFlow work)
        {
            if (m_currentWork != null)
            {                
                if (m_currentWork.HasPosition == true)
                {
                    if (m_currentWork.LastPosition != null)
                    {
                        m_frmMain2.LayoutForm.LastPos = m_currentWork.LastPosition;
                        m_frmMain2.LayoutForm.RemoveDisasterPos();
                    }
                }                
            }

            m_currentWork = work;
            if (m_currentWork != null && m_currentWork.HasPosition == true)
            {
                if (m_currentWork.LastPosition != null)
                {
                    SOPMonitoringSystem.HistoryDiasterPosition pos = m_currentWork.LastPosition;
                    m_frmMain2.LayoutForm.LastPos =  pos;
                    m_frmMain2.LayoutForm.AddDisasterPos(pos.DisasterName, pos.X, pos.Y, pos.Z);
                }                
            }
        }

        public void toolstripSetting(string str)
        {
            GetPageHome().toolstripSetting(str);
        }
        public Sections.WorkFlow RunWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;
            page.State = Sections.TabPageState.USE;
            page.CreateNew = false;
            page.VirtualMode = !FormMain.Instance.IsReal;
            Sections.TabPageManager.Instance.AddPage(page, !page.VirtualMode);
            int ActionID = page.ActionStepID;
            if (!manager.Exist(ActionID, !page.VirtualMode))
            {
                AddWorkflow(page);
            }

            if( HasControl == true)
                WriteCurrentActionStepID(ActionID, !page.VirtualMode);

            page.ActionStepID = ActionID;
            Sections.TabPageManager.Instance.SetUsePage(ActionID, true, !page.VirtualMode);

            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(ActionID);
            string szPath = node.FullPath;
            bool bHasPos = true;
            if (szPath.IndexOf("자연재해") != -1)
            {
                bHasPos = false;
            }
            string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);

            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            work.HasPosition = bHasPos;
            work.SOPName = sopName;
            if (work != null)
                work.Start();

            m_pageHome.GetDockScenario().AddGridRowScenario(szPath.Replace("\\", "/"), page.ActionStepID, !page.VirtualMode);

            m_frmStatus.StatusBoard(work.State);
            SetCurrentWorkflow(work);
            EnabledRunGroup();
            return work;
        }


        private ArrayList LoadHistoryDisasterPosition()
        {
            SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

            string strSQL = "select top 5 Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID ";
            strSQL += "from HistoryDisasterPos Where FloorIndex = -999 order by ID Desc";

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);

            ArrayList result = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i+= 7)
            {
                try
                {
                    string strNamePos = WebDBManager.GetStringField(arrResult[i], "");
                    float posX = WebDBManager.GetFloatField((string)arrResult[i + 1], 0.0f);
                    float posY = WebDBManager.GetFloatField((string)arrResult[i + 2], 0.0f);
                    float posZ = WebDBManager.GetFloatField((string)arrResult[i + 3], 0.0f);
                    float floorIdx = WebDBManager.GetFloatField((string)arrResult[i + 4], -999.0f);
                    string strDiasterName = WebDBManager.GetStringField((string)arrResult[i + 5], "");
                    string strBuildingID = WebDBManager.GetStringField((string)arrResult[i + 6], "");

                    HistoryDiasterPosition hisPos = new HistoryDiasterPosition();
                    hisPos.PoistionName = strNamePos;
                    System.Drawing.PointF pos = new System.Drawing.PointF(posX, posY);
                    hisPos.X = posX;
                    hisPos.Y = posY;
                    hisPos.Z = posZ;
                    hisPos.DisasterName = strDiasterName;
                    hisPos.FloorIndex = floorIdx;
                    hisPos.BuildingID = strBuildingID;
                    result.Add(hisPos);
                }
                catch (Exception)
                {
                }                   
            }
            return result;
        }

        public void RunWorkflowWithEvent()
        {                       
            TabPage page = m_pageHome.tabControl.SelectedTab;
            if (page == null)
            {
                return;
            }
            if (bStartWorkflowEvent == true)
            {
                return;
            }
            bStartWorkflowEvent = true;

            Sections.SectionTabPage tabPage = (Sections.SectionTabPage)page;
            int nActionStepID = GetTabActionStepID(tabPage);
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(nActionStepID);
            string szName = node.FullPath;
            bool bHasPos = true;
            if (szName.IndexOf("자연재해") != -1)
            {
                bHasPos = false;
            }

            string sopName = szName.Substring(szName.IndexOf("\\") + 1);
            string disasterName = szName.Substring(0, szName.IndexOf("\\"));
            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

            if (m_smsExternalCompanyMemberOn)
            {
                // 협력업체 직원들의 전화번호 추가(시작 및 종료시에만...)
                AddExternalCompanyMemberPhoneNumbers(arrListCall);
            }

            Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess();
            start.VirtualMode = !FormMain.Instance.IsReal;
            start.ActionStepID = nActionStepID;
            start.HasPosition = bHasPos;
            start.SOPName = sopName;
            start.CallList = arrListCall;
           
            start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);

            Popup.PopupStartEvent form = start.Popup;
            form.DisasterName = disasterName;

            ArrayList arPosList = LoadHistoryDisasterPosition();
            form.SetRecentPosition(arPosList);
           
            if (m_frmMain2.LayoutForm != null)
            {
                m_frmMain2.LayoutForm.SetCheckPoistion(form, true);
            }
            Process.ProcessManager.Instance.AddFirst(start);

            
        }

        private void AddExternalCompanyMemberPhoneNumbers(ArrayList arrListCall)
        {
            foreach (ExternalCompanyMember member in FormMain.Instance.SOPManager.ExternalCompanyMembers)
            {
                arrListCall.Add(member.PhoneNumber);
            }
        }

        public void DoneWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;

            int ActionID = page.ActionStepID;
            if (Sections.WorkFlowManager.Instance.DeleteComplete == true)
            {
                Sections.TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
            }

            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            
            if (this.HasControl == true)
                if (m_smsExternalCompanyMemberOn)
                {
                    // 협력업체 직원들의 전화번호 추가(시작 및 종료시에만...)
                    AddExternalCompanyMemberPhoneNumbers(arrListCall);
                }

            Process.WorkflowEndNotifyProcess endEvent = new Process.WorkflowEndNotifyProcess();
            endEvent.VirtualMode = (work.RunMode == WorkFlowMode.VIRTUAL);
            endEvent.HasPosition = work.HasPosition;
            endEvent.SOPName = work.SOPName;
            endEvent.CallList = arrListCall;

            if (this.HasControl == true)
                endEvent.UseSMS = work.BeginEndEventSendSMS;
            else
                endEvent.UseSMS = false;

            if (work.HasPosition == true)
            {
                if (work.LastPosition != null)
                    endEvent.PositionName = work.LastPosition.PoistionName;
                else
                    endEvent.PositionName = "";
            }
            
            if (work != null && work.HasPosition == true)
            {
                if (work.LastPosition != null)
                {
                    m_frmMain2.LayoutForm.LastPos = work.LastPosition;
                    m_frmMain2.LayoutForm.RemoveDisasterPos();
                    work.LastPosition = null;
                    m_frmMain2.LayoutForm.LastPos = null;
                }
            }

            if (this.HasControl == true)
            {
                Process.ProcessManager.Instance.AddFirst(endEvent);
            } 
            History.HistoryManager.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);

            if (HasControl == true)
                WriteCurrentActionStepID(-1, false);

            m_frmStatus.StatusBoard(work.State);
            SetCurrentWorkflow(work);
            EnabledRunGroup();
        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite = false)
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;

            StopWorkflow(dtStop, noDBWrite, page);
        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode)
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();

            foreach (Sections.SectionTabPage page in pageHome.TabControls.Controls)
            {
                if (page.ActionStepID == nActionStepID && !page.VirtualMode == isRealMode)
                {
                    StopWorkflow(dtStop, noDBWrite, page);
                    return;
                }
            }
        }

        private void StopWorkflow(DateTime dtStop, bool noDBWrite, Sections.SectionTabPage page)
        {
            int ActionID = page.ActionStepID;
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            if (work == null)
                return;

            manager.Remove(ActionID, !page.VirtualMode);

            Sections.TabPageManager.Instance.SetUsePage(ActionID, false, !page.VirtualMode);

            

            if (Sections.WorkFlowManager.Instance.DeleteComplete == true)
            {
                Sections.TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
            }          

            if (work != null && work.State == Sections.WorkFlowState.RUN)
            {                
                work.Stop(dtStop, noDBWrite);

                if (work.HasPosition == true)
                {
                    if (work.LastPosition != null)
                    {
                        m_frmMain2.LayoutForm.LastPos = work.LastPosition;
                        m_frmMain2.LayoutForm.RemoveDisasterPos();
                        work.LastPosition = null;
                        m_frmMain2.LayoutForm.LastPos = null;
                    }
                }
            }

            History.HistoryManager.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);

            if( HasControl == true)
                WriteCurrentActionStepID(-1, false);

            if (work == null)
                return;



            m_frmStatus.StatusBoard(Sections.WorkFlowState.STOP);
            SetCurrentWorkflow(null);
            EnabledRunGroup();
        }

        public void WaitWorkflow()
        {
            m_frmStatus.StatusBoard(Sections.WorkFlowState.STANDBY);
        }

        public void ChangeWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = FormMain.Instance.GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;
            if(page == null)
                return;
            if (page == null)
                return;

            int ActionID = page.ActionStepID;
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            if (work != null )
            {
                m_frmStatus.StatusBoard(work.State);
                SetCurrentWorkflow(work);                
            }
            else
            {
                m_frmStatus.StatusBoard(Sections.WorkFlowState.STANDBY);
            }
        }

        public void AllStopWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;

            DataGridView gridView = GetPageHome().GetDockScenario().GetGridView();
            foreach (DataGridViewRow row in gridView.Rows)
            {
                int nActionID = (int)row.Cells[3].Tag;

                bool isVirtual = (bool)row.Cells[0].Tag;
                Sections.TabPageManager.Instance.SetUsePage(nActionID, false, isVirtual);
                Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(nActionID, isVirtual);
				if (work != null)
				{
					work.Stop(DateTime.Now);

					if (work.HasPosition == true)
					{
						if (work.LastPosition != null)
						{
							m_frmMain2.LayoutForm.LastPos = work.LastPosition;
							m_frmMain2.LayoutForm.RemoveDisasterPos();
							work.LastPosition = null;
							m_frmMain2.LayoutForm.LastPos = null;
						}
					}
					m_frmStatus.StatusBoard(work.State);
				}
				else
				{
					m_frmStatus.StatusBoard(Sections.WorkFlowState.WAIT);					
				}
				SetCurrentWorkflow(work);
                EnabledRunGroup();
            }

            History.HistoryManager.Instance.HistoryDisasterPosition.Clear();
        }

        public Sections.WorkFlowState CheckWorkflow(int nActionID, bool isVirtual)
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(nActionID, !isVirtual);

            if (work == null)
                return Sections.WorkFlowState.STANDBY;
            
            return work.State;
        }
        // WORKFLOW - end
        //////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////
        // TTS 
        public void StopSpeech()
        {
            TTSManager.Instance.StopSpeech();
        }
        public void PauseSpeech()
        {
            TTSManager.Instance.PauseSpeech();
        }
        public void ResumeSpeech()
        {
            TTSManager.Instance.ResumeSpeech();
        }
        // TTS 
        //////////////////////////////////////////////////////////////////////////
        
        private void axCommandBars_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            switch (e.control.Id)
            {
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCUSTOMIZE:
                    axCommandBars.ShowCustomizeDialog(3);
                    break;
                case ID.ID_APP_ABOUT:
                    axCommandBars.AboutBox();
                    break;
                case ID.ID_THEME_OFFICE2003:
                    axCommandBars.VisualTheme = XtremeCommandBars.XTPVisualTheme.xtpThemeOffice2003;
                    axCommandBars.ToolTipContext.Style = XtremeCommandBars.XTPToolTipStyle.xtpToolTipLuna;
                    break;
                case ID.ID_THEME_OFFICE2007:
                    axCommandBars.VisualTheme = XtremeCommandBars.XTPVisualTheme.xtpThemeResource;
                    axCommandBars.ToolTipContext.Style = XtremeCommandBars.XTPToolTipStyle.xtpToolTipResource;
                    break;
                case ID.ID_FILE_OPTIONS:
                    break;
                case ID.ID_APP_EXIT:
                    this.Close();
                    break;
                case ID.ID_RUN_FRONT:
                    break;
                case ID.ID_CONTROL_CONTROL:
                    break;
                case ID.ID_CONTROL_RETURN: // 제어권 반납, 모니터링으로 변경

                    ctrlControlReturn.Visible = false;
                    ctrlControlRequest.Visible = true;
                    SetControl(false);
                    ControlUse(ID.ID_CONTROL_RETURN);

                    break;
                case ID.ID_CONTROL_MONITORING:
                    break;
                case ID.ID_CONTROL_REQUEST: //제어권 요청
                    ControlUse(ID.ID_CONTROL_REQUEST);
                    break;
                case ID.ID_RUN_PLAY:
                    if( HasControl == true)
                        Play();
                    break;
                case ID.ID_RUN_CANCEL:
                    if( HasControl == true)
                        StopWorkflow(DateTime.Now);
                    break;
                case ID.ID_RUN_COMPLETE:
                    {
                        TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().GetSelectedNode();
                        if (node == null) return;
                        m_pageHome.GetDockScenario().DeleteGridRowScenario(node.FullPath.Replace("\\", "/"));                        
                        DoneWorkflow();
                    }
                    break;
                case ID.ID_RUN_REPLAY:
                    break;
                case ID.ID_RUN_REAL:
                    ctrlReal.Checked = true;
                    ctrlVirtual.Checked = false;
                    ctrlReg.Checked = true;
                    ctrlNonReg.Checked = false;
                    ctrlNonReg.Enabled = false;
                    GetPageHome().GetDockScenario().GetBarLevelTree().SelectSop(null);

                    m_frmStatus.RealMode(ctrlReal.Checked);
                    ChangeMode();
                    break;
                case ID.ID_RUN_VIRTUAL:
                    ctrlReal.Checked = false;
                    ctrlVirtual.Checked = true;
                    ctrlNonReg.Enabled = true;
                    GetPageHome().GetDockScenario().GetBarLevelTree().SelectSop(null);

                    m_frmStatus.RealMode(ctrlReal.Checked);
                    break;
                case ID.ID_MODE_REGISTER:       // 등록 모드
                    ctrlReg.Checked = true;
                    ctrlNonReg.Checked = false;
                    ChangeMode();
                    break;
                case ID.ID_MODE_NONREGISTER:    // 미등록 모드
                    ctrlReg.Checked = false;
                    ctrlNonReg.Checked = true;
                    ChangeMode();
                    break;
                case ID.ID_MODE_WEEKDAY:        // 평일 모드
                    ctrlWeekday.Checked = true;
                    ctrlWeekend.Checked = false;
                    ChangeMode();
                    break;
                case ID.ID_MODE_WEEKEND:        // 주말 모드
                    ctrlWeekday.Checked = false;
                    ctrlWeekend.Checked = true;
                    ChangeMode();
                    break;
                case ID.ID_ANNOUNCE_PLAY:
                    ResumeSpeech();
                    break;
                case ID.ID_ANNOUNCE_PAUSE:
                    PauseSpeech();
                    break;
                case ID.ID_ANNOUNCE_STOP:
                    StopSpeech();
                    break;
                case ID.ID_VIEW_CURRENT: // zoomsection
                    ZoomCurrent();
                    break;
                case ID.ID_VIEW_SCALETOFIT:
                    ZoomScaletoFit();
                    break;
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCONTROLTAB:
                    System.Diagnostics.Debug.WriteLine("Selected Tab has Changed");
                    break;
                case ID.ID_RIBBON_EXPAND:
                    RibbonBar().Minimized = !RibbonBar().Minimized;
                    break;
                case ID.ID_RIBBON_MINIMIZE:
                    RibbonBar().Minimized = !RibbonBar().Minimized;
                    break;
                default:
                    MessageBox.Show(e.control.Caption + " clicked", "Button Clicked");
                    break;
            };
        }

        public bool Play()
        {
            if (ctrlPlay.Enabled == false)
                return false;

            GetPageHome().ClearProcess();

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || tapPage.GetType() != typeof(Sections.SectionTabPage))
                return false;
            Sections.SectionTabPage page = (Sections.SectionTabPage)tapPage;
            if (page == null)
                return false;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;

            RunWorkflowWithEvent();

            return true;
        }

		public bool PlayWithDisasterPosition(int nZoneID, int nSensorID)
		{
			if (ctrlPlay.Enabled == false)
                return false;

            GetPageHome().ClearProcess();

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || tapPage.GetType() != typeof(Sections.SectionTabPage))
                return false;
            Sections.SectionTabPage page = (Sections.SectionTabPage)tapPage;
            if (page == null)
                return false;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;

			RunWorkflowWithoutEvent(nZoneID, nSensorID);

            return true;
		}

		public void RunWorkflowWithoutEvent(int nZoneID, int nSensorID)
		{
			TabPage page = m_pageHome.tabControl.SelectedTab;
			if (page == null)
			{
				return;
			}

			Sections.SectionTabPage tabPage = (Sections.SectionTabPage)page;
			int nActionStepID = GetTabActionStepID(tabPage);
			BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
			TreeNode node = tree.FindActionStepNode(nActionStepID);
			string szName = node.FullPath;
			bool bHasPos = true;
			if (szName.IndexOf("자연재해") != -1)
			{
				bHasPos = false;
			}

			string sopName = szName.Substring(szName.IndexOf("\\") + 1);
			string disasterName = szName.Substring(0, szName.IndexOf("\\"));
			ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

			if (m_smsExternalCompanyMemberOn)
			{
				// 협력업체 직원들의 전화번호 추가(시작 및 종료시에만...)
				AddExternalCompanyMemberPhoneNumbers(arrListCall);
			}

			Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess();
			start.VirtualMode = !FormMain.Instance.IsReal;
			start.ActionStepID = nActionStepID;
			start.HasPosition = bHasPos;
			start.SOPName = sopName;
			start.CallList = arrListCall;
			start.NoPopup = true;
			start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);
			
			SOPDisasterSystem.Zone zone = SOPDisasterSystem.DataManager.Instance.GetZone(nZoneID);
			HistoryDiasterPosition disasterPos = new SOPMonitoringSystem.HistoryDiasterPosition();
			disasterPos.PoistionName = zone.BroadcastName;

			start.PositionName = zone.BroadcastName;

			Vertex2D pos3D = zone.Polygon.CalcWeightCenter();
			disasterPos.X = (float)pos3D.x;
			disasterPos.Y = 0.0f;
			disasterPos.Z = (float)pos3D.y;
			if (zone.IsOutdoor == true)
				disasterPos.FloorIndex = -999.0f;
			else
				disasterPos.FloorIndex = zone.Floor.FloorIndex;
			if (zone.Building != null)
				disasterPos.BuildingID = zone.Building.BuildingID;
			else
				disasterPos.BuildingID = "ZONE";
			disasterPos.DisasterName = "화재";
			Popup.PopupStartEvent form = start.Popup;
			form.DisasterName = disasterName;
			form.AddLastHistoryDisasterPoistion(disasterPos);

			if (m_frmMain2.LayoutForm != null)
			{
				m_frmMain2.LayoutForm.SetCheckPoistion(form, true);
			}
			Process.ProcessManager.Instance.AddFirst(start);
		}

        public void ChangeMode(bool isReal, bool isRegular, bool isNormal)
        {
            if (ctrlReal.Checked == isReal &&
                ctrlReg.Checked == isRegular &&
                ctrlWeekday.Checked == isNormal)
                return;

            ctrlReal.Checked = isReal;
            ctrlVirtual.Checked = !isReal;

            ctrlReg.Checked = isRegular;
            ctrlNonReg.Checked = !isRegular;

            ctrlWeekday.Checked = isNormal;
            ctrlWeekend.Checked = !isNormal;

            ChangeMode();
        }

        private void InitCompleteCount(Sections.SectionTabPage tabPage)
        {
            Type type = typeof(Sections.PanelSectionEx);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;
                    InitSectionCompleteCount(panel);
                }
            }
        }

        private void InitSectionCompleteCount(Sections.PanelSectionEx panel)
        {
            foreach (Sections.Section section in panel.Sections)
            {
                section.CompleteCount = 0;
            }
        }

        private void ChangeMode()
        {
            if (!m_sopMgr.IsOpened)
                return;

            bool isRegular = ctrlReg.Checked;
            bool isNormal = ctrlWeekday.Checked;

            BarLevelTree tree = m_pageHome.GetDockScenario().GetBarLevelTree();

            if (tree.IsRegular != isRegular || tree.IsNormal != isNormal)
                tree.Load(m_sopMgr, ctrlReg.Checked, ctrlWeekday.Checked);

            m_pageOption.mPreviewBox.Refresh();
        }

        private void axCommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {
            bool bPlay = false;
            bool bPause = false;
            //bool bStop = false;
            switch (e.control.Id)
            {
                case ID.ID_THEME_OFFICE2003:
                    e.control.Checked = axCommandBars.VisualTheme == XtremeCommandBars.XTPVisualTheme.xtpThemeOffice2003;
                    break;
                case ID.ID_THEME_OFFICE2007:
                    e.control.Checked = axCommandBars.VisualTheme == XtremeCommandBars.XTPVisualTheme.xtpThemeResource;
                    break;
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCONTROLTAB:
                    break;
                case ID.ID_RIBBON_EXPAND:
                    e.control.Visible = RibbonBar().Minimized;
                    break;
                case ID.ID_RIBBON_MINIMIZE:
                    e.control.Visible = !RibbonBar().Minimized;
                    break;

                //////////////////////////////////////////////////////////////////////////
                // SPEECH
                case ID.ID_ANNOUNCE_PLAY:
                    bPause = (TTSManager.Instance.State == SpeechState.PAUSE ? true : false);
                    e.control.Enabled = bPause;
                    break;
                case ID.ID_ANNOUNCE_PAUSE:
                    bPlay = (TTSManager.Instance.State == SpeechState.PLAY ? true : false);
                    bPause = (TTSManager.Instance.State == SpeechState.PAUSE ? true : false);
                    e.control.Enabled = (bPlay || bPause);
                    e.control.Checked = bPause;
                    break;
                case ID.ID_ANNOUNCE_STOP:
                    bPlay = (TTSManager.Instance.State == SpeechState.PLAY ? true : false);
                    bPause = (TTSManager.Instance.State == SpeechState.PAUSE ? true : false);
                    e.control.Enabled = (bPlay || bPause);                    
                    break;
                case ID.ID_ANNOUNCE_COUNT:
                        e.control.Caption = "-";
                    break;
            };
        }

        private void axCommandBars_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axCommandBars.GetClientRect(out left, out top, out right, out bottom);
            panelMain.SetBounds(left, top, right - left, bottom - top);

            if (m_frmStatus != null)
            {
                if (top > 25)
                    m_frmStatus.Visible = true;
                else
                    m_frmStatus.Visible = false;
            }

            if (m_pageHome != null)
            {
                foreach (SectionTabPage page in m_pageHome.TabControls.Controls)
                {
                    page.ReSizePanel();
                    GetPageHome().changeLocation(page.Height);
                }
            }
        }

        public void saveCSV(string str)
        {
            StreamWriter sw = new StreamWriter("RunningingSOP.csv", false, Encoding.Unicode);
            sw.WriteLine(str);
            sw.Close();
        }
        //////////////////////////////////////////////////////////////////////////
        public PageBackstageHome GetPageHome()
        {
            return m_pageHome;
        }

        public PageBackstageOption GetPageOption()
        {
            return m_pageOption;
        }

        public PopupProgressReport GetReport()
        {
            return m_frmReport;
        }

        public FormRealTimeInfo GetRealTime()
        {
            return m_frmReal;
        }

        public void InitReport()
        {
            m_frmReport = null;
        }

        public void ChangeMode(VersionInfo vInfo, ActionStepInfo aInfo, bool isRealMode)
        {
            if (vInfo.IsRegular == ctrlReg.Checked)
            {
                if (vInfo.IsNormal == ctrlWeekday.Checked)
                    return;
            }

            if (vInfo.IsRegular == true)
            {
                ctrlReg.Checked = true;
                ctrlNonReg.Checked = false;
            }
            else
            {
                ctrlReg.Checked = false;
                ctrlNonReg.Checked = true;
            }

            if (vInfo.IsNormal == true)
            {
                ctrlWeekday.Checked = true;
                ctrlWeekend.Checked = false;
            }
            else
            {
                ctrlWeekday.Checked = false;
                ctrlWeekend.Checked = true;
            }
            ChangeMode();
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(aInfo.ActionStepID);
            if (node != null)
            {
                TreeNode selectedNode = tree.GetSelectedNode();

                if (selectedNode != node)
                {
                    tree.SelectSop(node);

                    if (PageBackstageHome.IsWorkingMode(aInfo.ActionStepID, isRealMode))
                    {
                        // 현재 화면에 나타나고 있는 ActionStep을 기록한다.
                        WriteCurrentActionStepID(aInfo.ActionStepID, isRealMode);
                    }
                }
            }
        }

        public void WriteCurrentActionStepID(int nActionStepID, bool isRealMode)
        {
            if (!HasControl)
                return;

            SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);

            string strSQL = string.Format("Update CurrentActionStep set ActionStepID = {0}, RealMode = {1} where id = 1", nActionStepID, isRealMode ? 1 : 0);
            DBManager.GetResultData(strSQL, 0);
        }

        public void ChangeSOP(VersionInfo vInfo, ActionStepInfo aInfo, bool isRealMode)
        {
            ChangeMode(vInfo, aInfo, isRealMode);

            Sections.SectionTabPage page = (Sections.SectionTabPage)m_pageHome.TabControls.SelectedTab;
            if (page == null)
                return;
            TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return;
            RunWorkflow();
            m_pageHome.GetDockScenario().AddGridRowScenario(node.FullPath.Replace("\\", "/"), page.ActionStepID, !page.VirtualMode);
        }

        public void VirtualMode(bool bRun)
        {
            if (bRun == false)
            {
                ctrlReal.Checked = true;
                ctrlVirtual.Checked = false;
                if (m_frmStatus != null)
                {
                    m_frmStatus.RealMode(true);
                }
            }
            else
            {
                ctrlReal.Checked = false;
                ctrlVirtual.Checked = true;
                if (m_frmStatus != null)
                {
                    m_frmStatus.RealMode(false);
                }
            }
        }

        public void EnabledRunGroup()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;
			if (page != null)
			{
				int ActionID = page.ActionStepID;
				if (Sections.WorkFlowManager.Instance.DeleteComplete == true)
				{
					Sections.TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
				}
				Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);

				if (work != null)
				{
					switch (work.State)
					{
						case Sections.WorkFlowState.RUN: //시작
							ctrlPlay.Enabled = false;
							ctrlCancel.Enabled = true;
							//ctrlComplete.Enabled = true;
							break;
						case Sections.WorkFlowState.STOP: //실행취소
							ctrlPlay.Enabled = true;
							ctrlCancel.Enabled = false;
							//ctrlComplete.Enabled = false;
							break;
						case Sections.WorkFlowState.DONE: //완료
							ctrlPlay.Enabled = true;
							ctrlCancel.Enabled = false;
							//ctrlComplete.Enabled = false;
							break;
						case Sections.WorkFlowState.STANDBY: //대기
						case Sections.WorkFlowState.PAUSE:
						case Sections.WorkFlowState.WAIT:
						case Sections.WorkFlowState.DISABLE:
							break;
					}
				}
				else
				{
					ctrlPlay.Enabled = true;
					ctrlCancel.Enabled = false;
				}
			}
           
        }
        //////////////////////////////////////////////////////////////////////////
        public SOPManager SOPManager
        {
            get { return m_sopMgr; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        // 실제 모드인가?
        public bool IsReal
        {
            get { return ctrlReal.Checked; }
        }

        // 등록 버전인가?
        public bool IsRegular
        {
            get { return ctrlReg.Checked; }
        }

        // 평일 버전인가?
        public bool IsNormal
        {
            get { return ctrlWeekday.Checked; }
        }

        //////////////////////////////////////////////////////////////////////////
        public string GetAppVersion()
        {
            if (!m_isReadVersion)
                ReadAppVersion();

            return m_strVersion;
        }

        private void ReadAppVersion()
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader("svnMonitorInfo.txt", Encoding.Default);

                string strLine = reader.ReadLine();

                if (strLine != null)
                {
                    int nLen = strLine.Length;
                    int nFirstIndex = -1, nSecondIndex = -1;

                    for (int i = 0; i < nLen; i++)
                    {
                        char ch = strLine.ElementAt(i);

                        if (ch < '0' || ch > '9')
                        {
                            if (nFirstIndex < 0)
                                nFirstIndex = i;
                            else
                            {
                                nSecondIndex = i;
                                break;
                            }
                        }
                    }

                    if (nFirstIndex < 0)
                    {
                        m_strVersion += "." + strLine;
                    }
                    else if (nSecondIndex < 0)
                    {
                        m_strVersion += "." + strLine.Substring(0, nFirstIndex);
                    }
                    else
                    {
                        m_strVersion += "." + strLine.Substring(nFirstIndex + 1, nSecondIndex - nFirstIndex - 1);
                    }
                }

                reader.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
            }

            m_isReadVersion = true;
        }


        public ArrayList GetAllMemberPhoneNumber()
        {
            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();
            return arrListCall;
        }

        public string GetActionStepPath(int actionStepID)
        {
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(actionStepID);
            if (node == null)
                return "";

            string szName = node.FullPath.Replace('\\', '/');
            return szName;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 방송과 SMS전송 옵션 저장
            if( m_smsOn == true)
                DBManager.SaveIni("sms_on","1","Server Connection Info");
            else
                DBManager.SaveIni("sms_on","0","Server Connection Info");
          
            if( m_useBroadcast == true)
                DBManager.SaveIni("broadcast_on", "1", "Server Connection Info");
            else
                DBManager.SaveIni("broadcast_on", "0", "Server Connection Info");

            StopWriteDB();

            FormMain.Instance.CloseThread = true;

            Process.ProcessManager.Instance.Dispose();
            History.HistoryManager.Instance.Dispose();
            Process.TTSManager.Instance.Dispose();

            if (m_pageOption != null)
                m_pageOption.Dispose();
            if( m_pageHome != null)
                m_pageHome.Dispose();

            this.axCommandBars.Execute -= this.axCommandBars_Execute;
            this.axCommandBars.UpdateEvent -= this.axCommandBars_UpdateEvent;
            this.axCommandBars.ResizeEvent -= this.axCommandBars_ResizeEvent;

            axCommandBars.Controls.Clear();
            axSkinFramework.RemoveWindow(Handle.ToInt32());
            this.Controls.Remove(axCommandBars);
            //Marshal.FinalReleaseComObject(axCommandBars);
            //Marshal.FinalReleaseComObject(axSkinFramework);
                     

            Thread.Sleep(200);
        }

        public ArrayList GetLevelMember(int nTeamID)
        {
            ArrayList arrSOPMember = new ArrayList();
            string strSQL = "select ID, MemberName, LevelID from CompanyMember where LevelID in (select id from JobLevel where LevelNo = " + nTeamID.ToString() + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                Sections.SOPMember member = new Sections.SOPMember();
                member.MemberID = nID;
                member.MemberName = strMemberName;
                member.LevelID = nLevelID;

                arrSOPMember.Add(member);
            }
            return arrSOPMember;
        }
       
        public void DoorBellCheck(bool is_checked)
        {
            DoorBellChecked = is_checked;
        }

        public void PlayDoorBell()
        {
            if (DoorBellChecked)
            {
                m_player.Open(m_dbMgr.DoorBellPath, this);
                m_player.Play(false, this, instanceHandle);
            }
        }

        public void GetRealTimeInfo(string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, FormRealTimeInfo.MessageType type)
        {
            int nType = 0;
            nType = GetDisasterType();
            
            string strReport = "";
            if (strComponentType == "프로세스")
            {
                strReport = strTeamList + strTask + " " + strStatus;
            }
            else
            {
                strReport = strStepMemberName + strTask + " " + strStatus;
            }

            SOPLog log = FormMain.Instance.LogFile;
            log.Write(string.Format("In GetRealTimeInfo, strReport : {0}\n", strReport));

            m_frmReal.RealTimeInfo = strReport;
            m_frmReal.SetForeColor(type);
            m_frmReal.DrawMovingText();
            
        }

        private int GetDisasterType()
        {
            string strTitle = FormMain.Instance.GetPageHome().GetDockPropertiesLevel().GetTitle();
            string[] strDisaster = strTitle.Split('/');

            int nType = 0;

            if (strDisaster[0] == "자연재해")
                nType = 0;
            else if (strDisaster[0] == "화재")
                nType = 1;
            else if (strDisaster[0] == "유출사고")
                nType = 2;
            else if (strDisaster[0] == "테러")
                nType = 3;
            else if (strDisaster[0] == "인명구조 및 의료지원")
                nType = 4;
            else
                nType = 5;

            return nType;
        }
        
        private void ZoomCurrent()
        {
            if (m_currentSection == null)
                return;
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_currentSection.GetParent();
            if (panel != null)
            {
                panel.ZoomSection(m_currentSection);                
            }
        }

        public void FocusSection(Sections.Section section)
        {
            if (section == null) return;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();

            if (panel != null)
            {
                panel.FocusSection(section);
            }
        }

        private void ZoomScaletoFit()
        {
            ArrayList arrPanels = this.GetPageHome().GetPanels();
            if (arrPanels == null || arrPanels.Count == 0)
                return;

            if (arrPanels.Count == 1)
            {
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)arrPanels[0];
                panel.ZoomPanel();
                return;
            }

            Sections.PanelSectionEx panelCurrent = this.GetPageHome().GetCurrentPanel();
            panelCurrent.ZoomPanel();
        }

        private void OnEnabled(int nID)
        {
            switch(nID)
            {
                case ID.ID_CONTROL_REQUEST:
                    CommandBarControlEnabled(true);
                    EnabledRunGroup();
                    GetPageHome().GetDockScenario().Enabled = true;
                    GetPageHome().OnEnabled(true);
                    m_frmMain2.GetSpace().OnEnabled(true);
                    m_frmMain2.GetToolbar().Enabled = true;
                    break;

                case ID.ID_CONTROL_RETURN:
                    CommandBarControlEnabled(false);
                    GetPageHome().GetDockScenario().Enabled = false;
                    GetPageHome().OnEnabled(false);
                    m_frmMain2.GetSpace().OnEnabled(false);
                    m_frmMain2.GetToolbar().Enabled = false;
                    break;
            }
        }

        private void CommandBarControlEnabled(bool isFlag)
        {
            ctrlPlay.Enabled = isFlag;
            ctrlCancel.Enabled = isFlag;
            ctrlReal.Enabled = isFlag;
            ctrlVirtual.Enabled = isFlag;
            ctrlReg.Enabled = isFlag;
            ctrlNonReg.Enabled = isFlag;
            ctrlWeekday.Enabled = isFlag;
            ctrlWeekend.Enabled = isFlag;
            ctrPlayAnn.Enabled = isFlag;
            ctrPauseAnn.Enabled = isFlag;
            ctrStopAnn.Enabled = isFlag;
            ctrCount.Enabled = isFlag;
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
        }

        public string SOPGenUserRealName
        {
            get { return m_strSOPGenUserRealName; }
        }

        // 제어권을 요청한 사용자를 얻음
        public ArrayList GetRequestControl()
        {
            string strSQL = "select ControlCheck.id, ControlCheck.userid, ControlCheck.time, ControlCheck.controlcheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel " +
                            "from ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.controlcheck = 1 AND ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID AND SOPGenUser.UserLevel = SOPGenLevel.ID " +
                            "order by SOPGenUser.UserLevel desc";

            ArrayList arrSendControl = m_dbMgr.GetResultData(strSQL, 0);
            if (arrSendControl == null)
                return null;

            int nResultCount = arrSendControl.Count;
            DateTime dtDefult = new DateTime();
            ArrayList arrRequest = new ArrayList();

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                ControlCheck data = new ControlCheck();
                data.ID = WebDBManager.GetIntField(arrSendControl[i].ToString(), -1);
                data.UserID = WebDBManager.GetIntField(arrSendControl[i + 1].ToString(), -1);
                data.Time = WebDBManager.GetDateTimeField(arrSendControl[i + 2].ToString(), dtDefult);
                data.ControlChecked = WebDBManager.GetIntField(arrSendControl[i + 3].ToString(), -1);
                data.MemberName = WebDBManager.GetStringField(arrSendControl[i + 4].ToString(), "");
                data.MemberID = WebDBManager.GetStringField(arrSendControl[i + 5].ToString(), "");
                data.UserLevel = WebDBManager.GetIntField(arrSendControl[i + 6].ToString(), -1);

                arrRequest.Add(data);
            }
            return arrRequest;
        }

        private void UpdateUserInfo(int isControl)
        {
            if (m_arrConnectedUser.Count == 0)
            {
                string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where UserID = {1}", isControl, m_nSOPGenUserID);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                strSQL = string.Format("update ControlUser set UserID = null where UserID = {0}", m_nSOPGenUserID);
                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }
            else
            {
                foreach (UserInfo data in m_arrConnectedUser)
                {
                    if (data.UserID == m_nSOPGenUserID)
                    {
                        string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where id = {1}", isControl, data.ID);

                        if (m_dbMgr.GetResultData(strSQL, 0) == null)
                            return;
                        break;
                    }
                }
            }
        }

        private void WriteUserInfo(bool initControlCheck = false)
        {
            string strSQL = "";
            bool isCheck = false;

            if (m_isStop == true)
                return;

            ArrayList arrConnectedUser = LoadConnectedUser();
            
            Thread.Sleep(500);
            if (arrConnectedUser == null)
                return;

            foreach (UserInfo data in arrConnectedUser)
            {
                if (data.UserID == m_nSOPGenUserID)
                {
                    isCheck = true;

                    DateTime time = DateTime.Now;
                    string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", time.ToShortDateString(), time.Hour, time.Minute, time.Second);

                    if (initControlCheck)
                        strSQL = string.Format("update ControlCheck set Time = {0}, ControlCheck = -1 where id = {1}", strTime, data.ID);
                    else
                        strSQL = string.Format("update ControlCheck set Time = {0} where id = {1}", strTime, data.ID);
                    break;
                }
            }

            if (!isCheck)
            {
                isCheck = false;
                strSQL = "select Max(id) from ControlCheck";
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                if (nID < 0) nID = 0;

                DateTime time = DateTime.Now;
                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", time.ToShortDateString(), time.Hour, time.Minute, time.Second);

                strSQL = string.Format("insert into ControlCheck (id, UserID, Time, ControlCheck) values ({0}, {1}, {2}, {3})",
                                ++nID, m_nSOPGenUserID, strTime, -1);
            }

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;

            ArrayList arrRequest = GetRequestControl();
            if (arrRequest == null) return;

        }

        // 제어권을 갖고 있는 User
        private int LoadControlUser()
        {
            string strSQL = "SELECT ControlUser.UserID FROM ControlUser, ControlCheck Where ControlUser.UserID = ControlCheck.UserID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nControlUser = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nControlUser;
        }

        // 현재 접속중인 User 현황 - 접속 중인 User는 주기적으로 접속시간이 업데이트 됨.
        private ArrayList LoadConnectedUser()
        {
            ArrayList arrConnectedUser = new ArrayList();

            string strSQL = "SELECT  ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, " +
                            "CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return null;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                UserInfo data = new UserInfo();

                data.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                data.UserID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                data.Time = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
                data.ControlChecked = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                data.MemberName = WebDBManager.GetStringField(arrResult[i + 4], "");
                data.MemberID = WebDBManager.GetStringField(arrResult[i + 5], "");
                data.UserLevel = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                data.LevelName = WebDBManager.GetStringField(arrResult[i + 7], "");

                arrConnectedUser.Add(data);
            }

            return arrConnectedUser;
        }

        private void SendMessagetoContolUser(int nControlUser)
        {
            //제어권을 갖고있는 사용자이면 제어권 요청한 사용자의 목록을 출력
            if (nControlUser == m_nSOPGenUserID)
            {
                m_frmRequestControl = new PopupRequestControl(m_nSOPGenUserID, m_nSOPGenUserLevel);

                int nStrongUserID = m_frmRequestControl.StrongUserID;

                // 제어권 요청을 한 User 가운데 현재 제어권 가진 User보다 높은 레벨의 User가 존재하는가?
                if (nStrongUserID > 0)
                {
                    m_nChangeUserID = nStrongUserID;
                    if (m_frmRequestProgress != null && m_frmRequestProgress.Visible)
                        m_frmRequestProgress.Close();
                    ChangeControlUser(true);

                }
                else
                {
                    if (m_frmRequestControl.HasData())
                    {
                        m_frmRequestControl.Focus();

                        if (m_frmRequestControl.ShowDialog() == DialogResult.OK)
                        {
                            //제어요청 허락
                            ChangeControlUser(true);
                        }
                        else
                        {
                            //제어요청 거부
                            ChangeControlUser(false);
                        }
                    }
                }

                m_frmRequestControl = null;
            }
        }
        
        // 제어자가 제어요청을 받음
        private void ChangeControlUser(bool isContol)
        {
            if (!HasControl)
                return;

            if (isContol) //제어요청 허락
            {
                string strSQL = string.Format("update ControlUser set UserID = NULL");

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;             
            }
            else //제어요청 거부
            {
                ArrayList arrConnecedtUser = LoadConnectedUser();
                foreach (UserInfo data in arrConnecedtUser)
                {
                    if (data.ControlChecked == 1)
                    {
                        string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where UserID = {1}", -1, data.UserID);
                        if (m_dbMgr.GetResultData(strSQL, 0) == null)
                            return;
                    }
                }
            }
        }

        private void RejectRequest()
        {
            if (m_frmRequestProgress != null && m_frmRequestProgress.Visible == true)
            {
                m_frmRequestProgress.Invoke((MethodInvoker)delegate
                {
                    m_frmRequestProgress.SetMessage("거부되었습니다.");
                }); 
            }                       
        }

        public void ForceControl()
        {
            string strSQL = string.Format("update ControlUser set UserID = {0}", m_nSOPGenUserID);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;

            strSQL = string.Format("update ControlCheck set ControlCheck = -1 where UserID = {0}", m_nSOPGenUserID);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;
        }

        public void ControlUse(int nCtrlID)
        {
            // ControlCheck DB 테이블의 ControlCheck 컬럼에 update 
            // 제어권 필요없음: 0 필요함: -1 요청: 1
            switch (nCtrlID)
            {
            case ID.ID_CONTROL_RETURN:
                SetControl(false);
                OnEnabled(ID.ID_CONTROL_RETURN);
                UpdateUserInfo(0);  //제어권 반납
                break;
            case ID.ID_CONTROL_REQUEST:

                //제어권 요청
                m_bRequestControl = true;
                m_frmRequestProgress = new PopupRequestProgress();
                m_frmRequestProgress.StartPosition = FormStartPosition.Manual;
                Point p = GetMonitoringPosition();
                p.X += 100;
                p.Y += 100;
                int width = m_frmRequestProgress.Bounds.Width;
                int height = m_frmRequestProgress.Bounds.Height;
                m_frmRequestProgress.SetBounds(p.X, p.Y, width, height);
                UpdateUserInfo(1);
                m_frmRequestProgress.Show();
                break;
            }
        }

        private delegate void Invoke_SetControl(bool enabled);
        private delegate void Invoke_OnEnabled(int nID);

        public void WorkerThreadMethod()
        {
            Invoke_SetControl InvokeSetControl = new Invoke_SetControl(this.SetControl);

            // 처음 로딩시에는 ControlCheck를 무조건 -1로 준다.
            WriteUserInfo(true);

            int nThreadCount = 0;

            while (!m_isStop)
            {
                Thread.Sleep(1000);

                if (++nThreadCount >= 5)
                {
                    // 사용자 접속 정보 주기적으로 DB에 입력
                    WriteUserInfo();
                    nThreadCount = 0;
                }

                //제어자의 ID를 얻음
                int nUserID = LoadControlUser();
                if (nUserID < 0)
                {
                    if (m_bRequestControl == true)
                    {
                        if (m_frmRequestProgress != null && m_frmRequestProgress.Visible == true)
                        {
                            m_frmRequestProgress.Invoke((MethodInvoker)delegate
                            {
                                m_frmRequestProgress.Close();
                            });
                        }
                        m_bRequestControl = false;
                    }
                    ctrlControlRequest.Visible = true;
                    ctrlControlReturn.Visible = false;
                    this.Invoke(InvokeSetControl, false);
                    continue;
                }
                else
                {                   
                    //제어 중
                    if (nUserID == m_nSOPGenUserID && ctrlControl.Checked)
                    {
                        ArrayList arrRequest = GetRequestControl();
                        if (arrRequest == null || arrRequest.Count == 0) continue;

                        foreach (ControlCheck data in arrRequest)
                        {
                            if (data.ControlChecked == 1)
                            {
                                Thread t = new Thread(new ParameterizedThreadStart(TempWorkerThread));
                                t.Start(nThreadCount);

                                m_isWorkingTempThread = true;
                                SendMessagetoContolUser(nUserID);
                                m_isWorkingTempThread = false;
                                break;
                            }
                        }
                    }
                    else if (nUserID != m_nSOPGenUserID && ctrlControl.Checked)
                    {
                        m_bRequestControl = false;
                        //모니터링으로 변경
                        ctrlControlRequest.Visible = true;
                        ctrlControlReturn.Visible = false;
                        this.Invoke(InvokeSetControl, false);
                    }
                    else if (nUserID == m_nSOPGenUserID && !ctrlControl.Checked)
                    {
                        if (m_bRequestControl == true)
                        {
                            if (m_frmRequestProgress != null && m_frmRequestProgress.Visible == true)
                            {
                                m_frmRequestProgress.Invoke((MethodInvoker)delegate
                                {
                                    m_frmRequestProgress.Close();
                                }); 
                            }
                            m_bRequestControl = false;
                        }
                        ctrlControlRequest.Visible = false;
                        ctrlControlReturn.Visible = true;
                        this.Invoke(InvokeSetControl, true);
                    } 
                }

                if (m_bRequestControl == true)
                {
                    ArrayList arRequestUsers = GetRequestControl();
                    bool bfind = false;
                    foreach (ControlCheck check in arRequestUsers)
                    {
                        if (check.UserID == m_nSOPGenUserID)
                        {
                            bfind = true;
                            break;
                        }
                    }
                    if (bfind == false)
                    {
                        m_bRequestControl = false; 
                        RejectRequest();
                    }
                } 
            }
        }

        private void TempWorkerThread(object arg)
        {
            Invoke_SetControl InvokeSetControl = new Invoke_SetControl(this.SetControl);

            int nThreadCount = int.Parse(arg.ToString());

            while (m_isWorkingTempThread)
            {
                Thread.Sleep(1000);

                if (++nThreadCount >= 5)
                {
                    // 사용자 접속 정보 주기적으로 DB에 입력
                    WriteUserInfo();
                    nThreadCount = 0;
                }

                //제어자의 ID를 얻음
                int nUserID = LoadControlUser();
                if (nUserID < 0)
                {
                    ctrlControlRequest.Visible = true;
                    ctrlControlReturn.Visible = false;
                    this.Invoke(InvokeSetControl, false);
                    continue;
                }
                else
                {
                    //제어 중
                    if (nUserID == m_nSOPGenUserID && ctrlControl.Checked)
                    {
                    }
                    else if (nUserID != m_nSOPGenUserID && ctrlControl.Checked)
                    {                        
                        //모니터링으로 변경
                        ctrlControlRequest.Visible = true;
                        ctrlControlReturn.Visible = false;
                        this.Invoke(InvokeSetControl, false);
                    }
                    else if (nUserID == m_nSOPGenUserID && !ctrlControl.Checked)
                    {
                        ctrlControlRequest.Visible = false;
                        ctrlControlReturn.Visible = true;
                        this.Invoke(InvokeSetControl, true);
                    }
                }
            }
        }       

        private void SetControl(bool enableControl)
        {
            ctrlControl.Checked = enableControl;
            ctrlMonitoring.Checked = !enableControl;

            ctrlControl.Visible = enableControl;
            ctrlMonitoring.Visible = !enableControl;

            if (enableControl)
            {
                // 제어권을 가지게 되면 현재 진행중인 화재 상황에 대하여 SOP List를 팝업시킨다.
                m_netMgr.ShowDetectSignal();
                OnEnabled(ID.ID_CONTROL_REQUEST);
            }
            else
            {
                // 제어권을 잃었으므로 SOP List 창을 감춘다.
                Popup.PopupSensorOn.Instance.HideForm();

                // 제어권을 잃었으므로 제어권 요청창을 닫는다.
                if (m_frmRequestControl != null)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        m_frmRequestControl.CancelForm();
                    });
                }

                OnEnabled(ID.ID_CONTROL_RETURN);
            }
        }

        public bool HasControl
        {
            get
            {
                if (ctrlMonitoring == null)
                    return false;

                return !ctrlMonitoring.Checked;
            }
        }

        public bool SMSOn
        {
            get { return m_smsOn; }
            set { m_smsOn = value; }
        }
    }

    public class SOPLog
    {
        //private StreamWriter sw = new StreamWriter("log.txt", false, Encoding.UTF8);

        public void Write(string strLog)
        {
            //DateTime t = DateTime.Now;
            //sw.Write(string.Format("{0} {1} {2}", t.ToShortDateString(), t.ToShortTimeString(), strLog));
            //sw.Flush();
        }

        public void WriteLine(string strLog)
        {
            //DateTime t = DateTime.Now;
            //sw.WriteLine(string.Format("{0} {1} {2}", t.ToShortDateString(), t.ToShortTimeString(), strLog));
            //sw.Flush();
        }
    }
}
