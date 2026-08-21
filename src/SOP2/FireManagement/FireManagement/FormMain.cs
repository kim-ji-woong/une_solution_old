using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public partial class FormMain : Form
    {
        private XtremeCommandBars.RibbonBar RibbonBar = null;
        private XtremeCommandBars.CommandBarPopup ControlHome;
        private XtremeCommandBars.RibbonBackstageTab m_ControlOption;
        private XtremeCommandBars.RibbonTab m_TabFire = null;
        private XtremeCommandBars.RibbonTab m_TabAdmin = null;

        private XtremeCommandBars.CommandBarControl ctrlAdd = null;
        private XtremeCommandBars.CommandBarControl ctrlCheck = null;
        private XtremeCommandBars.CommandBarControl ctrlEditPos = null;
        private XtremeCommandBars.CommandBarControl ctrlDel = null;
        private XtremeCommandBars.CommandBarControl ctrlFireExtinguisher = null;
        private XtremeCommandBars.CommandBarControl ctrlFireplugVisible = null;
        private XtremeCommandBars.CommandBarControl ctrlFireAlarmVisible = null;
        private XtremeCommandBars.CommandBarControl ctrlAdminVersion = null;
        private XtremeCommandBars.CommandBarControl ctrlAdminProperties = null;
        private XtremeCommandBars.CommandBarControl ctrlAdminCheck = null;

        private PageBackstageNew m_pageNew;
        private PageBackstageOpen m_pageOpen;
        private PageBackstageSave m_pageSave;
        private PageBackstageSetup m_pageSetup;

        private DXFManager m_dxfManager = new DXFManager();
        private Dictionary<FireEquipment.EquipmentType, DXFViewer.Layer> m_dicEquipmentLayer = new Dictionary<FireEquipment.EquipmentType, DXFViewer.Layer>();

        private FormPanel m_frmPanel = null;

        private IOManager m_ioMgr = null;
        private WebDBManager m_dbMgr = null;
        private DataFileManager m_fileMgr = null;

        private bool m_isPCMode = true;

        // 도면내 설비들을 하나씩 Click해 나가면서 설비 ID를 입력해야 하는 모드인가?
        private bool m_tagInputMode = false;
        private int m_nBluetoothComport = 0;

        private Zone m_zoneCurrent = null;

        protected string m_strSkinFolder;

        private FormAddEquip m_frmAddEquip = null;
        private FormCheckEquip2 m_frmCheckEquip = null;
        private FormEquipHistory m_frmEquipHistory = null;

        private int m_nSOPGenUserID = 1;

        private Ubists.RFIDReader m_rfidReader = new Ubists.RFIDReader();

        //////////////////////////////////////////////////////////////////////////
        static public FormMain Instance;
        public FormMain()
        {
            InitializeComponent();

            Instance = this;
            InitMode();

            m_strSkinFolder = StylesPath();
            SkinLoad();

            m_dbMgr = new WebDBManager();
            m_ioMgr = new IOManager();
            m_fileMgr = new DataFileManager();

            m_frmAddEquip = new FormAddEquip();
            m_frmCheckEquip = new FormCheckEquip2();
            m_frmEquipHistory = new FormEquipHistory();

            LoadDB();
        }

        private void InitMode()
        {
            Utility m_ini = new Utility();
            string strPath = Application.StartupPath + "\\FMConfig.ini";

            string strValue = m_ini.getinivalue("General", "isPCMode", strPath);
            m_isPCMode = strValue == "1" ? true : false;

            strValue = m_ini.getinivalue("General", "TagInputMode", strPath);
            m_tagInputMode = strValue == "1" ? true : false;

            strValue = m_ini.getinivalue("General", "BluetoothComport", strPath);
            int.TryParse(strValue, out m_nBluetoothComport);
        }

        private void SkinLoad()
        {
            axSkinFramework.LoadSkin(m_strSkinFolder + "Vista.cjstyles", "");
            axSkinFramework.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

		//public string ResourcePath()
		//{
		//    string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

		//    if (System.IO.Directory.Exists(strExePath + "\\..\\res"))
		//        return strExePath + "\\..\\res\\";

		//    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\res"))
		//        return strExePath + "\\..\\..\\res\\";

		//    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\res"))
		//        return strExePath + "\\..\\..\\..\\res\\";

		//    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\src\\SOP2\\SOPMonitoringSystem\\SOPMonitoringSystem\\res"))
		//    {
		//        int nIndex = strExePath.LastIndexOf('\\');
		//        string strTemp = strExePath.Substring(0, nIndex);
		//        nIndex = strTemp.LastIndexOf('\\');
		//        string strTemp2 = strTemp.Substring(0, nIndex);
		//        nIndex = strTemp2.LastIndexOf('\\');
		//        string strTemp3 = strTemp2.Substring(0, nIndex);
		//        strExePath = strTemp3 + "\\src\\SOP2\\SOPMonitoringSystem\\SOPMonitoringSystem\\res\\";
		//        return strExePath;
		//    }

		//    return strExePath + "\\res\\";
		//}

        private void FormMain_Activated(object sender, EventArgs e)
        {
            if (m_frmPanel == null)
            {
                m_frmPanel = new FormPanel();

                m_frmPanel.Location = new Point(0, 0);
                m_frmPanel.Dock = DockStyle.Fill;
                m_frmPanel.TopLevel = false;
                m_frmPanel.Parent = this;
                panelMain.Controls.Add(m_frmPanel);
                m_frmPanel.Show();
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadIcons();
            CreateRibbonBar();

            int left, top, right, bottom;

            axCommandBars.GetClientRect(out left, out top, out right, out bottom);
            panelMain.SetBounds(left, top, right - left, bottom - top);

            StatusText = "";

            this.WindowState = FormWindowState.Maximized;

            //axCommandBars.VisualTheme = XtremeCommandBars.XTPVisualTheme.xtpThemeOffice2003;
            //axCommandBars.VisualTheme = XtremeCommandBars.XTPVisualTheme.xtpThemeResource;
        }

        private void LoadDB()
        {
            if (IsPCMode)
                m_ioMgr.LoadDB();
            else
            {
                bool isPCMode = !m_isPCMode;
                m_fileMgr.ImportData(System.Windows.Forms.Application.StartupPath + "\\" + IOManager.TabletDataFile, ref isPCMode);
            }
        }


		public static Bitmap GetImageByName(string imageName)
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("neutral");
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			string resourceName = "FireManagement.Properties.Resources";
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

            //string resDir = ResourcePath();

            AddBitmapFormRes("RibbonControl", new int[] { ID.ID_FACILITIES_ADD, ID.ID_FACILITIES_CHECK, ID.ID_FACILITIES_DEL, ID.ID_FACILITIES_EDITPOS, ID.ID_FIREEXTINGUISHER_VISIBLE, ID.ID_FIREPLUG_VISIBLE, ID.ID_FIREALARM_VISIBLE, ID.ID_HOME_SCREEN });
            AddBitmapFormRes("nent", new int[] { ID.ID_ADMIN_VERSION, ID.ID_ADMIN_CHECK, ID.ID_ADMIN_PROPERTIES });

			//axCommandBars.Icons.LoadBitmap(resDir + "RibbonControl.png", new int[] { ID.ID_FACILITIES_ADD, ID.ID_FACILITIES_CHECK, ID.ID_FACILITIES_DEL, ID.ID_FACILITIES_EDITPOS, ID.ID_FIREEXTINGUISHER_VISIBLE, ID.ID_FIREPLUG_VISIBLE, ID.ID_FIREALARM_VISIBLE, ID.ID_HOME_SCREEN }, XtremeCommandBars.XTPImageState.xtpImageNormal);
			//axCommandBars.Icons.LoadBitmap(resDir + "nent.png", new int[] { ID.ID_ADMIN_VERSION, ID.ID_ADMIN_CHECK, ID.ID_ADMIN_PROPERTIES }, XtremeCommandBars.XTPImageState.xtpImageNormal);

            //axCommandBars.Icons.LoadBitmap(resDir + "BackstageIcons.png", new int[] { ID.ID_FILE_NEWSOP, ID.ID_FILE_SAVE, ID.ID_FILE_SAVE_AS, ID.ID_FILE_OPEN, ID.ID_FILE_CLOSE, ID.ID_FILE_OPTIONS, ID.ID_APP_EXIT }, XtremeCommandBars.XTPImageState.xtpImageNormal);
            //axCommandBars.Icons.LoadBitmap(resDir + "RibbonRunIcons.png", new int[] { ID.ID_RUN_PLAY, ID.ID_RUN_CANCEL, ID.ID_RUN_COMPLETE }, XtremeCommandBars.XTPImageState.xtpImageNormal);
            //axCommandBars.Icons.LoadBitmap(resDir + "RibbonLargeIcons.png", new int[] { ID.ID_RUN_FRONT, ID.ID_ANNOUNCE_PLAY, ID.ID_RUN_FRONT, ID.ID_ANNOUNCE_PAUSE, ID.ID_ANNOUNCE_STOP, ID.ID_ANNOUNCE_COUNT }, XtremeCommandBars.XTPImageState.xtpImageNormal);

            XtremeCommandBars.ToolTipContext ToolTipContext = null;
            ToolTipContext = axCommandBars.ToolTipContext;
            ToolTipContext.Style = XtremeCommandBars.XTPToolTipStyle.xtpToolTipResource;
            ToolTipContext.ShowTitleAndDescription(true, XtremeCommandBars.XTPToolTipIcon.xtpToolTipIconNone);
            ToolTipContext.SetMargin(2, 2, 2, 2);
            ToolTipContext.MaxTipWidth = 180;
        }

        private void CreateRibbonBar()
        {
            RibbonBar = axCommandBars.AddRibbonBar("The Ribbon");
            RibbonBar.EnableDocking(XtremeCommandBars.XTPToolBarFlags.xtpFlagStretched);

            ControlHome = RibbonBar.AddSystemButton();
            ControlHome.IconId = ID.ID_TAB_HOME;
            ControlHome.Caption = "&홈";
            ControlHome.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonCaption;

            //CreateBackstageView();

            // 소방설비관리 탭
            m_TabFire = RibbonBar.InsertTab(0, "&소방설비관리");
            m_TabFire.Id = ID.ID_TAB_FIREMANAGEMENT;

            XtremeCommandBars.RibbonGroup GroupFire = null;
            XtremeCommandBars.RibbonGroup GroupFireSee = null;
            GroupFire = m_TabFire.Groups.AddGroup("&소방설비관리", ID.ID_GROUP_FIREMANAGEMENT);
            ctrlAdd = GroupFire.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FACILITIES_ADD, "&설비추가", false, false);
            ctrlCheck = GroupFire.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FACILITIES_CHECK, "&설비점검", false, false);
            ctrlEditPos = GroupFire.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FACILITIES_EDITPOS, "&위치수정", false, false);
            ctrlDel = GroupFire.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FACILITIES_DEL, "&설비삭제", false, false);
            GroupFireSee = m_TabFire.Groups.AddGroup("&가시화", ID.ID_GROUP_FIREMANAGEMENT);
            ctrlFireExtinguisher = GroupFireSee.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FIREEXTINGUISHER_VISIBLE, "&소화기", false, false);
            ctrlFireplugVisible = GroupFireSee.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FIREPLUG_VISIBLE, "&소화전", false, false);
            ctrlFireAlarmVisible = GroupFireSee.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FIREALARM_VISIBLE, "&발신기", false, false);
            XtremeCommandBars.RibbonGroup groupScreen = m_TabFire.Groups.AddGroup("&화면", ID.ID_GROUP_FIREMANAGEMENT);
            groupScreen.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_HOME_SCREEN, "&Home", false, false);
            
            //ctrlEditPos.Enabled = false;
            //ctrlDel.Enabled = false;

            ctrlFireExtinguisher.Checked = true;
            ctrlFireplugVisible.Checked = true;
            ctrlFireAlarmVisible.Checked = true;

            // 관리 탭
            m_TabAdmin = RibbonBar.InsertTab(1, "&관리");
            m_TabAdmin.Id = ID.ID_TAB_ADMINISTRATION;

            XtremeCommandBars.RibbonGroup GroupAdmin = null;
            GroupAdmin = m_TabAdmin.Groups.AddGroup("&관리", ID.ID_GROUP_ADMINISTRATION);
            ctrlAdminVersion = GroupAdmin.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_ADMIN_VERSION, "&버전관리", false, false);
            ctrlAdminProperties = GroupAdmin.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_ADMIN_PROPERTIES, "&속성정보 관리", false, false);
            ctrlAdminCheck = GroupAdmin.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_ADMIN_CHECK, "&점검이력 관리", false, false);
            XtremeCommandBars.RibbonGroup groupScreen2 = m_TabAdmin.Groups.AddGroup("&화면", ID.ID_GROUP_FIREMANAGEMENT);
            groupScreen2.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_HOME_SCREEN, "&Home", false, false);

            ctrlAdminVersion.Enabled = false;
            ctrlAdminProperties.Enabled = false;

            RibbonBar.ShowQuickAccess = false;
            RibbonBar.ShowCaptionAlways = false;

            ControlHome.Execute();
        }

        //private void CreateBackstageView()
        //{
        //    XtremeCommandBars.RibbonBar RibbonBar;
        //    RibbonBar = (XtremeCommandBars.RibbonBar)axCommandBars.ActiveMenuBar;

        //    XtremeCommandBars.RibbonBackstageView BackstageView;
        //    BackstageView = (XtremeCommandBars.RibbonBackstageView)axCommandBars.CreateCommandBar("CXTPRibbonBackstageView");

        //    RibbonBar.AddSystemButton().CommandBar = (XtremeCommandBars.CommandBar)BackstageView;

        //    // 파일 탭 메뉴
        //    if (m_pageNew == null)
        //        m_pageNew = new PageBackstageNew();
        //    if (m_pageOpen == null)
        //        m_pageOpen = new PageBackstageOpen();
        //    if (m_pageSave == null)
        //        m_pageSave = new PageBackstageSave();
        //    if (m_pageSetup == null)
        //        m_pageSetup = new PageBackstageSetup();

        //    m_ControlOption = BackstageView.AddTab(ID.ID_FILE_NEW, "NEW", m_pageNew.Handle.ToInt32());
        //    m_ControlOption.DefaultItem = true;
        //    m_ControlOption = BackstageView.AddTab(ID.ID_FILE_OPEN, "Open", m_pageOpen.Handle.ToInt32());
        //    m_ControlOption = BackstageView.AddTab(ID.ID_FILE_SAVE, "저장하기", m_pageSave.Handle.ToInt32());
        //    m_ControlOption = BackstageView.AddTab(ID.ID_FILE_SETUP, "환경설정", m_pageSetup.Handle.ToInt32());

        //    m_ControlOption.Enabled = false;

        //    BackstageView.AddCommand(ID.ID_APP_EXIT, "끝내기");
        //}

        private void DisableEdit()
        {
            ctrlEditPos.Checked = false;
            ctrlDel.Checked = false;
            ctrlEditPos.Enabled = false;
            ctrlDel.Enabled = false;
        }

        public void EnableEdit()
        {
            ctrlEditPos.Enabled = true;
            ctrlDel.Enabled = true;
        }

        private void axCommandBars_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            switch (e.control.Id)
            {
                case ID.ID_FACILITIES_ADD:
                    if (!CheckOtherPopups(m_frmAddEquip))
                        return;

                    DisableEdit();

                    if (m_frmAddEquip.IsDisposed)
                        m_frmAddEquip = new FormAddEquip();
                    m_frmAddEquip.Show();
                    break;

                case ID.ID_FACILITIES_CHECK:
                    if (!CheckOtherPopups(m_frmCheckEquip))
                        return;

                    DisableEdit();

                    if (m_frmCheckEquip.IsDisposed)
                        m_frmCheckEquip = new FormCheckEquip2();
                    m_frmCheckEquip.Show(ViewControl.LeftBar.SelectedEquipment);
                    break;

                case ID.ID_ADMIN_CHECK:
                    if (!CheckOtherPopups(m_frmEquipHistory))
                        return;

                    DisableEdit();

                    if (m_frmEquipHistory.IsDisposed)
                        m_frmEquipHistory = new FormEquipHistory();
                    m_frmEquipHistory.Show(ViewControl.LeftBar.SelectedEquipment);
                    break;

                case ID.ID_FACILITIES_EDITPOS:
                    ctrlEditPos.Checked = !ctrlEditPos.Checked;

                    if (ctrlEditPos.Checked)
                        ctrlDel.Checked = false;
                    break;

                case ID.ID_FACILITIES_DEL:
                    ctrlDel.Checked = !ctrlDel.Checked;

                    if (ctrlDel.Checked)
                        ctrlEditPos.Checked = false;
                    break;

                case ID.ID_HOME_SCREEN:
                    if (DXFControl != null)
                        DXFControl.LoadHomeMatrix(true);
                    break;

                case ID.ID_FIREEXTINGUISHER_VISIBLE:
                    ctrlFireExtinguisher.Checked = !ctrlFireExtinguisher.Checked;
                    m_frmPanel.LeftBar.Rearrange(FireEquipment.EquipmentType.FE, ctrlFireExtinguisher.Checked, ctrlFireplugVisible.Checked, ctrlFireAlarmVisible.Checked);
                    EquipmentLayerOn(FireEquipment.EquipmentType.FE, ctrlFireExtinguisher.Checked);
                    break;

                case ID.ID_FIREPLUG_VISIBLE:
                    ctrlFireplugVisible.Checked = !ctrlFireplugVisible.Checked;
                    m_frmPanel.LeftBar.Rearrange(FireEquipment.EquipmentType.HD, ctrlFireExtinguisher.Checked, ctrlFireplugVisible.Checked, ctrlFireAlarmVisible.Checked);
                    EquipmentLayerOn(FireEquipment.EquipmentType.HD, ctrlFireplugVisible.Checked);
                    break;

                case ID.ID_FIREALARM_VISIBLE:
                    ctrlFireAlarmVisible.Checked = !ctrlFireAlarmVisible.Checked;
                    m_frmPanel.LeftBar.Rearrange(FireEquipment.EquipmentType.FA, ctrlFireExtinguisher.Checked, ctrlFireplugVisible.Checked, ctrlFireAlarmVisible.Checked);
                    EquipmentLayerOn(FireEquipment.EquipmentType.FA, ctrlFireAlarmVisible.Checked);
                    break;

                //case ID.ID_APP_EXIT:
                //    this.Close();
                //    break;
            }
        }

        private bool CheckOtherPopups(Form frmPopup)
        {
            if (m_frmAddEquip != frmPopup)
            {
                if (!m_frmAddEquip.IsDisposed && m_frmAddEquip.IsWorking)
                {
                    m_frmAddEquip.TopMost = false;
                    MessageBox.Show("[설비추가] 창 작업중입니다.\r\n작업중인 창을 먼저 닫아주십시오.");
                    m_frmAddEquip.TopMost = true;
                    return false;
                }
            }

            if (m_frmCheckEquip != frmPopup)
            {
                if (!m_frmCheckEquip.IsDisposed && m_frmCheckEquip.IsWorking)
                {
                    m_frmCheckEquip.TopMost = false;
                    MessageBox.Show("[설비점검] 창 작업중입니다.\r\n작업중인 창을 먼저 닫아주십시오.");
                    m_frmCheckEquip.TopMost = true;
                    return false;
                }
            }

            if (m_frmEquipHistory != frmPopup)
            {
                if (!m_frmEquipHistory.IsDisposed && m_frmEquipHistory.IsWorking)
                {
                    m_frmEquipHistory.TopMost = false;
                    MessageBox.Show("[설비 점검이력] 창 작업중입니다.\r\n작업중인 창을 먼저 닫아주십시오.");
                    m_frmEquipHistory.TopMost = true;
                    return false;
                }
            }

            return true;
        }

        private void axCommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {
        }

        private void axCommandBars_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axCommandBars.GetClientRect(out left, out top, out right, out bottom);
            panelMain.SetBounds(left, top, right - left, bottom - top);
        }

        // DXFViewer의 단위계를 unitTrg으로 변환하기 위한 flag 값을 리턴한다.
        public float GetUnitFlag(DXFViewer.UnitOfLength unitTrg)
        {
            if (DXFControl == null)
                return 1.0f;

            DXFViewer.UnitOfLength unitSrc = DXFControl.UnitOfLength;

            if (unitSrc == DXFViewer.UnitOfLength.INCH)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 25.4f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 2.54f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.0254f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.FEET)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 12.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 304.8f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 30.48f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.3048f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.MILLIMETER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 25.4f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 25.4f / 12f;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 0.1f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.001f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.CENTIMETER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 2.54f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 2.54f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 10;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.01f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.METER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 0.0254f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 0.0254f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 1000.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 100.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 1.0f;
            }

            return 1.0f;
        }

        public void ClearEquipmentLayer()
        {
            m_dicEquipmentLayer.Clear();
        }

        public void SetEquipmentLayer(FireEquipment.EquipmentType type, DXFViewer.Layer layer)
        {
            m_dicEquipmentLayer[type] = layer;
        }

        public DXFViewer.Layer GetEquipmentLayer(FireEquipment.EquipmentType type)
        {
            if (m_dicEquipmentLayer.ContainsKey(type))
                return m_dicEquipmentLayer[type];

            return null;
        }

        public void EquipmentLayerOn(FireEquipment.EquipmentType type, bool turnOn, bool refresh = true)
        {
            if (m_dicEquipmentLayer.ContainsKey(type))
            {
                DXFViewer.Layer layer = m_dicEquipmentLayer[type];
                layer.Hidden = !turnOn;

                if (refresh)
                    ViewControl.Refresh();
            }
        }

        public void SetEquipmentLayerOnOff()
        {

            EquipmentLayerOn(FireEquipment.EquipmentType.FE, ctrlFireExtinguisher.Checked, false);
            EquipmentLayerOn(FireEquipment.EquipmentType.HD, ctrlFireplugVisible.Checked, false);
            EquipmentLayerOn(FireEquipment.EquipmentType.FA, ctrlFireAlarmVisible.Checked, false);

            ViewControl.LeftBar.Rearrange(ctrlFireExtinguisher.Checked, ctrlFireplugVisible.Checked, ctrlFireAlarmVisible.Checked);
        }

        public void ChangeTab(int nTabID)
        {
            if (nTabID == ID.ID_TAB_FIREMANAGEMENT)
            {
                m_frmPanel.SetRFIDOwner();
                m_TabFire.Selected = true;
                ControlHome.Execute();
            }
            else if (nTabID == ID.ID_TAB_ADMINISTRATION)
            {
                m_frmPanel.SetRFIDOwner();
                m_TabAdmin.Selected = true;
                ControlHome.Execute();
            }
        }

        public void DeleteEquipment(FireEquipment equip)
        {
            if (equip == null)
                return;

            m_frmPanel.LeftBar.DeleteEquipment(equip);
            IOManager.DeleteEquipment(equip);
            DXFManager.DeleteEquipment(equip);
        }

        public bool NeedScreenInput()
        {
            return m_frmAddEquip.IsWorking;
        }

        public void ScreenInput(double x, double y)
        {
            if (m_frmAddEquip.IsWorking)
                m_frmAddEquip.ScreenInput(x, y);
        }

        public string StatusText
        {
            set { toolStripStatusLabel1.Text = value; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public bool IsPCMode
        {
            get { return m_isPCMode; }
        }

        public FireManagement.IOManager IOManager
        {
            get { return m_ioMgr; }
        }

        public DXFViewer.DXFControl DXFControl
        {
            get { return m_frmPanel.DXFControl; }
        }

        // 길이 단위 변환을 위한 Flag
        public float UnitFlag
        {
            get { return GetUnitFlag(DXFViewer.UnitOfLength.METER); }
        }

        public FireManagement.DXFManager DXFManager
        {
            get { return m_dxfManager; }
        }

        public ArrayList CurrentEquipments
        {
            get { return m_dxfManager.Equipments; }
        }

        public Zone CurrentZone
        {
            get { return m_zoneCurrent; }
            set { m_zoneCurrent = value; }
        }

        public FormPanel ViewControl
        {
            get { return m_frmPanel; }
        }

        public Ubists.RFIDReader RFIDReader
        {
            get { return m_rfidReader; }
        }

        public FormCheckEquip2 EquipmentChecker
        {
            get { return m_frmCheckEquip; }
        }

        public FormEquipHistory EquipmentHistoryViewer
        {
            get { return m_frmEquipHistory; }
        }

        public DataFileManager FileManager
        {
            get { return m_fileMgr; }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
        }

        public PageBackstageNew PageNew
        {
            get { return m_pageNew; }
        }

        public PageBackstageOpen PageOpen
        {
            get { return m_pageOpen; }
        }

        public bool IsEditingMode
        {
            get { return ctrlEditPos.Checked; }
        }

        public bool IsDeletingMode
        {
            get { return ctrlDel.Checked; }
        }

        public bool TagInputMode
        {
            get { return m_tagInputMode; }
        }

        public int BluetoothComport
        {
            get { return m_nBluetoothComport; }
        }
    }
}
