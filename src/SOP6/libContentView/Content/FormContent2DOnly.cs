using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.AccessControl;
using UnE.Geometry;
using UnE.SOP;
using UnE.SOP.Workstate;
using UnE.Spatial;
using UnE.Sensor;
using UnE.View.Content;
using DBUtility2;
using SDMS;


namespace UnE.View.Content
{

    /// <summary>
    /// 실외, 실내 모두 2D인 ContentForm
    /// </summary>
	public partial class FormContent2DOnly : Form, IDisasterContainer, IFormContent
	{
        public void HidePoll(int nPollID)
        {

        }
        public void ShowEmPoll(int nPollID)
        {

        }
        public void ShowPollutionView(int windDirection, int windSpeed)
        {

        }
		// 1(Outside), 2(Both), 3(Inside)
		private int m_nLayout = 1;

		public int NumLayout
		{
			get { return m_nLayout; }
			set { SetLayoutMode(value); }
		}

        private global::Core.LayerManager m_layerOutside = null;

		public ILayerManager Layers
		{
			get { return m_layerOutside; }
		}

        private ImageViewCtrl mView2 = null;
        private ImageViewCtrl mView1 = null;

		private string m_strZipFileFolderPath = "";
		private string m_strOutsideDAE = "";
		private string m_strInsideDAE = "";
		private Building m_buildingCurrent = null;
		private Dictionary<string, string> m_dicInsideDAE = null;
		private string m_strOutDaeName = "";

		private ArrayList mViewList = new ArrayList();
        private IBaseView mCurrent = null;
		private string szIconPath = "";
		private string szMediaPath = "";

		private bool bExtractInside = false;
		private bool m_bLoadInsideMode = false;

		public bool LoadInside
		{
			get { return m_bLoadInsideMode; }
			set { m_bLoadInsideMode = value; }
		}

		private string szInsideFullPath = null;

		// mView2만 사용
		private Zone m_currentIndoorZone = null;

		private bool bLoadComplete = false;

		public bool LoadComplete
		{
			get { return bLoadComplete; }
			set { bLoadComplete = value; }
		}

		private float m_nCurrentFloor = 0.0f;

		private string szPrevFileName = "";

		private float nCurrentFloor = -999.0f;
        //private ToolStripContainer mMainToolStripContainer;
        //private ContextMenuStrip contextMenuStrip1;
        //private System.ComponentModel.IContainer components;
        //private ToolStripMenuItem toolStripMenuItem1;
        //private ToolStripMenuItem toolStripMenuItem8;
        //private ToolStripSeparator toolStripSeparator1;
        //private ToolStripMenuItem toolStripMenuItem9;
       // private ToolStripMenuItem toolStripMenuItem10;
       // private ToolStripSeparator toolStripSeparator2;
       // private ToolStripMenuItem toolStripMenuItem11;
       // private ToolStripMenuItem toolStripMenuItem12;
        //private ContextMenuStrip contextMenuStripBuilding;
        //private ContextMenuStrip contextMenuStripManualReport;
        //public ToolStripMenuItem menuAddDisasterPos;
        //public ToolStripMenuItem menuIndoor;
        //private ToolStripSeparator toolStripSeparator3;
        //public ToolStripMenuItem menuManualCCTV;
        //private ToolStripSeparator toolStripSeparator4;
        //public ToolStripMenuItem menuManualReport;

		private bool bInit = false;

        public void AddPythonFunction()
        {
            ScriptProxy.Instance.UserObject.SDMSEarthquakeEvent = new Action<int, float, string, bool>(EarthquakeEvent);
            ScriptProxy.Instance.UserObject.SDMSEarthquakeEventIsFinished = new Func<bool>(EarthquakeEventIsFinished);

            ScriptProxy.Instance.UserObject.SDMSShowBuildingCollapsed = new Action<string,string>(ShowBuildingCollapse);
            ScriptProxy.Instance.UserObject.SDMSCloseBuildingCollapsed = new Action<string>(CloseBuilingCollapse);
        }

        private string m_szToolKey = @"SDMS\Unity\Toolstrip";
        private string m_szPosSubKeyName = "MainToolStripPos";
        private string m_szToolStripName = "ToolboxStrip";
        public void AddMainToolStrip(ToolStrip strip, ViewType vType)
        {
            if (strip != null)
            {
                // read toolstrip position
                int nPos = ReadToolStripConfig();

                // Set StripName for using Key
                strip.Name = m_szToolStripName;

                // Add StripMenu
                SetToolStripMenu(strip, nPos); 
            }

            System.Threading.Thread t = new System.Threading.Thread(ReadyToLoad);
            t.Start();
        }

        private void ReadyToLoad()
        {

            Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
            formInvoke.Invoke((MethodInvoker)delegate
            {

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.OnReadyDataLoad();
                //FormMain.Instance.OnReadyDataLoad();                
            });
        } 

        public void RemoveMainToolStip(ToolStrip strip)
        {
            mMainToolStripContainer.RightToolStripPanel.Controls.Remove(strip);
            mMainToolStripContainer.LeftToolStripPanel.Controls.Remove(strip);
            mMainToolStripContainer.BottomToolStripPanel.Controls.Remove(strip);
            mMainToolStripContainer.TopToolStripPanel.Controls.Remove(strip);
        }

        private void SetToolStripMenu(ToolStrip strip, int nPos)
        {
            if (nPos == 1)
                mMainToolStripContainer.RightToolStripPanel.Controls.Add(strip);
            else if (nPos == 2)
                mMainToolStripContainer.LeftToolStripPanel.Controls.Add(strip);
            else if (nPos == 3)
                mMainToolStripContainer.BottomToolStripPanel.Controls.Add(strip);
            else
                mMainToolStripContainer.TopToolStripPanel.Controls.Add(strip);
        }

        private int ReadToolStripConfig()
        {
            int nResult = 0;
            try
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(m_szToolKey);
                if (rkey == null)
                {
                    return 0;
                }
                else
                {
                    nResult = (int)rkey.GetValue(m_szPosSubKeyName, 0);
                }
                if (rkey != null)
                    rkey.Close();
            }
            catch (System.Exception)
            {
            }
            return nResult;
        }

        private int GetToolStringPos()
        {
            if (mMainToolStripContainer.TopToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 0;
            }
            else if (mMainToolStripContainer.RightToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 1;
            }
            else if (mMainToolStripContainer.LeftToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 2;
            }
            else if (mMainToolStripContainer.BottomToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 3;
            }
            return 0;
        }

        private void WriteToolStripConfig(int nPos)
        {
            try
            {
                string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Deny));

                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(m_szToolKey, true);
                if (rkey == null)
                {
                    try
                    {
                        rkey = Registry.CurrentUser.CreateSubKey(m_szToolKey, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (rkey != null)
                {
                    rkey.SetValue(m_szPosSubKeyName, nPos);
                    rkey.Close();
                }
            }
            catch (System.Exception)
            {
            }
        }

        private SortedList<string, ToolStripMenuItem> m_MenuList = new SortedList<string, ToolStripMenuItem>();
        public ToolStripMenuItem GetMenu(string szName)
        {
            if(szName == "ManualReport")
            {
                return menuManualReport;
            }
            else if (szName == "ManualCCTV")
            {
                return menuManualCCTV;
            }
            else if (szName == "Indoor")
            {
                return menuIndoor;
            }
            return null;
        }

        public void SetMenu(string szName, ToolStripMenuItem menu)
        {
            if(m_MenuList.ContainsKey(szName))
            {
                m_MenuList.Remove(szName);
            }
            m_MenuList.Add(szName, menu);
        }

        public new bool IsDisposed
        {
            get { return base.IsDisposed; }
        }

		private bool m_bEditMode = false;

		public bool EditMode
		{
			get { return m_bEditMode; }
			set
			{
				m_bEditMode = value;
                ((ImageViewCtrl)mView1).EditMode = value;
                //((ImageViewCtrl)mView2).EditMode = value;
			}
		}

        public bool BlinkMode
        {
            set {}
        }

        private SplitContainer m_LayerContainer = null;

        public MouseWorkMode CurrentMouseWorkMode
		{
            get 
            {
                return ((ImageViewCtrl)mView1).CurrentMouseWorkMode; 
            }
			set
			{
                ((ImageViewCtrl)mView1).CurrentMouseWorkMode = value;
                //((ImageViewCtrl)mView2).CurrentMouseWorkMode = value;
			}
		}



        private global::Core.ZoneVolumeManager mVolmumeManagerIn = null;

        public global::Core.ZoneVolumeManager IndoorVolmumeManager
		{
			get { return mVolmumeManagerIn; }
			set { mVolmumeManagerIn = value; }
		}

        public ISensorTooltipOwner IndoorView
        {
            get { return null; }
        }

        public ISensorTooltipOwner OutdoorView
        {
            get { return mView1; }
        }

		private bool m_bFirstBothSide = true;
        		

        private bool m_bLODText = true;

        private string m_szFileName = "";

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			int i = 0;
			i++;
		}

		public override void Refresh()
		{
			RedrawWindow();
		}

        private IBaseViewOwner m_BaseViewOwner = null;
        public IBaseViewOwner BaseViewOwner
        {
            get { return m_BaseViewOwner; }
        }

		public FormContent2DOnly(IBaseViewOwner owner)
		{ 
            m_BaseViewOwner = owner;
            UnE.View.Content.ViewUtils.RegisterContentView(this);
            UnE.View.Content.IFormContentOwner owner2 = UnE.View.Content.ViewUtils.GetContentViewOwner();
			szMediaPath = owner2.ResourcePath + "Media\\";
			szIconPath = szMediaPath + "icons\\화재.ico";

			InitializeComponent();

            Create2DView();

            //mView2.Visible = false;
            mView1.Dock = DockStyle.Fill;
            mView1.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			MouseWheel += new MouseEventHandler(OnMouseWheel);

            CurrentMouseWorkMode = MouseWorkMode.ORBIT;

            mMainToolStripContainer.TopToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainToolStripContainer.LeftToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainToolStripContainer.RightToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainToolStripContainer.BottomToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);

            AddPythonFunction();

            //mView1.CreateCustomView();
		}

		private void Create2DView()
		{
			m_LayerContainer = new SplitContainer();
			m_LayerContainer.Dock = DockStyle.Fill;
			m_LayerContainer.Visible = false;

			Controls.Add(m_LayerContainer);

            //mView1 = new BaseViewEx2(this);// new Core.BaseView();
            //mView1.BackColor = System.Drawing.Color.Transparent;
            //mView1.Dock = System.Windows.Forms.DockStyle.Fill;
            //mView1.Location = new System.Drawing.Point(0, 0);
            //mView1.Name = "m3DView1";

            //mView1.MinimumSize = new System.Drawing.Size(640, 480);
            //mView1.Size = new System.Drawing.Size(1900, 1040);
            //mView1.TabIndex = 0;
            //mView1.Click += new System.EventHandler(this.View1Click);

            mView1 = new ImageViewCtrl(this, m_BaseViewOwner);
            mView1.Dock = System.Windows.Forms.DockStyle.Fill;
            mView1.Location = new System.Drawing.Point(0, 0);
            mView1.Size = new System.Drawing.Size(1920, 1080);
            mView1.TabIndex = 0;
            mView1.Click += new System.EventHandler(this.View1Click);
            mView1.IsIndoor = false;
            //mView1.SetImage(@"C:\UNE\bin\common12\DXF\main.png");
            mView1.InitImage(UnE.SOP.ProxySOP.Instance.SiteID.ToString());
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)   
                mView1.InitImage(UnE.SOP.ProxySOP.Instance.SiteID + "-1"); // 보운캠퍼스

            //mView1.SetImage(Application.StartupPath + @"\DXF\main.png");

            //mView2 = new ImageViewCtrl(this, m_BaseViewOwner);
            //mView2.Dock = System.Windows.Forms.DockStyle.Fill;
            //mView2.Location = new System.Drawing.Point(0, 0);
            //mView2.Size = new System.Drawing.Size(1920, 1080);
            //mView2.TabIndex = 0;
            ////mView2.InitImage();
            //mView2.IsIndoor = true;
            //mView2.Click += new System.EventHandler(this.View2Click);
            
            mMainToolStripContainer.ContentPanel.Controls.Add(mView1);
            //mMainToolStripContainer.ContentPanel.Controls.Add(mView2);

            m_layerOutside = new global::Core.LayerManager((IBaseView)mView1);

            m_layerOutside.AddLayer(ID.ID_LAYER_DETECTOR, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_COOLER, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_PERSURE, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_CCTV, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_FIREEXT, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_FIREHYD, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_ALARMSTA, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_RECIVER, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_TEXTPOI, true);
            m_layerOutside.AddLayer(ID.ID_LAYER_BUILDING_TEXT, true, 400.0f, 100.0f);
            m_layerOutside.AddLayer(ID.ID_LAYER_CCTVLOW, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_CCTV_DISCONNECTED, false);

			//mSceneManager = new Core.SceneManager(mView1);

			//mVolmumeManagerOut = new Core.ZoneVolumeManager(mView1);
			//mVolmumeManagerIn = new Core.ZoneVolumeManager(mView2);

			//mView1.ReadHomeView("Main");
            mView1.FitView();
		}

		private void FormContent_SizeChanged(object sender, EventArgs e)
		{
		}

		private void FormLayout_Resize(object sender, EventArgs e)
		{
		}

		private void FormContent_Shown(object sender, EventArgs e)
		{
		}

		public void Init3DView()
		{
			if (bInit == true)
				return;
			bInit = true;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			string szPath = owner.ResourcePath;

			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
			string szAppName = currentProcess.ProcessName;
			
            //mEngine.Init(szPath, szAppName);

			//mViewList.Add(mView1);


			mView1.Size = new Size(1280, 1024);
            //mView2.Size = new Size(1280, 1024);
            mCurrent = mView1;

            try
            {
                mView1.Popup = contextMenuStripManualReport;
                //mView1.InitBaseView();
            }
            catch (System.Exception ex1)
            { 
                Debug.WriteLine(ex1.StackTrace);
            }

            //mViewList.Add(mView2);
            //try
            //{
            //    mView2.Popup = contextMenuStripManualReport;

            //}
            //catch (System.Exception ex2)
            //{
            //    Debug.WriteLine(ex2.StackTrace);
            //}

            bool visibleCompass = mView1.GetVisibleCompass();
            if (visibleCompass)
                mView1.CreateCompass(0.0f);
            //mView2.CreateCompass(0.0f);

            bool bSimMode = UnE.SOP.ProxySOP.Instance.SimulationMode;

			// open floor mesh
            string szFloorFile = Application.StartupPath + "\\DXF\\#1-2 BOILER\\1r403-1-886-ea152-205-f-001.png";
            if (!File.Exists(szFloorFile) || (bSimMode || owner.ExtractInside == true))
			{
				try
				{
					//ExtractToTrg(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");
					//mView2.ExtractFile(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.StackTrace);
				}
			}
			bExtractInside = true;

			try
			{
				Building building = ZoneManager.Instance.GetBuilding("A-1");
				if (building != null)
				{
					Zone zone = (Zone)building.FloorList[0];
					SetCurrentBuilding(building, zone);
				}
			}
			catch (System.Exception)
			{
			}

			//mSceneManager.UpdateData();
			
            LayoutOutside();
			
            AddGroupName();
			AddBuildingName();
            Add3DText();

            AddZoneVolume();
            AddSafeZoneVolume();
		}

		private void FormLayout_Load(object sender, EventArgs e)
		{
            SetBuildingText();
           
		}

	
		public void LoadPOIs()
		{
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.LoadPOI(mView1, false);

            //if (mView2.IsHandleCreated == true && mView2.Visible == true)

            //if(UnE.SOP.ProxySOP.Instance.Use2D == true)
            //    FormMain.Instance.DataManager.LoadPOI(mView2, true);

			CCTVManager.Instance.LoadEquipZoneCCTV();
		}

		private void FormLayout_FormClosed(object sender, FormClosedEventArgs e)
		{
			//mEngine.EngineDispose();
		}

		public void View1Click(object sender, EventArgs e)
		{
			if (mView1 != null)
			{
				mCurrent = mView1;
                mView1.Focus();
			}

            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button == System.Windows.Forms.MouseButtons.Right)
            {
                

                return;
            }
		}

		public void View2Click(object sender, EventArgs e)
		{
            mCurrent = null;
            //mView2.Focus();
		}

        public void TopView()
        {
            //mView2.FitView();
            //mView2.Refresh();
        }

		public void FrontViw()
		{			
		}

		public void LeftView()
		{
		}

		public void RightView()
		{
		}

		public void RearView()
		{
        }

		public void HomeView(string szName)
		{
            if (mCurrent != null)
            {
                mView1.LoadHomeView(szName);
                //mView1.FitView();
                //mView1.Refresh();    
            }
            else
            {
                //mView2.FitView();
                //mView2.Refresh();    
            }
                    
		}

		public void FitView()
		{        
            if(mCurrent != null)
            {
                mView1.FitView();
                mView1.Refresh();        
            }
            else
            {
                //mView2.FitView();
                //mView2.Refresh();        
            }                
		}

        public void HideAllShelter()
        {  
        }

        // nType : ShelterPath의 Type
        //         CoreAPI의 UBaseView::ShowPath(int nType)의 인자로 사용된다.
        // nShelterType : UnE.Spatial.Shelter.ShelterTypes(화재, 누출, 지진...)
        //                재난종류별 대피소를 각각 지정할 수 있도록 한다.
        public void ShowShelter(int nType, int nShelterType)
        {            
        }

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
            if (mCurrent != null)
            {
                mView1.OnMouseWheel(sender, e);
            }
            else
            {
                //mView2.OnMouseWheel(sender, e);
            }   

			
		}


        private bool m_bShowCollapse = false;
        public void ShowBuildingCollapse(string szBuildingID, string szDisplayName)
        {
            if (m_bShowCollapse == true)
                return;

            m_bShowCollapse = true;
        }

        public void CloseBuilingCollapse(string szBuildingID)
        {
        }

        public void SelectBuilding(string strBuildingID)
        {
        }

        public bool EarthquakeEventIsFinished()
        {
            return true;
        }

        public void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        { 
        }

		public void ZoomIn()
		{
			if (mCurrent != null)
			{
                mView1.ZoomIn();
			}
            else
            {
                //mView2.ZoomIn();
            }
		}

		public void ZoomOut()
		{
			if (mCurrent != null)
			{
                mView1.ZoomOut();
				//mCurrent.UpdatePOI();
			}
            else
            {
                //mView2.ZoomOut();
            }
		}

        public void ZoomBuilding(string szCode)
        {
            if (Char.IsDigit(szCode[0]))
            {
                szCode = "z" + szCode;
            }

            if (szCode == "yhz1_1" || szCode == "yhz2_1" || szCode == "yhz3_1")
            {
                szCode = szCode.Replace("_1", "");
            }

            mView1.ZoomObject(szCode);
        }

		public void ZoomTarget(float x, float y, float z, bool isIndoor)
		{
            y = 0.0f;
            //if (isIndoor)
            //{
            //    //if (mView2.MeshOpened)
            //    {
            //        //mView2.OnViewTop();
            //        //mView2.ZoomTarget(new global::Core.Position3D(x, y, z), 20.0f);
            //        //mView2.Update();
            //    }
            //}
            //else
            {
                if (mView1 != null)
                {
                    mView1.ZoomTarget(new global::Core.Position3D(x, y, z), 20.0f);
                    mView1.Update();
                }

            }
		}

		public void SelectPOI(POI poi, bool isIndoor)
		{
            //if (isIndoor == true)
            //    mView2.SelectPOI(poi.ID);
            //else
                mView1.SelectPOI(poi.ID);
		}

        public void SelectPOILoadZone(POI poi, bool isIndoor)
        {
            //if( isIndoor == true)
            //{
            //    mView2.SelectPOI(poi.ID); 
            //}
            //else
            {
                mView1.SelectPOI(poi.ID);
            }                       
        }

        private static int m_nImageNum = 1;
        public string SaveToTempImage()
        {            
            
            string szPath2 = System.IO.Path.GetTempPath() + "view2"+ m_nImageNum + ".bmp";

            //if (mCurrent == null)
            //{
            //    mView2.SaveScreen(szPath2, false);
            //}
            //else
            {
                mView1.SaveScreen(szPath2, false);
            }
            
            m_nImageNum++;

            if (m_nImageNum == 1000)
                m_nImageNum = 1;
            return szPath2;
        }

		public void SaveToImage()
		{
			SaveFileDialog dlg = new SaveFileDialog();

			dlg.Filter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";
			string defaultName = "Untitled";
			dlg.FileName = defaultName;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string szPath = dlg.FileName;

                //if (mCurrent == null)
                //{
                //    mView2.SaveScreen(szPath);
                //}
                //else
                {
                    mView1.SaveScreen(szPath);
                }
			}
		}

		public void SetLayoutMode(int nLayout)
		{
            if (m_nLayout == nLayout)
                return;

            switch (nLayout)
            {
                case 1:

                    LayoutOutside();
                    break;

                case 2:
                    LayoutBothside();
                    break;

                case 3:
                    LayoutInside();
                    break;

                default:
                    break;
            };

            m_nLayout = nLayout;
		}

		public void LayoutBothside()
		{
            if (m_nLayout == 2)
                return;

            m_nLayout = 2;

            if (!mMainToolStripContainer.ContentPanel.Controls.Contains(m_LayerContainer))
                mMainToolStripContainer.ContentPanel.Controls.Add(m_LayerContainer);
            m_LayerContainer.Visible = true;
            m_LayerContainer.Dock = DockStyle.Fill;
            if (m_bFirstBothSide == true)
            {
                if (this.Size.Width == 0)
                    return;

                m_LayerContainer.SplitterDistance = this.Size.Width / 2;
            }

            if (!m_LayerContainer.Panel1.Controls.Contains(mView1))
            {
                mMainToolStripContainer.ContentPanel.Controls.Remove(mView1);
                m_LayerContainer.Panel1.Controls.Add(mView1);
            }

            //if (!m_LayerContainer.Panel2.Controls.Contains(mView2))
            //{
            //    mMainToolStripContainer.ContentPanel.Controls.Remove(mView2);
            //    m_LayerContainer.Panel2.Controls.Add(mView2);
            //}

            //mView2.Dock = DockStyle.Fill;
            //mView2.Visible = true;
            //mView2.BringToFront();
            //mView2.FitView();
            //mView2.Invalidate(true);

            mView1.Dock = DockStyle.Fill;
            mView1.Visible = true;
            mView1.Invalidate(true);



            m_buildingCurrent = null;
            m_bFirstBothSide = false;
		}

		public void LayoutInside()
		{
			if (m_nLayout == 3)
				return;
			m_nLayout = 3;

			m_LayerContainer.Visible = false;

            //if (m_LayerContainer.Panel2.Controls.Contains(mView2))
            //{
            //    m_LayerContainer.Panel2.Controls.Remove(mView2);
            //    mMainToolStripContainer.ContentPanel.Controls.Add(mView2);
            //}
            if (mMainToolStripContainer.ContentPanel.Controls.Contains(m_LayerContainer))
                mMainToolStripContainer.ContentPanel.Controls.Remove(m_LayerContainer);

            //mView2.Dock = DockStyle.Fill;
            //mView2.Visible = true;
            //mView2.BringToFront();

            //mView2.FitView();
            //mView2.Invalidate(true);

			m_buildingCurrent = null;
		}

		public void LayoutOutside()
		{
            if (m_nLayout == 1)
                return;
            m_nLayout = 1;

            m_LayerContainer.Visible = false;
            if (m_LayerContainer.Panel1.Controls.Contains(mView1))
            {
                m_LayerContainer.Panel1.Controls.Remove(mView1);
                mMainToolStripContainer.ContentPanel.Controls.Add(mView1);
            }

            //if (m_LayerContainer.Panel2.Controls.Contains(mView2))
            //{
            //    m_LayerContainer.Panel2.Controls.Remove(mView2);
            //    mMainToolStripContainer.ContentPanel.Controls.Add(mView2);
            //}

            if (mMainToolStripContainer.ContentPanel.Controls.Contains(m_LayerContainer))
                mMainToolStripContainer.ContentPanel.Controls.Remove(m_LayerContainer);

            mCurrent = mView1;

            //mView2.Visible = false;
            mView1.Visible = true;
            mView1.Dock = DockStyle.Fill;
            mView1.Invalidate(true);

            m_buildingCurrent = null;
		}

        internal class ViewState
        {
            private int m_nLayout = -1;
            public int Layout
            {
                get { return m_nLayout; }
                set { m_nLayout = value; }
            }

            private Zone m_nCurrentZone = null;
            public Zone Zone
            {
                get { return m_nCurrentZone; }
                set { m_nCurrentZone = value; }
            }

            private string m_szImagePath = "";
            public string ImagePath
            {
                get { return m_szImagePath; }
                set { m_szImagePath = value; }
            }

            private UnE.View.Content.ContentOwnerTab mTabNumber = UnE.View.Content.ContentOwnerTab.M3D_TAB;
            public UnE.View.Content.ContentOwnerTab TabNumber
            {
                get { return mTabNumber; }
                set { mTabNumber = value; }
            }
        }



        private string szPushViewStat = "SaveState";
        private ViewState GetCurrentViewState(bool bSavedCurrentTab)
        {
            ViewState state = new ViewState();
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            if (bSavedCurrentTab == false)
                state.TabNumber = owner.PreviousTab;
            else
                state.TabNumber = owner.CurrentTab;
            state.Layout = m_nLayout;

            //state.Zone = mView2.CurrentZone;
            //state.ImagePath = mView2.ImagePath;    

            return state;
        }


        private Stack<ViewState> mLayooutStack = new Stack<ViewState>();
        public void PushViewState(bool bSavedCurrentTab = false)
        {
            // View 상태는 최초 1개만 저장해야 한다.
            if (mLayooutStack.Count > 0)
                return;
            
            // View 상태를 저장
            ViewState state = GetCurrentViewState(bSavedCurrentTab);
            mLayooutStack.Push(state);

            //mView2.SaveViewState(szPushViewStat);
        }


        private bool m_bChangedTab = false;

        public void ClearTabState()
        {
            m_bChangedTab = true;
        }
        
        public void ClearViewState()
        {
            mLayooutStack.Clear();
            m_bChangedTab = false;
        }


        public void RestoreViewState()
        {

            if (mLayooutStack.Count < 1)
                return;

            int nLayout = -1;
            if(mLayooutStack.Count > 0)
            {
                ViewState state = mLayooutStack.Pop();
                nLayout = state.Layout;

                //mView2.LoadViewState(szPushViewStat);

                //if( state.Zone != null && state.ImagePath != "")
                //{
                //    mView2.SetImage(state.ImagePath, state.Zone);
                //}

                UnE.View.Content.ContentOwnerTab tab = state.TabNumber;

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.ChangeTab(tab);

                m_bChangedTab = false;
            }

            if (nLayout > 0)
            {
                // 1(Outside), 2(Both), 3(Inside)

                int nID = ID.ID_VIEW_OUTSIDE;
                switch(nLayout)
                {
                    case 1: // outside
                        nID = ID.ID_VIEW_OUTSIDE;
                        break;
                    case 2: // Both
                        nID = ID.ID_VIEW_BOTHSIDE;
                        break;
                    case 3:
                        nID = ID.ID_VIEW_INSIDE;
                        break;
                }

                
                SetLayoutMode(nLayout);

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.Check3DViewMode(nID);                 
            }                
        }


		public void AddOutZoneName()
		{			
		}

        //public void AddZoneName(Zone zone)
        //{
            //if (mView2 != null)
            //{
            //    try
            //    {
            //        string szName = string.Format("{0} [{1}]", zone.ZoneName, m_szFileName);
            //        mView2.AddZoneName(szName);
            //        //m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
            //    }
            //    catch (System.Exception ex)
            //    {
            //        System.Diagnostics.Trace.WriteLine(ex.Message);
            //        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            //    }
            //}
        //}

		public void AddGroupName()
		{			
		}

        public void SetBuildingText()
        {
            // BuildingGroup Text, MinPT.X, MinPT.Y, MaxPT.X, MaxPT.Y
            ArrayList distinctBuildingGroup = new ArrayList();
            // Building Text, MinPT.X, MinPT.Y, MaxPT.X, MaxPT.Y
            ArrayList distinctBuilding = new ArrayList();
                        
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
            {
                string strQuery = "Select GroupName, X, Y From BuildingGroup";
                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                DBUtility2.WebDBManager dbMgr = owner.DBManager;
                ArrayList arrResult = dbMgr.GetResultData(strQuery);

                if (arrResult == null || arrResult.Count == 0)
                    return;

                for (int i = 0; i < arrResult.Count; i += 3)
                {
                    string groupName = DBUtility2.WebDBManager.GetStringField(arrResult[i].ToString());
                    float x = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 1].ToString(), -1);
                    float y = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 2].ToString(), -1);

                    if (groupName.Length == 0 || x == -1 || y == -1)
                        continue;

                    distinctBuildingGroup.Add(groupName);
                    distinctBuildingGroup.Add(x);
                    distinctBuildingGroup.Add(y);
                }

                string strQuery2 = "Select BuildingName, X, Y From Building";                
                ArrayList arrResult2 = dbMgr.GetResultData(strQuery2);

                if (arrResult2 == null || arrResult2.Count == 0)
                    return;

                for (int i = 0; i < arrResult2.Count; i += 3)
                {
                    string buildingName = DBUtility2.WebDBManager.GetStringField(arrResult2[i].ToString());
                    float x = DBUtility2.WebDBManager.GetFloatField(arrResult2[i + 1].ToString(), -1);
                    float y = DBUtility2.WebDBManager.GetFloatField(arrResult2[i + 2].ToString(), -1);

                    if (buildingName.Length == 0 || x == -1 || y == -1)
                        continue;

                    distinctBuilding.Add(buildingName);
                    distinctBuilding.Add(x);
                    distinctBuilding.Add(y);
                }
            }
            else
            {
                // 전체 빌딩그룹
                foreach (KeyValuePair<int, BuildingGroup> item in ZoneManager.Instance.DicBuildingGroup)
                {
                    BuildingGroup buildingGroup = item.Value;
                    if (buildingGroup == null)
                        continue;

                    if (buildingGroup.BuildingList == null)
                        continue;

                    Point groupMinPt = new Point();
                    Point groupMaxPt = new Point();

                    // 전체 빌딩그룹별 빌딩리스트
                    for (int i = 0; i < buildingGroup.BuildingList.Count; i++)
                    {
                        Building building = buildingGroup.BuildingList[i] as Building;
                        if (building == null)
                            continue;

                        if (building.EquipZoneList == null)
                            continue;

                        Point minPt = new Point();
                        Point maxPt = new Point();

                        // 빌딩에 포함된 Equipzone 최대, 최소 Polygon 구하기
                        for (int j = 0; j < building.EquipZoneList.Count; j++)
                        {
                            EquipmentZone equipZone = building.EquipZoneList[j] as EquipmentZone;
                            if (equipZone == null || equipZone.Polygon == null)
                                continue;

                            Polygon polygon = equipZone.Polygon;

                            Vertex2D vertexMin = polygon.GetMin();
                            Vertex2D vertexMax = polygon.GetMax();

                            if (minPt.X == 0)
                                minPt.X = (int)vertexMin.x;
                            else
                                minPt.X = System.Math.Min((int)minPt.X, (int)vertexMin.x);

                            if ((int)vertexMax.x < 0)
                                maxPt.X = (int)vertexMax.x;
                            else
                                maxPt.X = System.Math.Max((int)maxPt.X, (int)vertexMax.x);

                            if (minPt.Y == 0)
                                minPt.Y = (int)vertexMin.y;
                            else
                                minPt.Y = System.Math.Min((int)minPt.Y, (int)vertexMin.y);

                            if ((int)vertexMax.y < 0)
                                maxPt.Y = (int)vertexMax.y;
                            else
                                maxPt.Y = System.Math.Max((int)maxPt.Y, (int)vertexMax.y);
                        }

                        if (groupMinPt.X == 0)
                            groupMinPt.X = (int)minPt.X;
                        else
                            groupMinPt.X = System.Math.Min((int)groupMinPt.X, (int)minPt.X);

                        if ((int)maxPt.X < 0)
                            groupMaxPt.X = (int)maxPt.X;
                        else
                            groupMaxPt.X = System.Math.Max((int)groupMaxPt.X, (int)maxPt.X);

                        if (groupMinPt.Y == 0)
                            groupMinPt.Y = (int)minPt.Y;
                        else
                            groupMinPt.Y = System.Math.Min((int)groupMinPt.Y, (int)minPt.Y);

                        if ((int)maxPt.Y < 0)
                            groupMaxPt.Y = (int)maxPt.Y;
                        else
                            groupMaxPt.Y = System.Math.Max((int)groupMaxPt.Y, (int)maxPt.Y);

                        string buildingName = building.DisplayText;
                        if (buildingName.Length == 0)
                            buildingName = building.BuildingName;

                        if (minPt.X == 0 && minPt.Y == 0 && maxPt.X == 0 && maxPt.Y == 0)
                            continue;

                        distinctBuilding.Add(buildingName);
                        distinctBuilding.Add(minPt.X);
                        distinctBuilding.Add(minPt.Y);
                        distinctBuilding.Add(maxPt.X);
                        distinctBuilding.Add(maxPt.Y);
                    }

                    string buildingGroupName = buildingGroup.DisplayName;
                    if (buildingGroupName.Length == 0)
                        buildingGroupName = buildingGroup.BuildingGroupName;

                    distinctBuildingGroup.Add(buildingGroupName);
                    distinctBuildingGroup.Add(groupMinPt.X);
                    distinctBuildingGroup.Add(groupMinPt.Y);
                    distinctBuildingGroup.Add(groupMaxPt.X);
                    distinctBuildingGroup.Add(groupMaxPt.Y);
                }
            }

            mView1.ArrBuildingGroupText = distinctBuildingGroup;
            mView1.ArrBuildingText = distinctBuilding;
        }

        public void ShowZoneVolume(int zoneID, bool bOutDoorWnd, bool bShow)
        {
            //Zone zone = ZoneManager.Instance.GetZone(zoneID);
            //if (zone == null)
            //    return;

            //m_bChangeIndoor = false;
            //string szID = zone.ZoneName;
            //mView2.ShowZonePolygon(zone, bShow);
        }     

		public void ShowZoneVolume(int zoneID, int nEquipZoneID, bool bOutDoorWnd, bool bShow)
		{
            Zone zone = ZoneManager.Instance.GetZone(zoneID);
            if (zone == null)
                return;

            if (bOutDoorWnd == true)
            {
                //EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                //if (equipZone != null)
                //{
                //    int nCount = equipZone.LinkedZoneList.Count;
                //    if (nCount > 1)
                //    {
                //        for (int i = 0; i < nCount; i++)
                //        {
                //            string szLinkID = "";
                //            Zone linkedZone = (Zone)equipZone.LinkedZoneList[i];
                //            if (zone.IsOutdoor == false)
                //            {
                //                szLinkID = zone.Building.BuildingID;
                //            }
                //            else
                //            {
                //                szLinkID = zone.ZoneName;
                //            }
                //        }
                //    }
                //}

                string szID = "";
                if (!zone.IsOutdoor)
                {
                    szID = zone.Building.BuildingID;
                    //if (Char.IsDigit(szID[0]))
                    //{
                    //    szID = "z" + szID;
                    //}
                    
                }
                else
                {
                    szID = zone.ZoneName;
                }

                //mView1.SelectObject(szID); 
            }
            else
            {
                if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                {
                    // 신호와 다른 캠퍼스일때 캠퍼스 View를 먼저 바꿔줌
                    if (!mView1.IsCampus && zone.Building.BuildingGroup.BuildingGroupName == "보운캠퍼스"
                     || mView1.IsCampus && zone.Building.BuildingGroup.BuildingGroupName == "대덕캠퍼스")
                    {
                        mView1.SetChangeCampus();
                    }
                }

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                // 수동신고는 equipzoneID가 넘어오지 않는다
                if (equipZone == null)
                {
                    mView1.ShowZone(zone, bShow);
                }              
                else
                {
                    if (equipZone.IsOutdoor == false)
                    {
                        m_bChangeIndoor = false;
                        string szID = equipZone.ZoneName;

                        if (bShow == true)
                            mView1.HideAllEquipmentZone();
                        
                        mView1.ShowEquipmentZone(equipZone, bShow);
                    }
                }
            }
		}

		private bool m_bChangeIndoor = false;

		public void HideZoneVolume()
		{
            m_bChangeIndoor = false;            
            mView1.HideAllEquipmentZone();			
		}


        public void AddSafeZoneVolume()
        {
        }

		public void AddZoneVolume()
		{			
		}

		public void AddBuildingName()
		{
		}

        public void Add3DText()
        {            
        }

		public void RedrawWindow()
		{
            //if (mView2 != null && mView2.Visible == true)
            //{
            //    mView2.Refresh();
            //}
		}

		public void Invalidate3DView(bool bErBack)
		{
            //if (mView2 != null && mView2.Visible == true)
            //{
            //    mView2.Invalidate(bErBack);
            //}
		}
        public void HidePollutioinView()
        {
            //3D 광교 오염물질 전용
        }
		public void SetFilePath(string strCMOFolderPath, string strOutsideFilePath, string strInsideFilePath, Dictionary<string, string> dicInsideCMO)
		{
			m_strZipFileFolderPath = strCMOFolderPath;
			m_strOutsideDAE = strOutsideFilePath;
			m_strInsideDAE = dicInsideCMO["Inside"];
			m_dicInsideDAE = dicInsideCMO;
		}

		public void SetCurrentBuilding(Building building, Zone showFloor)
		{
			if (m_buildingCurrent == building)
			{
			}

			m_buildingCurrent = building;
			ShowIndoor(showFloor);
		}

		private void SetCurrentBuilding(Building building)
		{
			if (m_buildingCurrent == building)
				return;

			m_buildingCurrent = building;

			if (m_nLayout == 2 || m_nLayout == 3)
			{
				Zone zone = (Zone)m_buildingCurrent.FloorList[0];
				ShowIndoor(zone);
			}
			this.Focus();
		}

		public Building GetCurrentBuilding(ref float nCurrentFloorIndex)
		{
			nCurrentFloorIndex = m_nCurrentFloor;
			return m_buildingCurrent;
		}

	
        public void ShowIndoor(Zone zone)
		{
			if (m_buildingCurrent != null)
			{
                if (zone.DXFFilePath == null || zone.DXFFilePath == "")
                    return;

				Floor floor = zone.Floor;
				m_currentIndoorZone = zone;
				string szCode = m_buildingCurrent.BuildingID;
				if (szCode == "")
					return;

                //float nFloor = floor.FloorIndex + 1;
                //if (szCode[0] >= '0' && szCode[0] <= '9')
                //{
                //    szCode = "z" + szCode;
                //}
                //nCurrentFloor = floor.FloorIndex;
                
                //string szFileName = null;

                //if (floor.FloorIndex < 0)
                //    szFileName = string.Format("{0}_B{1:f1}", szCode, -nCurrentFloor);
                //else
                //    szFileName = string.Format("{0}_{1:f1}", szCode, nFloor);

                //if (szFileName.EndsWith(".0"))
                //    szFileName = szFileName.Substring(0, szFileName.Length - 2);

                //m_szFileName = szFileName;

                //if (szFileName[szFileName.Length - 2] == '.')
                //{
                //    szFileName += "M.png";

                //    m_szFileName += "M";
                //}
                //else
                //{
                //    szFileName += ".png";
                //}
                //// find dae
                //string szFullPath = m_strZipFileFolderPath + "inside\\" + szFileName;
                //MessageBox.Show(szFullPath);
                szInsideFullPath = Application.StartupPath + "\\DXF\\" + zone.DXFFilePath.Replace(".dxf", ".png");
                if (szPrevFileName == szInsideFullPath)
				{
					return;
				}

				// clear current view
				try
				{
					if (m_bLoadInsideMode == true)
					{
                        //mView2.ClearAllData();
						m_bLoadInsideMode = false;
					}
				}
				catch (System.Exception)
				{
				}

				bool bExist = File.Exists(szInsideFullPath);
				if (bExist == false || bExtractInside == false)
				{
                    //mView2.Refresh();
					szPrevFileName = "";
					return;
				}

				m_nCurrentFloor = floor.FloorIndex;
				m_bChangeIndoor = true;
               
                //mView2.SetImage(szInsideFullPath, m_currentIndoorZone);
                // 테스트
                //AddZoneName(m_currentIndoorZone);

                //mView2.FitView();

                m_bLoadInsideMode = true;

                // 테스트
                //ShowZoneVolume(m_currentIndoorZone.ID, false, true);

                //mView2.Refresh();

                GC.Collect();

			}
		}

		public void OpenModel()
		{
			if (szInsideFullPath != null)
			{           
                //mView2.SetImage(szInsideFullPath, m_currentIndoorZone);  
				// 테스트
				//AddZoneName(m_currentIndoorZone);

                //mView2.FitView();

				m_bLoadInsideMode = true;

				// 테스트
				//ShowZoneVolume(m_currentIndoorZone.ID, false, true);

                //mView2.Refresh();

				GC.Collect();

				szPrevFileName = szInsideFullPath;
				FormModelLoading.iForm.Close();
			}
		}
        
        public void SetEvacCenter(EquipmentZone zone)
        {

        }
        public void ShowEvacCircle(int nLevel)
        {

        }

        public void SetEvacDistance(int nSensorID)
        {

        }
        public void HideEvacCircle()
        {

        }

		public bool ShowLayer(int id, bool bShow)
		{
            if (bShow == true)
                Layers.ShowLayer(id);
            else
                Layers.HideLayer(id);

            //mView2.ShowLayer(id, bShow);
			mView1.ShowLayer(id, bShow);

            //mView2.UpdatePOI();
			mView1.UpdatePOI();

			RedrawWindow();

			return false;
		}

        public void AttachView(System.Windows.Forms.Control view, bool isOutdoor)
        {
            if (isOutdoor)
                mView1 = (ImageViewCtrl)view;
            //else
            //    mView2 = (ImageViewCtrl)view;

            mMainToolStripContainer.ContentPanel.Controls.Add(view);

            if (isOutdoor == false)
            {
                if (m_nLayout == 1)
                {
                    m_nLayout = -1;

                    LayoutOutside();
                }
                if (m_nLayout == 2)
                {
                    m_nLayout = -1;
                    LayoutBothside();
                }
                if (m_nLayout == 3)
                {
                    m_nLayout = -1;
                    LayoutInside();
                }
            }
        }

        public System.Windows.Forms.Control DetachView(bool isOutdoor)
        {
            System.Windows.Forms.Control view = null;

            if (isOutdoor)
            {
                view = mView1;
            }
            else
            {
                //view = mView2;
            }

            mMainToolStripContainer.ContentPanel.Controls.Remove(view);

            return view;
        }

		public void ClearPOISelection()
		{
            //mView2.ClearPOISelection();
            mView1.ClearPOISelection();
		}

		public void HideAllPOIPopup()
		{
            mView1.HideAllPOIPopup();
            //mView2.HideAllPOIPopup();
		}

		public void IndoorMenuClick(object sender, EventArgs e)
		{
			if (sender == null)
				return;

			ToolStripMenuItem item = (ToolStripMenuItem)sender;

			Zone zone = (Zone)item.Tag;
			if (zone != null)
			{
                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
				owner.SelectIndoorZone(zone);
			}
		}

		private Zone m_ManualClickZone = null;

		public Zone ManualClickZone
		{
			get { return m_ManualClickZone; }
			set { m_ManualClickZone = value; }
		}


		/*private bool GetSMSConfig()
		{
			return FormSMSConfig.UseSMSOnDetectFire;
		}*/
        		

		public void ManualReportClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            object obj = item.Tag;
            if (obj == null)
                return;

            if (obj.GetType() == typeof(Building))
            {
                // Indoor
            }
            else
            {
                m_ManualClickZone = (Zone)obj;
                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.EnableFireReportBtn(true, 2);
                // outdoor zone
            }
        }

		public void ManualCCTVClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            object obj = item.Tag;
            if (obj == null)
                return;

            if (obj.GetType() == typeof(Building))
            {
                // Indoor
            }
            else
            {
                m_ManualClickZone = (Zone)obj;

                List<EquipmentZone> arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(m_ManualClickZone);
                if (arEquipzone != null && arEquipzone.Count > 0)
                {

                    //if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == false)
                    //{
                    //    // CCTV 탭으로 변경
                    //    FormMain.Instance.SelectCCTVTab(false);
                    //}
                    EquipmentZone equipZone = (EquipmentZone)arEquipzone[0];
                    //PageBackstageHome.Instance.ShowBigCCTV(equipZone, 0);
                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    owner.ShowEquipZoneCCTVs(equipZone.ID);
                }
                // outdoor zone
            }
        }

		private bool mCheckPosition = false;
		private HistoryDisasterPosition mLastPos = null;

		public HistoryDisasterPosition LastPos
		{
			get { return mLastPos; }
			set { mLastPos = value; }
		}

		private IWorkflowStartOption mFormPosition = null;

		public void SetCheckPoistion(IWorkflowStartOption form, bool bCheck)
		{
			mLastPos = null;
			mFormPosition = form;

			form.OnCheckPositionEnd += OnCheckEnd;

			mCheckPosition = true;

			//BaseView view1 = (BaseView)mView1;
            mView1.SetCheckPoistion(mCheckPosition);            			
            //mView2.SetCheckPoistion(mCheckPosition);

            //ArrayList ar = new ArrayList();
            //ZoneManager z = ZoneManager.Instance;
            
            //foreach(Building b in z.DicBuildings.Values)
            //{
            //    ar.Add(b.BuildingID);
            //}

		}

		public void OnCheckEnd(bool bResult)
		{
			if (mFormPosition == null)
				return;
			mFormPosition.OnCheckPositionEnd -= OnCheckEnd;
			LastPos = mFormPosition.LastPosition;
			mCheckPosition = false;
			szIconPath = szMediaPath + "icons\\" + mFormPosition.DisasterName + ".ico";

            Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
            formInvoke.Invoke((MethodInvoker)delegate
			{
                //IBaseView view2 = mView2;
				//BaseView view1 = (BaseView)mView2;
                //mView2.SetCheckPoistion(mCheckPosition);
                //view2.SetCheckPoistion(mCheckPosition);
				if (m_nLayout == 3)
				{
					if (bResult == true)
					{
						// view2.AddPOI(szIconPath);
                        //if (LastPos != null)
                        //{
                        //    int nID = view2.AddPOI(szIconPath, LastPos.X, LastPos.Y, LastPos.Z);
                        //    LastPos.IconID = nID;
                        //}
                        //else
                        //{
                        //    view2.AddPOI(szIconPath);
                        //    //LastPos.IconID = nID;
                        //}
                        //view2.UpdateWindow();
					}
				}
				else
				{
                    //if (bResult == true)
                    //{
                    //    if (LastPos != null)
                    //    {
                    //        float dx = 120894.0548f + 1008.531f;
                    //        float dy = 157659.0963f - 506.251f;
                    //        float ox = LastPos.X - dx;
                    //        float oz = dy - LastPos.Z;
                    //        int nID = mView2.AddPOI(szIconPath, ox, LastPos.Y);
                    //        LastPos.IconID = nID;
                    //    }
                    //    else
                    //        mView2.AddPOI(szIconPath);
                    //    mView2.Refresh();
                    //}
				}
			});
			mFormPosition = null;
		}

        public void ShowCCTVForm(bool bShow)
        {
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.ShowCCTVForm(true);
        }

		public void RemoveDisasterPos()
		{
            //IBaseView view1 = mView2;
            //if (view1 != null)
            //{
            //    if (LastPos != null)
            //    {
            //    //    float dx = 120894.0548f + 1008.531f;
            //    //    float dy = 157659.0963f - 506.251f;
            //        float ox = LastPos.X;
            //        float oz = - LastPos.Z;
            //        int nID = LastPos.IconID;
            //        if (nID != -1)
            //        {
            //            view1.RemovePOI(nID);
            //        }
            //        else
            //        {
            //            view1.RemovePOI(ox, LastPos.Y, oz);
            //        }
            //        view1.UpdateWindow();
            //    }
            //}

            //if (mView2 != null)
            //{
            //    if (LastPos != null)
            //    {
            //        int nID = LastPos.IconID;
            //        if (nID != -1)
            //        {
            //            view1.RemovePOI(nID);
            //        }
            //        else
            //        {
            //            //mView2.RemovePOI(LastPos.X, LastPos.Y);
            //        }
            //        mView2.Refresh();
            //    }
            //}
		}

		public void AddDisasterPos(string disastertype, float x, float y, float z)
		{
			if (LastPos == null)
				return;

            if (LastPos.BuildingID != "ZONE")
			{
				Building curBuilding = null;
				curBuilding = ZoneManager.Instance.GetBuilding(LastPos.BuildingID);
				if (curBuilding == null)
					return;
				Zone zone = ZoneManager.Instance.GetZone(curBuilding.BuildingID, LastPos.FloorIndex);
				if (zone == null)
					return;

				SetCurrentBuilding(curBuilding, zone);

				string path = szMediaPath + "icons\\" + disastertype + ".ico";

                //if (mView2 != null && zone != null && zone.Polygon != null)
				{
					UnE.Geometry.Vertex2D pos = zone.Polygon.CalcWeightCenter();
					//float dx = 120894.0548f + 1008.531f;
					//float dy = 157659.0963f - 506.251f;
					float ox = x + (float)pos.x;
					float oz = - z + (float)pos.y;
                    //int nID = mView2.AddPOI(path, x, y);
					//LastPos.IconID = nID;
                    //mView2.Refresh();
				}
				RedrawWindow();
			}
			else
			{
				string path = szMediaPath + "icons\\" + disastertype + ".ico";
                //IBaseView view1 = mView2;
                //if (view1 != null)
                //{
                //    //float dx = 120894.0548f + 1008.531f;
                //    //float dy = 157659.0963f - 506.251f;
                //    float ox = x;
                //    float oz = - z;
                //    int nID = view1.AddPOI(path, ox, y, oz);
                //    LastPos.IconID = nID;
                //    view1.UpdateWindow();
                //}
				RedrawWindow();
			}
		}

		public UnE.SOP.HistoryDisasterPosition GetLastDisasterPosition()
		{
			return mLastPos;
		}

		public void SetDisasterPos_Click(object sender, EventArgs e)
        {
            //if (m_nLayout == 3)
            //{
            //    if (mCurrent != null && mFormPosition != null)
            //    {
            //    }
            //}
            //else
            //{
            //    if (mCurrent != null && mFormPosition != null)
            //    {
            //        float ox = 1008.531f;
            //        float oy = 506.251f;

            //        float dx = 120894.0548f + ox;
            //        float dy = 157659.0963f - oy;

            //        string szBroadcastName = "";
            //        string szSelectedName = mCurrent.PopupObjName;
            //        Building curBuilding = (Building)this.menuManualCCTV.Tag;
            //        UnE.Util.Unity.Vector3 pos3D = (UnE.Util.Unity.Vector3)mCurrent.PopupMenu.Tag;

            //        bool isBuildingName = false;
            //        if (szSelectedName != null && szSelectedName != "")
            //        {
            //            if (curBuilding != null)
            //            {
            //                szSelectedName = curBuilding.DisplayText;
            //                szBroadcastName = curBuilding.BroadcastName;
            //                isBuildingName = true;
            //            }
            //        }

            //        if (isBuildingName == false)
            //        {
            //            szBroadcastName = ZoneManager.Instance.CheckZoneBroadcastName(pos3D.X, pos3D.Z);
            //            string szName = ZoneManager.Instance.CheckZoneName(pos3D.X, pos3D.Z);
            //            if (szName != "")
            //            {
            //                szSelectedName = szName;
            //            }
            //            else
            //            {
            //                szSelectedName = "";
            //                return;
            //            }
            //        }

            //        if (mFormPosition != null && mFormPosition.IsHandleCreated())
            //        {
            //            mLastPos = new HistoryDisasterPosition();
            //            mLastPos.PoistionName = szSelectedName;

            //            mLastPos.X = pos3D.X;
            //            mLastPos.Y = pos3D.Y;
            //            mLastPos.Z = pos3D.Z;
            //            mLastPos.FloorIndex = -999;
            //            mLastPos.BroadcastName = szBroadcastName;

            //            if (isBuildingName == true)
            //                mLastPos.BuildingID = curBuilding.BuildingID;
            //            else
            //                mLastPos.BuildingID = "ZONE";

            //            Form form = mFormPosition.GetInvokeForm();
            //            form.Invoke((MethodInvoker)delegate
            //            {
            //                mFormPosition.PositionName = szSelectedName;
            //            });

            //            form.Invoke((MethodInvoker)delegate
            //            {
            //                mLastPos.DisasterName = mFormPosition.DisasterName;
            //            });

            //            form.Invoke((MethodInvoker)delegate
            //            {
            //                mFormPosition.AddLastHistoryDisasterPoistion(mLastPos);
            //            });
            //        }
            //    }
            //}
        }		

        public void ToggleBuildingTextLayer()
        {
            m_bLODText = !m_bLODText;
            //mView1.SetTextLOD(m_bLODText);
        }


        private SortedList<int, int> tabLayoutList = new SortedList<int, int>();
        public void SaveCurrentTabLayout()
        {
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            int current = (int)owner.CurrentTab;            
            if(tabLayoutList.ContainsKey(current))
            {
                tabLayoutList.Remove(current);
            }
            tabLayoutList.Add(current, m_nLayout);           

        }

        public void LoadTabLayout(int tabNumber)
        {
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            if (tabLayoutList.ContainsKey(tabNumber))
            {
                int nLayout = tabLayoutList[tabNumber];
                //m_nLayout = nLayout;
                if( nLayout == 1)
                {
                    owner.OnClick3D();
                }
                else if (nLayout == 2)
                {
                    owner.OnClickBothView(false);
                }
                else if( nLayout == 3)
                {
                    owner.OnClick2D();
                }
            }
            else
            {
                if (tabNumber == 1) // 3d
                {
                    owner.OnClick3D();
                    //LayoutOutside();
                }
                if (tabNumber == 2) // admin
                {
                    owner.OnClick3D();
                }
                else if (tabNumber == 4) // 2d
                {
                    owner.OnClick2D();
                }
            }
        }

        public void ChangeCampus()
        {
            mView1.SetChangeCampus();
        }

        public void IsSameCampus(BuildingGroup group)
        {
            if (group == null)
                return;

            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
            {
                // 신호와 다른 캠퍼스일때 캠퍼스 View를 먼저 바꿔줌
                if (!mView1.IsCampus && group.BuildingGroupName == "보운캠퍼스"
                 || mView1.IsCampus && group.BuildingGroupName == "대덕캠퍼스")
                {
                    mView1.SetChangeCampus();
                }
            }
        }

        public void SelectScene(string strSceneName)
        {
        }

        public void ShowAlarmZone(string strZoneName, bool hideAllOthers)
        {
        }

        public void HideAlarmZone(string strZoneName)
        {
        }

        public void HideAllAlarmZones()
        {
        }

        public void VisibleViewButton(string strBtnName, bool visible)
        {
            
        }

        public void AddWall()
        {
        }

        public void AddDoor()
        {
        }

        public bool GetWalls(string strPath)
        {
            return true;
        }

        public bool LoadWalls(string strPath, string strSceneName)
        {
            return true;
        }

        public void SetWallSnap(bool bUse)
        {
            
        }

        public void SetWallEditMode(bool bEdit)
        {
            
        }

        public void AddSpaceText(string strTxt)
        {
            
        }

        public void LoadSpaceTexts(string strPath, string strScenName)
        {
            
        }

        public void GetSpaceTexts(string strPath)
        {
            
        }

        public void SetPoiLod(string strPOIType, bool useLOD)
        {
        }

        public void AddPoiLodValue(float fMinZoomValue, float fMaxZoomValue, float fDistance)
        {
        }

        public void ClearPoiLodValue()
        {
        }

        public void ChangeColorSpaceText(string hexColor)
        {
            
        }

        public void ChangeFontSpaceText(string name, float nSize, int nStyle)
        {
            
        }
    }
}