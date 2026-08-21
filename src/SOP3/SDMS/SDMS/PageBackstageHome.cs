using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremeCommandBars;
using DBUtility;
using System.Collections;
using System.Net;

using XtremeDockingPane;
using System.Diagnostics;
using System.Threading;

namespace SDMS
{
    public partial class PageBackstageHome : Form, IChangedDataManager
    {
        public const int OUTSIDE = 1;
        public const int BOTHSIDE = 2;
        public const int INSIDE = 3;

        public enum Tab { FILE_TAB = 0, MONITORING_TAB, ADMIN_TAB, REPORT_TAB };
        
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
        private Dictionary<int, bool> m_dicCheckedItem = new Dictionary<int, bool>();
        //////////////////////////////////////////////////////////////////////////

        private DockingPaneGlobalSettings DockingPaneGlobalSettings;
        
        private FormContent m_ContentForm = null;
        public FormContent ContentForm
        {
            get { return m_ContentForm; }
        }
        private FormReport m_ReportForm = null;

        public FormReport FrmReport
        {
            get { return m_ReportForm; }
        }      

        private static PageBackstageHome m_home = null;
        public static PageBackstageHome Instance
        {
            get { return m_home; }
        }   

        public AxXtremeDockingPane.AxDockingPane DockingPane
        {
            get { return m_axDockingPane; }
        }               

        private Pane m_paneLocation = null;
        private Pane m_paneProperties = null;
        private Pane m_paneFacilityList = null;

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

        private Form4CCTV m_frm4CCTV = null;
		public SDMS.Form4CCTV CCTVForm
		{
			get { return m_frm4CCTV; }
			set { m_frm4CCTV = value; }
		}
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


        private static PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();
		public static SDMS.PopupTranslucentForm TranslucentForm
		{
			get { return mTranslucentForm; }
			set { mTranslucentForm = value; }
		}
		public static void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
		{
			if (targetForm == null)
				return;

			if (nCommandID != ID.ID_VIEW_CCTV && nCommandID != ID.ID_VIEW_SMS)
				FormMain.Instance.ShowLeftLayer();

			FormMain.Instance.SetDisableToolBar();

			if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
				mTranslucentForm = new PopupTranslucentForm();
            
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
			if(  form == null)
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
			Debug.WriteLine("Page Start : " + DateTime.Now);
            m_home = this;

            InitializeComponent();

            // Report용 탭 Form
            CreateReportForm();
            // 3D 용 탭
            CreateContentForm();   

            CreateDockingPane();

            InitCheckedItem();

        }

		public int GetLayout()
		{
			if (m_ContentForm != null)
				return m_ContentForm.NumLayout;
			return OUTSIDE;
		}

        public bool ChangeFloor(BuildingGroup grp, Building building , Zone zoneFloor)
        {
            int nFloorIdx = zoneFloor.FloorIndex;

            if (m_ContentForm.NumLayout == OUTSIDE)
            {
                m_ContentForm.LayoutInside();
                Check3DViewMode(ID.ID_VIEW_INSIDE);
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
            m_ContentForm.Invalidate3DView(bEraBack);
            return true;
        }

        public int ChangeTab(Tab tab)
        {
            switch (tab)
            {
                case Tab.MONITORING_TAB:
                    m_ContentForm.EditMode = false;
                    if (m_ContentForm.EditMode == false)
                    {
                        if (m_ContentForm.CurrentMouseWorkMode != BaseViewEx.MouseWorkMode.ORBIT &&
                            m_ContentForm.CurrentMouseWorkMode != BaseViewEx.MouseWorkMode.PANNING)
                        {
                            CheckMouseWorkItem(ID.ID_VIEW_PICK, true);
                        }
                    }
                    m_ReportForm.Visible = false;
                    HideAllDockingPane();
                    break;

                case Tab.ADMIN_TAB:
                    m_ContentForm.EditMode = true;
                    m_ReportForm.Visible = false;
                    ShowAllDockingPane();
                    break;

                case Tab.REPORT_TAB:
                    m_ContentForm.EditMode = false;
                    if (m_ContentForm.CurrentMouseWorkMode != BaseViewEx.MouseWorkMode.ORBIT &&
                            m_ContentForm.CurrentMouseWorkMode != BaseViewEx.MouseWorkMode.PANNING)
                    {
                        CheckMouseWorkItem(ID.ID_VIEW_PICK, true);
                    }
                    m_ReportForm.Visible = true;
                    HideAllDockingPane();
                    break;
            }
            return (int)tab;
        }

        private void ShowAllDockingPane()
        {
            m_axDockingPane.ShowPane(m_paneLocation.Id);
            m_axDockingPane.ShowPane(m_paneFacilityList.Id);
            m_axDockingPane.ShowPane(m_paneProperties.Id);
        }

        private void HideAllDockingPane()
        {
            //m_axDockingPane.HidePane(m_paneLocation);
            //m_axDockingPane.HidePane(m_paneFacilityList);
            //m_axDockingPane.HidePane(m_paneProperties);
            m_paneLocation.Close();
            m_paneFacilityList.Close();
            m_paneProperties.Close();
        }

        /*private void CreateDockingPane()
        {
            m_dockFormLocation = new DockingFormLocation();
            m_dockFormProperties = new DockingFormProperties();
            m_dockFormFacilityList = new DockingFormFacilityList();

            this.Controls.Add(m_dockFormLocation);
            this.Controls.Add(m_dockFormProperties);
            this.Controls.Add(m_dockFormFacilityList);
        }*/

        private void CreateDockingPane()
        {
            m_dockFormLocation = new DockingFormLocation();
            m_dockFormProperties = new DockingFormProperties();
            m_dockFormFacilityList = new DockingFormFacilityList();

            DockingPane.Options.AlphaDockingContext = true;
            DockingPane.Options.ShowDockingContextStickers = true;
            DockingPane.Options.ThemedFloatingFrames = true;

            m_nPrevLocationPaneHeight = 200;
            m_nPrevPropertyPaneHeight = 600;
            m_nPrevFacilityPaneHeight = 200;
                        
            m_paneLocation = DockingPane.CreatePane(1, 300, m_nPrevLocationPaneHeight, DockingDirection.DockRightOf, null);
            m_paneProperties = DockingPane.CreatePane(2, 300, m_nPrevPropertyPaneHeight, DockingDirection.DockBottomOf, m_paneLocation);
            m_paneFacilityList = DockingPane.CreatePane(3, 300, m_nPrevFacilityPaneHeight, DockingDirection.DockBottomOf, m_paneProperties);

            m_paneLocation.Title = "위치 정보";
            m_paneProperties.Title = "속성 정보";
            m_paneFacilityList.Title = "센서구역/시설 리스트";

            m_paneLocation.Options = PaneOptions.PaneHasMenuButton | PaneOptions.PaneNoCloseable ;// | PaneOptions.PaneNoDockable;
            m_paneProperties.Options = PaneOptions.PaneHasMenuButton | PaneOptions.PaneNoCloseable ;//| PaneOptions.PaneNoDockable;
            m_paneFacilityList.Options = PaneOptions.PaneHasMenuButton | PaneOptions.PaneNoCloseable;// | PaneOptions.PaneNoDockable;

            DockingPane.PanePopupMenu += new AxXtremeDockingPane._DDockingPaneEvents_PanePopupMenuEventHandler(DockingPane_PanePopupMenu);
        }

        private void DockingPane_PanePopupMenu(object sender, AxXtremeDockingPane._DDockingPaneEvents_PanePopupMenuEvent e)
        {
            /*int left, right, bottom, top, titleHeight = 24;
            e.pane.GetClientRect(out left, out top, out right, out bottom);*/

            if (e.pane.Id == 1)
            {
                if (m_isClosedLocationPane)
                {
                    DockingPane.DockPane(m_paneLocation, 300, m_nPrevLocationPaneHeight, DockingDirection.DockTopOf, m_paneProperties);
                    m_isClosedLocationPane = false;
                }
                else
                {
                    DockingPane.DockPane(m_paneLocation, 300, 0, DockingDirection.DockTopOf, m_paneProperties);
                    //m_nPrevLocationPaneHeight = bottom - top + titleHeight;
                    m_isClosedLocationPane = true;
                }
            }
            else if (e.pane.Id == 2)
            {
                if (m_isClosedPropertyPane)
                {
                    DockingPane.DockPane(m_paneProperties, 300, m_nPrevPropertyPaneHeight, DockingDirection.DockBottomOf, m_paneLocation);
                    m_isClosedPropertyPane = false;
                }
                else
                {
                    DockingPane.DockPane(m_paneProperties, 300, 0, DockingDirection.DockBottomOf, m_paneLocation);
                    m_isClosedPropertyPane = true;
                }
            }
            else if (e.pane.Id == 3)
            {
                if (m_isClosedFacilityPane)
                {
                    DockingPane.DockPane(m_paneFacilityList, 300, m_nPrevFacilityPaneHeight, DockingDirection.DockBottomOf, m_paneProperties);
                    m_isClosedFacilityPane = false;
                }
                else
                {
                    DockingPane.DockPane(m_paneFacilityList, 300, 0, DockingDirection.DockBottomOf, m_paneProperties);
                    m_isClosedFacilityPane = true;
                }
            }
        }

        private void DockingPane_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;
            DockingPane.GetClientRect(out left, out top, out right, out bottom);
            
            m_ContentPanel.SetBounds(left, top, right - left, bottom - top);      
        }

        private void DockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
 
            if (e.item.Id == 1)
                e.item.Handle = m_dockFormLocation.Handle.ToInt32();
            else if (e.item.Id == 2)
                e.item.Handle = m_dockFormProperties.Handle.ToInt32();
            else if (e.item.Id == 3)
                e.item.Handle = m_dockFormFacilityList.Handle.ToInt32();
        }

        private void PageBackstageHome_Load(object sender, EventArgs e)
        {
			
			//Debug.WriteLine(DateTime.Now);
            //OnChangeTheme(ID.ID_OPTIONS_STYLEOFFCIE2010BLACK);
            string strSkinFolder = StylesPath();
			//Debug.WriteLine(DateTime.Now);
            m_ContentForm.Show();
			//Debug.WriteLine(DateTime.Now);
            ModelManager.Instance.TargetForm = m_ContentForm;                
            ModelManager.Instance.Read3DModel();
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

        private void SkinLoad(string strSkinFolder)
        {
            axSkinFramework.LoadSkin(strSkinFolder + "Office2010.cjstyles", "Normalblue.ini");
            axSkinFramework.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BACKGROUND);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");
            return strExePath + "\\Styles\\";
        }

        private void CreateContentForm()
        {
            try
            {
                m_ContentForm = new FormContent();
            }
            catch (System.Exception)
            {
                MessageBox.Show("3D 환경을 초기화 하지 못하였습니다.\n모니터링을 종료합니다.");
                Application.Exit();
                return;
            }

            m_ContentForm.TopLevel = false;
            m_ContentForm.Parent = m_ContentPanel;
            m_ContentForm.Dock = DockStyle.Fill;
            m_ContentPanel.Controls.Add(m_ContentForm);  
        }

        private void CreateReportForm()
        {
            m_ReportForm = new FormReport();

            m_ReportForm.TopLevel = false;
            m_ReportForm.Parent = m_ContentPanel;
            m_ReportForm.Dock = DockStyle.Fill;

            m_ContentPanel.Controls.Add(m_ContentForm);           
        }               

        public void OnUpdateChangeLayer(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {
            switch (e.control.Id)
            {
                case ID.ID_LAYER_DETECTOR:
                    e.control.Checked = m_bShowDetectorLayer;
                    break;
                case ID.ID_LAYER_COOLER:
                    e.control.Checked = m_bShowCoolerLayer;
                    break;
                case ID.ID_LAYER_PERSURE:
                    e.control.Checked = m_bShowPressureLayer;
                    break;
                case ID.ID_LAYER_CCTV:
                    e.control.Checked = m_bShowCCTVLayer;
                    break;
                case ID.ID_LAYER_FIREEXT:
                    e.control.Checked = m_bShowFireExtinguisherLayer;
                    break;
                case ID.ID_LAYER_FIREHYD:
                    e.control.Checked = m_bShowFireHydrantLayer;
                    break;
                case ID.ID_LAYER_ALARMSTA:
                    e.control.Checked = m_bShowAlarmStationLayer;
                    break;
                case ID.ID_LAYER_RECIVER:
                    e.control.Checked = m_bShowReciverLayer;
                    break;
                case ID.ID_LAYER_TEXTPOI:
                    e.control.Checked = m_bShowTextPOILayer;
                    break;
                case ID.ID_LAYER_CCTVLOW:
                    e.control.Checked = m_bShowLowCCTVLayer;
                    break;
                default:
                    break;
            };
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
                default:
                    break;
            };            
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
                case ID.ID_VIEW_HOME:
                    m_ContentForm.HomeView();
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
                    m_ContentForm.LayoutOutside();
                    Check3DViewMode(nID);
                    break;
                case ID.ID_VIEW_BOTHSIDE:
                    m_ContentForm.LayoutBothside();
                    Check3DViewMode(nID);
                    break;
                case ID.ID_VIEW_INSIDE:
                    m_ContentForm.LayoutInside();
                    Check3DViewMode(nID);
                    break;
                case ID.ID_VIEW_CCTV:
                    OnClickBigCCTV();
                    break;
                case ID.ID_VIEW_SCREENSHOT:
                    m_ContentForm.SaveToImage();
                    //m_ContentForm.HideZoneVolume();
                    //m_ContentForm.RedrawWindow();
                    break;               
                default:
                    break;
            };
            m_ContentForm.RedrawWindow();
        }

        private void OnClickBigCCTV()
        {
            m_ContentForm.LayoutBothside();

            FireDetectProcess process = FormMain.Instance.CurrentFireDetectProcess;

            Zone zoneTarget = null;
            bool bOutterZone = false, bSituation = true;

            if (process != null)
            {
                EquipmentZone equipZone = process.TargetZone;

                if (equipZone != null)
                {
                    zoneTarget = equipZone.LinkedZone;

                    if (zoneTarget != null)
                        bOutterZone = zoneTarget.Building == null;
                }
            }
            
            ShowBigCCTV(zoneTarget, bSituation, bOutterZone);
        }

        public void ShowBigCCTV(Zone zoneTarget = null, bool bSituation = true, bool bOutterZone = false)
        {
			ArrayList arrCCTVs = GetZoneCCTVArray(zoneTarget, bSituation, bOutterZone);
            ShowBigCCTV(zoneTarget, arrCCTVs);
        }

		public void ShowBigCCTV(EquipmentZone equipZone, bool bSituation)
		{
			if (bSituation == true)
			{
				ShowBigCCTV(equipZone.LinkedZone, bSituation);

			}
			else
			{
				ArrayList arrCCTVs = GetEquipZoneCCTVList(equipZone);
				ShowBigCCTV(equipZone.LinkedZone, arrCCTVs);
			}
		}

        private void ShowBigCCTV(Zone zoneTarget, ArrayList arrCCTVs)
        {
            if (arrCCTVs == null)
                return;

            int nCCTVCount = arrCCTVs.Count;

            FormMain.Instance.ShowLeftThumbnail();

            m_frm4CCTV = new Form4CCTV(this);
            m_frm4CCTV.SetCCTV(arrCCTVs, zoneTarget);
            m_frm4CCTV.SetCCTVMode(FormMain.Instance.GetCCTVMode());

            ShowTranslucentForm(m_frm4CCTV, 0, 0, m_frm4CCTV.Size.Width, m_frm4CCTV.Size.Height, ID.ID_VIEW_CCTV);
            FormMain.Instance.SetEnableToolBar();
        }

        public void SetCCTVMode(CCTVMode mode)
        {
            if (m_frm4CCTV != null)
            {
                m_frm4CCTV.SetCCTVMode(mode);
            }
        }

        public void HideAllPOIPopup()
        {
            if (m_ContentForm != null)
                m_ContentForm.HideAllPOIPopup();
        }
		
        private ArrayList GetZoneCCTVArray(Zone zoneTarget, bool bSituation, bool bOutterZone = false)
        {
            FormMain.Instance.ShowEquipZoneCCTV = true;

            m_ContentForm.HideAllPOIPopup();
            m_ContentForm.EditMode = false;
            if (m_ContentForm.CurrentMouseWorkMode != BaseViewEx.MouseWorkMode.PICK)
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
            FireDetectProcess currentProcess = FormMain.Instance.CurrentFireDetectProcess;
            EquipmentZone currentEquipZone = FormMain.Instance.CurrentEquipZone;

            // 영역별 CCTV List 설정 모드이거나 화재가 탐지된 상황일 경우
            if ((FormMain.Instance.ShowEquipZoneCCTV || currentProcess != null) &&
                currentEquipZone != null)
            {
                EquipmentZone targetEquipZone = null;

                if (currentProcess != null)
                {
                    if (currentProcess.TargetSensor != null)
                    {
                        targetEquipZone = ZoneManager.Instance.GetEquipZone(currentProcess.TargetSensor.EquipZoneID);
                    }
                }

                if (targetEquipZone == null)
                    targetEquipZone = currentEquipZone;

				if (bOutterZone == true)
				{
					ArrayList arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zoneTarget);
					if (arEquipzone != null && arEquipzone.Count > 0)
					{
						EquipmentZone equipZone = (EquipmentZone)arEquipzone[0];
						arrCCTVs = GetEquipZoneCCTVList(equipZone);
					}
				}
				else
					arrCCTVs = GetEquipZoneCCTVList(targetEquipZone);
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

        public void ShowBigCCTV(EquipmentZone equipZone)
        {
            //Zone zone = null;

            if (equipZone != null)
            {
                ArrayList arrCCTVs = GetZoneCCTVArray(equipZone.LinkedZone, true);

                if (arrCCTVs == null)
                    arrCCTVs = new ArrayList();

                m_frm4CCTV.SetCCTV(arrCCTVs, equipZone.LinkedZone);
            }
        }

        private ArrayList GetEquipZoneCCTVList(EquipmentZone equipZone)
        {
            ArrayList arr = new ArrayList();

            CCTV[] arrCCTVs = CCTVManager.Instance.GetCCTVArray(equipZone);

            System.Diagnostics.Trace.WriteLine("GetEquipzoneCCTVList equipZoneName : " + equipZone.ZoneName);

            if (arrCCTVs == null)
            {
                System.Diagnostics.Trace.WriteLine("CCTV is null");
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

                    if (arrCCTVs[i] != null)
                        System.Diagnostics.Trace.WriteLine(i.ToString() + " : " + arrCCTVs[i].AccessKey);
                }
            }

            return arr;
        }

        private void Check3DViewMode(int nID)
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
						ShowTranslucentForm(form, 200,200, form.Size.Width, form.Size.Height, nID);
                    }
                    break;
                case ID.ID_MANAGE_MANAGER:
                    {
                        FormManager frm = new FormManager();
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
                    break;
                case ID.ID_SAVE_DATA:
                    if (m_arrChangedData.Count > 0)
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
                default:
                    break;
            };
            m_ContentForm.RedrawWindow();
        }

        private bool ValidPassword()
        {
            FormEditPassword frm = new FormEditPassword();

            if (FormMain.Instance.CCTVList != null)
                FormMain.Instance.CCTVList.Hide();

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return true;

            return false;
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
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.PICK;
            else if (frmMain.IsChecked(ID.ID_VIEW_PAN))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.PANNING;
            else if (frmMain.IsChecked(ID.ID_VIEW_ORBIT))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.ORBIT;
            else if (frmMain.IsChecked(ID.ID_NEW_FIRE_SENSOR))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.NEW_FIRE_SENSOR;
            else if (frmMain.IsChecked(ID.ID_NEW_COOLER_SENSOR))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.NEW_COOLER_SENSOR;
            else if (frmMain.IsChecked(ID.ID_NEW_PRESSURE_SENSOR))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.NEW_PRESSURE_SENSOR;
            else if (frmMain.IsChecked(ID.ID_NEW_CCTV))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.NEW_CCTV;
            else if (frmMain.IsChecked(ID.ID_DEL_FACILITY))
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.DEL_FACILITY;
            else
                m_ContentForm.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.NONE;
        }

        public void ShowRightDockingPane()
        {
            m_paneLocation.Hidden = false;
            m_paneProperties.Hidden = false;
            m_paneProperties.Hidden = false;

            m_dockFormLocation.SetPOI(m_poiSelected);
            m_dockFormProperties.SetPOI(m_poiSelected);
        }

        public void HideRightDockingPane()
        {
            m_paneFacilityList.Hidden = true;
            m_paneProperties.Hidden = true;
            m_paneLocation.Hidden = true;
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

            Button btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
            btn.Enabled = false;
            FormMain.Instance.CheckButton(btn, false);
        }

        private void PostChangedData()
        {
            if (m_isChangedFacilityManager)
            {
                NetworkManager.Instance.SendChangeFacilityManager();
            }

			if (m_isChangedEquipZoneCCTV)
			{
				int nID = FormMain.Instance.CurrentEquipZone.ID;
				NetworkManager.Instance.SendChangeEquipZoneCCTV(nID);				
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
            DockingPaneGlobalSettings = new DockingPaneGlobalSettings();
            DockingPane.VisualTheme = VisualTheme.ThemeVisualStudio2010;
            DockingPane.RecalcLayout();
            DockingPane.RedrawPanes();
        }


		private SensorZone m_DetectSensor = null;
		private EquipmentZone m_DetectZone = null;
        private int m_nSensorHistoryID = -1;

		public void FireDetect(SensorZone sensor, EquipmentZone zone, int nSensorHistoryID)
		{
			m_DetectSensor = sensor;
			m_DetectZone = zone;
            m_nSensorHistoryID = nSensorHistoryID;
		}

		public void EndFireProcess(int nSensorID)
		{
			m_DetectSensor = null;
			m_DetectZone = null;
		}

        public void OnTranslucentFormClosing()
        {
            if (m_nTranslucentCommandID == ID.ID_VIEW_CCTV)
            {
                m_frm4CCTV.OnFormClosing();
                FormMain.Instance.ShowEquipZoneCCTV = false;
                FormMain.Instance.ShowLeftLayer();
                m_frm4CCTV = null;
            }

			if (m_bFireDetectCCTVMode == true)
			{
				m_bFireDetectCCTVMode = false;
				if (m_DetectSensor != null)
				{
					// edit by skkim 2014-02-06 : 이미 화재 신고된 탐지신호에 대해 다시 선택 다이어로그가 뜨는 오류 수정
					// 탐지된 센서가 현재 유효한 프로세스인지 검사
					FireDetectProcess process =  (FireDetectProcess)ProcessManager.Instance.GetProcess(m_DetectSensor.ID);
					if (process != null)
					{
						// 프로세스의 현재 상태가 탐지인경우, 선택 다이어로그를 추가한다.
						if (process.LastLog.ReactionType == (int)ReactionType.BEGIN_STATUS)
						{
                            SeletCaseData form = new SeletCaseData(m_DetectSensor, m_nSensorHistoryID);

							ConfirmDialogManager.Instance.AddDialogFirst(form);
							if (!FormMain.Instance.ShowEquipZoneCCTV)
							{
                                SeletCaseData form2 = ConfirmDialogManager.Instance.ShowDialogNext();
								if (form2 != null)
								{
									int nID = form2.SensorHistoryID;
									int nSensorID = form2.Sensor.ID;
									FormMain.Instance.SelectFireDetectProcess(nID, nSensorID);
								}
							}
						}						
					}				

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
						FormMain.Instance.SelectFireDetectProcess(nID, nSensorID);
					}
				}
			}

            FormCCTVList frmCCTVList = FormMain.Instance.CCTVList;

            if (frmCCTVList != null && frmCCTVList.Visible)
                frmCCTVList.Close();

			FormMain.Instance.SetEnableToolBar();
        }

        public Point SelectCaseFirestPosition()
        {
            FormContent frmContent = PageBackstageHome.Instance.ContentForm;

            BaseViewEx view = null;

            if (frmContent.NumLayout == 1)
                view = frmContent.OutdoorView;
            else if (frmContent.NumLayout == 3)
                view = frmContent.IndoorView;
            else if (frmContent.NumLayout == 2)
            {
                Point pt = frmContent.IndoorView.PointToScreen(frmContent.IndoorView.Location);
                return pt;
            }

            Point pt1 = new Point(view.Location.X + view.Size.Width / 2, view.Location.Y + view.Size.Height / 2);
            Point pt2 = view.PointToScreen(pt1);
            return pt2;
        }

        public void OnPostPickPOI(POI poi)
        {
            if (FormMain.Instance.ThumbnailMode && m_frm4CCTV != null)
            {
                if (poi.Type == Facility.FacilityType.CCTV)
                {
                    CCTV cctv = (CCTV)poi.Facility;
                    Form4CCTV.CCTV_POSITION pos = m_frm4CCTV.SetCCTV(cctv);

                    if (cctv != null && FormMain.Instance.CCTVList != null)
                        FormMain.Instance.CCTVList.SelectCCTV(cctv.ID);

                    if (FormMain.Instance.ShowEquipZoneCCTV && pos != Form4CCTV.CCTV_POSITION.UNKNOWN)
                    {
                        EditEquipZoneCCTV editEquipZoneCCTV = CCTVManager.Instance.UpdateEquipZoneCCTV((int)pos, poi.Facility.ID, FormMain.Instance.CurrentEquipZone);

                        if (editEquipZoneCCTV != null)
                            editEquipZoneCCTV.AddToManager(this);
                    }
                }
            }
        }

        private ArrayList GetCCTVList()
        {
            ArrayList arrCCTVs = new ArrayList();

            int nLayout = m_ContentForm.NumLayout;
            BaseViewEx viewOutdoor = m_ContentForm.OutdoorView;
            BaseViewEx viewIndoor = m_ContentForm.IndoorView;

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

        private void InsertCCTVList(ArrayList arrSrc, ArrayList arrPOIs, BaseViewEx view)
        {
            foreach (int nPOIID in arrPOIs)
            {
                POI poi = view.FindPOI(nPOIID);
                if (poi == null)
                    continue;

                CCTV cctv = (CCTV)poi.Facility;

                if (!arrSrc.Contains(cctv))
                    arrSrc.Add(cctv);
            }
        }

        public void ShowEquipZoneCCTVs(EquipmentZone equipZone)
        {
            if (equipZone == null)
                return;

            if (m_frm4CCTV == null)
                return;

            CCTV[] arrCCTVs = CCTVManager.Instance.GetCCTVArray(equipZone);

            if (arrCCTVs == null)
            {
                m_frm4CCTV.SetCCTV(Form4CCTV.CCTV_POSITION.TL, null);
                m_frm4CCTV.SetCCTV(Form4CCTV.CCTV_POSITION.BL, null);
                m_frm4CCTV.SetCCTV(Form4CCTV.CCTV_POSITION.BR, null);
                m_frm4CCTV.SetCCTV(Form4CCTV.CCTV_POSITION.TR, null);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    m_frm4CCTV.SetCCTV((Form4CCTV.CCTV_POSITION)i, arrCCTVs[i]);
                }
            }

            m_frm4CCTV.UpdateCCTVGuide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
            {
                if (m_ContentForm != null && !m_ContentForm.IsDisposed)
                {
                    m_ContentForm.HideZoneVolume();
                    m_ContentForm.RedrawWindow();
                }                
            }
        }
    }
}
