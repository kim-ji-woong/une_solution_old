using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using Core;
using System.Reflection;

namespace SDMS
{
    public enum CCTVMode { CCTV_ONLY = 0, NORMAL }
    public enum LAYER_TYPE
    {
        FIRE_DETECT = 1,
        SPRING_COOLER = 2,
        PUMP = 4,
        CCTV = 8,
        CCTV_X = 16,
        FE = 32,
        HD = 64,
        FA = 128,
        FR = 256
    }

    public partial class FormMain : Form, ITextPictureBoxOwner, IRibbonButtonOwner
    {
        class ControlInitPos
        {
            private static ControlInitPos m_instance = null;

            public static ControlInitPos Instance
            {
                get
                {
                    if (m_instance == null)
                        m_instance = new ControlInitPos();

                    return m_instance;
                }
            }

            private int m_nLabelSelectZoneInitPos = 0;
            private int m_nComboBoxBuildingGroupInitPos = 0;
            private int m_nComboBoxBuildingInitPos = 0;
            private int m_nComboBoxFloorInitPos = 0;
            private int m_nButtonSelectZoneInitPos = 0;
            private int m_nButton1InitPos = 0;
            private int m_nButton2InitPos = 0;
            private int m_nLabelFireDetectInitPos = 0;
            private int m_nComboBoxFireDetectInitPos = 0;
            private int m_nPanelMiddleInitSize = 0;

            public int LabelSelectZoneInitPos
            {
                get { return m_nLabelSelectZoneInitPos; }
            }

            public int ComboBoxBuildingGroupInitPos
            {
                get { return m_nComboBoxBuildingGroupInitPos; }
            }

            public int ComboBoxBuildingInitPos
            {
                get { return m_nComboBoxBuildingInitPos; }
            }

            public int ComboBoxFloorInitPos
            {
                get { return m_nComboBoxFloorInitPos; }
            }

            public int ButtonSelectZoneInitPos
            {
                get { return m_nButtonSelectZoneInitPos; }
            }

            public int Button1InitPos
            {
                get { return m_nButton1InitPos; }
            }

            public int Button2InitPos
            {
                get { return m_nButton2InitPos; }
            }

            public int LabelFireDetectInitPos
            {
                get { return m_nLabelFireDetectInitPos; }
            }

            public int ComboBoxFireDetectInitPos
            {
                get { return m_nComboBoxFireDetectInitPos; }
            }

            public int PanelMiddleInitSize
            {
                get { return m_nPanelMiddleInitSize; }
            }

           
            private ControlInitPos()
            {
                m_nLabelSelectZoneInitPos = FormMain.Instance.labelSelectZone.Location.X;
                m_nComboBoxBuildingGroupInitPos = FormMain.Instance.cboBuildingGroup.Location.X;
                m_nComboBoxBuildingInitPos = FormMain.Instance.cboBuilding.Location.X;
                m_nComboBoxFloorInitPos = FormMain.Instance.cboFloor.Location.X;
                m_nButtonSelectZoneInitPos = FormMain.Instance.btnSelectZone.Location.X;
                m_nButton1InitPos = FormMain.Instance.button1.Location.X;
                m_nButton2InitPos = FormMain.Instance.btnSaveHWP.Location.X;
                m_nLabelFireDetectInitPos = FormMain.Instance.labelFireDetect.Location.X;
                m_nComboBoxFireDetectInitPos = FormMain.Instance.cmbFireDetect.Location.X;

                m_nPanelMiddleInitSize = FormMain.Instance.panelMiddle.Size.Width;
            }
        }

		//public const int WM_NCLBUTTONDOWN = 0xA1;
		//public const int HT_CAPTION = 0x2;

		//[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		//public static extern int SendMessage(IntPtr hWnd,
		//                 int Msg, int wParam, int lParam);
		//[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		//public static extern bool ReleaseCapture();

        private CCTVMode m_cctvMode = CCTVMode.NORMAL;

        private int m_nPanelTopHeight = 154;//169;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }
        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager();
        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        //////////////////////////////////////////////////////////////////////////
        // Reprot
        //1 - 탐지, 2 - 처리, 3 - 대응
        private int m_nReportPage = 1;

        private PageBackstageHome m_PageHome = null;
        public PageBackstageHome PageHome
        {
            get { return m_PageHome; }
        }

        private FormClock m_ClockForm = null;
        public FormClock ClockForm
        {
            get { return m_ClockForm; }
        }

        private FormStatus m_StatusForm = null;
        public FormStatus StatusForm
        {
            get { return m_StatusForm; }
        }


        private FormReportFire m_ReportFireForm = null;
        public FormReportFire ReportFireForm
        {
            get { return m_ReportFireForm; }
            set { m_ReportFireForm = value; }
        }


        public System.Windows.Forms.ComboBox ReactComboSearchType
        {
            get { return react_cboSearchType; }
            set { react_cboSearchType = value; }
        }

        private DataManager m_dataMgr = null;
        public SDMS.DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        private int m_nSystemButtonSpace = 0;
        //private DateTime m_timeTopClicked = new DateTime();
        
        private bool m_bExit = false;

        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

		// Form Move 를 위한 Panel Top 클릭 체크
		private bool m_bLeftMouseDown = false;
		// Form Move 를 위한 Panel Top 클릭 점
		private Point m_ptMove;

        private NetworkManager m_netMgr = null;

		private bool m_bDetectFireSensor = false;
		public bool DetectFireSensor
		{
			get { return m_bDetectFireSensor; }
			set { m_bDetectFireSensor = value; }
		}

        private FormViewOutdoor m_frmOutdoor = null;
        private FormViewIndoor m_frmIndoor = null;
        private FormCCTVGuide m_frmCCTVGuide = null;

        private int m_nOriginLeftPanelWidth = 0;

        private bool m_isFirstReport = true;
        private DateTime m_dtLastReport = new DateTime();

        private bool m_isThumbnailMode = false;

        public bool ThumbnailMode
        {
            get { return m_isThumbnailMode; }
        }

        public FormCCTVGuide CCTVGuide
        {
            get { return m_frmCCTVGuide; }
        }

        public Form MainFrame
        {
            get { return this; }//FormFrame.Instance; }
        }

        private static int m_nDefaultLayerState =
            (int)LAYER_TYPE.FIRE_DETECT | (int)LAYER_TYPE.SPRING_COOLER |
            (int)LAYER_TYPE.PUMP | (int)LAYER_TYPE.CCTV |
            (int)LAYER_TYPE.FE | (int)LAYER_TYPE.HD;

        private int m_nSOPGenUserID = -1;
        private string m_strSOPGenUserRealName = "";
        private bool m_isVisibleEquipZoneCCTV = false;

        private FormCCTVList m_frmCCTVList = null;

        public FormCCTVList CCTVList
        {
            get { return m_frmCCTVList; }
            set { m_frmCCTVList = value; }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
        }

        public string SOPGenUserRealName
        {
            get { return m_strSOPGenUserRealName; }
        }

        public bool ShowEquipZoneCCTV
        {
            get 
            {
                if( PageHome == null)
                    return false;
                if( PageHome.CCTVForm == null)
                    return false;

                return PageHome.CCTVForm.Visible; 
            }
            set
            {
                checkBoxEquipZoneCCTV.Visible = value;
                btnShowCCTVList.Visible = value;

                btnSensorMonitor.Visible = !value;
                labelSensorMonitor.Visible = !value;

                if (value == false)
                    checkBoxEquipZoneCCTV.Checked = false;

                if (value)
                    SetEquipZoneCCTV(checkBoxEquipZoneCCTV.Checked);
                else
                    SetEquipZoneCCTV(false);
            }
        }

        public EquipmentZone CurrentEquipZone
        {
            get
            {
                if (cboEquipZone.SelectedIndex < 0)
                    return null;

                return (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
            }
        }

        public FireDetectProcess CurrentFireDetectProcess
        {
            get
            {
                if (cmbFireDetect.SelectedIndex < 0)
                    return null;

                return (FireDetectProcess)cmbFireDetect.Items[cmbFireDetect.SelectedIndex];
            }
        }

		public FireDetectProcess LastFireDetectProcess
		{
			get
			{
				int nCount = cmbFireDetect.Items.Count;
				if (nCount == 0)
                    return null;
				
				return (FireDetectProcess)cmbFireDetect.Items[nCount-1];
			}
		}

        public void ClearAllFireDetect()
        {
            cmbFireDetect.Items.Clear();
            cmbFireDetect.SelectedIndex = -1;

            SetNormalMode(-1);
        }

		public void SelectLastFireDectectProcess()
		{
			int nCount = cmbFireDetect.Items.Count;
			if (nCount == 0)
				return;
			cmbFireDetect.SelectedIndex = nCount - 1;
		}

		public void SelectFireDetectProcess(int nSensorHistoryID, int nSensorID)
		{
			int nIdx = 0;
			foreach (FireDetectProcess process in cmbFireDetect.Items)
			{
				if (process.SensorHistoryID == nSensorHistoryID && process.DetectSensorID == nSensorID)
				{
					cmbFireDetect.SelectedIndex = nIdx;
					bool bSelected = process.Select();
					if (bSelected)
					{				
						ReactionLogManager.Instance.ProcessLog(process.LastLog, true);
					}
					
					break;
				}
				nIdx++;
			}
		}
		public void SelectFireDetectProcess(int nIdx)
		{
			if (nIdx < 0)
				return;

			int nCount = cmbFireDetect.Items.Count;
			if (nCount == 0 || nIdx >= nCount)
				return;

			cmbFireDetect.SelectedIndex = nIdx;
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

		private int m_nMonitor = 1;

        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName, int nMonitor)
        {
            m_instance = this;
			m_nMonitor = nMonitor;
			

            m_nSOPGenUserID = nSOPGenUserID;
            m_strSOPGenUserRealName = strSOPGenUserRealName;

			Debug.WriteLine("Start : "+DateTime.Now);
            m_dataMgr = new DataManager(m_dbMgr);
			//Debug.WriteLine(DateTime.Now);

            LoadBaseData();
			//Debug.WriteLine(DateTime.Now);
            InitializeComponent();
			//Debug.WriteLine(DateTime.Now);
            m_nOriginLeftPanelWidth = panelLeft.Size.Width;

            this.Name = "SDMS";
            this.FormClosing += FormMain_FormClosing;
            this.FormClosed += FormMain_FormClosed;
            this.Load += FormMain_Load;            
            this.Resize += new System.EventHandler(this.FormMain_Resize);
			//Debug.WriteLine(DateTime.Now);
            AddPythonFunction();
			//Debug.WriteLine(DateTime.Now);
            m_nSystemButtonSpace = btnMax.Location.X - (btnMin.Location.X + btnMin.Size.Width);
            InitTab();
			//Debug.WriteLine(DateTime.Now);
            CreateBackstageHome();
			//Debug.WriteLine(DateTime.Now);
        }

        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.SelectMonitoringTab = new Func<bool>(SelectMonitoringTab);
            proxy.UserObject.SelectAdminTab = new Func<bool>(SelectAdminTab);
            proxy.UserObject.SelectReportTab = new Func<bool>(SelectReportTab);
        }


        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

		//private string m_strSOPGenUserAlias = "";
        /// <summary>
        /// UI 생성되기 이전에 필요한 DB Data 로드
        /// Form 생성 이전에 호출
        /// </summary>
        public void LoadBaseData()
        {
            ZoneManager.Instance.LoadBuildingData();
            ZoneManager.Instance.LoadZones();
			ZoneManager.Instance.LoadEquipmentZone();
            //m_dataMgr.LoadFireEquipment();
            //m_dataMgr.LoadFacilityManager();
			
			ReciverManager.Instance.LoadReciverList();
        }

        /// <summary>
        /// UI가 생성된 이후에 사용될 DB Data 로드
        /// Form Load 이벤트에서 호출
        /// </summary>
        public void LoadExtraData()
        {
            SensorManager.Instance.ReadAllSensorData();

			
        }

		public void SetDisableToolBar()
		{
			panelMiddle.Enabled = false;
			panelLeft.Enabled = false;
		}

		public void SetEnableToolBar()
		{
			panelMiddle.Enabled = true;
			panelLeft.Enabled = true;
		}
		
        private void InitTab()
        {
            pictureBoxMonitoring.SetPictureBoxOwner(this);
            pictureBoxAdmin.SetPictureBoxOwner(this);
            pictureBoxReport.SetPictureBoxOwner(this);

            pictureBoxMonitoring.Text = "모니터링";
            pictureBoxAdmin.Text = "관리";
            pictureBoxReport.Text = "리포트";

            panelTop.Size = new Size(this.Size.Width, m_nPanelTopHeight);
            panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);
        }


        private bool m_bCmbLocBtm = false;
        private void ResizePanels()
        {
            int nHeight = 39;
            
            if (this.Size.Width > 1300)
            {
                panelMiddle.Size = new Size(this.Size.Width, nHeight);
                panelProcessHistory.Size = panelMiddle.Size;
                panelReactionHistory.Size = panelMiddle.Size;

                panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);

                if (!m_isThumbnailMode || (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
                {
                    
                    panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                    panelLeft.Show();

                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                else
                {
                    panelLeft.Hide();
                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                m_bCmbLocBtm = false;
            }
            else
            {
                panelMiddle.Size = new Size(this.Size.Width, nHeight * 2);
                panelProcessHistory.Size = panelMiddle.Size;
                panelReactionHistory.Size = panelMiddle.Size;

                panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);

                if (!m_isThumbnailMode || (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
                {
                    panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - nHeight);
                    panelLeft.Show();

                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                else
                {
                    panelLeft.Hide();

                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }

                m_bCmbLocBtm = true;
            }
        }

        private void ResizeComboBox()
        {
            int nPanelWidth = panelMiddle.Size.Width;

            if (m_bCmbLocBtm == false)
            {
                labelSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.LabelSelectZoneInitPos), 13);
                cboBuildingGroup.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingGroupInitPos), 10);
                cboBuilding.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingInitPos), 10);
                cboFloor.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxFloorInitPos), 10);
                btnSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ButtonSelectZoneInitPos), 8);

                button1.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button1InitPos), button1.Location.Y);
                btnSaveHWP.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button2InitPos), btnSaveHWP.Location.Y);

                labelFireDetect.Location = new Point(864, labelFireDetect.Location.Y);
                cmbFireDetect.Location = new Point(932, cmbFireDetect.Location.Y);
            }
            else
            {
                int nHeight = panelMiddle.Size.Height / 2 - 5;
                //int nWidth = 630;
                labelSelectZone.Location = new Point(684, 13 + nHeight);
                cboBuildingGroup.Location = new Point(742, 10 + nHeight);
                cboBuilding.Location = new Point(886, 10 + nHeight);
                cboFloor.Location = new Point(1141, 10 + nHeight);
                btnSelectZone.Location = new Point(1204, 8 + nHeight);

                button1.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button1InitPos), button1.Location.Y);
                btnSaveHWP.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button2InitPos), btnSaveHWP.Location.Y);

                labelFireDetect.Location = new Point(864, labelFireDetect.Location.Y);
                cmbFireDetect.Location = new Point(932, cmbFireDetect.Location.Y);
            }

            if (m_isVisibleEquipZoneCCTV)
                ResizeEquipZoneCCTVControl(true, false);
        }

        /*
        private void ResizePanels()
        {
            panelMiddle.Size = new Size(this.Size.Width, panelMiddle.Size.Height);
            panelProcessHistory.Size = panelMiddle.Size;
            panelReactionHistory.Size = panelMiddle.Size;

            if (!m_isThumbnailMode || (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
            {
                panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                panelLeft.Show();

                int nBottomHeight = panelLeft.Size.Height;
                panelBottom.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, panelLeft.Location.Y);
                panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
            }
            else
            {
                //panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                panelLeft.Hide();

                int nBottomHeight = panelLeft.Size.Height;
                panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
            }
        }

        private void ResizeComboBox()
        {
            int nPanelWidth = panelMiddle.Size.Width;

            //if (ShowEquipZoneCCTV)
            {
                labelSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.LabelSelectZoneInitPos), labelSelectZone.Location.Y);
                cboBuildingGroup.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingGroupInitPos), cboBuildingGroup.Location.Y);
                cboBuilding.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingInitPos), cboBuilding.Location.Y);
                cboFloor.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxFloorInitPos), cboFloor.Location.Y);
                btnSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ButtonSelectZoneInitPos), btnSelectZone.Location.Y);

                button1.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button1InitPos), button1.Location.Y);
                button2.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button2InitPos), button2.Location.Y);

                labelFireDetect.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.LabelFireDetectInitPos), labelFireDetect.Location.Y);
                cmbFireDetect.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxFireDetectInitPos), cmbFireDetect.Location.Y);

                if (m_isVisibleEquipZoneCCTV)
                    ResizeEquipZoneCCTVControl(true, false);
            } 
        }
        */

        private void FormMain_Resize(object sender, EventArgs e)
        {
            ResizePanels();
            ResizeComboBox();

            ResizeAdminRibbonBar();
            ResizeReportRibbonBar();

            m_PageHome.SetBounds(0, 0, panelBottom.Size.Width, panelLeft.Size.Height);
            
            ResizeSystemButtons();
            ResizeButtons();
        }

        private void ResizeAdminRibbonBar()
        {
            panelAdminRibbonBarLeft.Location = panelClock.Location;
            panelAdminRibbonBarMiddle.Location = new Point(40, panelAdminRibbonBarLeft.Location.Y + 1);
            panelAdminRibbonBarRight.Location = new Point(this.Size.Width - panelAdminRibbonBarRight.Size.Width, panelAdminRibbonBarLeft.Location.Y + 1);

            panelAdminRibbonBarMiddle.Size = new Size(panelAdminRibbonBarRight.Location.X - panelAdminRibbonBarMiddle.Location.X, panelAdminRibbonBarLeft.Size.Height);
        }

        private void ResizeReportRibbonBar()
        {
            panelReportRibbonBarLeft.Location = panelClock.Location;
            panelReportRibbonBarMiddle.Location = new Point(40, panelReportRibbonBarLeft.Location.Y + 1);
            panelReportRibbonBarRight.Location = new Point(this.Size.Width - panelReportRibbonBarRight.Size.Width, panelReportRibbonBarLeft.Location.Y + 1);

            panelReportRibbonBarMiddle.Size = new Size(panelReportRibbonBarRight.Location.X - panelReportRibbonBarMiddle.Location.X, panelReportRibbonBarLeft.Size.Height);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            labelDate.Text = string.Format("{0}년 {1}월 {2}일", dtNow.Year, dtNow.Month, dtNow.Day);
            labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_PageHome.Show();
            
			LoadExtraData();

            InitPanels();
            InitButtons();
            InitComboBox();

            m_PageHome.FrmReport.SetComboText(proc_btnStartDate.Text, proc_btnEndDate.Text);

            SelectMonitoringTab();

			SetMonitorForm(MainFrame, m_nMonitor);

            TextPictureBox_MouseDown(pictureBoxMonitoring, null);
            mClockTimer.Start();
            
            m_MainTimer.Enabled = true;
            m_MainTimer.Start();

            DatePickerStart.Visible = false;
            DatePickerEnd.Visible = false;

            m_netMgr = NetworkManager.Instance;

			
			btnFire.Enabled = false;

			Debug.WriteLine("Load : " + DateTime.Now);
			
			m_CheckReciver.Enabled = true;
			m_CheckReciver.Interval = 3000;
			m_CheckReciver.Start();
            //Test();
        }

        /*private void Test()
        {
            int nID = 1;

            foreach (KeyValuePair<int, EquipmentZone> pair in ZoneManager.Instance.DicEquipZones)
            {
                ArrayList arrResult = CCTVManager.Instance.GetAutoCCTVList(pair.Value.LinkedZone);
                WriteToDB(ref nID, pair.Value, arrResult);
            }
        }

        private void WriteToDB(ref int nID, EquipmentZone equipZone, ArrayList arrResult)
        {
            string strSQL = "Insert into EquipZoneCCTVTemp (ID, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, CCTV5, CCTV6, CCTV7,";
            strSQL += " CCTV8, CCTV9, CCTV10, CCTV11, CCTV12, CCTV13, CCTV14, CCTV15, Description) values (";
            strSQL += nID.ToString() + ", " + equipZone.ID.ToString() + ", ";
            //strSQL += "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, NULL)";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                CCTV cctv = (CCTV)arrResult[i];
                strSQL += cctv.ID.ToString() + ", ";
            }

            for (int i = nResultCount; i < 15; i++)
            {
                strSQL += "NULL, ";
            }

            strSQL += "NULL)";

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;

            nID++;
        }*/

        private void InitPanels()
        {
            panelProcessHistory.Location = panelMiddle.Location;
            panelReactionHistory.Location = panelMiddle.Location;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;

            m_frmOutdoor = new FormViewOutdoor();
            m_frmOutdoor.TopLevel = false;
            panelLeft.Controls.Add(m_frmOutdoor);

            m_frmIndoor = new FormViewIndoor();
            m_frmIndoor.TopLevel = false;
            panelLeft.Controls.Add(m_frmIndoor);

            m_frmCCTVGuide = new FormCCTVGuide();
            m_frmCCTVGuide.TopLevel = false;
            panelLeft.Controls.Add(m_frmCCTVGuide);
        }
		
        private void InitComboBox()
        {
            proc_cboBuildingGroup.Items.Add("모든 건물 그룹");
            proc_cboBuilding.Items.Add("모든 건물");
            proc_cboFloor.Items.Add("모든 층");

			ComboHelper.InitBuildingGroupComboBox(cboBuildingGroup);
			ComboHelper.InitBuildingGroupComboBox(proc_cboBuildingGroup);

            cboBuildingGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);
            proc_cboBuildingGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);

            proc_cboLatelyDate.Items.Add("기간 선택");
            proc_cboLatelyDate.Items.Add("최근 6개월");
            proc_cboLatelyDate.Items.Add("최근 3개월");
            proc_cboLatelyDate.Items.Add("최근 1개월");

            for (int i = 0; i < 24; i++)
            {
                react_cboStartTime.Items.Add(i + "시");
            }

            for (int i = 1; i < 25; i++)
            {
                react_cboEndTime.Items.Add(i + "시");
            }

            react_btnStartDate.Text = DateTime.Now.AddDays(-7).ToString().Substring(0, 10);
            react_btnEndDate.Text = DateTime.Now.ToString().Substring(0, 10);

            //react_cboSearchType.Items.Add("가장 최근 데이터");
            react_cboSearchType.Items.Add("화재신고만");
            react_cboSearchType.Items.Add("오작동 처리 포함");
            react_cboSearchType.Items.Add("처리되지 않은 신호 포함");

            react_cboStartTime.Enabled = false;
            react_cboEndTime.Enabled = false;


            //탐지,처리이력 콤보박스 설정
            if (cboBuildingGroup.Items.Count > 0)
                cboBuildingGroup.SelectedIndex = 0;

            if (proc_cboBuildingGroup.Items.Count > 0)
                proc_cboBuildingGroup.SelectedIndex = 0;

            if (proc_cboLatelyDate.Items.Count > 0)
                proc_cboLatelyDate.SelectedIndex = 1;
        }

        private void InitButtons()
        {
            TextData data = new TextData();
            data.Brush = new SolidBrush(Color.White);
            data.Text = "화재신고";
            data.Rectangle = new Rectangle(5, 65, 60, 12);

            btnFire.ExtraImage = global::SDMS.Properties.Resources.Fire_Icon;
            btnFire.X = 20;
            btnFire.Y = 5;
            btnFire.TextData = data;

            /// 가로 바
            SetButtonID(btnHome, ID.ID_VIEW_HOME, "초기 화면");
            SetButtonID(btnFullScreen, ID.ID_VIEW_FULLSCREEN, "전체 화면");
            SetButtonID(btnPick, ID.ID_VIEW_PICK, "선택");
            SetButtonID(btnPanning, ID.ID_VIEW_PAN, "화면 이동");
            SetButtonID(btnOrbit, ID.ID_VIEW_ORBIT, "화면 회전");
            SetButtonID(btnZoomIn, ID.ID_VIEW_ZOOMIN, "확대");
            SetButtonID(btnZoomOut, ID.ID_VIEW_ZOOMOUT, "축소");
            SetButtonID(btnOutside, ID.ID_VIEW_OUTSIDE, "외부공간 보기");
            SetButtonID(btnBoth, ID.ID_VIEW_BOTHSIDE, "외부/실내 같이 보기");
			btnBoth.Enabled = false;
            SetButtonID(btnInside, ID.ID_VIEW_INSIDE, "실내공간 보기");
			btnInside.Enabled = false;
			SetButtonID(btnMultiCCTV, ID.ID_VIEW_CCTV, "CCTV 크게 보기");
			SetButtonID(btnScreenShot, ID.ID_VIEW_SCREENSHOT, "화면 캡쳐");

            CheckButton(btnOrbit, true);
            CheckButton(btnOutside, true);
            //////////////////////////////////////////

            /// 세로바(Layer)
            SetButtonID(btnLayerFire, ID.ID_LAYER_DETECTOR);
            SetButtonID(btnLayerSpringCooler, ID.ID_LAYER_COOLER);
            SetButtonID(btnLayerPump, ID.ID_LAYER_PERSURE);
            SetButtonID(btnLayerCCTV, ID.ID_LAYER_CCTV);
            SetButtonID(btnLayerFE, ID.ID_LAYER_FIREEXT);
            SetButtonID(btnLayerHD, ID.ID_LAYER_FIREHYD);
            SetButtonID(btnLayerFA, ID.ID_LAYER_ALARMSTA);
            SetButtonID(btnLayerFR, ID.ID_LAYER_RECIVER);
            SetButtonID(btnLayerLowCCTV, ID.ID_LAYER_CCTVLOW);

            int nLayerState = ReadLayerState();
            InitLayerButtonCheck(nLayerState);
            /*CheckButton(btnLayerFire, true);
            CheckButton(btnLayerSpringCooler, true);
            CheckButton(btnLayerPump, true);
            CheckButton(btnLayerCCTV, true);
            CheckButton(btnLayerFE, true);
            CheckButton(btnLayerHD, true);
            CheckButton(btnLayerFA, true);
            CheckButton(btnLayerFR, true);
            CheckButton(btnLayerLowCCTV, false);*/
            //////////////////////////////////////////

            InitReportRibbonButtons();
            InitAdminRibbonButtons();

            labelSensorMonitor.Text = "수신반 연결상태 알수없음";

            int nSpace = labelSensorMonitor.Location.X - (btnSensorMonitor.Location.X + btnSensorMonitor.Size.Width);
            btnSensorMonitor.Location = new Point(checkBoxEquipZoneCCTV.Location.X, btnSensorMonitor.Location.Y);
            labelSensorMonitor.Location = new Point(btnSensorMonitor.Location.X + btnSensorMonitor.Size.Width + nSpace, labelSensorMonitor.Location.Y);
        }

        private void InitLayerButtonCheck(int nLayerState)
        {
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FIRE_DETECT, btnLayerFire);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.SPRING_COOLER, btnLayerSpringCooler);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.PUMP, btnLayerPump);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV, btnLayerCCTV);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV_X, btnLayerLowCCTV);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FE, btnLayerFE);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.HD, btnLayerHD);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FA, btnLayerFA);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FR, btnLayerFR);
        }

        private void InitLayerButtonCheck(int nLayerState, LAYER_TYPE type, Button btn)
        {
            bool checkLayerState = (nLayerState & (int)type) == (int)type;
            CheckButton(btn, checkLayerState);
            m_PageHome.OnChangeLayer(GetButtonID(btn), checkLayerState);
        }

        private int ReadLayerState()
        {
            string strPath = Application.ExecutablePath;
            string szParentPath = Path.GetDirectoryName(strPath);
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string szAssemName = currentProcess.ProcessName;
            string strFilePath = szParentPath + "\\" + szAssemName + "LayerState.ini";


            int nResult = m_nDefaultLayerState;

            if (File.Exists(strFilePath))
            {
                StreamReader reader = new StreamReader(strFilePath);

                while (!reader.EndOfStream)
                {
                    string strLine = reader.ReadLine();

                    strLine = strLine.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                    strLine = strLine.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                    if (strLine.Length == 0)
                        continue;

                    int.TryParse(strLine, out nResult);
                    break;
                }

                reader.Close();
            }
            else
            {
                WriteLayerState(m_nDefaultLayerState);
            }

            return nResult;
        }

        private void WriteLayerState(int nLayerState)
        {
            string strPath = Application.ExecutablePath;
            string szParentPath = Path.GetDirectoryName(strPath);
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string szAssemName = currentProcess.ProcessName;
            string strFilePath = szParentPath + "\\" + szAssemName + "LayerState.ini";

            StreamWriter writer = new StreamWriter(strFilePath);
            writer.Write(nLayerState);
            writer.Close();
        }

        private void InitReportRibbonButtons()
        {
            Image imgMouseOverBkgnd = global::SDMS.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SDMS.Properties.Resources.RibbonChecked_bkgnd;

            InitRibbonButton(btnDetectHistory, ID.ID_BTN_DETECT, "탐지이력", global::SDMS.Properties.Resources.FindHistory_Normal, global::SDMS.Properties.Resources.FindHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnProcessHistory, ID.ID_BTN_NOTOPERATION, "처리이력", global::SDMS.Properties.Resources.ProcessHistory_Normal, global::SDMS.Properties.Resources.ProcessHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnReactionHistory, ID.ID_BTN_ACTION, "대응이력", global::SDMS.Properties.Resources.ReactionHistory_Normal, global::SDMS.Properties.Resources.ReactionHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnSMSHistory, ID.ID_BTN_SMSREPORT, "문자이력", global::SDMS.Properties.Resources.Manage_SMS_Normal, global::SDMS.Properties.Resources.Manage_SMS_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
                        
            btnDetectHistory.IsChecked = true;
        }

        private void InitAdminRibbonButtons()
        {
            Image imgMouseOverBkgnd = global::SDMS.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SDMS.Properties.Resources.RibbonChecked_bkgnd;

            btnCreateFire.Visible = btnCreateSpringCooler.Visible = false;
            btnEditFacilityZone.Location = btnCreateFire.Location;

            //InitRibbonButton(btnCreateFire, ID.ID_NEW_FIRE_SENSOR, "화재탐지", global::SDMS.Properties.Resources.Create_Fire_Normal, global::SDMS.Properties.Resources.Create_Fire_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnCreateSpringCooler, ID.ID_NEW_COOLER_SENSOR, "스프링쿨러", global::SDMS.Properties.Resources.Create_SpringCooler_Normal, global::SDMS.Properties.Resources.Create_SpringCooler_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnEditFacilityZone, ID.ID_EDIT_FACILITY_ZONE, "설비영역", global::SDMS.Properties.Resources.EditFacilityZone_Normal, global::SDMS.Properties.Resources.EditFacilityZone_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnCreatePump, ID.ID_NEW_PRESSURE_SENSOR, "펌프압력", global::SDMS.Properties.Resources.Create_Pump_Normal, global::SDMS.Properties.Resources.Create_Pump_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnCreateCCTV, ID.ID_NEW_CCTV, "CCTV", global::SDMS.Properties.Resources.Create_CCTV_Normal, global::SDMS.Properties.Resources.Create_CCTV_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnDelete, ID.ID_DEL_FACILITY, "삭제", global::SDMS.Properties.Resources.Del_Normal, global::SDMS.Properties.Resources.Del_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnShowList, ID.ID_SHOW_LIST_FACILITY, "리스트보기", global::SDMS.Properties.Resources.Show_List_Normal, global::SDMS.Properties.Resources.Show_List_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageManager, ID.ID_MANAGE_MANAGER, "담당자관리", global::SDMS.Properties.Resources.Manage_Manager_Normal, global::SDMS.Properties.Resources.Manage_Manager_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageSMS, ID.ID_MANAGE_MESSAGE, "메시지관리", global::SDMS.Properties.Resources.Manage_SMS_Normal, global::SDMS.Properties.Resources.Manage_SMS_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageBroadcast, ID.ID_MANAGE_BROADCAST, "방송관리", global::SDMS.Properties.Resources.Manage_Broadcast_Normal, global::SDMS.Properties.Resources.Manage_Broadcast_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManagePrint, ID.ID_MANAGE_PRINT, "도면관리", global::SDMS.Properties.Resources.Manage_Print_Normal, global::SDMS.Properties.Resources.Manage_Print_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageFacility, ID.ID_MANAGE_FACILITY, "장비현황", global::SDMS.Properties.Resources.Manage_Facility_Normal, global::SDMS.Properties.Resources.Manage_Facility_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageDetect, ID.ID_MANAGE_DETECT, "탐지관리", global::SDMS.Properties.Resources.Manage_Find_Normal, global::SDMS.Properties.Resources.Manage_Find_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnBackupDB, ID.ID_MANAGE_BACKUPDB, "백업/복원", global::SDMS.Properties.Resources.Backup_Restore, global::SDMS.Properties.Resources.Backup_restore_checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnSave, ID.ID_SAVE_DATA, "저장", global::SDMS.Properties.Resources.Save_Normal, global::SDMS.Properties.Resources.Save_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);

            ArrangeAdminRibbonButtons();
            ArrangeReportRibbonButtons();

            btnSave.Enabled = false;
        }

        private void InitRibbonButton(RibbonButton btn, int nID, string strTitle, Image imgNormal, Image imgChecked, Image imgMouseOverBkgnd, Image imgCheckedBkgnd)
        {
            btn.NormalImage = imgNormal;
            btn.CheckedImage = imgChecked;
            btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            btn.CheckedBkgndImage = imgCheckedBkgnd;
            btn.Title = strTitle;
            btn.Owner = this;

            SetButtonID(btn, nID);
        }
        
        private void ArrangeAdminRibbonButtons()
        {
            //ArrangeRibbonButton(btnCreateFire, btnCreateSpringCooler);
            //ArrangeRibbonButton(btnCreateSpringCooler, btnEditFacilityZone);
            ArrangeRibbonButton(btnEditFacilityZone, btnCreatePump);
            ArrangeRibbonButton(btnCreatePump, btnCreateCCTV);

            ArrangeRibbonButton(btnCreateCCTV, pictureBoxAdminRibbon1, (RibbonButton)btnDelete);
            ArrangeRibbonButton(btnDelete, pictureBoxAdminRibbon2, btnShowList);
            ArrangeRibbonButton(btnShowList, pictureBoxAdminRibbon3, btnManageManager);

            ArrangeRibbonButton(btnManageManager, btnManageSMS);
            ArrangeRibbonButton(btnManageSMS, btnManageBroadcast);
            ArrangeRibbonButton(btnManageBroadcast, btnManagePrint);
            ArrangeRibbonButton(btnManagePrint, btnManageFacility);
            ArrangeRibbonButton(btnManageFacility, btnManageDetect);
            ArrangeRibbonButton(btnManageDetect, btnBackupDB);

            ArrangeRibbonButton(btnBackupDB, pictureBoxAdminRibbon4, btnSave);
        }

        private void ArrangeReportRibbonButtons()
        {
            ArrangeRibbonButton(btnDetectHistory, btnProcessHistory);
            ArrangeRibbonButton(btnProcessHistory, btnReactionHistory);
            ArrangeRibbonButton(btnReactionHistory, btnSMSHistory);
        }

        private void ArrangeRibbonButton(RibbonButton btnPrev, RibbonButton btnNext)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
        }

        private void ArrangeRibbonButton(RibbonButton btnPrev, PictureBox pictureBoxMiddle, RibbonButton btnNext)
        {
            pictureBoxMiddle.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width - 3, pictureBoxMiddle.Location.Y);
            btnNext.Location = new Point(pictureBoxMiddle.Location.X + pictureBoxMiddle.Size.Width - 3, btnPrev.Location.Y);
        }

        private void SetButtonID(Button btn, int nID, string strTooltipText = "")
        {
            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        public Button GetButton(int nID)
        {
            if (m_dicIDButtons.ContainsKey(nID))
                return m_dicIDButtons[nID];

            return null;
        }

        public int GetButtonID(Button btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        public bool IsChecked(int nButtonID)
        {
            if (!m_dicIDButtons.ContainsKey(nButtonID))
                return false;

            Button btn = m_dicIDButtons[nButtonID];
            return IsChecked(btn);
        }

        public bool IsChecked(Button btn)
        {
            if (m_dicButtonChecked.ContainsKey(btn))
                return m_dicButtonChecked[btn];

            return false;
        }

        public void CheckButton(int nButtonID, bool isChecked)
        {
            if (!m_dicIDButtons.ContainsKey(nButtonID))
                return;

            Button btn = m_dicIDButtons[nButtonID];
            CheckButton(btn, isChecked);
        }

        public void CheckButton(Button btn, bool isChecked)
        {
            if (!m_dicButtonChecked.ContainsKey(btn))
                return;

            bool checkedOld = m_dicButtonChecked[btn];
            m_dicButtonChecked[btn] = isChecked;

            if (btn == btnHome)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Home_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Home_Normal;
            }
            else if (btn == btnFullScreen)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.FullScreen_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.FullScreen_Normal;
            }
            else if (btn == btnPick)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Pick_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Pick_Normal;
            }
            else if (btn == btnPanning)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Panning_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Panning_Normal;
            }
            else if (btn == btnOrbit)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Orbit_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Orbit_Normal;
            }
            else if (btn == btnZoomIn)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomIn_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomIn_Normal;
            }
            else if (btn == btnZoomOut)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomOut_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomOut_Normal;
            }
            else if (btn == btnOutside)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Outside_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Outside_Normal;
            }
            else if (btn == btnBoth)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Both_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Both_Normal;
            }
            else if (btn == btnInside)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Inside_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Inside_Normal;
            }
            else if (btn == btnMultiCCTV)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.CCTV_Normal;
            }
            else if (btn == btnScreenShot)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ScreenShot_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ScreenShot_Normal;
            }
            else if (btn == btnLayerFire)
            {
                WriteLayerState(LAYER_TYPE.FIRE_DETECT, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Fire_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Fire_Normal;
            }
            else if (btn == btnLayerSpringCooler)
            {
                WriteLayerState(LAYER_TYPE.SPRING_COOLER, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_SpringCooler_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_SpringCooler_Normal;
            }
            else if (btn == btnLayerPump)
            {
                WriteLayerState(LAYER_TYPE.PUMP, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Pump_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Pump_Normal;
            }
            else if (btn == btnLayerCCTV)
            {
                WriteLayerState(LAYER_TYPE.CCTV, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            }
            else if (btn == btnLayerFE)
            {
                WriteLayerState(LAYER_TYPE.FE, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FE_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FE_Normal;
            }
            else if (btn == btnLayerHD)
            {
                WriteLayerState(LAYER_TYPE.HD, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_HD_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_HD_Normal;
            }
            else if (btn == btnLayerFA)
            {
                WriteLayerState(LAYER_TYPE.FA, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FA_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FA_Normal;
            }
            else if (btn == btnLayerFR)
            {
                WriteLayerState(LAYER_TYPE.FR, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FR_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FR_Normal;
            }
            else if (btn == btnLayerLowCCTV)
            {
                WriteLayerState(LAYER_TYPE.CCTV_X, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            }

            else if (btn.GetType() == typeof(RibbonButton))
            {
                RibbonButton ribbonButton = (RibbonButton)btn;
                ribbonButton.IsChecked = isChecked;
            }

            if (checkedOld != isChecked)
                btn.Refresh();
        }

        private void WriteLayerState(LAYER_TYPE type, bool isChecked)
        {
            int nLayerState = ReadLayerState();

            if (isChecked)
                nLayerState = nLayerState | (int)type;
            else
                nLayerState = nLayerState & (~(int)type);

            WriteLayerState(nLayerState);
        }

        public static void SetInfoMessage(string szMessage)
        {
			((RealTimeInfoPane)m_instance.panelLog).RealTimeInfo = szMessage;
			((RealTimeInfoPane)m_instance.panelLog).DrawMovingText();
        }

        private void ResizeButtons()
        {
            int nClockRight = panelClock.Location.X + panelClock.Size.Width;
            int nPanelSpace = panelStatus.Location.X - nClockRight;

            int nStatusRight = panelStatus.Location.X + panelStatus.Size.Width;
            panelLog.Location = new Point(nStatusRight + nPanelSpace, panelLog.Location.Y);

            btnFire.Location = new Point(panelTop.Size.Width - btnFire.Size.Width, btnFire.Location.Y);
            panelLog.Size = new Size(btnFire.Location.X - nPanelSpace - panelLog.Location.X, panelLog.Size.Height);
        }

        private void ResizeSystemButtons()
        {
            int nWidth = this.Size.Width;
            int nImageWidth = btnMax.Width;

            btnClose.Location = new Point(nWidth - nImageWidth, btnClose.Location.Y);
            btnMax.Location = new Point(btnClose.Location.X - m_nSystemButtonSpace - nImageWidth, btnMax.Location.Y);
            btnMin.Location = new Point(btnMax.Location.X - m_nSystemButtonSpace - nImageWidth, btnMin.Location.Y);
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            MainFrame.WindowState = FormWindowState.Minimized;
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (MainFrame.WindowState == FormWindowState.Normal)
            {
                MainFrame.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.NormalWindow_Normal;
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.MaxWindow_Normal;
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            MainFrame.Close();
        }


		#region Top패널 Mouse 이벤트 , Maximized, Minimized, Move

		private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = panelTop.PointToScreen(new Point(e.X, e.Y));              
            }
			FormMain.Instance.EnableFireReportBtn(false);
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
			FormMain.Instance.EnableFireReportBtn(false);
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = panelTop.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = MainFrame.Location;
                        MainFrame.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }                
            }
        }

        private void panelTop_DoubleClick(object sender, EventArgs e)
        {
            if (MainFrame.WindowState == FormWindowState.Normal)
            {
                MainFrame.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.NormalWindow_Normal;
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                Size sizeCur = MainFrame.Size;
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.MaxWindow_Normal;
                Size sizeNormal = MainFrame.Size;

                double hRate = (double)sizeNormal.Height / (double)sizeCur.Height;
                MainFrame.Size = new Size((int)(sizeCur.Width * hRate), sizeNormal.Height);
            }
        }

		#endregion


		private void CreateBackstageHome()
        {
            m_PageHome = new PageBackstageHome();
            m_PageHome.Location = new Point(0, 0);
            m_PageHome.Dock = DockStyle.Fill;
            m_PageHome.TopLevel = false;
            m_PageHome.Parent = panelBottom;

            panelBottom.Controls.Add(m_PageHome);
        }

        public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }

            if (pictureBox == pictureBoxMonitoring)
            {
                SelectMonitoringTab();
            }
            else if (pictureBox == pictureBoxAdmin)
            {
                SelectAdminTab();                
            }
            else if (pictureBox == pictureBoxReport)
            {
                SelectReportTab();

            }
        }


        private object m_LockObject = new object();
        public void SetFloorStatus(BuildingGroup grp, Building building, Zone zoneFloor)
        {
			//FormMain.Instance.EnableFireReportBtn(false);
            lock (m_LockObject)
			{
				m_PageHome.ChangeFloor(grp, building, zoneFloor);
			}
            
        }

        #region (ChangeTab)
        /////////////////////////////////////////////////////////////////////////////////        
        public bool SelectMonitoringTab()
        {
            // 직전 모드가 Report 탭이었다면...
            if (panelReportRibbonBarLeft.Visible)
                m_dtLastReport = DateTime.Now;

            panelClock.Visible = true;
            panelStatus.Visible = true;
            panelLog.Visible = true;
            btnFire.Visible = true;
            panelMiddle.Visible = true;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;

            panelAdminRibbonBarLeft.Visible = false;
            panelAdminRibbonBarMiddle.Visible = false;
            panelAdminRibbonBarRight.Visible = false;

            panelReportRibbonBarLeft.Visible = false;
            panelReportRibbonBarMiddle.Visible = false;
            panelReportRibbonBarRight.Visible = false;


			labelFireDetect.Visible = true;
			cmbFireDetect.Visible = true;

            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            OnSelectMonitoringTab();
            return true;
        }
        public bool SelectAdminTab()
        {
            // 직전 모드가 Report 탭이었다면...
            if (panelReportRibbonBarLeft.Visible)
                m_dtLastReport = DateTime.Now;

            panelClock.Visible = false;
            panelStatus.Visible = false;
            panelLog.Visible = false;
            btnFire.Visible = false;
            panelMiddle.Visible = true;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;

            panelAdminRibbonBarLeft.Visible = true;
            panelAdminRibbonBarMiddle.Visible = true;
            panelAdminRibbonBarRight.Visible = true;

            panelReportRibbonBarLeft.Visible = false;
            panelReportRibbonBarMiddle.Visible = false;
            panelReportRibbonBarRight.Visible = false;

			labelFireDetect.Visible = false;
			cmbFireDetect.Visible = false;

            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            OnSelectAdminTab();
            return true;
        }

        public bool SelectReportTab()
        {
            panelClock.Visible = false;
            panelStatus.Visible = false;
            panelLog.Visible = false;
            btnFire.Visible = false;
            panelMiddle.Visible = false;

            CheckReportPage();

            panelAdminRibbonBarLeft.Visible = false;
            panelAdminRibbonBarMiddle.Visible = false;
            panelAdminRibbonBarRight.Visible = false;

            panelReportRibbonBarLeft.Visible = true;
            panelReportRibbonBarMiddle.Visible = true;
            panelReportRibbonBarRight.Visible = true;

			labelFireDetect.Visible = false;
			cmbFireDetect.Visible = false;

            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            OnSelectReportTab();
            return true;
        }

        private void CheckReportPage()
        {
            int nPage = m_PageHome.FrmReport.ReportPage;
            if (nPage == 1 || nPage == 2 || nPage == 4)
            {
                panelProcessHistory.Visible = true;
                panelReactionHistory.Visible = false;
            }
            else
            {
                panelProcessHistory.Visible = false;
                panelReactionHistory.Visible = true;
            }
        }

        private void OnSelectMonitoringTab()
        {
			FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(PageBackstageHome.Tab.MONITORING_TAB);
        }

        private void OnSelectAdminTab()
        {
			FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(PageBackstageHome.Tab.ADMIN_TAB);

			//this.Close();
			//Application.Restart();
        }

        private void OnSelectReportTab()
        {
            proc_cboLatelyDate_SelectedIndexChanged(null, null);

            if (m_isFirstReport)
            {
                m_isFirstReport = false;
            }
            else
            {
                DateTime dtNow = DateTime.Now;

                // 마지막에 리포트탭을 보았던 날짜가 아닐 경우 리포트 데이터를 새로 로딩한다.
                if (m_dtLastReport.Year != dtNow.Year || m_dtLastReport.Month != dtNow.Month || m_dtLastReport.Day != dtNow.Day)
                    proc_btnSelectZone_Click(null, null);
            }

			EnableFireReportBtn(false);

            PageHome.ChangeTab(PageBackstageHome.Tab.REPORT_TAB);
        }    
        //////////////////////////////////////////////////////////////////////////
        #endregion

        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
			if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
			if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            return szMainPath;
        }
               
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            mClockTimer.Stop();
            mClockTimer.Enabled = false;

            m_CheckReciver.Stop();
            m_CheckReciver.Enabled = false;

            m_bExit = true;

            m_netMgr.ReleaseThread();
            
            SensorSignalReciver.Instance.Dispose();
            ProcessManager.Instance.Dispose();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            m_MainTimer.Enabled = false;
            m_MainTimer.Stop();

            if (m_bExit != true && m_PageHome != null)
            {
                m_PageHome.Redraw3DView();
            }

            if (m_bExit != true)
            {
                m_MainTimer.Enabled = true;
                m_MainTimer.Start();
            }
        }        

        private void OnClickToolBarButton(object sender, EventArgs e)
        {
            m_PageHome.OnClickToolBarButton((Button)sender);
        }	

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;
            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];
			ComboHelper.InitBuildingComboBox(cboBuilding, buildingGroup);
            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;

            if (buildingGroup != ZoneManager.Instance.OutdoorBuildingGroup)
                cboFloor.Enabled = true;
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Object obj = cboBuilding.Items[nSelectedIndex];
            if (obj.GetType() == typeof(Building))
            {
				ComboHelper.InitFloorComboBox(cboFloor, (Building)obj);
                btnSelectZone.Enabled = true;
            }
            else
            {
                cboFloor.Items.Clear();
                btnSelectZone.Enabled = false;

                if (checkBoxEquipZoneCCTV.Checked)
                {
                    cboFloor.Enabled = false;

                    cboEquipZone.Items.Clear();
                    
                    if (obj.GetType() != typeof(Zone))
                        return;

                    Zone zone = (Zone)obj;
                    ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);

                    foreach (EquipmentZone equipZone in arrEquipZones)
                    {
                        cboEquipZone.Items.Add(equipZone);
                    }

                    if (cboEquipZone.Items.Count > 0)
                        cboEquipZone.SelectedIndex = 0;
                }
                else
                {
                    cboFloor.Enabled = true;
                    cboFloor.Items.Add("-");
                }
            }

            if (cboFloor.Items.Count > 0)
                cboFloor.SelectedIndex = 0;
        }

        private void btnSelectZone_Click(object sender, EventArgs e)
        {
            Zone zoneSelected = null;
            if (cboFloor.Text.Length == 0)
                return;
            else if (cboFloor.Text == "-")
            {
                zoneSelected = (Zone)cboBuilding.Items[cboBuilding.SelectedIndex];
            }
            else
            {
                Floor floor = (Floor)cboFloor.Items[cboFloor.SelectedIndex];
                zoneSelected = floor.Zone;
            }

            if (zoneSelected == null)
                return;

            BuildingGroup buildingGroupSelected = (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex];
            m_PageHome.ChangeFloor(buildingGroupSelected, zoneSelected.Building, zoneSelected);

			EnableChangeViewBtn();
        }

		public void EnableChangeViewBtn()
		{
			btnBoth.Enabled = true;
			btnInside.Enabled = true;
		}

		public void SelectIndoorZone(Zone zone)
		{
			if (zone == null || zone.Building == null)
				return;

			BuildingGroup buildingGroupSelected = zone.Building.BuildingGroup;
			if (buildingGroupSelected != null)
			{
				m_PageHome.ChangeFloor(buildingGroupSelected, zone.Building, zone);

				EnableChangeViewBtn();

				ChangeZoneComboBox(zone);
			}			
		}

		public void EnableFireReportBtn(bool bEnable, int nCase = 1)
		{
			if (btnFire.Text == "화재 신고")
				btnFire.Enabled = bEnable;	
	
			if (nCase == 2)
			{
				btnFire.Text = "화재 신고";
			}
			else if(nCase == 1)
			{
				btnFire.Text = "화재 종료";
			}	

			if( nCase == 2)
				btnFire.Enabled = bEnable;			
		}

        private void OnClickLayerToolBarButton(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            m_PageHome.OnChangeLayer(GetButtonID(btn));

            bool isChecked = IsChecked(btn);
            CheckButton(btn, !isChecked);
        }

        private void btnLayerFR_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerFA_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerHD_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerFE_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerCCTV_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerPump_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerSpringCooler_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerFire_Click(object sender, EventArgs e)
        {
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            m_PageHome.OnCommandExcute(GetButtonID(btn));

            CheckReportButton(btn);
        }

        private void CheckReportButton(Button btn)
        {
            if (btn == btnDetectHistory)
            {
                panelMiddle.Visible = false;
                panelProcessHistory.Visible = true;
                panelReactionHistory.Visible = false;

                m_PageHome.FrmReport.ShowDetectReport();

                btnDetectHistory.IsChecked = true;
                btnDetectHistory.Refresh();

                RefreshReportButton(btnProcessHistory, btnReactionHistory);
                RefreshReportButton(btnReactionHistory, btnSMSHistory);
                RefreshReportButton(btnSMSHistory, btnProcessHistory);  
            }
            else if (btn == btnProcessHistory)
            {
                panelMiddle.Visible = false;
                panelProcessHistory.Visible = true;
                panelReactionHistory.Visible = false;

                m_PageHome.FrmReport.ShowProcessHistoryReport();

                btnProcessHistory.IsChecked = true;
                btnProcessHistory.Refresh();
            
                RefreshReportButton(btnDetectHistory, btnReactionHistory);
                RefreshReportButton(btnReactionHistory, btnSMSHistory);
                RefreshReportButton(btnSMSHistory, btnDetectHistory);           

            }

            else if(btn == btnReactionHistory)
            {
                DateTime dtToday = DateTime.Now;

                SetReactionEndDate(dtToday, dtToday);
                SetReactionStartDate(dtToday.Subtract(TimeSpan.FromDays(7)), dtToday);

                panelMiddle.Visible = false;
                panelProcessHistory.Visible = false;
                panelReactionHistory.Visible = true;

                m_PageHome.FrmReport.ShowReactionHistoryReport();             

                btnReactionHistory.IsChecked = true;
                btnReactionHistory.Refresh();

                RefreshReportButton(btnDetectHistory, btnProcessHistory);
                RefreshReportButton(btnProcessHistory, btnSMSHistory);
                RefreshReportButton(btnSMSHistory, btnDetectHistory);           
            }
            else if (btn == btnSMSHistory)
            {
                panelMiddle.Visible = false;
                panelProcessHistory.Visible = true;
                panelReactionHistory.Visible = false;

                m_PageHome.FrmReport.ShowSmsHistoryReport();

                btnSMSHistory.IsChecked = true;
                btnSMSHistory.Refresh();

                RefreshReportButton(btnDetectHistory, btnProcessHistory);              
                RefreshReportButton(btnProcessHistory, btnReactionHistory);
                RefreshReportButton(btnDetectHistory, btnDetectHistory);           
            }
        }

        private void RefreshReportButton(RibbonButton btn1, RibbonButton btn2)
        {
            if (btn1.IsChecked == true)
            {
                btn1.IsChecked = false;
                btn1.Refresh();
            }
            else if (btn2.IsChecked == true)
            {
                btn2.IsChecked = false;
                btn2.Refresh();
            }
        }

        private void Proc_cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (proc_cboBuildingGroup.SelectedIndex == 0)
            {
                proc_cboBuilding.SelectedIndex = 0;
                proc_cboFloor.SelectedIndex = 0;
                proc_cboBuilding.Enabled = false;
                proc_cboFloor.Enabled = false;
                return;
            }
            proc_cboBuilding.Enabled = true;            

            int nSelectedIndex = proc_cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;
			          
            BuildingGroup buildingGroup = (BuildingGroup)proc_cboBuildingGroup.Items[nSelectedIndex];
			
			ComboHelper.InitBuildingComboBox(proc_cboBuilding, buildingGroup);
			proc_cboBuilding.Items.Insert(0, "모든 건물");
			
            if (proc_cboBuilding.Items.Count > 0)
                proc_cboBuilding.SelectedIndex = 0;
        }

        private void proc_cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (proc_cboBuilding.SelectedIndex == 0)
            {
                proc_cboFloor.SelectedIndex = 0;
                proc_cboFloor.Enabled = false;
                return;
            }
            proc_cboFloor.Enabled = true;

            int nSelectedIndex = proc_cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;
			            		
            Object obj = proc_cboBuilding.Items[nSelectedIndex];
            if (obj.GetType() == typeof(Building))
            {
				ComboHelper.InitFloorComboBox(proc_cboFloor, (Building)obj);
				proc_cboFloor.Items.Insert(0, "모든 층");                
            }
            else
            {
				proc_cboFloor.Items.Clear();
                proc_cboFloor.Items.Add("-");
            }

            if (proc_cboFloor.Items.Count > 0)
                proc_cboFloor.SelectedIndex = 0;
        }

        private bool IsDate(string sDate)
        {
            try
            {
                DateTime dtDate = DateTime.Parse(sDate);
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        private void proc_btnStartDate_Click(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            if (IsDate(proc_btnStartDate.Text))
                DatePickerStart.EnsureVisible(System.Convert.ToDateTime(proc_btnStartDate.Text));

            DatePickerStart.Left = proc_btnStartDate.Left - panelLeft.Width;
            DatePickerStart.Top = (proc_btnStartDate.Top + proc_btnStartDate.Height) - panelMiddle.Height;

            int nCount = 0;
            if (DatePickerStart.ShowModal(1, 1))
            {
                nCount = DatePickerStart.Selection.BlocksCount;
                if (nCount > 0)
                {
                    proc_btnStartDate.Refresh();
                    string szText = DatePickerStart.Selection[0].DateBegin.ToShortDateString();
                    DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
                    
                    if (dtszText > dtToday)
                    {
                        MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                        return;
                    }

                    proc_btnStartDate.Text = szText;
                }
            }
            proc_btnStartDate.Refresh();
        }

        private void proc_btnEndDate_Click(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            if (IsDate(proc_btnEndDate.Text))
                DatePickerEnd.EnsureVisible(System.Convert.ToDateTime(proc_btnEndDate.Text));

            DatePickerEnd.Left = proc_btnEndDate.Left - panelLeft.Width;
            DatePickerEnd.Top = (proc_btnEndDate.Top + proc_btnEndDate.Height) - panelMiddle.Height;

            int nCount = 0;
            if (DatePickerEnd.ShowModal(1, 1))
            {
                nCount = DatePickerEnd.Selection.BlocksCount;
                if (nCount > 0)
                {
                    proc_btnEndDate.Refresh();
                    string szText = DatePickerEnd.Selection[0].DateBegin.ToShortDateString();

                    DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
                    

                    if (dtszText > dtToday)
                    {
                        MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                        return;
                    }
                    proc_btnEndDate.Text = szText;
                }
            }
            proc_btnEndDate.Refresh();
        }

        public void proc_cboLatelyDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (proc_cboLatelyDate.SelectedIndex == 0)
                return;

            DateTime dt = DateTime.Now;
            DateTime dtOld = new DateTime();

            if (proc_cboLatelyDate.SelectedIndex == 1)
                dtOld = dt.AddMonths(-6);
            else if(proc_cboLatelyDate.SelectedIndex == 2)
                dtOld = dt.AddMonths(-3);
            else if (proc_cboLatelyDate.SelectedIndex == 3)
                dtOld = dt.AddMonths(-1);

            proc_btnStartDate.Text = dtOld.ToString().Substring(0, 10);
            proc_btnEndDate.Text = dt.ToString().Substring(0, 10);
        }

        private void react_btnStartDate_Click(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            if (IsDate(react_btnStartDate.Text))
                DatePickerStart.EnsureVisible(System.Convert.ToDateTime(react_btnStartDate.Text));

            DatePickerStart.Left = react_btnStartDate.Left - panelLeft.Width;
            DatePickerStart.Top = (react_btnStartDate.Top + react_btnStartDate.Height) - panelMiddle.Height;

            int nCount = 0;
            if (DatePickerStart.ShowModal(1, 1))
            {
                nCount = DatePickerStart.Selection.BlocksCount;
                if (nCount > 0)
                {
                    SetReactionStartDate(DatePickerStart.Selection[0].DateBegin, dtToday);
                }
            }           
            react_btnStartDate.Refresh();
        }

        private void SetReactionStartDate(DateTime dtTime, DateTime dtToday)
        {
            react_btnStartDate.Refresh();
            string szText = dtTime.ToShortDateString();

            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
            DateTime dtToday_Compare = DateTime.ParseExact(dtToday.ToShortDateString(), "yyyy-MM-dd", null);
            if (dtszText > dtToday_Compare)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                react_btnStartDate.Text = DateTime.Now.ToShortDateString();
                return;
            }

            if (react_btnStartDate.Text == "시작 일" || react_btnEndDate.Text == "끝 일")
            {
            }
            else
            {
                DateTime dtEndDate = DateTime.ParseExact(react_btnEndDate.Text, "yyyy-MM-dd", null);
                if (dtEndDate < dtszText)
                {
                    MessageBox.Show("시작 일이 끝 일보다 클 수 없습니다.");
                    return;
                }
            }
            react_btnStartDate.Text = szText;
        }

        private void react_btnEndDate_Click(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            if (IsDate(react_btnEndDate.Text))
                DatePickerEnd.EnsureVisible(System.Convert.ToDateTime(react_btnEndDate.Text));

            DatePickerEnd.Left = react_btnEndDate.Left - panelLeft.Width;
            DatePickerEnd.Top = (react_btnEndDate.Top + react_btnEndDate.Height) - panelMiddle.Height;

            int nCount = 0;
            if (DatePickerEnd.ShowModal(1, 1))
            {
                nCount = DatePickerEnd.Selection.BlocksCount;
                if (nCount > 0)
                {
                    SetReactionEndDate(DatePickerEnd.Selection[0].DateBegin, dtToday);
                }
            }           
            react_btnEndDate.Refresh();
        }

        private void SetReactionEndDate(DateTime dtTime, DateTime dtToday)
        {
            react_btnEndDate.Refresh();
            string szText = dtTime.ToShortDateString();

            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
            DateTime dtToday_Compare = DateTime.ParseExact(dtToday.ToShortDateString(), "yyyy-MM-dd", null);
            if (dtszText > dtToday_Compare)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                react_btnEndDate.Text = DateTime.Now.ToShortDateString();
                return;
            }

            if (react_btnStartDate.Text == "시작 일" || react_btnEndDate.Text == "끝 일")
            {
            }
            else
            {
                DateTime dtStartDate = DateTime.ParseExact(react_btnStartDate.Text, "yyyy-MM-dd", null);
                if (dtStartDate > dtszText)
                {
                    MessageBox.Show("시작 일이 끝 일보다 클 수 없습니다.");
                    return;
                }
            }

            react_btnEndDate.Text = szText;          
        }


        private void proc_btnSelectZone_Click(object sender, EventArgs e)
        {
            if (proc_btnStartDate.Text == "시작 일" || proc_btnEndDate.Text == "끝 일")
            {
                MessageBox.Show("날짜를 입력해주세요");
                return;
            }

            DateTime startDate = DateTime.ParseExact(proc_btnStartDate.Text, "yyyy-MM-dd", null);
            DateTime EndDate = DateTime.ParseExact(proc_btnEndDate.Text, "yyyy-MM-dd", null);

            if (startDate > EndDate)
            {
                MessageBox.Show("시작 일이 더 클 수 없습니다.");
                return;
            }

            string strGroupName = proc_cboBuildingGroup.Text.ToString();
            string strBuildingName = proc_cboBuilding.Text.ToString();
            string strFloorName = proc_cboFloor.Text.ToString();
            
            m_PageHome.FrmReport.SelectReport(strGroupName, strBuildingName, strFloorName, startDate, EndDate);
        }

        public void ChangeZoneComboBox(Zone zone)
        {
            if (zone.Building == null)
                return;

            if (zone.Building.BuildingGroup == null)
                return;

            cboBuildingGroup.SelectedItem = zone.Building.BuildingGroup;
            cboBuilding.SelectedItem = zone.Building;

            foreach (Floor floor in cboFloor.Items)
            {
                if (floor.ToString() == zone.Floor.ToString())
                {
                    cboFloor.SelectedItem = floor;
                    break;
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            MainFrame.Close();
        }
		    
        public void ShowLeftThumbnail()
        {
            btnLayerFire.Visible = false;
            btnLayerSpringCooler.Visible = false;
            btnLayerPump.Visible = false;
            btnLayerCCTV.Visible = false;
            btnLayerFE.Visible = false;
            btnLayerHD.Visible = false;
            btnLayerFA.Visible = false;
            btnLayerFR.Visible = false;
            btnLayerLowCCTV.Visible = false;

            labelFire.Visible = false;
            labelCooler.Visible = false;
            labelPump.Visible = false;
            labelCCTV.Visible = false;
            labelFE.Visible = false;
            labelHD.Visible = false;
            labelFA.Visible = false;
            labelFR.Visible = false;
            labelCCTVLow.Visible = false;

            int nLineThick = 5;
            int nFrmWidth = m_frmOutdoor.Size.Width;
            int nFrmHeight = (panelLeft.Size.Height - nLineThick * 4) / 3;

            panelLeft.Size = new Size(m_frmOutdoor.Size.Width + nLineThick, panelLeft.Size.Height);

            m_frmOutdoor.Size = new Size(nFrmWidth, nFrmHeight);
            m_frmOutdoor.Location = new Point(nLineThick, nLineThick);

            m_frmIndoor.Size = new Size(nFrmWidth, nFrmHeight);
            m_frmIndoor.Location = new Point(nLineThick, nLineThick + nFrmHeight + nLineThick);

            m_frmCCTVGuide.Size = new Size(nFrmWidth, nFrmHeight);
            m_frmCCTVGuide.Location = new Point(nLineThick, nLineThick * 3 + nFrmHeight * 2);

            panelLeft.BackgroundImage = null;
            panelLeft.BackColor = Color.FromArgb(51, 71, 103);

            m_frmOutdoor.AttachView(PageHome.ContentForm.DetachView(true));
            m_frmIndoor.AttachView(PageHome.ContentForm.DetachView(false));

            m_frmOutdoor.Visible = true;
            m_frmIndoor.Visible = true;
            m_frmCCTVGuide.Visible = true;

            m_isThumbnailMode = true;
            FormMain_Resize(null, null);
        }

        public void ShowLeftLayer()
        {
			if (!m_isThumbnailMode)
				return;

            btnLayerFire.Visible = true;
            btnLayerSpringCooler.Visible = true;
            btnLayerPump.Visible = true;
            btnLayerCCTV.Visible = true;
            btnLayerFE.Visible = true;
            btnLayerHD.Visible = true;
            btnLayerFA.Visible = true;
            btnLayerFR.Visible = true;
            btnLayerLowCCTV.Visible = true;

            labelFire.Visible = true;
            labelCooler.Visible = true;
            labelPump.Visible = true;
            labelCCTV.Visible = true;
            labelFE.Visible = true;
            labelHD.Visible = true;
            labelFA.Visible = true;
            labelFR.Visible = true;
            labelCCTVLow.Visible = true;

            panelLeft.Size = new Size(m_nOriginLeftPanelWidth, panelLeft.Size.Height);

            panelLeft.BackgroundImage = SDMS.Properties.Resources.VToolbar_bkgnd;
            panelLeft.BackColor = Color.Transparent;

            m_frmOutdoor.Visible = false;
            m_frmIndoor.Visible = false;
            m_frmCCTVGuide.Visible = false;

            PageHome.ContentForm.AttachView(m_frmOutdoor.DetachView(), true);
            PageHome.ContentForm.AttachView(m_frmIndoor.DetachView(), false);

            m_isThumbnailMode = false;
            FormMain_Resize(null, null);
        }


		private Zone SetDetectZoneName(int nHistoryID)
		{
			if (nHistoryID == -1)
			{
				mLabelZone.Text = "";
				return null;
			}

			int nSensorID = SensorHistoryManager.Instance.GetSensorID(nHistoryID);
			if (nSensorID != -1)
			{
				SensorZone sensor = SensorManager.Instance.FindSensor(nSensorID);
				if (sensor != null)
				{
					int nEquipZoneID = sensor.EquipZoneID;
					EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
					if (zone != null)
					{
						if (zone.Building != null)
						{
							string text1 = zone.Building.BuildingGroup.BuildingGroupName;
							string szZoneName = zone.LinkedZone.BroadcastName;
							mLabelZone.Text = text1 + "," + szZoneName;
							return zone.LinkedZone;
						}						
					}
					else
					{
						mLabelZone.Text = "";
					}
				}
				else
				{
					mLabelZone.Text = "";
				}
			}
			else
			{
				mLabelZone.Text = "";
			}
			return null;
		}

		public void BeginFireProcess(ReactionLog log)
		{
			int nHistoryID = log.SensorHistoryID;
			if (log.Message.IndexOf("[훈련상황]") != -1)
				mLabelStatus.Text = "[훈련]화재 발생";
			else
				mLabelStatus.Text = "화재 발생"; 
			((RealTimeInfoPane)panelLog).TextColor = Color.Red;
			mLabelStatus.ForeColor = Color.Red;
			mLabelZone.ForeColor = Color.Red;

			Zone zone = SetDetectZoneName(nHistoryID);
			if (zone != null)
			{

			}
			
		}

		public void EndFireProcess(ReactionLog log)
		{
			int nHistoryID = log.SensorHistoryID;
			int nSensorID = SensorHistoryManager.Instance.GetSensorID(nHistoryID);
			if (nSensorID != -1)
			{
				FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.GetProcess(nSensorID);
				
				ProcessManager.Instance.EndProcess(nSensorID);				
			}
			SetNormalMode(log);
		}

		public void SetFireDetectMode(ReactionLog log)
		{
			int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
				mLabelStatus.Text = "[훈련]화재 탐지";
			else
				mLabelStatus.Text = "화재 탐지"; 
			((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
			mLabelStatus.ForeColor = Color.Orange;
			mLabelZone.ForeColor = Color.Orange;
			//mLabelZone.Text = "";

			SetDetectZoneName(nHistoryID);			
		}

		public void SetRunSOPMode(ReactionLog log)
        {
			int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
				mLabelStatus.Text = "[훈련]SOP 실행중";
			else
				mLabelStatus.Text = "SOP 실행중"; 

            ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            mLabelStatus.ForeColor = Color.Orange;
            mLabelZone.ForeColor = Color.Orange;

            SetDetectZoneName(nHistoryID);
        }

		public void SetRunNCancelSOPMode(ReactionLog log)
        {
			int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
				mLabelStatus.Text = "[훈련]상황종료(SOP 실행취소)";
			else
				mLabelStatus.Text = "상황종료(SOP 실행취소)";    
            //mLabelStatus.Text = "상황종료(SOP 실행취소)";

            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

		public void SetFinishSOPMode(ReactionLog log)
        {
			int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
				mLabelStatus.Text = "[훈련]상황종료(SOP 종료)";
			else
				mLabelStatus.Text = "상황종료(SOP 종료)";            
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

        public void SetIgnoreSOPMode(ReactionLog log)
        {
			int nHistoryID = log.SensorHistoryID;
			if( log.Message.IndexOf("[훈련상황]") != -1)
				mLabelStatus.Text = "[훈련]상황종료";
			else
				mLabelStatus.Text = "상황종료";
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

		public void SetNormalMode(int nHistoryID)
		{
			mLabelStatus.Text = "화재 탐지 없음";
			((RealTimeInfoPane)panelLog).TextColor = Color.White;
			mLabelStatus.ForeColor = Color.White;
			mLabelZone.ForeColor = Color.White;
			SetDetectZoneName(-1);

			((RealTimeInfoPane)panelLog).RealTimeInfo = "";
			((RealTimeInfoPane)panelLog).DrawMovingText();

			PageHome.ContentForm.HideZoneVolume();
           // PageHome.ContentForm.HideAllPOIPopup();
		}
		public void SetNormalMode(ReactionLog log)
		{
			int nHistoryID = log.SensorHistoryID;
			mLabelStatus.Text = "화재 탐지 없음";
			((RealTimeInfoPane)panelLog).TextColor = Color.White;
			mLabelStatus.ForeColor = Color.White;
			mLabelZone.ForeColor = Color.White;
			SetDetectZoneName(-1);

            ((RealTimeInfoPane)panelLog).RealTimeInfo = "";
            ((RealTimeInfoPane)panelLog).DrawMovingText();

			PageHome.ContentForm.HideZoneVolume();
            //PageHome.ContentForm.HideAllPOIPopup();
		}

		public void AddLogMessage(ReactionLog log)
		{
			if( log == null)
				return;
			((RealTimeInfoPane)panelLog).RealTimeInfo = log.ToString();
			mLabelLog.Text = "";
			mLabelLog.Tag = log;
			((RealTimeInfoPane)panelLog).DrawMovingText();
		}

        private void btnFire_Click(object sender, EventArgs e)
        {
			
			btnFire.Enabled = false;

			if (btnFire.Text == "화재 신고")
			{
				Zone zone = PageBackstageHome.Instance.ContentForm.ManualClickZone;
				
				ArrayList arEquipZone = ZoneManager.Instance.GetEquipmentZoneList(zone);

				FormMain.Instance.PageHome.ShowBigCCTV(zone, true, true);

				if (zone != null)
				{
					int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
					NetworkManager.Instance.SendMessage(1, TCP_ID.FIRE_DETECT_REPORT, 0, zone.ID, 0, nSOPGenUserID);
					SendFireDetectMessageToSOPSimulator();
				}
			}
			else
			{
				if (CurrentFireDetectProcess != null)
				{
					int nHistoryID = CurrentFireDetectProcess.SensorHistoryID;
					int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
					NetworkManager.Instance.SendMessage(1, TCP_ID.CLEAR_DETECT_REPORT, nHistoryID, nSOPGenUserID);
				}
			}
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            m_PageHome.FrmReport.SaveHWP2();
            button1.Enabled = true;
        }

        private void process_Exited(object sender, EventArgs e)
        {           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btnSaveHWP.Enabled = false;
            m_PageHome.FrmReport.SaveHWP();
            btnSaveHWP.Enabled = true;
        }

		private void btnFire_KeyDown(object sender, KeyEventArgs e)
		{
			FormMain.Instance.EnableFireReportBtn(false);
		}
		
		public void SendFireDetectMessageToSOPSimulator()
		{
			Debug.WriteLine("Run Simulator");
			bool bRun = FormSMSConfig.ReadRunSimulator();
			if (bRun == true)
			{
				string strFilePath = Application.StartupPath + "\\SOPMonitoringSystem.exe";

				if (RunCheckProcess(strFilePath))
				{
				}
				else
					RunStartProcess(strFilePath, m_nSOPGenUserID.ToString() + " " + m_strSOPGenUserRealName);
			}			
		}

		private void RunStartProcess(string strFileName, string args)
		{
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.FileName = strFileName;
			startInfo.WorkingDirectory = Application.StartupPath;
			startInfo.ErrorDialog = true;
			System.Diagnostics.Process process;
			try
			{
				process = System.Diagnostics.Process.Start(strFileName, args);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private bool RunCheckProcess(string strProcessName)
		{
			int nIndex2 = strProcessName.LastIndexOf('.');
			int nIndex1 = strProcessName.LastIndexOf('\\');

			if (nIndex1 < 0 || nIndex2 < 0)
				return false;

			strProcessName = strProcessName.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

			System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

			foreach (System.Diagnostics.Process process in processList)
			{
				if (process.ProcessName == strProcessName  )
					return true;

				/*if (process.ProcessName  ==(strProcessName + ".vshost"))
					return true;*/
			}

			return false;
		}



		private void cmbFireDetect_SelectionChangeCommitted(object sender, EventArgs e)
		{
			FireDetectProcess process = (FireDetectProcess)cmbFireDetect.SelectedItem;
			if (process != null)
			{
				bool bSelected = process.Select();
				if (bSelected)
				{
					int nSensorID = process.DetectSensorID;
					int nHistoryID = process.SensorHistoryID;
					if (!ShowEquipZoneCCTV)
					{
                        if (process.LastLog.ReactionType == (int)(ReactionType.BEGIN_STATUS))
                        {
                            SeletCaseData form = new SeletCaseData(process.TargetSensor, nHistoryID);
                            ConfirmDialogManager.Instance.AddDialogFirst(form);
                            ConfirmDialogManager.Instance.ShowDialogNext();
                        }
                        else
                        {
                            ConfirmDialogManager.Instance.CloseAllDialog();
                        }
					}						
					ReactionLogManager.Instance.ProcessLog(process.LastLog, true);

					if (process.TargetZone != null)
					{
						Zone zone = process.TargetZone.LinkedZone;
						if (zone != null)
						{
							int nZoneId = zone.ID;
							int nMode = nSensorID / 1000;
							// 수동 신고임
							if (nMode == nZoneId)
							{
								btnFire.Text = "화재 종료";
								btnFire.Enabled = true;
							}
							else
							{
								btnFire.Text = "화재 신고";
								btnFire.Enabled = false;
							}
						}
					}
				}
			}
		}

		public void RemoveFireDetect(FireDetectProcess process)
		{
			int nCurIdx = cmbFireDetect.SelectedIndex;

			cmbFireDetect.Items.Remove(process);
			Debug.WriteLine(process);
			int nCount = cmbFireDetect.Items.Count;
			if (nCount > 0)
			{
				cmbFireDetect.SelectedIndex = (nCount - 1);
				FireDetectProcess processSelect = (FireDetectProcess)cmbFireDetect.SelectedItem;
				if (processSelect != null)
				{
					bool bSelected = processSelect.Select();
					if (bSelected)
					{						
						ReactionLogManager.Instance.ProcessLog(processSelect.LastLog, true);
					}


					if (!ShowEquipZoneCCTV)
					{
                        if (processSelect != null && processSelect.LastLog != null)
                        {
                            if (processSelect.LastLog.ReactionType == (int)(ReactionType.BEGIN_STATUS))
                            {
                                int nSensorID = processSelect.DetectSensorID;
                                int nHistoryID = processSelect.SensorHistoryID;
                                SeletCaseData form = new SeletCaseData(processSelect.TargetSensor, nHistoryID);
                                ConfirmDialogManager.Instance.AddDialogFirst(form);
                                SeletCaseData form2 = ConfirmDialogManager.Instance.ShowDialogNext();
                                if (form2 != null)
                                {
                                    int nID = form2.SensorHistoryID;
                                    int nSensorID2 = form2.Sensor.ID;
                                    FormMain.Instance.SelectFireDetectProcess(nID, nSensorID2);
                                }
                            }
                        }
					}
				}
			}
            else
            {
                SetNormalMode(0);

                OnClickToolBarButton(btnOutside, null);
                OnClickToolBarButton(btnHome, null);
            }

            process.HideCCTV();
		}

		public void AddFireDectect(FireDetectProcess process, bool bAddSelect = true )
		{
			int nCurIdx = cmbFireDetect.SelectedIndex;

			if (!cmbFireDetect.Items.Contains(process))
			{
				int nIdx = cmbFireDetect.Items.Add(process);

				if (bAddSelect == true)
				{
					cmbFireDetect.SelectedIndex = (nIdx);
					FireDetectProcess processSelect = (FireDetectProcess)cmbFireDetect.SelectedItem;
					if (processSelect != null)
					{
						bool bSelected = processSelect.Select();
						if (bSelected)
						{
							ReactionLogManager.Instance.ProcessLog(processSelect.LastLog, true);
						}

					}
				}
				else
				{
					if (nCurIdx != -1)
						cmbFireDetect.SelectedIndex = nCurIdx;
				}
				
			}
		}

        private void btnLayerLowCCTV_Click(object sender, EventArgs e)
        {

        }

        private void btnLayerLowCCTV_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBoxEquipZoneCCTV_CheckedChanged(object sender, EventArgs e)
        {
            SetEquipZoneCCTV(checkBoxEquipZoneCCTV.Checked);
        }

		private void SetEquipZoneCCTV(bool enable)
        {
            if (m_isVisibleEquipZoneCCTV == enable)
                return;

            m_isVisibleEquipZoneCCTV = enable;

            ResizeEquipZoneCCTVControl(enable, true);
        }

        private void ResizeEquipZoneCCTVControl(bool enable, bool showCCTV)
        {
            int nGap = cboBuildingGroup.Location.X - (labelSelectZone.Location.X + labelSelectZone.Size.Width);

            cboEquipZone.Size = new Size(323, cboEquipZone.Size.Height);

            if (enable)
            {
                cboEquipZone.Visible = true;
                labelFireDetect.Visible = false;
                cmbFireDetect.Visible = false;

                cboEquipZone.Location = new Point(btnSelectZone.Location.X - nGap - cboEquipZone.Size.Width, cboFloor.Location.Y);
                cboFloor.Location = new Point(cboEquipZone.Location.X - nGap - cboFloor.Size.Width, cboFloor.Location.Y);
            }
            else
            {
                cboEquipZone.Visible = false;
                labelFireDetect.Visible = true;
                cmbFireDetect.Visible = true;

                cboFloor.Location = new Point(btnSelectZone.Location.X - nGap - cboFloor.Size.Width, cboFloor.Location.Y);
            }

            cboBuilding.Location = new Point(cboFloor.Location.X - nGap - cboBuilding.Size.Width, cboBuilding.Location.Y);
            cboBuildingGroup.Location = new Point(cboBuilding.Location.X - nGap - cboBuildingGroup.Size.Width, cboBuildingGroup.Location.Y);
            labelSelectZone.Location = new Point(cboBuildingGroup.Location.X - nGap - labelSelectZone.Size.Width, labelSelectZone.Location.Y);

            if (enable && showCCTV)
                PageHome.ShowEquipZoneCCTVs(CurrentEquipZone);
            
        }

        private void cboFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboEquipZone.Items.Clear();

            Object obj = cboFloor.Items[nSelectedIndex];
            Type type = obj.GetType();

            Zone zone = null;

            if (type == typeof(Floor))
            {
                Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];
                Floor floor = (Floor)obj;
                zone = ZoneManager.Instance.GetZone(building.BuildingID, floor.FloorIndex);
            }

            if (zone == null || zone.ID <= 0)
                return;

            ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
            if (arrEquipZones == null)
                return;

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                cboEquipZone.Items.Add(equipZone);
            }

            if (cboEquipZone.Items.Count > 0)
                cboEquipZone.SelectedIndex = 0;
        }

        private void cboEquipZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageHome.ShowEquipZoneCCTVs(CurrentEquipZone);
        }

        public ArrayList GetContorl()
        {
            ArrayList arContorl = new ArrayList();           
            arContorl.Add(react_btnEndDate);
            arContorl.Add(react_btnStartDate);
            arContorl.Add(react_cboSearchType);
            arContorl.Add(cboFireSelect);
            arContorl.Add(react_cboEndTime);
            arContorl.Add(react_cboStartTime);
            return arContorl;  
        }

        private void btnShowCCTVList_Click(object sender, EventArgs e)
        {
            m_frmCCTVList = new FormCCTVList();
            m_frmCCTVList.Show();
        }

        private void btnSensorMonitor_Click(object sender, EventArgs e)
        {
            FormReciverState frm = new FormReciverState();
            PageBackstageHome.ShowTranslucentForm(frm, 300, 200, frm.Size.Width, frm.Size.Height, ID.ID_VIEW_SENSOR_MONITOR);
        }

        private int nReciveReadCount = 0;
		private void m_CheckReciver_Tick(object sender, EventArgs e)
        {
            nReciveReadCount++;
            if (nReciveReadCount == 7)
            {
               ReciverManager.Instance.LoadReciverList();
               nReciveReadCount = 0;
            }

            try
            {

                ArrayList arReciverList = ReciverManager.Instance.GetReciverList();

                bool bFailConnection = false;
                int nFailCount = 0;
                string szText = "수신반 : ";
                foreach (Reciver reciver in arReciverList)
                {
                    if (reciver.State <= 0)
                    {
                        bFailConnection = true;
                        nFailCount++;
                    }
                }

                if (bFailConnection == true)
                {
                    szText += string.Format("통신 상태 불량 ({0})", nFailCount);
                    labelSensorMonitor.Text = szText;
                    labelSensorMonitor.ForeColor = Color.Red;
                }
                else
                {
                    szText += "통신 상태 양호";
                    labelSensorMonitor.Text = szText;
                    labelSensorMonitor.ForeColor = Color.Green;
                }
            }
            catch (System.Exception ex)
            {
            	
            }

        }

		private void cmbFireDetect_SelectedIndexChanged(object sender, EventArgs e)
		{
			FireDetectProcess process = (FireDetectProcess)cmbFireDetect.SelectedItem;
			if (process != null)
			{
				if (process.TargetZone != null)
				{
					int nSensorID = process.DetectSensorID;
					int nHistoryID = process.SensorHistoryID;
					Zone zone = process.TargetZone.LinkedZone;
					if (zone != null)
					{
						int nZoneId = zone.ID;
						int nMode = nSensorID / 1000;
						// 수동 신고임
						if (nMode == nZoneId)
						{
							btnFire.Text = "화재 종료";
							btnFire.Enabled = true;
						}
						else
						{
							btnFire.Text = "화재 신고";
							btnFire.Enabled = false;
						}
					}
					else
					{
						int i = 0;
						i++;
					}
				}				
			}
		}

		private void btnSaveHome_Click(object sender, EventArgs e)
		{
			BaseViewEx view = PageBackstageHome.Instance.ContentForm.OutdoorView;
			if (view != null)
			{
				view.SaveHomeView();				
			}
		}

        private void toolStripMenuItemBig_Click(object sender, EventArgs e)
        {
            SetCCTVMode(CCTVMode.CCTV_ONLY);
            
            ResizePanels();

            if (PageHome.CCTVForm != null)
            {
                if (PageBackstageHome.TranslucentForm.InnerForm != null)
                {
                    if (PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(Form4CCTV))
                    {
                        int x = PageHome.CCTVForm.Location.X;
                        int y = PageHome.CCTVForm.Location.Y;
                        int width = PageHome.CCTVForm.Size.Width;
                        int height = PageHome.CCTVForm.Size.Height;
                        PageBackstageHome.TranslucentForm.ResizeInner(x, y, width, height);
                    }
                }
            }  
        }

        private void toolStripMenuItemNormal_Click(object sender, EventArgs e)
        {
            SetCCTVMode(CCTVMode.NORMAL);
            ResizePanels();

            if( PageHome.CCTVForm != null)
            {
                if (PageBackstageHome.TranslucentForm.InnerForm != null)
                {
                    if (PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(Form4CCTV))
                    {
                        int x = PageHome.CCTVForm.Location.X;
                        int y = PageHome.CCTVForm.Location.Y;
                        int width = PageHome.CCTVForm.Size.Width;
                        int height = PageHome.CCTVForm.Size.Height;
                        PageBackstageHome.TranslucentForm.ResizeInner(x, y, width, height);
                    }
                }                
            }            
        }

        private void panelMiddle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(panelMiddle, e.X, e.Y);
            }
        }

        public CCTVMode GetCCTVMode()
        {
            return m_cctvMode;
        }

        public void SetCCTVMode(CCTVMode mode)
        {
            if (m_cctvMode == mode)
                return;

            m_cctvMode = mode;

            if (m_isThumbnailMode)
            {
                m_PageHome.SetCCTVMode(mode);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string strIDList = "";

            foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
            {
                if (strIDList.Length == 0)
                    strIDList = pair.Value.TargetSensor.ID.ToString();
                else
                    strIDList += "\r\n" + pair.Value.TargetSensor.ID.ToString();
            }

            MessageBox.Show("CurrentSensorIDList\r\n" + strIDList);
        }

        public void PerformClickSelectReport()
        {
            proc_btnSelectZone.PerformClick();
        }
	}
}
