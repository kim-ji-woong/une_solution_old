using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;
using UnE.View.Content;

namespace SDMS
{
    public partial class PageBackstageHome : Form, IChangedDataManager, IBaseViewOwner, IFormContentOwner
	{
		public const int OUTSIDE = 1;
		public const int BOTHSIDE = 2;
		public const int INSIDE = 3;

        private const string RightDockingPaneOptionTag = "RightDockingPaneOption";
        private const string SDMSUIOptionSection = "SDMSUIOption";

		//public enum Tab { FILE_TAB = 0, M3D_TAB, ADMIN_TAB, REPORT_TAB, M2D_TAB, CCTV_TAB, BOTH};
        //public enum TabRightDockingMode { NONE = 0, SHOW_PSM, SHOW_LOCATION, SHOW_CCTV };

		//////////////////////////////////////////////////////////////////////////
		// Menu check items
		private bool m_bShowDetectorLayer = true;

		private bool m_bShowCoolerLayer = true;
		private bool m_bShowPressureLayer = true;
		private bool m_bShowCCTVLayer = true;
		private bool m_bShowFireExtinguisherLayer = true;
		private bool m_bShowFireHydrantLayer = true;
		private bool m_bShowAlarmStationLayer = true;
		private bool m_bShowReciverLayer = true;
		private bool m_bShowTextPOILayer = true;
		private bool m_bShowLowCCTVLayer = false;
        private bool m_bShowBuildingTextLayer = true; 
        private bool m_bShowDisconnectedCCTVLayer = false;
        private bool m_bShowNotice = false;
		private Dictionary<int, bool> m_dicCheckedItem = new Dictionary<int, bool>();
		//////////////////////////////////////////////////////////////////////////

        private NoProcessDisaster m_noProcessDisaster = new NoProcessDisaster();

        private IFormContent m_ContentForm = null;

        public IFormContent ContentForm
		{
			get { return m_ContentForm; }
		}

		private FormReport m_ReportForm = null;

		public FormReport FrmReport
		{
			get { return m_ReportForm; }
		}
        public void OnBeepFinish()
        {
            if (EarthquakeSoundProcess.SoundPlayer.SoundLocation != null)
                EarthquakeSoundProcess.SoundPlayer.Stop();
        }
		private static PageBackstageHome m_home = null;

		public static PageBackstageHome Instance
		{
			get { return m_home; }
		}

		private DockingFormLocation m_dockFormLocation = null;

		public SDMS.DockingFormLocation DockFormLocation
		{
			get { return m_dockFormLocation; }
			set { m_dockFormLocation = value; }
		}

		private DockingFormProperties m_dockFormProperties = null;

		public SDMS.DockingFormProperties DockFormProperties
		{
			get { return m_dockFormProperties; }
			set { m_dockFormProperties = value; }
		}

		private DockingFormFacilityList m_dockFormFacilityList = null;

		public SDMS.DockingFormFacilityList DockFormFacilityList
		{
			get { return m_dockFormFacilityList; }
			set { m_dockFormFacilityList = value; }
		}

		private POI m_poiSelected = null;

		public POI SelectedPOI
		{
			get { return m_poiSelected; }
			set
			{
				m_poiSelected = value;
				ShowRightDockingPane();
			}
		}

        //private UnE.CCTV.CCTVFormFrame m_frameCCTV = null;


        //private Form4CCTV m_frm4CCTV = null;
        //public SDMS.Form4CCTV CCTVForm
        //{
        //    get { return m_frm4CCTV; }
        //    set { m_frm4CCTV = value; }
        //}

		private int m_nPrevLocationPaneHeight = -1;
		private int m_nPrevPropertyPaneHeight = -1;
		private int m_nPrevFacilityPaneHeight = -1;
		private bool m_isClosedLocationPane = false;
		private bool m_isClosedPropertyPane = false;
		private bool m_isClosedFacilityPane = false;

		private bool m_isChangedFacilityManager = false;
		public bool IsChangedFacilityManager
		{
			get { return m_isChangedFacilityManager; }
			set { m_isChangedFacilityManager = value; }
		}

		private bool m_isChangedEquipZoneCCTV = false;
		public bool IsChangedEquipZoneCCTV
		{
			get { return m_isChangedEquipZoneCCTV; }
			set { m_isChangedEquipZoneCCTV = value; }
		}

		private ArrayList m_arrChangedData = new ArrayList();
		private static int m_nTranslucentCommandID = -1;

        private bool m_bFireDetectCCTVMode = false;
        public bool FireDetectCCTVMode
        {
            get { return m_bFireDetectCCTVMode; }
        }

		private static PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();

		public static SDMS.PopupTranslucentForm TranslucentForm
		{
			get { return mTranslucentForm; }
			set { mTranslucentForm = value; }
		}

        private PopupDialog.FormPSMList m_frmPSMList = new PopupDialog.FormPSMList();
        private PopupDialog.FormCCTVList m_frmCCTVList = new PopupDialog.FormCCTVList();
        private PopupDialog.DisasterPrevention.FormDisasterPreventionManagement m_frmDisasterMgr = null;

		public static void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
		{
			if (targetForm == null)
				return;

			if (nCommandID != ID.ID_VIEW_CCTV)
				FormMain.Instance.ShowLeftLayer();
            
			FormMain.Instance.SetDisableToolBar();

			if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
				mTranslucentForm = new PopupTranslucentForm();

            if (targetForm.FormBorderStyle != FormBorderStyle.None)
            {
                mTranslucentForm.UseCloseButton = false;
            }
            else
            {
                mTranslucentForm.UseCloseButton = true;
            }
            if (nCommandID == ID.ID_VIEW_CCTV || PageBackstageHome.Instance.FireDetectCCTVMode == true)
            {
                if (FormMain.Instance.IsShowCCTVForm() == false)
                    mTranslucentForm.UseCloseButton = false;
            }

			m_nTranslucentCommandID = nCommandID;
			targetForm.ShowInTaskbar = false;
			if (mTranslucentForm.Visible == true)
			{
				mTranslucentForm.Detach();
			}

			targetForm.StartPosition = FormStartPosition.Manual;
			mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height);
			mTranslucentForm.Parent = m_home;
			mTranslucentForm.ShowInTaskbar = false;
			mTranslucentForm.Show(m_home);
		}

		public static DialogResult ShowTranslucentSubForm(Form form)
		{
			if (form == null)
				return DialogResult.Cancel;
			if (mTranslucentForm.Visible == true)
			{
				FormStartPosition pos = form.StartPosition;
				form.StartPosition = FormStartPosition.Manual;
				form.ShowInTaskbar = false;
				DialogResult result = mTranslucentForm.AddSubModalForm(form);
				form.StartPosition = pos;
				return result;
			}
			return DialogResult.Cancel;
		}

		public PageBackstageHome()
		{
            this.DoubleBuffered = true;

			Debug.WriteLine("Page Start : " + DateTime.Now);
			m_home = this;

			InitializeComponent();

			// Report용 탭 Form
			CreateReportForm();
			// 3D 용 탭
			CreateContentForm();

			CreateDockingPane();

			InitCheckedItem();

            Init3DView();


            FormMain.SetDoubleBuffer(splitContainer2.Panel2, true);
            FormMain.SetDoubleBuffer(splitContainer2.Panel1, true);
		}

		public int GetLayout()
		{
			if (m_ContentForm != null)
				return m_ContentForm.NumLayout;
			return OUTSIDE;
		}

		public bool ChangeFloor(BuildingGroup grp, Building building, Zone zoneFloor)
		{
			int nFloorIdx = zoneFloor.FloorIndex;

			if (m_ContentForm.NumLayout == OUTSIDE)
			{
			}
			if (GetLayout() == BOTHSIDE)
			{
				Check3DViewMode(ID.ID_VIEW_BOTHSIDE);
			}

			m_ContentForm.SetCurrentBuilding(building, zoneFloor);
			return true;
		}

		public void SetCheckBothSide()
		{
			if (GetLayout() == BOTHSIDE)
			{
				Check3DViewMode(ID.ID_VIEW_BOTHSIDE);
			}
		}

		public bool ShowLayer(int id, bool bShow)
		{
			return m_ContentForm.ShowLayer(id, bShow);
		}

		public bool Redraw3DView()
		{
			m_ContentForm.RedrawWindow();
			return true;
		}

		public bool Invalidate3DView(bool bEraBack)
		{
            if (m_ContentForm != null)
            {
                m_ContentForm.Invalidate3DView(bEraBack);
            }
			return true;
		}

        private ContentOwnerTab mCurrentTab = ContentOwnerTab.M3D_TAB;
        public ContentOwnerTab CurrentTab
        {
            get { return mCurrentTab; }
        }
        private ContentOwnerTab mPrevTab = ContentOwnerTab.M3D_TAB;
        public ContentOwnerTab PreviousTab
        {
            get { return mPrevTab; }
        }

        // Tab별 오른쪽 Docking Panel 모드
        private Dictionary<ContentOwnerTab, ContentOwnerTabRightDockingMode> m_dicTabDockingMode = new Dictionary<ContentOwnerTab, ContentOwnerTabRightDockingMode>();

        public int ChangeTab(ContentOwnerTab tab)
		{
            if (mCurrentTab == tab)
                return (int)tab;

            SaveDockingPane(mCurrentTab);

            mPrevTab = mCurrentTab;
            mCurrentTab = tab;
			switch (tab)
			{
                case ContentOwnerTab.M3D_TAB:

                case ContentOwnerTab.M2D_TAB:
					m_ContentForm.EditMode = false;
					if (m_ContentForm.EditMode == false)
					{
                        if (m_ContentForm.CurrentMouseWorkMode != MouseWorkMode.ORBIT &&
                            m_ContentForm.CurrentMouseWorkMode != MouseWorkMode.PANNING)
						{
							CheckMouseWorkItem(ID.ID_VIEW_PICK, true);
						}
					}

                    if(m_ContentForm.Visible == false)
                    {
                        m_ContentForm.Visible = true;
                    }
					m_ReportForm.Visible = false;
                    ShowDockingPane(mCurrentTab);
                    if (tab == ContentOwnerTab.M3D_TAB)
                    {
                        m_ContentForm.View1Click(null, null);
                    }
                    else
                    {
                        m_ContentForm.View2Click(null, null);
                    }
                    FormMain.Instance.ShowToolbar();
					break;

                case ContentOwnerTab.ADMIN_TAB:
					m_ContentForm.EditMode = true;
					m_ReportForm.Visible = false;

                    ShowDockingPane(mCurrentTab);
					break;

                case ContentOwnerTab.REPORT_TAB:
					m_ContentForm.EditMode = false;
                    if (m_ContentForm.CurrentMouseWorkMode != MouseWorkMode.ORBIT &&
                            m_ContentForm.CurrentMouseWorkMode != MouseWorkMode.PANNING)
					{
						CheckMouseWorkItem(ID.ID_VIEW_PICK, true);
					}
					m_ReportForm.Visible = true;
                    ShowDockingPane(mCurrentTab);
					break;
			}
			return (int)tab;
		}

        private Dictionary<int, ContentOwnerTab> m_dicTabType = null;
        public bool ToTab(int nTab, out ContentOwnerTab result)
        {
            if (m_dicTabType == null)
            {
                m_dicTabType = new Dictionary<int, ContentOwnerTab>();

                foreach (ContentOwnerTab type in Enum.GetValues(typeof(ContentOwnerTab)))
                {
                    m_dicTabType[(int)type] = type;
                }
            }

            ContentOwnerTab tType;

            if (m_dicTabType.TryGetValue(nTab, out tType))
            {
                result = tType;
                return true;
            }

            result = ContentOwnerTab.FILE_TAB;
            return false;
        }

        private Dictionary<int, ContentOwnerTabRightDockingMode> m_dicTabModeType = null;
        public bool ToTabRightDockingMode(int nTab, out ContentOwnerTabRightDockingMode result)
        {
            if (m_dicTabModeType == null)
            {
                m_dicTabModeType = new Dictionary<int, ContentOwnerTabRightDockingMode>();

                foreach (ContentOwnerTabRightDockingMode type in Enum.GetValues(typeof(ContentOwnerTabRightDockingMode)))
                {
                    m_dicTabModeType[(int)type] = type;
                }
            }

            ContentOwnerTabRightDockingMode tType;

            if (m_dicTabModeType.TryGetValue(nTab, out tType))
            {
                result = tType;
                return true;
            }

            result = ContentOwnerTabRightDockingMode.NONE;
            return false;
        }

        private void LoadDockingPane()
        {
            string strOptions = FormMain.Instance.DBManager.LoadIni(RightDockingPaneOptionTag, SDMSUIOptionSection).Trim();

            if (strOptions.Length == 0)
                return;

            string[] options = strOptions.Split(',');

            ContentOwnerTab tab;
            ContentOwnerTabRightDockingMode tabMode;
            int nTabNo, nTabOption;

            foreach (string strOption in options)
            {
                int nIndex = strOption.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strTab = strOption.Substring(0, nIndex).Trim();
                string strTabOption = strOption.Substring(nIndex + 1).Trim();

                if (!int.TryParse(strTab, out nTabNo) || !int.TryParse(strTabOption, out nTabOption))
                    continue;

                if (ToTab(nTabNo, out tab) && ToTabRightDockingMode(nTabOption, out tabMode))
                {
                    m_dicTabDockingMode[tab] = tabMode;
                }
            }
        }

        // Tab별 오른쪽 DockingPane의 상태를 저장한다.
        private void SaveDockingPane(ContentOwnerTab tab)
        {
            if (DockingPaneIsHidden())
                m_dicTabDockingMode[tab] = ContentOwnerTabRightDockingMode.NONE;
            else
            {
                DockingFormLocation.Mode mode = m_dockFormLocation.GetCurrentMode();

                if (mode == DockingFormLocation.Mode.PSM)
                    m_dicTabDockingMode[tab] = ContentOwnerTabRightDockingMode.SHOW_PSM;
                else if (mode == DockingFormLocation.Mode.Location)
                    m_dicTabDockingMode[tab] = ContentOwnerTabRightDockingMode.SHOW_LOCATION;
                else if (mode == DockingFormLocation.Mode.CCTV)
                    m_dicTabDockingMode[tab] = ContentOwnerTabRightDockingMode.SHOW_CCTV;
                else if (mode == DockingFormLocation.Mode.DISASTER)
                    m_dicTabDockingMode[tab] = ContentOwnerTabRightDockingMode.SHOW_DISASTER;
            }

            string strOptions = "";

            foreach (KeyValuePair<ContentOwnerTab, ContentOwnerTabRightDockingMode> pair in m_dicTabDockingMode)
            {
                string strOption = ((int)pair.Key).ToString() + ":" + ((int)pair.Value).ToString();

                if (strOptions.Length == 0)
                    strOptions = strOption;
                else
                    strOptions += "," + strOption;
            }

            FormMain.Instance.DBManager.SaveIni(RightDockingPaneOptionTag, strOptions, SDMSUIOptionSection);
        }

        private void ShowDockingPane(ContentOwnerTab tab)
        { 
            m_ShowCCTVList.Enabled = false;
            m_ShowPSMList.Enabled = false;
            m_ShowDisasterMgr.Enabled = false;

            ContentOwnerTabRightDockingMode mode = ContentOwnerTabRightDockingMode.NONE;

            if (m_dicTabDockingMode.TryGetValue(tab, out mode))
            {
                if (mode == ContentOwnerTabRightDockingMode.SHOW_LOCATION)
                {
                    ShowLocationList();
                }
                else if (mode == ContentOwnerTabRightDockingMode.SHOW_PSM)
                {
                    if (m_dockFormLocation.GetCurrentMode() != DockingFormLocation.Mode.PSM)
                        ShowPSMList();
                    else
                        ShowAllDockingPane();
                }
                else if (mode == ContentOwnerTabRightDockingMode.SHOW_CCTV)
                {
                    if (m_dockFormLocation.GetCurrentMode() != DockingFormLocation.Mode.CCTV)
                        ShowCCTVList();
                    else
                        ShowAllDockingPane();
                }
                else if (mode == ContentOwnerTabRightDockingMode.SHOW_DISASTER)
                {
                    if (m_dockFormLocation.GetCurrentMode() != DockingFormLocation.Mode.DISASTER)
                    {
                        if (m_frmDisasterMgr == null)
                            m_frmDisasterMgr = new PopupDialog.DisasterPrevention.FormDisasterPreventionManagement();
                        ShowDisasterMgr();
                    }
                    else
                        ShowAllDockingPane();
                }
                else
                    HideAllDockingPane();
            }
            else
                HideAllDockingPane();
        }

        private void ShowLocationList()
        {

            DockingFormLocation.Mode mode = m_dockFormLocation.GetCurrentMode();

            m_dockFormLocation.PSMMode = false;
            m_dockFormLocation.CCTVMode = false;

            splitContainer2.Panel2Collapsed = false;
            m_dockFormLocation.SetTitle(DockingFormLocation.OriginTitle);

            if (mode == DockingFormLocation.Mode.Location)
                ShowAllDockingPane();
            else
            {
                m_dockFormLocation.RemoveControl();
                ShowAllDockingPane();
            }
        }

		public void ShowAllDockingPane()
		{
			splitContainer1.Panel2Collapsed = false;

            int nSplitDistance = m_dockFormLocation.GetSplitDistance(splitContainer1);

            if (nSplitDistance < 0)
                nSplitDistance = 0;
            splitContainer1.SplitterDistance = nSplitDistance;    
		}

		private void HideAllDockingPane()
		{
			splitContainer1.Panel2Collapsed = true;
		}

        private bool DockingPaneIsHidden()
        {
            return splitContainer1.Panel2Collapsed;
        }

		private void CreateDockingPane()
		{
			m_dockFormLocation = new DockingFormLocation();
			m_dockFormProperties = new DockingFormProperties();
			m_dockFormFacilityList = new DockingFormFacilityList();

			m_nPrevLocationPaneHeight = 200;
			m_nPrevPropertyPaneHeight = 600;
			m_nPrevFacilityPaneHeight = 200;

			m_dockFormLocation.TopLevel = false;
			m_dockFormProperties.TopLevel = false;
			m_dockFormFacilityList.TopLevel = false;

			m_dockFormLocation.Dock = DockStyle.Fill;
			m_dockFormFacilityList.Dock = DockStyle.Bottom;
			m_dockFormProperties.Dock = DockStyle.Fill;

			splitContainer2.Panel1.Controls.Add(m_dockFormLocation);
			splitContainer2.Panel2.Controls.Add(m_dockFormFacilityList);
			splitContainer2.Panel2.Controls.Add(m_dockFormProperties);

			m_dockFormLocation.Show();
			m_dockFormProperties.Show();
			m_dockFormFacilityList.Show();
			m_PanelLeft.Width = 250;
		
			m_dockFormLocation.SetTitle(DockingFormLocation.OriginTitle);
			m_dockFormFacilityList.SetTitle("센서구역/시설 리스트");
			m_dockFormProperties.SetTitle("속성 정보");
		}


		private bool m_bInit3DView = false;
		public void Init3DView()
		{
			if (m_bInit3DView == true)
				return;

			m_bInit3DView = true;

			string strSkinFolder = StylesPath();
			//Debug.WriteLine(DateTime.Now);
			m_ContentForm.Show();
			//Debug.WriteLine(DateTime.Now);


            if( UnE.SOP.ProxySOP.Instance.SiteID == 2)
            {                
                string szFilePath = Application.StartupPath + "\\models\\";
                Dictionary<string, string> dicInsideCMO = new Dictionary<string, string>();
                dicInsideCMO.Add("Inside", szFilePath + "Inside.zip");

                m_ContentForm.SetFilePath(szFilePath, szFilePath + "Outside.zip", szFilePath, dicInsideCMO);
                
            }
            //else
            //{
            //    if (UnE.SOP.ProxySOP.Instance.SimulationMode == false)
            //    {
            //        ModelManager.Instance.TargetForm = m_ContentForm;
            //        ModelManager.Instance.Read3DModel();
            //    }
            //    else
            //    {
            //        string szFilePath = Application.StartupPath + "\\models\\";
            //        Dictionary<string, string> dicInsideCMO = new Dictionary<string, string>();
            //        dicInsideCMO.Add("Inside", szFilePath + "Inside.zip");

            //        m_ContentForm.SetFilePath(szFilePath, szFilePath + "Outside.zip", szFilePath, dicInsideCMO);
            //    }
            //}
            
			
			//Debug.WriteLine(DateTime.Now);
			m_ContentForm.Init3DView();
			//Debug.WriteLine(DateTime.Now);
			OnChangeTheme(0);
			//Debug.WriteLine(DateTime.Now);
			ShowLayer(ID.ID_LAYER_ALARMSTA, false);
			ShowLayer(ID.ID_LAYER_RECIVER, false);
			Debug.WriteLine("Page Load: " + DateTime.Now);

			timer1.Interval = 5000;
			timer1.Enabled = true;
		}
        public void LoadPOI()
        {
            m_ContentForm.LoadPOIs();


            if (m_frmCCTVList == null || m_frmCCTVList.IsDisposed == true)
            {
                m_frmCCTVList = new PopupDialog.FormCCTVList();
            }
            m_frmCCTVList.LoadData();

        }

		private void PageBackstageHome_Load(object sender, EventArgs e)
		{
            LoadDockingPane();

            m_dockFormLocation.SetSplitDistance(DockingFormLocation.Mode.Location, splitContainer1.Width - splitContainer1.SplitterDistance);
            m_dockFormLocation.SetSplitDistance(DockingFormLocation.Mode.PSM, PopupDialog.FormPSMList.DockingWidth);
            m_dockFormLocation.SetSplitDistance(DockingFormLocation.Mode.DISASTER, PopupDialog.DisasterPrevention.FormDisasterPreventionManagement.DockingWidth);
            m_dockFormLocation.SetSplitDistance(DockingFormLocation.Mode.CCTV, PopupDialog.FormPSMList.DockingWidth);


            // Unity Load완료 시점으로 이동 edit by skkim 2016-08-04
            //ShowDockingPane(mCurrentTab);
            //HideAllDockingPane();
		}

		public string StylesPath()
		{
			string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			System.IO.Directory.Exists(strExePath + "\\Styles\\");
			return strExePath + "\\Styles\\";
		}

       


        private IFormContent CreateContentFormBySiteID(int nSiteID)
        {
            UnE.View.Content.ViewUtils.RegisterContentViewOwner(this);

            if (nSiteID == 1 || nSiteID == 100 || nSiteID == 101)
            {
               
                FormContentUnity form = new FormContentUnity(this);
                //.View.Content.ViewUtils.RegisterContentView(form);
                m_ContentForm = form;
                form.TopLevel = false;
                form.Parent = m_ContentPanel;
                form.Dock = DockStyle.Fill;
                m_ContentPanel.Controls.Add(form);
                return form;               
            }

            if (nSiteID == 2)
            {
                // 실내, 실외 모두 2D Image 사용
                //FormContent2DOnly form = new FormContent2DOnly(this);

                //m_ContentForm = form;
                //form.TopLevel = false;
                //form.Parent = m_ContentPanel;
                //form.Dock = DockStyle.Fill;
                //m_ContentPanel.Controls.Add(form);
                //return form;

                // 실내는 2D, 실외는 Ogre 3D 사용
                FormContent2D form = new FormContent2D(this);
                m_ContentForm = form;
                form.TopLevel = false;
                form.Parent = m_ContentPanel;
                form.Dock = DockStyle.Fill;
                m_ContentPanel.Controls.Add(form);
                return form;
            }
            else
            {
                // 실외는 Unity, 실내는 없음
                FormContentUnity form = new FormContentUnity(this);
                m_ContentForm = form;
                form.TopLevel = false;
                form.Parent = m_ContentPanel;
                form.Dock = DockStyle.Fill;
                m_ContentPanel.Controls.Add(form);
                return form;               
            }

            return null;
        }

		private void CreateContentForm()
		{
            try
			{
                int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
                m_ContentForm = CreateContentFormBySiteID(nSiteID);
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("3D 환경을 초기화 하지 못하였습니다.\n모니터링을 종료합니다.");
                System.Diagnostics.Trace.WriteLine(ex.Message);

				Application.Exit();
				return;
			}		
		}

		private void CreateReportForm()
		{
			m_ReportForm = new FormReport();

			m_ReportForm.TopLevel = false;
			m_ReportForm.Parent = m_ContentPanel;
			m_ReportForm.Dock = DockStyle.Fill;

            m_ContentPanel.Controls.Add(m_ReportForm);
		}

		public void OnChangeLayer(int nID)
		{
			switch (nID)
			{
				case ID.ID_LAYER_DETECTOR:
					m_bShowDetectorLayer = !m_bShowDetectorLayer;
					ShowLayer(nID, m_bShowDetectorLayer);
					break;

				case ID.ID_LAYER_COOLER:
					m_bShowCoolerLayer = !m_bShowCoolerLayer;
					ShowLayer(nID, m_bShowCoolerLayer);
					break;

				case ID.ID_LAYER_PERSURE:
					m_bShowPressureLayer = !m_bShowPressureLayer;
					ShowLayer(nID, m_bShowPressureLayer);
					break;

				case ID.ID_LAYER_CCTV:
					m_bShowCCTVLayer = !m_bShowCCTVLayer;
					ShowLayer(nID, m_bShowCCTVLayer);
					break;

				case ID.ID_LAYER_FIREEXT:
					m_bShowFireExtinguisherLayer = !m_bShowFireExtinguisherLayer;
					ShowLayer(nID, m_bShowFireExtinguisherLayer);
					break;

				case ID.ID_LAYER_FIREHYD:
					m_bShowFireHydrantLayer = !m_bShowFireHydrantLayer;
					ShowLayer(nID, m_bShowFireHydrantLayer);
					break;

				case ID.ID_LAYER_ALARMSTA:
					m_bShowAlarmStationLayer = !m_bShowAlarmStationLayer;
					ShowLayer(nID, m_bShowAlarmStationLayer);
					break;

				case ID.ID_LAYER_RECIVER:
					m_bShowReciverLayer = !m_bShowReciverLayer;
					ShowLayer(nID, m_bShowReciverLayer);
					break;

				case ID.ID_LAYER_TEXTPOI:
					m_bShowTextPOILayer = !m_bShowTextPOILayer;
					ShowLayer(nID, m_bShowTextPOILayer);
					break;

				case ID.ID_LAYER_CCTVLOW:
					SDMS.FireDetectProcess.ShowFireDetectTooltipCCTV = !SDMS.FireDetectProcess.ShowFireDetectTooltipCCTV;
					/*m_bShowLowCCTVLayer = !m_bShowLowCCTVLayer;
					ShowLayer(nID, m_bShowLowCCTVLayer);*/
					break;

                case ID.ID_LAYER_CCTV_DISCONNECTED:
                    m_bShowDisconnectedCCTVLayer = !m_bShowDisconnectedCCTVLayer;
                    ShowLayer(nID, m_bShowDisconnectedCCTVLayer);
                    break;

                case ID.ID_LAYER_BUILDING_TEXT:
                    m_bShowBuildingTextLayer = !m_bShowBuildingTextLayer;
                    ShowLayer(nID, m_bShowBuildingTextLayer);
                    break;

                case ID.ID_LAYER_NOTICE:
                    m_bShowNotice = !m_bShowNotice;
                    ShowLayer(nID, m_bShowNotice);
                    break;

				default:
					break;
			};
		}

        public void OnReadyDataLoad()
        {
            ShowDockingPane(mCurrentTab);

            ShowLayer(ID.ID_LAYER_BUILDING_TEXT, m_bShowBuildingTextLayer);

            FormMain.Instance.OnReadyDataLoad();
        }

		public void OnChangeLayer(int nID, bool visible)
		{
			switch (nID)
			{
				case ID.ID_LAYER_DETECTOR:
					m_bShowDetectorLayer = visible;
					ShowLayer(nID, m_bShowDetectorLayer);
					break;

				case ID.ID_LAYER_COOLER:
					m_bShowCoolerLayer = visible;
					ShowLayer(nID, m_bShowCoolerLayer);
					break;

				case ID.ID_LAYER_PERSURE:
					m_bShowPressureLayer = visible;
					ShowLayer(nID, m_bShowPressureLayer);
					break;

				case ID.ID_LAYER_CCTV:
					m_bShowCCTVLayer = visible;
					ShowLayer(nID, m_bShowCCTVLayer);
					break;

				case ID.ID_LAYER_FIREEXT:
					m_bShowFireExtinguisherLayer = visible;
					ShowLayer(nID, m_bShowFireExtinguisherLayer);
					break;

				case ID.ID_LAYER_FIREHYD:
					m_bShowFireHydrantLayer = visible;
					ShowLayer(nID, m_bShowFireHydrantLayer);
					break;

				case ID.ID_LAYER_ALARMSTA:
					m_bShowAlarmStationLayer = visible;
					ShowLayer(nID, m_bShowAlarmStationLayer);
					break;

				case ID.ID_LAYER_RECIVER:
					m_bShowReciverLayer = visible;
					ShowLayer(nID, m_bShowReciverLayer);
					break;

				case ID.ID_LAYER_TEXTPOI:
					m_bShowTextPOILayer = visible;
					ShowLayer(nID, m_bShowTextPOILayer);
					break;

				case ID.ID_LAYER_CCTVLOW:
					SDMS.FireDetectProcess.ShowFireDetectTooltipCCTV = visible;
					/*m_bShowLowCCTVLayer = visible;
					ShowLayer(nID, m_bShowLowCCTVLayer);*/
					break;

                case ID.ID_LAYER_CCTV_DISCONNECTED:
                    m_bShowDisconnectedCCTVLayer = visible;
                    ShowLayer(nID, m_bShowDisconnectedCCTVLayer);
                    break;

                case ID.ID_LAYER_BUILDING_TEXT:
                    m_bShowBuildingTextLayer = visible;
                    ShowLayer(nID, visible);
                    break;

                case ID.ID_LAYER_NOTICE:
                    m_bShowNotice = visible;
                    ShowLayer(nID, m_bShowNotice);
                    break;
				default:
					break;
			};
		}

		public void OnClickToolBarButton(Button btn)
		{
			FormMain frmMain = FormMain.Instance;

			int nID = frmMain.GetButtonID(btn);
			if (nID < 0)
				return;

			bool isChecked = frmMain.IsChecked(btn);

			switch (nID)
			{
				//case ID.ID_VIEW_HOME:
				//	m_ContentForm.HomeView();
				//	break;
                case ID.ID_VIEW_HOME_MAIN:
                    m_ContentForm.HomeView("Main");

                    break;
                case ID.ID_VIEW_HOME_14:
                    m_ContentForm.HomeView("14");
                    break;
                case ID.ID_VIEW_HOME_56:
                    m_ContentForm.HomeView("56");
                    break;
                case ID.ID_VIEW_HOME_COAL:
                    m_ContentForm.HomeView("Coal");
                    break;      
				case ID.ID_VIEW_FULLSCREEN:
					m_ContentForm.TopView();
					break;

				case ID.ID_VIEW_PICK:
				case ID.ID_VIEW_PAN:
				case ID.ID_VIEW_ORBIT:
					CheckMouseWorkItem(nID, true);
					break;

				case ID.ID_VIEW_ZOOMIN:
					m_ContentForm.ZoomIn();
					break;

				case ID.ID_VIEW_ZOOMOUT:
					m_ContentForm.ZoomOut();
					break;

				case ID.ID_VIEW_OUTSIDE:
                    OnClick3D();
					break;

				case ID.ID_VIEW_BOTHSIDE:
                    OnClickBothView(isChecked);					
					break;

				case ID.ID_VIEW_INSIDE:
                    OnClick2D();
					break;

                //case ID.ID_VIEW_CCTV:
                //    OnClickBigCCTV();
                //    break;
                case ID.ID_VIEW_CCTV:
                    OnClickCCTVList();
                    break;

				case ID.ID_VIEW_SCREENSHOT:
                    OnClickSaveImage();
                    //ShowFormCaptureImages();
					break;

                case ID.ID_VIEW_WEATHER_INFO:
                    OnClickWeatherInfo(isChecked);
                    break;

                case ID.ID_VIEW_PSM:
                    OnClickPSMList();
                    break;

                case ID.ID_VIEW_DISASTER:
                    OnClickDisasterMgr();
                    break;

				default:
					break;
			};
			m_ContentForm.RedrawWindow();
		}

        public void OnClickBothView(bool isChecked)
        {
            FormMain.Instance.CheckButton(ID.ID_VIEW_BOTHSIDE, !isChecked);

            if (!isChecked)
            {                
                m_ContentForm.LayoutBothside();
                Check3DViewMode(ID.ID_VIEW_BOTHSIDE);
            }
            else
            {
                if (mCurrentTab == ContentOwnerTab.M2D_TAB)
                {
                    m_ContentForm.LayoutInside();
                    Check3DViewMode(ID.ID_VIEW_INSIDE);
                }
                else
                {
                    m_ContentForm.LayoutOutside();
                    Check3DViewMode(ID.ID_VIEW_OUTSIDE);
                }
            }
        }
        
        public void OnClick3D()
        {
			m_ContentForm.LayoutOutside();
			Check3DViewMode(ID.ID_VIEW_OUTSIDE);
        }

        public void OnClick2D()
        {
            m_ContentForm.LayoutInside();
            Check3DViewMode(ID.ID_VIEW_INSIDE);
        }

        private void ShowFormCaptureImages()
        {
            Utility.FormCaptureImages frm = new Utility.FormCaptureImages();
            frm.Show(this);
        }

        public void OnClickSaveImage()
        {
            m_ContentForm.SaveToImage();
        }

        public void OnClickWeatherInfo(bool isChecked)
        {
            FormMain.Instance.CheckButton(ID.ID_VIEW_WEATHER_INFO, !isChecked);

            if (!isChecked)
                FormMain.Instance.ShowWeatherInfo();
            else
                FormMain.Instance.HideWeatherInfo();
        }

		public void OnClickBigCCTV()
		{

            m_ContentForm.PushViewState();
			m_ContentForm.LayoutBothside();

			ProcessIF process = FormMain.Instance.CurrentSensorDetectProcess;

			Zone zoneTarget = null;
            bool bOutterZone = false;
            
            int nSituation = 0;

			if (process != null)
			{
				EquipmentZone equipZone = process.TargetZone;

				if (equipZone != null)
				{
					zoneTarget = equipZone.LinkedZone;

					if (zoneTarget != null)
						bOutterZone = zoneTarget.Building == null;
				}
                nSituation = (process.ProcessType == ProcessType.FireAlarm ? 1 : 2);
			}

			ShowBigCCTV(zoneTarget, nSituation, bOutterZone);
		}

        public void ShowNormalCCTV()
        {
            //if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == false)
            //{
                // 내부CCTV는 사용하지 않음 comment by skkim 2016-10-14
                //if (CCTVForm.GetContent(0) != null && CCTVForm.GetContent(0).GetType() == typeof(Panel4Unity))
                //{
                //    ContentForm.AttachView(CCTVForm.GetContent(0), true);
                //    CCTVForm.SetPanel(0, null, true);
                //}

                //if (CCTVForm.GetContent(3) != null && CCTVForm.GetContent(3).GetType() == typeof(Panel4Unity))
                //{
                //    ContentForm.AttachView(CCTVForm.GetContent(3), false);
                //    CCTVForm.SetPanel(3, null, true);
                //}
            //}
            //else
            {
                FormMain.Instance.CCTVPipe.Send("ShowNormalCCTV()");
            }
            
        }

        public void ShowSituationCCTV(bool bSituation)
        {
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == false)
            {
                // 내부CCTV는 사용하지 않음 comment by skkim 2016-10-14
                //if (CCTVForm != null)
                //{
                //    if (bSituation == true)
                //    {
                //        if (CCTVForm.GetContent(0) != null && CCTVForm.GetContent(0).GetType() == typeof(BigCCTVCtrl))
                //            CCTVForm.SetPanel(0, ContentForm.DetachView(true), false);
                //        if (CCTVForm.GetContent(3) != null && CCTVForm.GetContent(3).GetType() == typeof(BigCCTVCtrl))
                //            CCTVForm.SetPanel(3, ContentForm.DetachView(false), false);
                //    }
                //    else
                //    {
                //        if (CCTVForm.Tag  == null)
                //            ShowDefaultCCTV();
                //    }
                //}
            }
            else
            {
                if (FormMain.Instance.IsShowCCTVForm() == false)
                {
                    FormMain.Instance.ShowCCTVForm();
                }
                if (bSituation == true)
                {

                    int nType = 0;
                    if(FormMain.Instance.CurrentSensorDetectProcess != null)
                    {
                        if (FormMain.Instance.CurrentSensorDetectProcess.ProcessType == ProcessType.PSMAlarm)
                            nType = 2;
                        else
                            nType = 1;

                        //nType = (FormMain.Instance.CurrentSensorDetectProcess.ProcessType == ProcessType.FireAlarm ? 1 : 2);
                        EquipmentZone zone = FormMain.Instance.CurrentSensorDetectProcess.TargetZone;

                        if (FormMain.Instance.CurrentSensorDetectProcess.ProcessType == ProcessType.PSMAlarm)
                        //if( nType == 2)
                        {
                            ArrayList arPaths = DownloadPSMImage(FormMain.Instance.CurrentSensorDetectProcess.TargetSensor.OrgSensorID);
                            if (arPaths != null)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    string tPath = arPaths[i].ToString().Replace("\\", "/");
                                    string strImageTitle = arPaths[i + 3].ToString();
                                    FormMain.Instance.CCTVPipe.Send("SetViewerImage(" + (i + 2) + ",'" + tPath + "', '" + strImageTitle + "')");
                                }
                            }                           
                        }                       
                        else
                        {
                            string szPath2 = "";
                            DownloadEquipZoneImage(zone, out szPath2);
                            szPath2 = szPath2.Replace("\\", "/");
                            FormMain.Instance.CCTVPipe.Send("SetViewerImage(2,'" + szPath2 + "', '" + zone.ZoneName + "')");
                        }

                        string szPath1 = "";
                        DownloadBuildingImage(zone, out szPath1);
                        szPath1 = szPath1.Replace("\\", "/");

                        FormMain.Instance.CCTVPipe.Send("SetViewerImage(1,'" + szPath1 + "', '" + zone.ZoneName + "')");
                    }                    
                    //FormMain.Instance.CCTVPipe.Send("ShowSituationCCTV(" + nType + ")");
                    
                    //    FormMain.Instance.CCTVPipe.Send("SetPreset(" + nType + ")");
                   
                }
                else
                {
                    FormMain.Instance.CCTVPipe.Send("ShowDefaultCCTV()");
                }
            }
        }


        static int nImageCount = 0;
        public ArrayList DownloadPSMImage(int nSensorID)
        {
            

            if (nSensorID < 0)
                return null;

            string szText = "SELECT Image01 ,Image02 ,Image03, ImageTitle01, ImageTitle02, ImageTitle03 FROM PSMSensorLinkedPicture WHERE SensorID = {0}";
            string szSQL = string.Format(szText, nSensorID);


            ArrayList arResult = FormMain.Instance.DBManager.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count < 6)
                return null;

            string szImgPath1 = DBUtility.WebDBManager.GetStringField(arResult[0], "");
            string szImgPath2 = DBUtility.WebDBManager.GetStringField(arResult[1], "");
            string szImgPath3 = DBUtility.WebDBManager.GetStringField(arResult[2], "");
            string strImgTitle1 = DBUtility.WebDBManager.GetStringField(arResult[3], "");
            string strImgTitle2 = DBUtility.WebDBManager.GetStringField(arResult[4], "");
            string strImgTitle3 = DBUtility.WebDBManager.GetStringField(arResult[5], "");

            string strRootURL = GetRootURL();
            string strIndoorURL = GetTargetPropertyURL("PSMSensorLinkedImageFolder");

            ArrayList arReturn = new ArrayList();
           for( int i = 0; i < 3 ; i++)
           {
               System.Net.WebClient web = new System.Net.WebClient();
               string strImageFileName = arResult[i].ToString();
               string szLocalFileName = strImageFileName.Replace("/", "");
               if (strIndoorURL.Length > 0)
               {
                   string szPath = DownloadFile(web, strIndoorURL + strImageFileName, "psm11_" + szLocalFileName);
                   nImageCount++;
                   if (nImageCount == 10000)
                       nImageCount = 0;
                   arReturn.Add(szPath);
               }

               web.Dispose();
           }

           if (arReturn.Count == 3)
           {
               arReturn.Add(strImgTitle1);
               arReturn.Add(strImgTitle2);
               arReturn.Add(strImgTitle3);
           }
           else
               return null;

           return arReturn;
        }


        private string GetTargetPropertyURL(string szTargetName)
        {
            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSDMS where PropertyName = '{0}' and SiteID = {1}",
                szTargetName, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return "";

            int nResultCount = arrResult.Count;

            string strRootURL = GetRootURL();
            string strIndoorURL = "";

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyValue == null || strPropertyValue.Length == 0)
                    continue;

                strPropertyValue = strPropertyValue.Trim();

                if (!strPropertyValue.StartsWith("/"))
                    strPropertyValue = "/" + strPropertyValue;

                else if (strPropertyName == szTargetName)
                    strIndoorURL = strRootURL + strPropertyValue;
            }
            return strIndoorURL;
        }

        private static int m_nImageCount = 1;
        private void DownloadBuildingImage(UnE.Spatial.EquipmentZone equipZone, out string strIndoorFilePath)
        {
            strIndoorFilePath = "";

            string strIndoorURL = GetTargetPropertyURL("EquipZoneOutdoorFolder");

            System.Net.WebClient web = new System.Net.WebClient();
            string strImageFileName = "/" + equipZone.ID.ToString() + ".png";
            if (strIndoorURL.Length > 0)
                strIndoorFilePath = DownloadFile(web, strIndoorURL + strImageFileName, "EquipZoneOutdoorImage" + m_nImageCount + ".png");

            m_nImageCount++;
            if (m_nImageCount > 1000)
                m_nImageCount = 1;
            web.Dispose();
        }

        private void DownloadEquipZoneImage(UnE.Spatial.EquipmentZone equipZone,out string strIndoorFilePath)
        {
            strIndoorFilePath = "";

            string strIndoorURL = GetTargetPropertyURL("EquipZoneIndoorFolder");

            System.Net.WebClient web = new System.Net.WebClient();
            string strImageFileName = "/" + equipZone.ID.ToString() + ".png";
            if (strIndoorURL.Length > 0)
                strIndoorFilePath = DownloadFile(web, strIndoorURL + strImageFileName, "EquipZoneIndoorImage"+ m_nImageCount + ".png");
            m_nImageCount++;
            if (m_nImageCount > 1000)
                m_nImageCount = 1;
            web.Dispose();
        }
        private static log4net.ILog logger = null;
        private string DownloadFile(System.Net.WebClient web, string strURL, string strLocalFileName)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            string strFolder = System.IO.Path.GetTempPath();
            string strFilePath = strFolder + strLocalFileName;

            //MessageBox.Show(strFilePath);
            try
            {
               

                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);

                System.Diagnostics.Trace.WriteLine(strFilePath);

                web.DownloadFile(strURL, strFilePath);
            }
            catch(Exception e)
            {
                logger.Debug("Donwload error", e);
            }
           
            return strFilePath;
        }

        private string GetRootURL()
        {
            string strURL = FormMain.Instance.DBManager.WebServerURL;

            int nIndex = strURL.IndexOf("//");

            if (nIndex >= 0)
            {
                int nIndex2 = strURL.IndexOf('/', nIndex + 2);

                if (nIndex2 >= 0)
                    strURL = strURL.Substring(0, nIndex2);
            }
            else
            {
                int nIndex2 = strURL.IndexOf('/');

                if (nIndex2 >= 0)
                    strURL = strURL.Substring(0, nIndex2);
            }

            return strURL;
        }

        // nEquipZoneID가 0이면 EquipZone CCTV 대신 Default CCTV를 띄운다.
        public void ShowEquipZoneCCTVs(int nEquipZoneID)
        {
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == false)
            {
                //if (CCTVForm != null)
                //{
                //    if (nEquipZoneID > 0)
                //        FormMain.Instance.CCTVPipe.Send("ShowEquipZoneCCTVs(" + nEquipZoneID.ToString() + ")");
                //    else
                //        FormMain.Instance.CCTVPipe.Send("ShowDefaultCCTV()");
                //}
            }
            else
            {
                if (FormMain.Instance.IsShowCCTVForm() == false)
                {
                    FormMain.Instance.ShowCCTVForm();
                }

                if (nEquipZoneID > 0)
                    FormMain.Instance.CCTVPipe.Send("ShowEquipZoneCCTVs(" + nEquipZoneID.ToString() + ")");
                else
                    FormMain.Instance.CCTVPipe.Send("ShowDefaultCCTV()");
            }
        }

        public void CloseBigCCTV()
        {
            //if(m_frameCCTV != null && m_frameCCTV.IsDisposed == false)
            //{
            //    if (m_frameCCTV.Visible == true)
            //    {
            //        m_frameCCTV.Visible = false;
            //        m_frameCCTV.Close();
            //    }
            //}          
        }


		public void ShowBigCCTV(Zone zoneTarget = null, int nSituation = 0, bool bOutterZone = false)
		{
            bool bSituation = (nSituation == 0) ? false : true;
			ArrayList arrCCTVs = GetZoneCCTVArray(zoneTarget, bSituation, bOutterZone);

            ShowBigCCTV(zoneTarget, arrCCTVs, nSituation);
		}

		public void ShowBigCCTV(EquipmentZone equipZone, int nSituation)
		{
            bool bSituation = (nSituation == 0) ? false : true;
			if (bSituation == true)
			{
                ShowBigCCTV(equipZone.LinkedZone, nSituation);
			}
			else
			{            
                if(equipZone != null)
                {
                    ArrayList arrCCTVs = GetEquipZoneCCTVList(equipZone);
                    ShowBigCCTV(equipZone.LinkedZone, arrCCTVs, nSituation);
                }				
			}
		}

        public void ShowBigCCTV(string strCCTVTitle, ArrayList arrCCTVs)
        {
            //FormMain.Instance.ShowLeftThumbnail(false);

            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                FormMain.Instance.CCTVPipe.Send("SetTitle('" + strCCTVTitle + "')");

                string strCCTVIDs = "";
                foreach (CCTV cctv in arrCCTVs)
                {
                    if (strCCTVIDs != "")
                    {
                        strCCTVIDs += ",";
                    }

                    if (cctv == null)
                        strCCTVIDs += -1;
                    else
                        strCCTVIDs += cctv.ID;
                }
                FormMain.Instance.CCTVPipe.Send("SetCCTV(" + strCCTVIDs + ")");
            }
            else
            {
                /*if (m_frm4CCTV != null)
                {
                    //Form을 삭제하기 전에 기존에 연결되어 있는 뷰를 제거한다.
                    ShowNormalCCTV();

                    m_frm4CCTV.Dispose();
                }
                m_frm4CCTV = new Form4CCTV(this, "SDMS");
                m_frm4CCTV.SetOwner(FormMain.Instance);

                m_frm4CCTV.Tag = zoneTarget;

                if (bSituation == true)
                    m_frm4CCTV.SetCCTV(arrCCTVs, zoneTarget);

                ShowSituationCCTV(bSituation);
                ShowTranslucentForm(m_frm4CCTV, 0, 0, m_frm4CCTV.Size.Width, m_frm4CCTV.Size.Height, ID.ID_VIEW_CCTV);*/
            }

            FormMain.Instance.SetEnableToolBar();
        }

		private void ShowBigCCTV(Zone zoneTarget, ArrayList arrCCTVs, int nSituation)
		{
            bool bSituation = (nSituation == 0) ? false : true;
            FormMain.Instance.ShowLeftThumbnail(bSituation);


            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                if (bSituation == true)
                {
                    FormMain.Instance.CCTVPipe.Send("SetVisible2(True)");


                    FormMain.Instance.CCTVPipe.Send("ShowSituationCCTV("+nSituation+")");
                }
                else
                    FormMain.Instance.CCTVPipe.Send("ShowDefaultCCTV()");

                if (zoneTarget == null)
                    FormMain.Instance.CCTVPipe.Send("SetTargetZone(-1)");
                else
                    FormMain.Instance.CCTVPipe.Send("SetTargetZone(" + zoneTarget.ID + ")");

                if (bSituation == true)
                    FormMain.Instance.CCTVPipe.Send("SetPreset(" + nSituation + ")");

                string strCCTVIDs = "";
                foreach (CCTV cctv in arrCCTVs)
                {
                    if (strCCTVIDs != "")
                    {
                        strCCTVIDs += ",";
                    }

                    if (cctv == null)
                        strCCTVIDs += -1;
                    else
                        strCCTVIDs += cctv.ID;
                }
                FormMain.Instance.CCTVPipe.Send("SetCCTV(" + strCCTVIDs + ")");


                if (bSituation == true)
                    FormMain.Instance.CCTVPipe.Send("SetPreset(" + nSituation + ")");
                
            }
            else
            {                
                //if (m_frm4CCTV != null && m_frm4CCTV.IsDisposed == false)
                //{
                //    if (m_frm4CCTV.Tag == zoneTarget && m_frm4CCTV.Visible == true)
                //    {
                //        return;
                //    }
                //    else if (m_frm4CCTV.Tag == zoneTarget && m_frm4CCTV.Visible == false)
                //    {
                //        ShowTranslucentForm(m_frm4CCTV, 0, 0, m_frm4CCTV.Size.Width, m_frm4CCTV.Size.Height, ID.ID_VIEW_CCTV);
                //        FormMain.Instance.SetEnableToolBar();
                //        return;
                //    }
                //}

                //if (m_frm4CCTV != null)
                //{
                //    //Form을 삭제하기 전에 기존에 연결되어 있는 뷰를 제거한다.
                //    ShowNormalCCTV();

                //    m_frm4CCTV.Dispose();
                //}
                //m_frm4CCTV = new Form4CCTV(this, "SDMS");
                //m_frm4CCTV.SetOwner(FormMain.Instance);

                //m_frm4CCTV.Tag = zoneTarget;

                //if (bSituation == true)
                //    m_frm4CCTV.SetCCTV(arrCCTVs, zoneTarget);

                //ShowSituationCCTV(bSituation);
                //ShowTranslucentForm(m_frm4CCTV, 0, 0, m_frm4CCTV.Size.Width, m_frm4CCTV.Size.Height, ID.ID_VIEW_CCTV);
            }
           
			FormMain.Instance.SetEnableToolBar();
		}

        //public void SetCCTVMode(CCTVMode mode)
        //{
        //    if (m_frm4CCTV != null)
        //    {
        //        m_frm4CCTV.SetCCTVMode(mode);
        //    }
        //}

		public void HideAllPOIPopup()
		{
			if (m_ContentForm != null)
				m_ContentForm.HideAllPOIPopup();
		}
        public void HideAllPopup()
        {
            FormMain.Instance.HideAllPopup();
        }
		private ArrayList GetZoneCCTVArray(Zone zoneTarget, bool bSituation, bool bOutterZone = false)
		{
			//FormMain.Instance.ShowEquipZoneCCTV = true;

			m_ContentForm.HideAllPOIPopup();
			m_ContentForm.EditMode = false;
            if (m_ContentForm.CurrentMouseWorkMode != MouseWorkMode.PICK)
			{
				CheckMouseWorkItem(ID.ID_VIEW_PICK, true);
			}

			// 화재 발생 모드인지 사용자 모드인지 결정
			if (zoneTarget != null && bSituation == true)
			{
				m_bFireDetectCCTVMode = true;
			}
			else
			{
				m_bFireDetectCCTVMode = false;
			}

			if (zoneTarget == null)
			{
				foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
				{
					zoneTarget = pair.Value.TargetZone.LinkedZone;
					break;
				}
			}

			ArrayList arrCCTVs = null;
            ProcessIF currentProcess = FormMain.Instance.CurrentSensorDetectProcess;
			EquipmentZone currentEquipZone = FormMain.Instance.CurrentEquipZone;

			// 영역별 CCTV List 설정 모드이거나 화재가 탐지된 상황일 경우
			if ((currentProcess != null) &&	currentEquipZone != null)
			{
				EquipmentZone targetEquipZone = null;

				if (currentProcess != null)
				{
					if (currentProcess.TargetSensor != null)
					{
						targetEquipZone = ZoneManager.Instance.GetEquipZone(currentProcess.TargetSensor.EquipZoneID);
					}
				}

                if (bOutterZone == true)
                {
                    List<EquipmentZone> arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zoneTarget);
                    if (arEquipzone != null && arEquipzone.Count > 0)
                    {
                        EquipmentZone equipZone = (EquipmentZone)arEquipzone[0];
                        arrCCTVs = GetEquipZoneCCTVList(equipZone);
                    }
                }
                else
                {
                    if (targetEquipZone == null)
                        targetEquipZone = currentEquipZone;
                    arrCCTVs = GetEquipZoneCCTVList(targetEquipZone);
                }
			}
			else
			{
				if (zoneTarget == null)
					arrCCTVs = GetCCTVList();
				else
					arrCCTVs = CCTVManager.Instance.AutoPopupCCTV(zoneTarget);
			}

			return arrCCTVs;
		}

        //public void ShowDefaultCCTV()
        //{
        //    m_frm4CCTV.SetDefaultCCTV();
        //    m_frm4CCTV.Tag = null;
        //    CCTVList cvList = m_frm4CCTV.GetCCTVList(null);
        //    if(cvList != null)
        //    {
        //        ArrayList arrCCTVs = cvList.GetAllCCTV();
        //        if (arrCCTVs != null)
        //            m_frm4CCTV.SetCCTV(arrCCTVs, null);
        //    }            
        //}

        //public void ShowBigCCTV(EquipmentZone equipZone)
        //{
        //    if (equipZone != null)
        //    {
        //        ArrayList arrCCTVs = GetZoneCCTVArray(equipZone.LinkedZone, true);

        //        if (arrCCTVs == null)
        //            arrCCTVs = new ArrayList();

        //        m_frm4CCTV.SetCCTV(arrCCTVs, equipZone.LinkedZone);
        //    }
        //    else
        //    {
        //        FormMain.Instance.ShowLeftLayer(false);
        //        ShowDefaultCCTV();
        //    }
        //}

		public ArrayList GetEquipZoneCCTVList(EquipmentZone equipZone)
		{
			ArrayList arr = new ArrayList();

            if( equipZone == null)
            {
                return null;
            }

			CCTV[] arrCCTVs = CCTVManager.Instance.GetCCTVArray(equipZone);

			//System.Diagnostics.Trace.WriteLine("GetEquipzoneCCTVList equipZoneName : " + equipZone.ZoneName);

			if (arrCCTVs == null)
			{
				//System.Diagnostics.Trace.WriteLine("CCTV is null");
				// 설정된 것이 없으면 자동탐지를 사용한다.
				if (equipZone.LinkedZoneList.Count > 0)
					return CCTVManager.Instance.AutoPopupCCTV((Zone)equipZone.LinkedZoneList[0]);
				else
				{
					for (int i = 0; i < 4; i++)
						arr.Add(null);
				}
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					arr.Add(arrCCTVs[i]);

					/*if (arrCCTVs[i] != null)
						System.Diagnostics.Trace.WriteLine(i.ToString() + " : " + arrCCTVs[i].AccessKey);*/
				}
			}

			return arr;
		}

		public void Check3DViewMode(int nID)
		{
			FormMain frmMain = FormMain.Instance;

			if (nID == ID.ID_VIEW_OUTSIDE)
			{
				frmMain.CheckButton(ID.ID_VIEW_BOTHSIDE, false);
				frmMain.CheckButton(ID.ID_VIEW_INSIDE, false);
				frmMain.CheckButton(ID.ID_VIEW_OUTSIDE, true);
			}
			else if (nID == ID.ID_VIEW_BOTHSIDE)
			{
				frmMain.CheckButton(ID.ID_VIEW_BOTHSIDE, true);
				frmMain.CheckButton(ID.ID_VIEW_INSIDE, false);
				frmMain.CheckButton(ID.ID_VIEW_OUTSIDE, false);
			}
			else if (nID == ID.ID_VIEW_INSIDE)
			{
				frmMain.CheckButton(ID.ID_VIEW_BOTHSIDE, false);
				frmMain.CheckButton(ID.ID_VIEW_INSIDE, true);
				frmMain.CheckButton(ID.ID_VIEW_OUTSIDE, false);
			}
		}

		public void OnCommandExcute(int nID)
		{
			FormMain frmMain = FormMain.Instance;
			bool isChecked = FormMain.Instance.IsChecked(nID);

			switch (nID)
			{

                case ID.ID_MANAGE_SENSOR :
                    {
                        FormSensorMgrList sensorFrm = new FormSensorMgrList();
                        ShowTranslucentForm(sensorFrm, 200, 100, sensorFrm.Size.Width, sensorFrm.Size.Height, nID);
                    }
                    break;
				case ID.ID_SHOW_LIST_FACILITY:
					{
						FormSensorList frm = new FormSensorList();
						ShowTranslucentForm(frm, 200, 100, frm.Size.Width, frm.Size.Height, nID);
					}
					break;

				case ID.ID_MANAGE_PRINT:
					{
						FormDXFManager form = new FormDXFManager();
						ShowTranslucentForm(form, 200, 200, form.Size.Width, form.Size.Height, nID);
					}
					break;

				case ID.ID_MANAGE_DETECT:
					{
						FormSensorDetectPolicy form = new FormSensorDetectPolicy();
						ShowTranslucentForm(form, 200, 200, form.Size.Width, form.Size.Height, nID);
					}
					break;

				case ID.ID_MANAGE_BACKUPDB:
					{
						FormDataBackup form = new FormDataBackup();
						ShowTranslucentForm(form, 200, 200, form.Size.Width, form.Size.Height, nID);
					}
					break;

				case ID.ID_MANAGE_MANAGER:
					{
						Form frm = new FormManager();

						ShowTranslucentForm(frm, 200, 200, frm.Size.Width, frm.Size.Height, nID);
					}
					break;

				case ID.ID_MANAGE_MESSAGE:
					{
						FormSMSConfig frm = new FormSMSConfig();
						ShowTranslucentForm(frm, 300, 200, frm.Size.Width, frm.Size.Height, nID);
					}
					break;

				case ID.ID_MANAGE_BROADCAST:
					{
						FormBroadcastConfig frm = new FormBroadcastConfig();
						ShowTranslucentForm(frm, 200, 200, frm.Size.Width, frm.Size.Height, nID);
					}
					break;

				case ID.ID_NEW_FIRE_SENSOR:
				case ID.ID_NEW_COOLER_SENSOR:
				case ID.ID_NEW_PRESSURE_SENSOR:
				case ID.ID_NEW_CCTV:
				case ID.ID_DEL_FACILITY:

					CheckMouseWorkItem(nID, !isChecked);
					if (!isChecked)
					{
						frmMain.CheckButton(ID.ID_VIEW_PICK, true);
					}
                    ShowLocationList();
					break;

				case ID.ID_SAVE_DATA:
                    //if (m_arrChangedData.Count > 0)
					{
						if (ValidPassword())
							SaveChangedData();
					}
					break;

				case ID.ID_EDIT_FACILITY_ZONE:
					{
						FormFacilityZone frm = new FormFacilityZone();
						ShowTranslucentForm(frm, 200, 100, frm.Size.Width, frm.Size.Height, nID);
					}
					break;

                case ID.ID_MANAGE_EARTHQUAKE:
                    {
                        PopupDialog.FormEarthquakeOption frm = new PopupDialog.FormEarthquakeOption(FormMain.Instance.DBManager);
                        ShowTranslucentForm(frm, 200, 200, frm.Size.Width, frm.Size.Height, nID);
                    }
                    break;

				default:
					break;
			};
			m_ContentForm.RedrawWindow();
		}

		private bool ValidPassword()
		{
            return true;

            // 비밀번호를 묻지 않도록 무조건 True 리턴

            //FormEditPassword frm = new FormEditPassword();

            //if (FormMain.Instance.CCTVList != null)
            //    FormMain.Instance.CCTVList.Hide();

            //if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    return true;

            //return false;
		}

		private void InitCheckedItem()
		{
			m_dicCheckedItem[ID.ID_VIEW_PICK] = false;
			m_dicCheckedItem[ID.ID_VIEW_PAN] = false;
			m_dicCheckedItem[ID.ID_VIEW_ORBIT] = true;
			m_dicCheckedItem[ID.ID_NEW_FIRE_SENSOR] = false;
			m_dicCheckedItem[ID.ID_NEW_COOLER_SENSOR] = false;
			m_dicCheckedItem[ID.ID_NEW_PRESSURE_SENSOR] = false;
			m_dicCheckedItem[ID.ID_NEW_CCTV] = false;
			m_dicCheckedItem[ID.ID_DEL_FACILITY] = false;
		}

		private void CheckMouseWorkItem(int id, bool isChecked)
		{
			FormMain frmMain = FormMain.Instance;

			if (isChecked)
			{
				frmMain.CheckButton(ID.ID_VIEW_PICK, false);
				frmMain.CheckButton(ID.ID_VIEW_PAN, false);
				frmMain.CheckButton(ID.ID_VIEW_ORBIT, false);
				frmMain.CheckButton(ID.ID_NEW_FIRE_SENSOR, false);
				frmMain.CheckButton(ID.ID_NEW_COOLER_SENSOR, false);
				frmMain.CheckButton(ID.ID_EDIT_FACILITY_ZONE, false);
				frmMain.CheckButton(ID.ID_NEW_PRESSURE_SENSOR, false);
				frmMain.CheckButton(ID.ID_NEW_CCTV, false);
				frmMain.CheckButton(ID.ID_DEL_FACILITY, false);
			}

			frmMain.CheckButton(id, isChecked);

			if (frmMain.IsChecked(ID.ID_VIEW_PICK))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.PICK;
			else if (frmMain.IsChecked(ID.ID_VIEW_PAN))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.PANNING;
			else if (frmMain.IsChecked(ID.ID_VIEW_ORBIT))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.ORBIT;
			else if (frmMain.IsChecked(ID.ID_NEW_FIRE_SENSOR))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.NEW_FIRE_SENSOR;
			else if (frmMain.IsChecked(ID.ID_NEW_COOLER_SENSOR))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.NEW_COOLER_SENSOR;
			else if (frmMain.IsChecked(ID.ID_NEW_PRESSURE_SENSOR))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.NEW_PRESSURE_SENSOR;
			else if (frmMain.IsChecked(ID.ID_NEW_CCTV))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.NEW_CCTV;
			else if (frmMain.IsChecked(ID.ID_DEL_FACILITY))
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.DEL_FACILITY;
			else
                m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.NONE;
		}

		public void ShowRightDockingPane()
		{
            if (FormMain.Instance.CurrentTab == ContentOwnerTab.ADMIN_TAB)
            {
                if ((m_dockFormLocation.GetTitle() != DockingFormLocation.OriginTitle)||splitContainer2.Panel2Collapsed == false)
                {
                    ShowLocationList();
                }
            }            
			m_dockFormLocation.SetPOI(m_poiSelected);
			m_dockFormProperties.SetPOI(m_poiSelected);
		}

		public void HideRightDockingPane()
		{
            HideAllDockingPane();

            splitContainer2.Panel2Collapsed = false;

            m_dockFormLocation.RemoveControl();
            m_dockFormLocation.SetTitle(DockingFormLocation.OriginTitle);
		}

		public void SomethingChanged(ChangedData data)
		{
			if (data != null)
				m_arrChangedData.Add(data);

			Button btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
			btn.Enabled = m_arrChangedData.Count > 0;
			FormMain.Instance.CheckButton(btn, btn.Enabled);
		}

		public void RemoveData(ChangedData data)
		{
			m_arrChangedData.Remove(data);

			Button btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
			btn.Enabled = m_arrChangedData.Count > 0;
			FormMain.Instance.CheckButton(btn, btn.Enabled);
		}

		public void RemoveEquipZoneCCTVData()
		{
			ArrayList arDelete = new ArrayList();
			foreach (object data in m_arrChangedData)
			{
				if (data.GetType() == typeof(EditEquipZoneCCTV))
				{
					arDelete.Add(data);
				}
			}

			foreach (object data in arDelete)
			{
				m_arrChangedData.Remove(data);
			}
		}

		public void RemoveEditManagerData()
		{
			ArrayList arDelete = new ArrayList();
			foreach (object data in m_arrChangedData)
			{
				if (data.GetType() == typeof(EditFacilityManager))
				{
					arDelete.Add(data);
				}
			}

			foreach (object data in arDelete)
			{
				m_arrChangedData.Remove(data);
			}
		}

		public void SaveChangedData()
		{
			DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

			foreach (ChangedData data in m_arrChangedData)
			{
				data.Update(dbMgr);
			}

			PostChangedData();

            if (FormMain.Instance.DataManager.SaveDisasterPreventionEquipment() == false)
                return;

			Button btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
			btn.Enabled = false;
			FormMain.Instance.CheckButton(btn, false);
		}

		private void PostChangedData()
		{
			if (m_isChangedFacilityManager)
			{
                SDMS.NetworkManager.Instance.SendChangeFacilityManager();
			}

			if (m_isChangedEquipZoneCCTV)
			{
				int nID = FormMain.Instance.CurrentEquipZone.ID;
                SDMS.NetworkManager.Instance.SendChangeEquipZoneCCTV(nID);
			}
            
			m_arrChangedData.Clear();
			m_isChangedFacilityManager = false;
			m_isChangedEquipZoneCCTV = false;
		}

		public ArrayList GetDataList()
		{
			return m_arrChangedData;
		}

		private void PageBackstageHome_Resize(object sender, EventArgs e)
		{
		}

		public void OnChangeTheme(int nID)
		{
		}

		private ISensor m_DetectSensor = null;
		private EquipmentZone m_DetectZone = null;
		private int m_nSensorHistoryID = -1;

		public void FireDetect(ISensor sensor, EquipmentZone zone, int nSensorHistoryID)
		{
			m_DetectSensor = sensor;
			m_DetectZone = zone;
			m_nSensorHistoryID = nSensorHistoryID;
		}

        public void SetDetectSensor(ISensor sensor)
        {
            m_DetectSensor = sensor;
        }

		public void EndFireProcess(int nSensorID)
		{
			m_DetectSensor = null;
			m_DetectZone = null;
		}

        public void CloseExternal()
        {
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.CloseExternal();
            }            
        }

		public void OnTranslucentFormClosing()
		{
			if (m_bFireDetectCCTVMode == true)
			{
				m_bFireDetectCCTVMode = false;
				if (m_DetectSensor != null)
				{
					// edit by skkim 2014-02-06 : 이미 화재 신고된 탐지신호에 대해 다시 선택 다이어로그가 뜨는 오류 수정
					// 탐지된 센서가 현재 유효한 프로세스인지 검사
					ProcessIF process = ProcessManager.Instance.GetProcess(m_DetectSensor.ID);
					if (process != null)
					{
						// 프로세스의 현재 상태가 탐지인경우, 선택 다이어로그를 추가한다.
						if (process.LastLog != null && process.LastLog.ReactionType == (int)ReactionType.BEGIN_STATUS)
						{
                            SeletCaseData form = new SeletCaseData(process.ProcessType, m_DetectSensor, m_nSensorHistoryID, process.ShowOpenSOP, process.DetectTime);

							ConfirmDialogManager.Instance.AddDialogFirst(form);
							//if (!FormMain.Instance.ShowEquipZoneCCTV)
							//{

                                // 화재시 CCTV보기 종료시 3D 탭으로 이동한다. 2015-07-08 skkim
                           //     FormMain.Instance.SelectMonitoringTab();

                                // 종료되지 않은 화재가 있다면 선택한다.
                            //    SeletCaseData form2 = ConfirmDialogManager.Instance.ShowDialogNext();
                            //    if (form2 != null)
                            //    {
                            //        int nID = form2.SensorHistoryID;
                            //        int nSensorID = form2.Sensor.ID;
                            //        FormMain.Instance.SelectSensorDetectProcess(nID, nSensorID);
                            //    }
                            //}
						}
					}
                    else
                    {
                        // 더이상 화재가 없는경우 원래 뷰로 복귀한다. 2015-06-30. skkim
                        FormMain.Instance.PageHome.ContentForm.RestoreViewState();
                    }
				}
                else
                {
                    // 더이상 화재가 없는경우 원래 뷰로 복귀한다. 2015-06-30. skkim
                    FormMain.Instance.PageHome.ContentForm.RestoreViewState();
                }
			}
			else
			{
				if (m_nTranslucentCommandID == ID.ID_VIEW_CCTV)
				{
					SeletCaseData form = (SeletCaseData)(ConfirmDialogManager.Instance.ShowDialogNext());
					if (form != null)
					{
						int nID = form.SensorHistoryID;
						int nSensorID = form.Sensor.ID;
						FormMain.Instance.SelectSensorDetectProcess(nID, nSensorID);
					}
                    else
                    {

                        if( UnE.SOP.ProxySOP.Instance.ShowCCTVForm)
                        {
                            FormMain.Instance.CCTVPipe.Send("ShowDefaultCCTV()");
                        }
                        // 더이상 화재가 없는경우 원래 뷰로 복귀한다. 2015-06-30. skkim
                        FormMain.Instance.PageHome.ContentForm.RestoreViewState();
                    }
				}
			}

			FormCCTVList frmCCTVList = FormMain.Instance.CCTVList;

			if (frmCCTVList != null && frmCCTVList.Visible)
				frmCCTVList.Close();

			FormMain.Instance.SetEnableToolBar();
            
		}

		public Point SelectCaseDetectPosition()
		{

            Point pt = FormMain.Instance.SelectCaseDlgLocation();
            Point pt2 = FormMain.Instance.PointToScreen(pt);
            return pt2;

            //FormContent2D frmContent = PageBackstageHome.Instance.ContentForm;
            //Point pt2 = new Point();
            //if (frmContent.NumLayout == 1)
            //{
            //    Point pt1 = new Point(frmContent.OutdoorView.Location.X + frmContent.OutdoorView.Size.Width / 2, frmContent.OutdoorView.Location.Y + frmContent.OutdoorView.Size.Height / 2);
            //    pt2 = frmContent.OutdoorView.PointToScreen(pt1);
            //    return pt2;
            //}
            //else if (frmContent.NumLayout == 3)
            //{
            //    Point pt1 = new Point(frmContent.IndoorView.Location.X + frmContent.IndoorView.Size.Width / 2, frmContent.IndoorView.Location.Y + frmContent.IndoorView.Size.Height / 2);
            //    pt2 = frmContent.IndoorView.PointToScreen(pt1);
            //    return pt2;
            //}
            //else if (frmContent.NumLayout == 2)
            //{
            //    Point pt = frmContent.IndoorView.PointToScreen(frmContent.IndoorView.Location);
            //    return pt;
            //}
            //return pt2;
		}

		public void OnPostPickPOI(POI poi)
		{
            // CCTV보기 모드가 아닌 모든 경우 frm4CCTV는 null임
            //Form4CCTV frm4CCTV = CCTVSelectionManager.Instance.GetCurrent();
            //if( frm4CCTV == null)
            //{
            //    CCTVSelectionManager.Instance.SetCurrent(m_frm4CCTV);

            //    // CCTV보기 모드가 아닌 모든 경우 m_frm4CCTV는 null임
            //    frm4CCTV = m_frm4CCTV;
            //}

            //// Simulator와 SDMS의 CCTVForm의 선택이 변경되는 경우
            //if( frm4CCTV != m_frm4CCTV)
            //{
            //    if(FormMain.Instance.EquipZoneCCTVMode && FormMain.Instance.ShowEquipZoneCCTV)
            //    {
            //        return;
            //    }
            //}

            // CCTV보기 모드이고 3D와 CCTV가 함께 보이는 경우
            // EquipZone CCTV 설정하기인 경우
            
            if( UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                if (poi.Type == IFacility.FacilityType.CCTV)
                {
                    CCTV cctv = (CCTV)poi.Facility;
                    FormMain.Instance.CCTVPipe.Send("EditEquipZoneCCTV(" + cctv.ID + ")");  
                }                    
            }
            else
            {
            //    if (frm4CCTV != null)
            //    {
            //        bool bThumbnailMode = frm4CCTV.GetOwner().ThumbnailMode;
            //        if (bThumbnailMode)
            //        {
            //            if (poi.Type == IFacility.FacilityType.CCTV)
            //            {
            //                CCTV cctv = (CCTV)poi.Facility;
            //                Form4CCTV.CCTV_POSITION pos = frm4CCTV.SetCCTV(cctv);

            //                if (frm4CCTV == m_frm4CCTV)
            //                {
            //                    if (cctv != null && FormMain.Instance.CCTVList != null)
            //                        FormMain.Instance.CCTVList.SelectCCTV(cctv.ID);

            //                    if (FormMain.Instance.EquipZoneCCTVMode && FormMain.Instance.ShowEquipZoneCCTV && pos != Form4CCTV.CCTV_POSITION.UNKNOWN)
            //                    {
            //                        int nCCTVID = poi.Facility == null ? -1 : poi.Facility.ID;

            //                        int nIdx = -1;
            //                        if (pos == Form4CCTV.CCTV_POSITION.TM)
            //                            nIdx = 0;
            //                        else if (pos == Form4CCTV.CCTV_POSITION.BM)
            //                            nIdx = 1;
            //                        else if (pos == Form4CCTV.CCTV_POSITION.BR)
            //                            nIdx = 2;
            //                        else if (pos == Form4CCTV.CCTV_POSITION.TR)
            //                            nIdx = 3;
            //                        else if (pos == Form4CCTV.CCTV_POSITION.TL)
            //                            nIdx = 4;
            //                        else if (pos == Form4CCTV.CCTV_POSITION.BL)
            //                            nIdx = 5;

            //                        if (nIdx >= 0)
            //                        {
            //                            EditEquipZoneCCTV editEquipZoneCCTV = CCTVManager.Instance.UpdateEquipZoneCCTV(nIdx, nCCTVID, FormMain.Instance.CurrentEquipZone);
            //                            if (editEquipZoneCCTV != null)
            //                                editEquipZoneCCTV.Update(FormMain.Instance.DBManager);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
                
                
            }            
		}

		private ArrayList GetCCTVList()
		{
			ArrayList arrCCTVs = new ArrayList();

			int nLayout = m_ContentForm.NumLayout;
            ISensorTooltipOwner viewOutdoor = m_ContentForm.OutdoorView;
            ISensorTooltipOwner viewIndoor = m_ContentForm.IndoorView;

			if (nLayout == 1)
			{
				InsertCCTVList(arrCCTVs, viewOutdoor.SelectedPOIList, viewOutdoor);
			}
			else if (nLayout == 2)
			{
				InsertCCTVList(arrCCTVs, viewOutdoor.SelectedPOIList, viewOutdoor);
				InsertCCTVList(arrCCTVs, viewIndoor.SelectedPOIList, viewIndoor);
			}
			else if (nLayout == 3)
			{
				InsertCCTVList(arrCCTVs, viewIndoor.SelectedPOIList, viewIndoor);
			}

			return arrCCTVs;
		}

        private void InsertCCTVList(ArrayList arrSrc, ArrayList arrPOIs, ISensorTooltipOwner view)
		{
			foreach (string szPOIID in arrPOIs)
			{
                POI poi = view.FindPOI(szPOIID);
				if (poi == null)
					continue;

				CCTV cctv = (CCTV)poi.Facility;

				if (!arrSrc.Contains(cctv))
					arrSrc.Add(cctv);
			}
		}        		

		private void timer1_Tick(object sender, EventArgs e)
		{
            GetEnableScreenName();

			if (ProcessManager.Instance.CurrentDetectProcess.Count == 0 && m_noProcessDisaster.CheckTimeout())
			{
				if (m_ContentForm != null && !m_ContentForm.IsDisposed)
				{
					m_ContentForm.HideZoneVolume();
					//m_ContentForm.RedrawWindow();
				}
			}
		}

        public void GetEnableScreenName()
        {
            string strScreenName = "재난탐지 시스템";
            /*string strScreenName = String.Empty;

            if (m_ReportForm.Visible == true)
            {
                strScreenName = "리포트";
            }
            else if (mTranslucentForm.Visible == true)
            {
                if (mTranslucentForm.InnerForm is Form4CCTV)
                    strScreenName = "화재개소 CCTV 화면";
            }

            if (String.IsNullOrWhiteSpace(strScreenName))
            {
                switch (m_ContentForm.NumLayout)
                {
                    case 1:
                        strScreenName = "3D 전체화면";
                        break;
                    case 2:
                        strScreenName = "2D / 3D 같이보기";
                        break;
                    case 3:
                        strScreenName = "2D 실내도면";
                        break;
                }
            }*/

            FormMain.Instance.SetTitle(strScreenName);
        }



        public void OnDeletePOI(POI poi)
        {
            switch (poi.Type)
            {
                case IFacility.FacilityType.CCTV:
                    EditCCTV cctv = new EditCCTV((CCTV)poi.Facility);
                    cctv.IsDeleting = true;
                    cctv.AddToManager(FormMain.Instance.PageHome);
                    break;

                case IFacility.FacilityType.FIRE_SENSOR:
                    EditFireSensor fireSensor = new EditFireSensor((FireSensor)poi.Facility);
                    fireSensor.IsDeleting = true;
                    fireSensor.AddToManager(FormMain.Instance.PageHome);
                    break;

                case IFacility.FacilityType.COOLER_SENSOR:
                    EditSpringCooler coolingSensor = new EditSpringCooler((SpringCooler)poi.Facility);
                    coolingSensor.IsDeleting = true;
                    coolingSensor.AddToManager(FormMain.Instance.PageHome);
                    break;

                case IFacility.FacilityType.PRESSURE_SENSOR:
                    EditPumpPressuerSensor pressureSensor = new EditPumpPressuerSensor((PumpPressureSensor)poi.Facility);
                    pressureSensor.IsDeleting = true;
                    pressureSensor.AddToManager(FormMain.Instance.PageHome);
                    break;
            }
        }

        public void AddCCTVEditData(POI poi, Zone zone)
        {
            CCTV cctv = (CCTV)poi.Facility;
            if (cctv == null)
                return;

            EditCCTV editCCTV = new EditCCTV(cctv);
            editCCTV.Position = new UnE.Geometry.Vertex3F(poi.X, poi.Y, poi.Z);
            editCCTV.Zone = zone;
            editCCTV.AddToManager(FormMain.Instance.PageHome);

            poi.Zone = editCCTV.Zone;
        }

        public void AddPressureSensorEditData(POI poi, Zone zone)
        {
            PumpPressureSensor facility = (PumpPressureSensor)poi.Facility;
            if (facility == null)
                return;

            EditPumpPressuerSensor editFacility = new EditPumpPressuerSensor(facility);
            editFacility.Position = new UnE.Geometry.Vertex3F(poi.X, poi.Y, poi.Z);
            editFacility.Zone = zone;
            editFacility.AddToManager(FormMain.Instance.PageHome);

            poi.Zone = editFacility.Zone;
        }

        public ToolStripMenuItem MenuIndoor
        {
            get { return ContentForm.GetMenu("Indoor"); }
        }

        public ToolStripMenuItem MenuManualReport
        {
            get { return ContentForm.GetMenu("ManualReport"); }
        }

        public ToolStripMenuItem MenuManualCCTV
        {
            get { return ContentForm.GetMenu("ManualCCTV"); }
        }

        public void MenuIndoorClicked(object sender, EventArgs e)
        {
            ContentForm.IndoorMenuClick(sender, e);
        }
        public void MenualReportClicked(object sender, EventArgs e)
        {
            ContentForm.ManualReportClick(sender, e);
        }
        public void ManualCCTVClicked(object sender, EventArgs e)
        {
            ContentForm.ManualCCTVClick(sender, e);
        }

        public void RemoveCCTVPOI(int nID)
        {
            ContentForm.Layers.GetLayer(ID.ID_LAYER_CCTV).Remove(nID);
            ContentForm.Layers.GetLayer(ID.ID_LAYER_CCTVLOW).Remove(nID);
            ContentForm.Layers.GetLayer(ID.ID_LAYER_CCTV_DISCONNECTED).Remove(nID);
        }

        public void RemoveCCTVPOI(int nLayerID, int nID)
        {
            ContentForm.Layers.GetLayer(nLayerID).Remove(nID);
        }

        public Building GetBuilding(string szBuildingName)
        {
            string szTemp = szBuildingName;

            if (UnE.SOP.ProxySOP.Instance.SiteID == 100)
            {
                // 서울대 버전은 z제거
                if (szBuildingName.StartsWith("z"))
                {
                    szTemp = szBuildingName.Remove(0, 1);
                }
            }

            if (UnE.SOP.ProxySOP.Instance.SiteID == 3)
            {
                // 에너지 광교 버전은 _MeshPart변환
                if (szBuildingName.Contains("_MeshPart"))
                {
                    string[] sp = szBuildingName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sp != null)
                    {
                        szTemp = sp[0];
                    }                
                }
            }

            return ZoneManager.Instance.GetBuilding(szTemp);
        }

        public Zone GetOutsideZone(float x, float y)
        {
            return ZoneManager.Instance.GetOutsideZone(x, y);
        }

        public Zone GetZone(string szBuildingID, int nFloor)
        {
            return ZoneManager.Instance.GetZone(szBuildingID, nFloor);
        }

        public EquipmentZone CheckEquipmentZone(Zone zone, float x, float y)
        {
            return ZoneManager.Instance.CheckEquipmentZone(zone, x, y);
        }

        public void EditCCTV(CCTV cctv )
        {
            EditCCTV editCCTV = new EditCCTV(cctv);
            editCCTV.AddToManager(this);
        }

        public void EditCCTV(CCTV cctv, string szDescription)
        {
            EditCCTV editCCTV = new EditCCTV(cctv);
            if (szDescription != null && szDescription != "")
            {
                editCCTV.Description = szDescription;
            }
            editCCTV.AddToManager(this);
        }


        public void EditPumpPressureSensor(PumpPressureSensor sensor)
        {
            EditPumpPressuerSensor editPump = new EditPumpPressuerSensor(sensor);
            editPump.AddToManager(this);
        }


        public void EditSpringCooler(SpringCooler sensor)
        {
            EditSpringCooler editSpringCooler = new EditSpringCooler(sensor);
            editSpringCooler.AddToManager(this);
        }

        public void EditFireSensor(FireSensor sensor)
        {
            EditFireSensor editFireSensor = new EditFireSensor(sensor);
            editFireSensor.AddToManager(this);
        }

        public ArrayList GetFireEquipments(Zone currentIndoorZone)
        {
            return FormMain.Instance.DataManager.GetFireEquipments(currentIndoorZone);
        }

        public void OnChangeIndoorZone(Zone currentZone)
        {
            FormMain.Instance.ChangeZoneComboBox(currentZone);
        }

        private void OnClickCCTVList()
        {
            if (m_dockFormLocation.GetTitle() == DockingFormLocation.OriginTitle)
            {
                ShowCCTVList();
            }
            else
            {
                // 이미 오른쪽 DockingPane이 닫혀있는 상태라면...
                if (splitContainer1.Panel2Collapsed)
                {
                    ShowCCTVList();
                }
                else
                {
                    // CCTV리스트를 표시하고 있다면.
                    if (m_dockFormLocation.CCTVMode == true)
                    {
                        HideRightDockingPane();
                    }
                    else
                    {
                        // PSM리스트를 표시하고 있다면.
                        ShowCCTVList();
                    }
                }
            }
        }

        private Timer m_ShowCCTVList = new Timer();
        private void ShowCCTVList()
        {
            if (m_frmCCTVList == null || m_frmCCTVList.IsDisposed)
            {
                m_frmCCTVList = new PopupDialog.FormCCTVList();
                m_frmCCTVList.LoadData();
                m_frmCCTVList.TopLevel = false;
            }
            else
                m_frmCCTVList.TopLevel = false;

            if (m_frmPSMList != null && m_frmPSMList.IsDisposed == false)
            {
                if (m_frmPSMList.Visible == true)
                    m_frmPSMList.Visible = false;
            }

            if (m_frmDisasterMgr != null && m_frmPSMList.IsDisposed == false)
            {
                if (m_frmDisasterMgr.Visible == true)
                    m_frmDisasterMgr.Visible = false;
            }

            m_dockFormLocation.SetTitle("CCTV 리스트");
            m_dockFormLocation.CCTVMode = true;
            m_dockFormLocation.SetPOI(null);

            splitContainer2.Panel2Collapsed = true;

            ShowAllDockingPane();

            splitContainer2.Refresh();
            
            //m_ShowCCTVList_Tick(null, null);
            if (m_ShowCCTVList.Enabled == true)
                m_ShowCCTVList.Enabled = false;

            m_ShowCCTVList = new Timer();
            m_ShowCCTVList.Interval = 1000;
            m_ShowCCTVList.Tick += m_ShowCCTVList_Tick;
            m_ShowCCTVList.Enabled = true;
        }

        void m_ShowCCTVList_Tick(object sender, EventArgs e)
        {
            m_ShowCCTVList.Enabled = false;
            
            ClearEditMode();

            m_dockFormLocation.AddControl(m_frmCCTVList);   
         
            m_frmCCTVList.BringToFront();
            m_frmCCTVList.Focus();
            m_frmCCTVList.SetFocusUser();

        }
        
        private void OnClickPSMList()
        {            
            if (m_dockFormLocation.GetTitle() == DockingFormLocation.OriginTitle)
            {
                ShowPSMList();
            }
            else
            {
                // 이미 오른쪽 DockingPane이 닫혀있는 상태라면...
                if (splitContainer1.Panel2Collapsed)
                    ShowPSMList();
                else
                {
                    if(m_dockFormLocation.PSMMode == true)
                    {
                        HideRightDockingPane();
                    }
                    else
                    {
                        ShowPSMList();
                    }

                }
            }
        }

        private Timer m_ShowPSMList = new Timer();
        private void ShowPSMList()
        {
            if (m_frmPSMList == null || m_frmPSMList.IsDisposed)
            {
                m_frmPSMList = new PopupDialog.FormPSMList();
                m_frmPSMList.TopLevel = false;              
            }
            else
                m_frmPSMList.TopLevel = false;

            if (m_frmCCTVList != null && m_frmPSMList.IsDisposed == false)
            {
                if (m_frmCCTVList.Visible == true)
                    m_frmCCTVList.Visible = false;
            }

            if (m_frmDisasterMgr != null && m_frmPSMList.IsDisposed == false)
            {
                if (m_frmDisasterMgr.Visible == true)
                    m_frmDisasterMgr.Visible = false;
            }

            m_dockFormLocation.SetTitle("유해 화학물질 리스트");
            m_dockFormLocation.PSMMode = true;
            m_dockFormLocation.SetPOI(null);

            splitContainer2.Panel2Collapsed = true;

            ShowAllDockingPane();

            splitContainer2.Refresh();

            if (m_ShowPSMList.Enabled == true)
                m_ShowPSMList.Enabled = false;

            m_ShowPSMList = new Timer();
            m_ShowPSMList.Interval = 1000;
            m_ShowPSMList.Tick += m_ShowPSMList_Tick;
            m_ShowPSMList.Enabled = true;            
        }
        void m_ShowPSMList_Tick(object sender, EventArgs e)
        {
            m_ShowPSMList.Enabled = false;

            ClearEditMode();

            m_dockFormLocation.AddControl(m_frmPSMList);

            m_frmPSMList.BringToFront();
            m_frmPSMList.Focus();
            m_frmPSMList.SetFocusUser();
        }

        private void OnClickDisasterMgr()
        {
            if (m_dockFormLocation.GetTitle() == DockingFormLocation.OriginTitle)
            {
                ShowDisasterMgr();
            }
            else
            {
                // 이미 오른쪽 DockingPane이 닫혀있는 상태라면...
                if (splitContainer1.Panel2Collapsed)
                    ShowDisasterMgr();
                else
                {
                    if (m_dockFormLocation.DisasterMgrMode == true)
                    {
                        HideRightDockingPane();
                    }
                    else
                    {
                        ShowDisasterMgr();
                    }

                }
            }
        }
         
        private Timer m_ShowDisasterMgr = new Timer();
        private void ShowDisasterMgr()
        {
            if (m_frmDisasterMgr == null || m_frmDisasterMgr.IsDisposed)
            {
                m_frmDisasterMgr = new PopupDialog.DisasterPrevention.FormDisasterPreventionManagement();
                m_frmDisasterMgr.TopLevel = false;
            }
            else
                m_frmDisasterMgr.TopLevel = false;

            if (m_frmCCTVList != null && m_frmDisasterMgr.IsDisposed == false)
            {
                if (m_frmCCTVList.Visible == true)
                    m_frmCCTVList.Visible = false;
            }

            if (m_frmPSMList != null && m_frmDisasterMgr.IsDisposed == false)
            {
                if (m_frmPSMList.Visible == true)
                    m_frmPSMList.Visible = false;
            }

            m_dockFormLocation.SetTitle("방재장비 관리");
            m_dockFormLocation.DisasterMgrMode = true;
            m_dockFormLocation.SetPOI(null);

            splitContainer2.Panel2Collapsed = true;

            ShowAllDockingPane();

            splitContainer2.Refresh();

            if (m_ShowDisasterMgr.Enabled == true)
                m_ShowDisasterMgr.Enabled = false;

            m_ShowDisasterMgr = new Timer();
            m_ShowDisasterMgr.Interval = 1000;
            m_ShowDisasterMgr.Tick += m_ShowDisasterPrevention_Tick;
            m_ShowDisasterMgr.Enabled = true;
        }
        void m_ShowDisasterPrevention_Tick(object sender, EventArgs e)
        {
            m_ShowDisasterMgr.Enabled = false;

            ClearEditMode();

            m_dockFormLocation.AddControl(m_frmDisasterMgr);

            m_frmDisasterMgr.BringToFront();
            m_frmDisasterMgr.Focus(); 
        } 

        private void ClearEditMode()
        {
            bool bChecked = FormMain.Instance.IsChecked(ID.ID_VIEW_PICK);

            if (FormMain.Instance.IsChecked(ID.ID_NEW_FIRE_SENSOR))
                CheckMouseWorkItem(ID.ID_NEW_FIRE_SENSOR, false);
            if (FormMain.Instance.IsChecked(ID.ID_NEW_COOLER_SENSOR))
                CheckMouseWorkItem(ID.ID_NEW_COOLER_SENSOR, false);
            if (FormMain.Instance.IsChecked(ID.ID_NEW_PRESSURE_SENSOR))
                CheckMouseWorkItem(ID.ID_NEW_PRESSURE_SENSOR, false);
            if (FormMain.Instance.IsChecked(ID.ID_NEW_CCTV))
                CheckMouseWorkItem(ID.ID_NEW_CCTV, false);
            if (FormMain.Instance.IsChecked(ID.ID_DEL_FACILITY))
                CheckMouseWorkItem(ID.ID_DEL_FACILITY, false);

            FormMain.Instance.CheckButton(ID.ID_VIEW_PICK, bChecked);
        } 

        public void SetPSMSensorStatus(int nSensorID, byte status, long beginWorkTime, long endWorkTime)
        {
            if (m_frmPSMList != null && !m_frmPSMList.IsDisposed)
            {
                m_frmPSMList.SetPSMSensorStatus(nSensorID, status, beginWorkTime, endWorkTime);
            }
        }

        public void RequestOutdoor()
        {
            if (m_ContentForm != null)
                m_ContentForm.LayoutOutside();
        }

        public void OnPostPanelMouseDown()
        {
            FormMain.Instance.EnableFireReportBtn(false);
        }

        private void PageBackstageHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseExternal();
        }


#region IFormContentViewOwner Section

        public void LoadPOI(UnE.Sensor.ISensorTooltipOwner view, bool bIndoor)
        {
            FormMain.Instance.DataManager.LoadPOI(view, bIndoor);
        }

        public void EnableFireReportBtn(bool bEnabled)
        {
            FormMain.Instance.EnableFireReportBtn(bEnabled);
        }

        public void EnableFireReportBtn(bool bEnabled, int nCase)
        {
            FormMain.Instance.EnableFireReportBtn(bEnabled, nCase);
        }
        
        public void ChangeZoneComboBox(UnE.Spatial.Zone zone)
        {
            FormMain.Instance.ChangeZoneComboBox(zone);
        }

        public void SetBuilingCollapseDetect(string strPosition, bool realMode)
        {
            FormMain.Instance.SetBuilingCollapseDetect(strPosition, realMode);
        }

        public void SetEarthquakeDetect(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {
            FormMain.Instance.SetEarthquakeDetect(nIntensity, fMagnitude, strPosition, isRealMode);
        }

        public void OnFinishEarthquake()
        {
            FormMain.Instance.SetEarthquakeDetect(0, 0.0f, "", true);
        }
        public void OnCollapseBuilding(string buildingID, bool isReal=false)
        {
            FormMain.Instance.ShowCollapseBuilding(isReal, buildingID);
            Building building = ZoneManager.Instance.GetBuilding(buildingID);
            
        }
     
        public void SelectIndoorZone(Zone zone)
        {
            FormMain.Instance.SelectIndoorZone(zone);
        }

        public void ShowCCTVForm(bool bShow)
        {
            FormMain.Instance.ShowCCTVForm(true);
        }

        public UnE.PSM.PSMSensor GetPSMSensor(int nID)
        {
            return PSMManager.Instance.GetSensor(nID);
        }

        public UnE.PSM.PSMMaterial GetPSMMaterial(int nID)
        {
            return PSMManager.Instance.GetMaterial(nID);
        }

        public DBUtility.WebDBManager DBManager
        {
            get { return FormMain.Instance.DBManager; }
        }
        public Form InvokeForm
        {
            get { return this; }
        }

        public string ResourcePath
        {
            get { return FormMain.EnginPath(); }
        }

        public IChangedDataManager IChangedDataManager
        {
            get { return this; }
        }

        public bool ExtractOutside
        {
            get { return ModelManager.Instance.ExtractInside; }
            set { ModelManager.Instance.ExtractInside = value; }
        }

        public bool ExtractInside
        {
            get { return ModelManager.Instance.ExtractInside; }
            set { ModelManager.Instance.ExtractInside = value; }
        }

        public void SetNoProcessDisaster(NoProcessDisaster.DisasterType type)
        {
            m_noProcessDisaster.DisasterTime = DateTime.Now;
            m_noProcessDisaster.Type = type;
        }
#endregion
	}

    // 재난이력에 남지는 않지만(재난 ComboBox & 리포트) 실제 재난상황과 동일하게 작동하는 신호
    public class NoProcessDisaster
    {
        public enum DisasterType { None, CollapseBuilding, Earthquake };

        private DisasterType m_type = DisasterType.None;
        private DateTime m_dtDisaster = new DateTime();

        public DisasterType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        // 재난 발생시간
        public DateTime DisasterTime
        {
            get { return m_dtDisaster; }
            set { m_dtDisaster = value; }
        }

        // 현재 발생한 재난이 있을 경우 해당 재난에 대한 유효시간이 지났는지 알려준다.
        // Return 값 : true(이미 유효기간이 지났다.)
        //             false(아직 유효한 재난이다.)
        public bool CheckTimeout()
        {
            if (m_type == DisasterType.None)
                return true;

            TimeSpan span = DateTime.Now - m_dtDisaster;
            bool timeout = false;

            switch (m_type)
            {
                case DisasterType.CollapseBuilding:
                    if (span.TotalSeconds >= 30.0)
                        timeout = true;
                    break;

                case DisasterType.Earthquake:
                    if (span.TotalSeconds >= 30.0)
                        timeout = true;
                    break;
            }

            if (timeout)
            {
                m_type = DisasterType.None;
                FormMain.Instance.SetNormalMode(0);
                FormMain.Instance.PageHome.ContentForm.RestoreViewState();
            }

            return timeout;
        }
    }
}