using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using XtremeDockingPane;
using System.Net;
using SOPMonitoringSystem;

namespace SOPDisasterSystem
{
	public partial class FormMain : Form
	{
		private FormLeftSpace m_frmSpace = null;
		private FormBottomLog m_frmLog = new FormBottomLog();
		private FormRightSummary m_frmSummary = new FormRightSummary();
		private FormRightSituation m_frmSituation = null;
		
		
        private Form[] m_arrDocking = new Form[8];
		private SOPMonitoringSystem.FormMain m_frmMain = null;

		private Dictionary<string, string> m_dicInsideCMO = new Dictionary<string, string>();
		//private string m_strOutsideCMO = "";
		//private ArrayList m_arrTempResult = null;

		ArrayList m_arrBuildingInfo = new ArrayList();
		//ArrayList m_arrGroup = new ArrayList();
		//ArrayList m_arrBuilding = new ArrayList();

        private int m_nLayout = -1;
        public int NumLayout
        {
            get { return m_nLayout; }
            set { m_nLayout = value; }
        }
        // 1에서 5층까지
        //private int m_nMinFloorIndex = 0, m_nMaxFloorIndex = 4;

		protected string m_strSkinFolder;



        //////////////////////////////////////////////////////////////////////////
        private SOPMonitoringSystem.DockingLeftScenario m_dockScenario = null;
        private SOPMonitoringSystem.DockingLeftPropertiesLevel m_dockPropertiesLevel = null;
        private SOPMonitoringSystem.DockingLeftProperties m_dockProperties = null;
        private SOPMonitoringSystem.DockingRightProgress m_dockProgress = null;
        private SOPMonitoringSystem.DockingRightPersonnel m_dockPersonnel = null;
        private SOPMonitoringSystem.DockingReceiveMessage m_dockMessage = null;

        private Pane m_paneProperties = null;

        private FormLayout m_Layout = null;
        public SOPDisasterSystem.FormLayout LayoutForm
        {
            get { return m_Layout; }
            set { m_Layout = value; }
        }

		public FormMain(SOPMonitoringSystem.FormMain main)
		{
			InitializeComponent();

			m_frmMain = main;

			string strSkinFolder = StylesPath();
			Skin_Load(strSkinFolder);

			CreatePane();
			tsViewCtrl_ImageLoad();

			FormLayoutLoad();

			ModelManager.Instance.TargetForm = m_Layout;
			ModelManager.Instance.Read3DModel();
			//ReadCMO();
			

			mCmbFloor.SelectedIndexChanged += new EventHandler(SelectFloorChange);
		}
		
		public void Skin_Load(string strSkinFolder)
		{
            axSkinFramework1.LoadSkin(strSkinFolder + "Office2010.cjstyles", "Normalblue.ini");
			axSkinFramework1.ApplyWindow(this.Handle.ToInt32());
			this.BackColor = axSkinFramework1.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
		}

		public string StylesPath()
		{
			string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			System.IO.Directory.Exists(strExePath + "\\Styles\\");

			return strExePath + "\\Styles\\";
		}

		//private void ReadCMO()
		//{
		//    SOPMonitoringSystem.WebDBManager dbMgr = m_frmMain.DBManager;

		//    string strSQL = "Select Name, URL, AccessedTime from BluePrint where SiteID = 1";
		//    m_arrTempResult = dbMgr.GetResultData(strSQL, 0);

		//    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ReadCMO));
		//    t.Start(this);
		//}

		//static void ReadCMOHistory(ref Dictionary<string, string> dicCMOHistory)
		//{
		//    string tempPath = System.IO.Path.GetTempPath();

		//    System.IO.StreamReader reader = null;

		//    try
		//    {
		//        reader = new System.IO.StreamReader(tempPath + "Outside.log", Encoding.Default);
		//        string strOutsideTime = reader.ReadLine();
		//        reader.Close();

		//        dicCMOHistory["Outside"] = strOutsideTime;
		//    }
		//    catch (System.IO.FileNotFoundException)
		//    {
		//    }

		//    try
		//    {
		//        reader = new System.IO.StreamReader(tempPath + "Inside.log", Encoding.Default);
		//        string strInsideTime = reader.ReadLine();
		//        reader.Close();

		//        dicCMOHistory["Inside"] = strInsideTime;
		//    }
		//    catch (System.IO.FileNotFoundException)
		//    {
		//    }
		//}

		//static private void DownloadCMOFile(Dictionary<string, string> dicCMOHistory, string strTag, string strShortTime, WebClient web, string strURL, Dictionary<string, string> dicCMO, ref string strPath)
		//{
		//    string tempPath = System.IO.Path.GetTempPath();
		//    string localPath = tempPath + strTag + ".zip";

		//    if (dicCMOHistory.ContainsKey(strTag) && dicCMOHistory[strTag] == strShortTime)
		//    {
		//        if (System.IO.File.Exists(localPath))
		//        {
		//            if (dicCMO == null)
		//                strPath = localPath;
		//            else
		//                dicCMO[strTag] = localPath;

		//            return;
		//        }
		//    }

		//    web.DownloadFile(strURL, localPath);

		//    if (dicCMO == null)
		//        strPath = localPath;
		//    else
		//    {
		//        strPath = localPath;
		//        dicCMO[strTag] = localPath;
		//    }
				

		//    System.IO.StreamWriter sw = new System.IO.StreamWriter(tempPath + strTag + ".log", false, Encoding.Default);
		//    sw.WriteLine(strShortTime);
		//    sw.Close();
		//}

		//static private void ReadCMO(object param)
		//{
		//    FormMain frm = (FormMain)param;
		//    SOPMonitoringSystem.WebDBManager dbMgr = frm.m_frmMain.DBManager;

		//    //string strSQL = "Select Name, URL, AccessedTime from BluePrint where SiteID = 1";
		//    ArrayList arrResult = frm.m_arrTempResult;//dbMgr.GetResultData(strSQL, 0);

		//    if (arrResult == null || arrResult.Count == 0)
		//    {
		//        MessageBox.Show("3D Model 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요", "File Download Error");
		//        Application.Exit();
		//        return;
		//    }

		//    WebClient web = new WebClient();
		//    string strNULL = null;

		//    Dictionary<string, string> dicCMOHistory = new Dictionary<string, string>();
		//    ReadCMOHistory(ref dicCMOHistory);            

		//    DateTime dtDefault = new DateTime();

		//    for (int i = 0; i < arrResult.Count - 2; i += 3)
		//    {
		//        string strName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i].ToString(), "");
		//        string strURL = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
		//        DateTime dtAccessed = SOPMonitoringSystem.WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);

		//        string strShortTime = dtAccessed.ToShortDateString() + " " + dtAccessed.ToShortTimeString();

		//        if (!strURL.Contains("http:"))
		//            continue;

		//        if (strName == "All")
		//        {
		//            //strURL = "http://unes.iptime.org:9808/SOP/Download_Outside2.jsp";
		//            DownloadCMOFile(dicCMOHistory, "Outside", strShortTime, web, strURL, null, ref frm.m_strOutsideCMO);
		//        }
		//        else if (strName == "Inside")
		//        {
		//            //strURL = "http://unes.iptime.org:9808/SOP/Download_Inside2.jsp";
		//            DownloadCMOFile(dicCMOHistory, "Inside", strShortTime, web, strURL, frm.m_dicInsideCMO, ref strNULL);
		//        }
		//    }

		//    if (frm.m_strOutsideCMO.Length == 0 || frm.m_dicInsideCMO.Count == 0)
		//    {
		//        MessageBox.Show("3D 모델 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요", "File Download Error");
		//        Application.Exit();
		//        return;
		//    }

		//    if (frm != null && frm.m_Layout != null)
		//        frm.m_Layout.SetFilePath(System.IO.Path.GetTempPath(), frm.m_strOutsideCMO, strNULL, frm.m_dicInsideCMO);
		//}

        bool bProcessBound = false;
        private void SetVirtoolPaneBound(int left, int top, int right, int bottom)
        {
            if (bProcessBound == true)
                return;
            bProcessBound = true;
            if (left != right && top != bottom)
                panelVirtool.SetBounds(left, top, right - left, bottom - top);

            bProcessBound = false;
        }
		private void FormMain2_Resize(object sender, EventArgs e)
		{
			int left, top, right, bottom;

			axDockingPane.GetClientRect(out left, out top, out right, out bottom);

            SetVirtoolPaneBound(left, top, right, bottom);
		}

		private void tabCtrlMonitoring_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(tabDisaster.Controls.Count > 0)
				tabDisaster.Controls.Remove(panelMain);
            //if (tabEquipment.Controls.Count > 0)
            //    tabEquipment.Controls.Remove(panelMain);
            //if (tabSensor.Controls.Count > 0)
            //    tabSensor.Controls.Remove(panelMain);
            //if (tabCCTV.Controls.Count > 0)
            //    tabCCTV.Controls.Remove(panelMain);

			switch (tabCtrlMonitoring.SelectedIndex)
			{
				case 0:
					tabDisaster.Controls.Add(panelMain);
					break;
                //case 1:
                //    tabEquipment.Controls.Add(panelMain);
                //    break;
                //case 2:
                //    tabSensor.Controls.Add(panelMain);
                //    break;
                //case 3:
                //    tabCCTV.Controls.Add(panelMain);
                //    break;
			}
		}

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                this.axSkinFramework1.ApplyWindow(this.Handle.ToInt32());
            }
        }

        public void ApplyWindow(int hWnd)
        {
            axSkinFramework1.ApplyWindow(hWnd);            
        }

		public void CreatePane()
		{
			// Bottom
			//Pane paneLog = axDockingPane.CreatePane(1, 300, 170, DockingDirection.DockBottomOf, null);
			//paneLog.Title = "SOP Log";
			//paneLog.Options = PaneOptions.PaneNoCloseable;

            Pane paneMessage = axDockingPane.CreatePane(7, 290, 200, DockingDirection.DockBottomOf, null);
            paneMessage.Title = "받은 메시지 리스트";
            paneMessage.Options = PaneOptions.PaneNoCloseable;

            int nLeftPaneWidth = 350;

            // Left
            Pane paneScenario = axDockingPane.CreatePane(1, nLeftPaneWidth, 250, DockingDirection.DockLeftOf, null);
            paneScenario.Title = "운용 중 시나리오";
            paneScenario.Options = PaneOptions.PaneNoCloseable;


            m_paneProperties = axDockingPane.CreatePane(3, nLeftPaneWidth, 170, DockingDirection.DockTopOf, paneScenario);
            m_paneProperties.Title = "컴포넌트 속성";
            m_paneProperties.Options = PaneOptions.PaneNoCloseable;

            Pane panePropertiesLevel = axDockingPane.CreatePane(2, nLeftPaneWidth, 170, DockingDirection.DockTopOf, m_paneProperties);
            panePropertiesLevel.Title = "위기관리 활동단계 속성";
            panePropertiesLevel.Options = PaneOptions.PaneNoCloseable;

            m_paneProperties.AttachTo(paneScenario);
            panePropertiesLevel.AttachTo(m_paneProperties);
            //paneScenario.AttachTo(panePropertiesLevel);
            paneScenario.Select();



           
            
			//Right
            Pane paneSpace = axDockingPane.CreatePane(0, 280, 190, DockingDirection.DockRightOf, null);
            paneSpace.Title = "공간구조";
            paneSpace.Options = PaneOptions.PaneNoCloseable;

            Pane panePersonnel = axDockingPane.CreatePane(6, 290, 200, DockingDirection.DockTopOf, paneSpace);
            panePersonnel.Title = "SOP 요원 현황";
            panePersonnel.Options = PaneOptions.PaneNoCloseable;

            Pane paneProgress = axDockingPane.CreatePane(5, 290, 200, DockingDirection.DockTopOf, panePersonnel);
            paneProgress.Title = "SOP 진행 현황";
            paneProgress.Options = PaneOptions.PaneNoCloseable;

            Pane paneSituation = axDockingPane.CreatePane(4, 290, 300, DockingDirection.DockTopOf, paneSpace);
            paneSituation.Title = "상황";
            paneSituation.Options = PaneOptions.PaneNoCloseable;

            panePersonnel.AttachTo(paneSpace);
            paneProgress.AttachTo(panePersonnel);           
            paneSpace.Select();


            m_arrDocking[0] = new FormLeftSpace(this);
            m_frmSpace = (FormLeftSpace)m_arrDocking[0];

			//arrDocking[1] = new FormBottomLog();
			//m_frmLog = (FormBottomLog)arrDocking[1];

            m_arrDocking[1] = new SOPMonitoringSystem.DockingLeftScenario();
            m_dockScenario = (SOPMonitoringSystem.DockingLeftScenario)m_arrDocking[1];

            m_arrDocking[2] = new SOPMonitoringSystem.DockingLeftPropertiesLevel();
            m_dockPropertiesLevel = (SOPMonitoringSystem.DockingLeftPropertiesLevel)m_arrDocking[2];

            m_arrDocking[3] = new SOPMonitoringSystem.DockingLeftProperties();
            m_dockProperties = (SOPMonitoringSystem.DockingLeftProperties)m_arrDocking[3];

            m_arrDocking[4] = new FormRightSituation(this);
            m_frmSituation = (FormRightSituation)m_arrDocking[4];

            m_arrDocking[5] = new SOPMonitoringSystem.DockingRightProgress();
            m_dockProgress = (SOPMonitoringSystem.DockingRightProgress)m_arrDocking[5];

            m_arrDocking[6] = new SOPMonitoringSystem.DockingRightPersonnel();
            m_dockPersonnel = (SOPMonitoringSystem.DockingRightPersonnel)m_arrDocking[6];

            m_arrDocking[7] = new SOPMonitoringSystem.DockingReceiveMessage();
            m_dockMessage = (SOPMonitoringSystem.DockingReceiveMessage)m_arrDocking[7];

            m_frmMain.GetPageHome().DockScenario = m_dockScenario;
            m_frmMain.GetPageHome().DockPropertiesLevel = m_dockPropertiesLevel;
            m_frmMain.GetPageHome().DockProperties = m_dockProperties;
            m_frmMain.GetPageHome().DockProgress = m_dockProgress;
            m_frmMain.GetPageHome().DockPersonnel = m_dockPersonnel;
            m_frmMain.GetPageHome().PaneProperties = m_paneProperties;
            m_frmMain.GetPageHome().DockingMessage = m_dockMessage;
		}

		private void axDockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
		{
			int nIndex = e.item.Id;

			if (nIndex == 0)
				e.item.Handle = m_arrDocking[0].Handle.ToInt32();
			else if (nIndex == 1)
				e.item.Handle = m_arrDocking[1].Handle.ToInt32();
			else if (nIndex == 2)
				e.item.Handle = m_arrDocking[2].Handle.ToInt32();
            else if (nIndex == 3)
                e.item.Handle = m_arrDocking[3].Handle.ToInt32();
            else if (nIndex == 4)
                e.item.Handle = m_arrDocking[4].Handle.ToInt32();
            else if (nIndex == 5)
                e.item.Handle = m_arrDocking[5].Handle.ToInt32();
            else if (nIndex == 6)
                e.item.Handle = m_arrDocking[6].Handle.ToInt32();
            else if (nIndex == 7)
                e.item.Handle = m_arrDocking[7].Handle.ToInt32();
		}

		private void axDockingPane_ResizeEvent(object sender, EventArgs e)
		{
			int left, top, right, bottom;

			axDockingPane.GetClientRect(out left, out top, out right, out bottom);
            SetVirtoolPaneBound(left, top, right, bottom);
		}

		private void tsViewCtrl_ImageLoad()
		{
			Bitmap bmpViewCtrl = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.toolbar_ViewControl);
			ImageList ListViewCtrl = new ImageList();
			ListViewCtrl.ImageSize = new Size(24, 24);
			ListViewCtrl.Images.AddStrip(bmpViewCtrl);

			tsViewCtrl.ImageList = ListViewCtrl;

			tsbtnHomeView.ImageIndex = 0;
			tsbtnFullScreen.ImageIndex = 1;
			tsbtnZoomin.ImageIndex = 2;
			tsbtnZoomout.ImageIndex = 3;
			tsbtnMove.ImageIndex = 4;
			tsbtnPick.ImageIndex = 5;
			tsbtnOrbit.ImageIndex = 6;
			tsbtnLayout1.ImageIndex = 7;
			tsbtnLayout2.ImageIndex = 8;
			tsbtnLayout3.ImageIndex = 9;
			tsbtnLayout4.ImageIndex = 10;
		}

		private void FormLayoutLoad()
		{
            try
            {
                m_Layout = new FormLayout(this);
            }
            catch (System.Exception)
            {
                MessageBox.Show("3D 환경을 초기화 하지 못하였습니다.\n모니터링을 종료합니다.");
                Application.Exit();
                return;
            }
            
			m_Layout.TopLevel = false;
			m_Layout.Parent = this;
			//splitContainer.Panel2.Controls.Add(m_Layout);
            m_Layout.Controls.Add(this.panel1);
            panelVirtool.Controls.Add(m_Layout);
			m_Layout.Dock = DockStyle.Fill;
			m_Layout.Show();
		}

		public SOPMonitoringSystem.FormMain GetMain()
		{
			return m_frmMain;
		}

		public FormLeftSpace GetSpace()
		{
			return m_frmSpace;
		}

		public FormBottomLog GetLog()
		{ 
			return m_frmLog;
		}
		
		public FormRightSummary GetSummary()
		{
			return m_frmSummary;
		}
		
		public FormRightSituation GetSituation()
		{
			return m_frmSituation;
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			GetSituation().GridViewClearSelection();
			this.WindowState = FormWindowState.Maximized;
			this.Text += " " + m_frmMain.GetAppVersion();

            m_Layout.LoadComplete = true;
		}

		private void tsbtnLayout1_Click(object sender, EventArgs e)
		{
			LayoutView(1);
		}

		private void tsbtnLayout3_Click(object sender, EventArgs e)
		{
			LayoutView(3);
		}

		public void LayoutView(int nLayout)
		{
            if (m_nLayout == nLayout)
                return;
            if (m_Layout == null)
                return;
			HideLayout();
			switch (nLayout)
			{
				case 1:
					m_Layout.SetLayoutMode(1);
					m_Layout.Layout1();                    
					break;
				case 3:
					m_Layout.SetLayoutMode(3);
					m_Layout.Layout3();
					break;
			}

            m_nLayout = nLayout;
		 }

		private void HideLayout()
		{
		}

		public Panel GetPaneVirtool()
		{
            return panelVirtool;
		}

        public Panel GetToolbar()
        {
            return panel1;
        }

		public void SetCurrentBuilding(SOPMonitoringSystem.Data_Building building)
        {
			if (m_Layout != null)
			{
				m_Layout.SetCurrentBuilding(building);
			}               
        }
    
		public ArrayList GetBuildingList()
		{
			return m_arrBuildingInfo;
		}
		
		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{            
		}

		private void tsbtnZoomin_MouseDown(object sender, MouseEventArgs e)
		{
            m_Layout.ZoomIn();
		}

		private void tsbtnZoomout_MouseDown(object sender, MouseEventArgs e)
		{
            m_Layout.ZoomOut();
		}
		
		private void tsbtnHomeView_Click(object sender, EventArgs e)
		{
            m_Layout.btnHome_Click_1(sender, e);			
		}

		public void SetFloorStatus(bool enable, ArrayList arFloor)
		{
			mCmbFloor.DropDownStyle = ComboBoxStyle.DropDownList;
			mCmbFloor.Items.Clear();
			if (arFloor != null)
			{
				for (int i = 0; i < arFloor.Count; i++)
				{
					mCmbFloor.BeginUpdate();
					Zone zone =  (Zone)arFloor[i];
					Floor floor = zone.Floor;
					mCmbFloor.Items.Add(floor);
					mCmbFloor.EndUpdate();
				}
			}			
		}

		private void SelectFloorChange(object sender, EventArgs e)
		{
			object item = mCmbFloor.SelectedItem;
			if (item != null)
			{
				Floor floor = (Floor)item;
				m_Layout.ShowIndoor(floor.FloorIndex, floor);
			}			
		}
		
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            this.Visible = false;
            if (m_frmMain != null && m_frmMain.Visible == true)
            {
                m_frmMain.Visible = false;
                m_frmMain.Invoke((MethodInvoker)delegate
                {
                    if (SOPMonitoringSystem.FormMain.Instance.MainFrame == SOPMonitoringSystem.FormMain.Instance)
                    {
                        m_frmMain.Close();
                        m_frmMain.Dispose();
                    }
                    else
                        FormFrame.Instance.Close();
                });
            }
            m_frmSpace.Close();
            m_dockScenario.Close();
            m_dockPropertiesLevel.Close();
            m_dockProperties.Close();

            m_frmSituation.Close();
            m_dockProgress.Close();
            m_dockPersonnel.Close();
            m_dockMessage.Close();

            m_frmSpace = null;
            m_dockScenario = null;
            m_dockPropertiesLevel = null;
            m_dockProperties = null;

            m_frmSituation = null;
            m_dockProgress = null;
            m_dockPersonnel = null;
            m_dockMessage = null;

            axDockingPane.DestroyAll();

            
        }

        
	}
}
