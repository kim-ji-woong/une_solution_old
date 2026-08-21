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
using Core;
using UnE.Geometry;
using UnE.SOP;
using UnE.SOP.Workstate;
using UnE.Spatial;
using UnE.Sensor;
using UnE.View.Content;
using DBUtility;
using SDMS;

namespace UnE.View.Content
{
	public partial class FormContent2D : Form, IDisasterContainer, IFormContent
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

        private BaseViewEx2 mView1 = null;
        private ImageViewCtrl mView2 = null;

        private global::Core.Engine mEngine = new global::Core.Engine();

		private string m_strZipFileFolderPath = "";
		private string m_strOutsideDAE = "";
		private string m_strInsideDAE = "";
		private Building m_buildingCurrent = null;
		private Dictionary<string, string> m_dicInsideDAE = null;
		private string m_strOutDaeName = "";

		private ArrayList mViewList = new ArrayList();
        private BaseViewEx2 mCurrent = null;
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
            // read toolstrip position
            if (strip != null)
            {
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

        public void RemoveMainToolStrip(ToolStrip strip)
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
                ((BaseViewEx2)mView1).EditMode = value;
                ((ImageViewCtrl)mView2).EditMode = value;
			}
		}

		private SplitContainer m_LayerContainer = null;

        public MouseWorkMode CurrentMouseWorkMode
		{
            get { return ((BaseViewEx2)mView1).CurrentMouseWorkMode; }
			set
			{
                ((BaseViewEx2)mView1).CurrentMouseWorkMode = value;
                ((ImageViewCtrl)mView2).CurrentMouseWorkMode = value;
			}
		}

        private global::Core.SceneManager mSceneManager = null;

        public global::Core.SceneManager SceneManager
		{
			get { return mSceneManager; }
			set { mSceneManager = value; }
		}

        private global::Core.ZoneVolumeManager mVolmumeManagerOut = null;

        public global::Core.ZoneVolumeManager OutdoorVolmumeManager
		{
			get { return mVolmumeManagerOut; }
			set { mVolmumeManagerOut = value; }
		}

        private global::Core.ZoneVolumeManager mVolmumeManagerIn = null;

        public global::Core.ZoneVolumeManager IndoorVolmumeManager
		{
			get { return mVolmumeManagerIn; }
			set { mVolmumeManagerIn = value; }
		}

        public ISensorTooltipOwner OutdoorView
		{
			get { return mView1; }
		}

        public ISensorTooltipOwner IndoorView
		{
			get { return mView2; }
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

		public FormContent2D(IBaseViewOwner owner)
		{
            m_BaseViewOwner = owner;
            UnE.View.Content.ViewUtils.RegisterContentView(this);
            UnE.View.Content.IFormContentOwner owner2 = UnE.View.Content.ViewUtils.GetContentViewOwner();
			szMediaPath = owner2.ResourcePath + "Media\\";
			szIconPath = szMediaPath + "icons\\화재.ico";

			InitializeComponent();

            Create3DView();

			mView2.Visible = false;
			mView1.Dock = DockStyle.Fill;
			mView1.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			MouseWheel += new MouseEventHandler(OnMouseWheel);

            CurrentMouseWorkMode = MouseWorkMode.ORBIT;

            mMainToolStripContainer.TopToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainToolStripContainer.LeftToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainToolStripContainer.RightToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainToolStripContainer.BottomToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);

            AddPythonFunction();

            mView1.CreateCustomView();
		}

		private void Create3DView()
		{
			m_LayerContainer = new SplitContainer();
			m_LayerContainer.Dock = DockStyle.Fill;
			m_LayerContainer.Visible = false;

			//Controls.Add(m_LayerContainer);

            mView1 = new BaseViewEx2(this);// new Core.BaseView();
			mView1.BackColor = System.Drawing.Color.Transparent;
			mView1.Dock = System.Windows.Forms.DockStyle.Fill;
			mView1.Location = new System.Drawing.Point(0, 0);
			mView1.Name = "m3DView1";

            mView1.MinimumSize = new System.Drawing.Size(640, 480);
			mView1.Size = new System.Drawing.Size(1900, 1040);
			mView1.TabIndex = 0;
			mView1.Click += new System.EventHandler(this.View1Click);


            mView2 = new ImageViewCtrl(this, m_BaseViewOwner);
            mView2.Dock = System.Windows.Forms.DockStyle.Fill;
            mView2.Location = new System.Drawing.Point(0, 0);
            mView2.Size = new System.Drawing.Size(1920, 1080);
            mView2.TabIndex = 0;
            mView2.Click += new System.EventHandler(this.View2Click);
                      

			mMainToolStripContainer.ContentPanel.Controls.Add(mView1);
            mMainToolStripContainer.ContentPanel.Controls.Add(mView2);

			m_layerOutside = new global::Core.LayerManager(mView1);

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

            mSceneManager = new global::Core.SceneManager(mView1);

            mVolmumeManagerOut = new global::Core.ZoneVolumeManager(mView1);
			//mVolmumeManagerIn = new Core.ZoneVolumeManager(mView2);

			mView1.ReadHomeView("Main");
		}

		private void FormContent_SizeChanged(object sender, EventArgs e)
		{
		}

		private void FormLayout_Resize(object sender, EventArgs e)
		{
		}

		private void FormContent_Shown(object sender, EventArgs e)
		{
			if (mView1 != null)
				mView1.ProcessCCTVLOD();
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
			mEngine.Init(szPath, szAppName);

			mViewList.Add(mView1);


			mView1.Size = new Size(1280, 1024);
			mView2.Size = new Size(1280, 1024);
			mCurrent = mView1;

			try
			{
				mView1.Popup = contextMenuStripManualReport;
				mView1.InitBaseView();
			}
			catch (System.Exception ex1)
			{
				Debug.WriteLine(ex1.StackTrace);
			}

			mViewList.Add(mView2);
			try
			{
				mView2.Popup = contextMenuStripManualReport;
				
			}
			catch (System.Exception ex2)
			{
				Debug.WriteLine(ex2.StackTrace);
			}

            mView1.CreateCompass(0.0f);
            mView2.CreateCompass(0.0f);

            bool bSimMode = UnE.SOP.ProxySOP.Instance.SimulationMode;

			//m_strOutDaeName = m_strZipFileFolderPath + "outside\\ND_0326l.DAE";
            m_strOutDaeName = m_strZipFileFolderPath + "\\yh20150424.scene";

            if (!File.Exists(m_strOutDaeName) || (bSimMode  || owner.ExtractOutside == true))
			{
				try
				{
                    if (File.Exists(m_strOutsideDAE))
                    {
                        ExtractToTrg(m_strOutsideDAE, m_strZipFileFolderPath + "\\");
                        //mView1.ExtractFile(m_strOutsideDAE, m_strZipFileFolderPath);
                    }
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.StackTrace);
				}
			}

			try
			{
				mView1.OpenMesh(m_strOutDaeName, false);
                //mView1.OnViewTop();
				mView1.OnViewFix("Main");
			}
			catch (System.Exception)
			{
			}

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

			mSceneManager.UpdateData();
			mView1.SetIconPOISize(32.0f, 32.0f);
			
            LayoutOutside();
			
            AddGroupName();
			AddBuildingName();
            Add3DText();
            
            //LoadPOIs();
            mView1.AddComponent(0, 0, 0);
            
            AddZoneVolume();
            AddSafeZoneVolume();

			mView1.UpdateWindow();
			Button b = new Button();
			b.Size = new Size(1, 1);
			mView1.Controls.Add(b);
			b.Show();

		}

		private void FormLayout_Load(object sender, EventArgs e)
		{

           
		}

		private bool ExtractToTrg(string strSrcFile, string strTrgPath)
		{
			try
			{
                if (Directory.Exists(strTrgPath))
                    UnE.Util.UtilMethods.DeleteFolder(strTrgPath);

               if (!Directory.Exists(strTrgPath))
                    Directory.CreateDirectory(strTrgPath);

                System.IO.Compression.ZipFile.ExtractToDirectory(strSrcFile, strTrgPath);                
            }
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
				return false;
			}

			return true;			
		}

		public void LoadPOIs()
		{
            IFormContentOwner owner = ViewUtils.GetContentViewOwner();
            owner.LoadPOI(mView1, false);

            //if (mView2.IsHandleCreated == true && mView2.Visible == true)

            //if(UnE.SOP.ProxySOP.Instance.Use2D == true)
            //    FormMain.Instance.DataManager.LoadPOI(mView2, true);

			CCTVManager.Instance.LoadEquipZoneCCTV();
		}

		private void FormLayout_FormClosed(object sender, FormClosedEventArgs e)
		{
			mEngine.EngineDispose();
		}

		public void View1Click(object sender, EventArgs e)
		{
			if (mView1 != null)
			{
				mCurrent = mView1;
				mCurrent.Focus();
			}
		}

		public void View2Click(object sender, EventArgs e)
		{
			mCurrent = null;
            mView2.Focus();
		}

		public void TopView()
		{
			if (mCurrent != null)
            {
                if (mCurrent == mView1)
                    mView1.OnViewTop();
                else
                {
                    mView2.FitView();
                    mView2.Refresh();
                }
            }
            else
            {
                mView2.FitView();
                mView2.Refresh();
            }
		}

		public void FrontViw()
		{
			//if (mCurrent != null)
            mView1.OnViewFront();
           
		}

		public void LeftView()
		{
			//if (mCurrent != null)
            mView1.OnViewLeft();
		}

		public void RightView()
		{
			//if (mCurrent != null)
                mView1.OnViewRight();
		}

		public void RearView()
		{
			//if (mCurrent != null)
            mView1.OnViewRear();
		}

		public void HomeView(string szName)
		{
			if (mCurrent != null)
			{
                if (mCurrent == mView1)
                    mView1.OnViewFix(szName);
                else
                {
                    mView2.FitView();
                    mView2.Refresh();
                }
			}
            else
            {
                mView2.FitView();
                mView2.Refresh();
            }
		}

		public void FitView()
		{
            if (mCurrent != null)
            {
                if( mCurrent == mView1)
                    mCurrent.OnViewFit();
                else
                {
                    mView2.FitView();
                    mView2.Refresh();
                }
            }
            else
            {
                mView2.FitView();
                mView2.Refresh();
            }
		}

        public void HideAllShelter()
        {
        
            //if( this.InvokeRequired == true )
            {
                Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
                formInvoke.Invoke((MethodInvoker)delegate
                {
                    if (mView1 != null)
                        mView1.HideAllShelter();

                    foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
                    {
                        Dictionary<int, Spatial.Shelter> dicShelters = ZoneManager.Instance.GetShelters(type);

                        if (dicShelters == null)
                            continue;

                        foreach (KeyValuePair<int, Spatial.Shelter> pair in dicShelters)
                        //foreach (KeyValuePair<int, Spatial.Shelter> pair in ZoneManager.Instance.DicSafeZones)
                        {
                            Spatial.Shelter zone = pair.Value;
                            if (zone != null)
                            {
                                string szID = "safe" + zone.ShelterName;
                                try
                                {
                                    ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                                    if (volume != null)
                                        volume.SetVisible(false);
                                }
                                catch (System.AccessViolationException ex)
                                {
                                    Debug.WriteLine(ex.Message + " " + szID);
                                }
                            }
                        }
                    }
                });
            }
            //else

            //{
            //    if (mView1 != null)
            //        mView1.HideAllShelter();
            //    foreach (KeyValuePair<int, Shelter> pair in ZoneManager.Instance.DicSafeZones)
            //    {
            //        Shelter zone = pair.Value;
            //        if (zone != null)
            //        {
            //            string szID = "safe" + zone.ShelterName;
            //            try
            //            {
            //                ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
            //                if (volume != null)
            //                    volume.SetVisible(false);
            //            }
            //            catch (System.AccessViolationException ex)
            //            {
            //                Debug.WriteLine(ex.Message + " " + szID);
            //            }
            //        }
            //    }
            //}
            
        }

        // nType : ShelterPath의 Type
        //         CoreAPI의 UBaseView::ShowPath(int nType)의 인자로 사용된다.
        // nShelterType : UnE.Spatial.Shelter.ShelterTypes(화재, 누출, 지진...)
        //                재난종류별 대피소를 각각 지정할 수 있도록 한다.
        public void ShowShelter(int nType, int nShelterType)
        {
            if( nType == 3 )
            {
                foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
                {
                    if (nShelterType != (int)type)
                        continue;

                    Dictionary<int, Spatial.Shelter> dicShelters = ZoneManager.Instance.GetShelters(type);

                    if (dicShelters == null)
                        continue;

                    foreach (KeyValuePair<int, Spatial.Shelter> pair in dicShelters)
                    //foreach (KeyValuePair<int, Spatial.Shelter> pair in ZoneManager.Instance.DicSafeZones)
                    {
                        Spatial.Shelter zone = pair.Value;
                        if (zone != null)
                        {
                            string szID = "safe" + zone.ShelterName;
                            try
                            {
                                ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                                volume.SetVisible(true);
                                mView1.ShowShelterPath(nType);
                            }
                            catch (System.AccessViolationException ex)
                            {
                                Debug.WriteLine(ex.Message + " " + szID);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
                {
                    if (nShelterType != (int)type)
                        continue;

                    Dictionary<int, Spatial.Shelter> dicShelters = ZoneManager.Instance.GetShelters(type);

                    if (dicShelters == null)
                        continue;

                    foreach (KeyValuePair<int, Spatial.Shelter> pair in dicShelters)
                    //foreach (KeyValuePair<int, Spatial.Shelter> pair in ZoneManager.Instance.DicSafeZones)
                    {
                        Spatial.Shelter zone = pair.Value;
                        if (zone != null && nType == zone.ID)
                        {
                            string szID = "safe" + zone.ShelterName;
                            try
                            {
                                ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                                volume.SetVisible(true);
                                mView1.ShowShelterPath(nType);
                            }
                            catch (System.AccessViolationException ex)
                            {
                                Debug.WriteLine(ex.Message + " " + szID);
                            }
                        }
                    }
                }
            }
            if (mView1 != null)
                mView1.Refresh();
        }

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
			if (mCurrent != null)
			{
				mCurrent.OnMouseWheel(e.X, e.Y, e.Delta);
			}
            else
            {

                mView2.OnMouseWheel(sender, e);

            }
		}


        private bool m_bShowCollapse = false;
        public void ShowBuildingCollapse(string szBuildingID, string szDisplayName)
        {
            if (m_bShowCollapse == true)
                return;

            m_bShowCollapse = true;
            
            if( mView1 != null)
            {
                Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
                formInvoke.Invoke((MethodInvoker)delegate
                {
                    //ZoomBuilding(szBuildingID);
                    mView1.OnViewFix("Custom1");

                    global::Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szBuildingID);
                    if (volume != null)
                    {
                        mCurrentOutdoorVolume = volume;
                        volume.SetVisible(true);
                    }

                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    owner.SetBuilingCollapseDetect(szDisplayName, true);

                    mView1.RedrawScene();
                });   
            }
        }

        public void CloseBuilingCollapse(string szBuildingID)
        {
            global::Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szBuildingID);
            if (volume != null)
            {
                mCurrentOutdoorVolume = volume;
                volume.SetVisible(false);
            }
        }

        public void SelectBuilding(string strBuildingID)
        {
        }

        public bool EarthquakeEventIsFinished()
        {
            if (mView1 == null)
                return true;

            return mView1.EarthquakeMotionIsFinished();
        }

        public void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {
            if (mView1 != null)
            {
                mView1.InitEarthquakeMotion();

                Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
                formInvoke.Invoke((MethodInvoker)delegate
                {

                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    owner.SetEarthquakeDetect(nIntensity, fMagnitude, strPosition, isRealMode);

                    DBUtility.Utility util = new DBUtility.Utility();
                    string strTime = util.getinivalue("SDMS", "earthquake_time");
                    string strScale = util.getinivalue("SDMS", "earthquake_scale");
                    string strColorTime = util.getinivalue("SDMS", "earthquake_colorChange");
                    int nTime = int.Parse(strTime);
                    int nScale = int.Parse(strScale);
                    int nColotTime = int.Parse(strColorTime);

                    m_bShowCollapse = false;

                    mView1.OnViewFix("Main");

                    mView1.SetEarthquakeMotion(nTime, nScale, nColotTime);

                });
                //mView1.SetTempMaterial(255, 0, 0, 255);
            }
        }

		public void ZoomIn()
		{
			if (mCurrent != null)
			{
				mCurrent.OnMouseWheel(0, 0, 2400);
				//mCurrent.UpdatePOI();
			}
            else
            {
                mView2.ZoomIn();
            }
		}

		public void ZoomOut()
		{
			if (mCurrent != null)
			{
				mCurrent.OnMouseWheel(0, 0, -2400);
				//mCurrent.UpdatePOI();
			}
            else
            {
                mView2.ZoomOut();
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

#if SAFE_KOREA_YH_2017
            // 영흥 안전한국 훈련용 코드 2017월 11-2일 이후는 필요 없으므로 제거할것.
            if (szCode == "yhz85")
            {
                mView1.OnViewFix("Custom1");
            }
            else if(szCode.StartsWith("user1"))
            {
                mView1.OnViewFix("Custom2");
            }
            else if( szCode == "yhz215")
            {
                mView1.OnViewFix2("Custom1");

            }
            else
#endif
                mView1.ZoomObject(szCode);
        }

		public void ZoomTarget(float x, float y, float z, bool isIndoor)
		{
			y = 0.0f;
			if (isIndoor)
			{
				//if (mView2.MeshOpened)
				{
					//mView2.OnViewTop();
					//mView2.ZoomTarget(new Position3D(x, y, z), 20.0f);
					//mView2.Update();
				}
			}
			else
			{
				//if (mView1 != null)
				mView1.OnViewTop(false);
				mView1.ZoomTarget(new Position3D(x, y, z), 20.0f);
				mView1.Update();
				//mView1.Refresh();
			}
		}

		public void SelectPOI(POI poi, bool isIndoor)
		{
			if (isIndoor)
			{
				//if (mView2.MeshOpened)
				//	mView2.SelectPOI(poi.ID);
			}
			else
			{
				//mView1.SelectPOI(poi.ID);
			}
		}

        public void SelectPOILoadZone(POI poi, bool isIndoor)
        {
            if (isIndoor)
            {
                //SetCurrentBuilding(poi.Zone.Building, poi.Zone);
                //if (mView2.MeshOpened)
                    mView2.SelectPOI(poi.ID);
            }
            else
            {
                mView1.SelectPOI(poi.ID, poi.Type.ToString());
            }
        }

        private static int m_nImageNum = 1;
        public string SaveToTempImage()
        {
            
            string szPath1 = System.IO.Path.GetTempPath() + "view1"+ m_nImageNum + ".bmp";
            string szPath2 = System.IO.Path.GetTempPath() + "view2"+ m_nImageNum + ".bmp";
            mView1.SaveScreen(szPath1);
            mView2.SaveScreen(szPath2, false);
            m_nImageNum++;

            if (m_nImageNum == 1000)
                m_nImageNum = 1;
            return szPath1;
        }

		public void SaveToImage()
		{
            //string szResult = "";
			SaveFileDialog dlg = new SaveFileDialog();

			dlg.Filter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";
			string defaultName = "Untitled";
			dlg.FileName = defaultName;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string szPath = dlg.FileName;
                //szResult = szPath;
				switch (m_nLayout)
				{
					case 1:
						mView1.SaveScreen(szPath);
						break;

					case 2:
						if (mCurrent != null)
							mCurrent.SaveScreen(szPath);
						break;

					case 3:
						mView2.SaveScreen(szPath);
						break;

					default:
						break;
				};
			}
            //return szResult;
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

			if (!m_LayerContainer.Panel2.Controls.Contains(mView2))
			{
                mMainToolStripContainer.ContentPanel.Controls.Remove(mView2);
				m_LayerContainer.Panel2.Controls.Add(mView2);
			}

            mView2.Dock = DockStyle.Fill;
            mView2.Visible = true;
            mView2.BringToFront();
            mView2.FitView();
            mView2.Invalidate(true);

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
			if (m_LayerContainer.Panel1.Controls.Contains(mView1))
			{
				m_LayerContainer.Panel1.Controls.Remove(mView1);
                mMainToolStripContainer.ContentPanel.Controls.Add(mView1);
			}

			if (m_LayerContainer.Panel2.Controls.Contains(mView2))
			{
				m_LayerContainer.Panel2.Controls.Remove(mView2);
                mMainToolStripContainer.ContentPanel.Controls.Add(mView2);
			}
            if (mMainToolStripContainer.ContentPanel.Controls.Contains(m_LayerContainer))
                mMainToolStripContainer.ContentPanel.Controls.Remove(m_LayerContainer);


			mView1.Visible = false;

			mView2.Dock = DockStyle.Fill;
            mView2.Visible = true;
            mView2.BringToFront();

            mView2.FitView();
			mView2.Invalidate(true);

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

			if (m_LayerContainer.Panel2.Controls.Contains(mView2))
			{
				m_LayerContainer.Panel2.Controls.Remove(mView2);
                mMainToolStripContainer.ContentPanel.Controls.Add(mView2);
			}

            if (mMainToolStripContainer.ContentPanel.Controls.Contains(m_LayerContainer))
                mMainToolStripContainer.ContentPanel.Controls.Remove(m_LayerContainer);

			mCurrent = mView1;

			mView2.Visible = false;

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

            private ContentOwnerTab mTabNumber = ContentOwnerTab.M3D_TAB;
            public ContentOwnerTab TabNumber
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

            state.Zone = mView2.CurrentZone;
            state.ImagePath = mView2.ImagePath;    

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

            mView1.SaveViewState(szPushViewStat);
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

                mView1.LoadViewState(szPushViewStat);

                if( state.Zone != null && state.ImagePath != "")
                {
                    mView2.SetImage(state.ImagePath, state.Zone);
                }

                ContentOwnerTab tab = state.TabNumber;

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
			if (mView1 != null)
			{
				Dictionary<int, Zone> m_dicBuildingGroup = ZoneManager.Instance.DicOutdoorZones;
				foreach (KeyValuePair<int, Zone> kv in m_dicBuildingGroup)
				{
					Zone zone = kv.Value;
					try
					{
                        global::Core.ZonePolygon area = new global::Core.ZonePolygon(mView1);
						int count = zone.Polygon.GetVertexCount();
						for (int i = 0; i < count; i++)
						{
							UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx) / 1000.0f;
							float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y) / 1000.0f;

                            
							area.AddVertex(new Position3D(pos3DX, 0, pos3DZ));
						}
						area.Height = 0;
						area.CreatePolygon();
                        global::Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, 20, zone.Building.BroadcastName);
						if (volume != null)
							volume.SetVisible(false);
						Vertex2D pos2 = zone.Polygon.CalcWeightCenter();
						float pos3DX2 = ((float)pos2.x - ZoneManager.Instance.Dx);
						float pos3DZ2 = ZoneManager.Instance.Dy - (float)pos2.y;
                        pos3DX2 /= 1000.0f;
                        pos3DZ2 /= 1000.0f;
						string szName = string.Format("{0} [{1}]", zone.ZoneName, zone.DXFFileName);
						int nID = mView1.AddZoneName(szName, pos3DX2, 20.0f, pos3DZ2);
					}
					catch (System.Exception)
					{
					}
				}
			}
		}

		public void AddZoneName(Zone zone)
		{
			if (mView2 != null)
			{
				try
				{
					string szName = string.Format("{0} [{1}]", zone.ZoneName, m_szFileName);
					mView2.AddZoneName(szName);
					//m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
				}
				catch (System.Exception ex)
				{
                    System.Diagnostics.Trace.WriteLine(ex.Source);
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
				}
			}
		}

		public void AddGroupName()
		{
			if (mView1 != null)
			{
				Dictionary<int, BuildingGroup> m_dicBuildingGroup = ZoneManager.Instance.DicBuildingGroup;
				foreach (KeyValuePair<int, BuildingGroup> kv in m_dicBuildingGroup)
				{
					BuildingGroup obj = kv.Value;

                    if (obj.DisplayName.Trim().Length == 0)
                        continue;

					try
					{
						float pos3DX = (obj.TextCenterX - ZoneManager.Instance.Dx) / 1000.0f;
						float pos3DZ = (ZoneManager.Instance.Dy - obj.TextCenterY) / 1000.0f;

                        int nID = mView1.AddGroupName(obj.DisplayName, pos3DX, 100.0f, pos3DZ);
						//int nID = mView1.AddGroupName(obj.BuildingGroupName, pos3DX, 100.0f, pos3DZ);
						//m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
					}
					catch (System.Exception)
					{
					}
				}
			}
		}

		public void ShowZoneVolume(int zoneID, bool bOutDoorWnd, bool bShow)
		{
			Zone zone = ZoneManager.Instance.GetZone(zoneID);
			if (zone == null)
				return;// null;

			if (bOutDoorWnd == true)
			{
				string szID = "";
				if (zone.IsOutdoor == false)
				{
					szID = zone.Building.BuildingID;
				}
				else
				{
					szID = zone.ZoneName;
				}
                global::Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
				mCurrentOutdoorVolume = volume;
				volume.SetVisible(bShow);
				//return volume;
			}
			else
			{
				if (zone.IsOutdoor == false)
				{
					m_bChangeIndoor = false;
					string szID = zone.ZoneName;

                    mView2.ShowZone(zone, bShow);

				}
			}

		}

     

		public void ShowZoneVolume(int zoneID, int nEquipZoneID, bool bOutDoorWnd, bool bShow)
		{
			Zone zone = ZoneManager.Instance.GetZone(zoneID);
			if (zone == null)
				return;


			if (bOutDoorWnd == true)
			{
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                if (equipZone != null)
                {
                    int nCount = equipZone.LinkedZoneList.Count;
                    if (equipZone.LinkedZoneList.Count > 1)
                    {
                        for (int i = 0; i < nCount; i++)
                        {
                            string szLinkID = "";
                            Zone linkedZone = (Zone)equipZone.LinkedZoneList[i];
                            if (zone.IsOutdoor == false)
                            {
                                szLinkID = zone.Building.BuildingID;
                            }
                            else
                            {
                                szLinkID = zone.ZoneName;
                            }
                            global::Core.ZoneVolume lvolume = mVolmumeManagerOut.FindZoneVolume(szLinkID);
                            lvolume.SetVisible(bShow);
                        }
                    }
                }

				string szID = "";
				if (zone.IsOutdoor == false && zone.Building.BuildingID != "yhNONE")
				{
					szID = zone.Building.BuildingID;
				}
				else
				{
					szID = zone.ZoneName;
				}
                global::Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                if (volume != null)
                {
                    mCurrentOutdoorVolume = volume;
                    volume.SetVisible(bShow);
                }				
                else
                {
                    //mVolmumeManagerOut.SetVisibleAll(true);
                    int i = 0;
                    i++;
                }
				return;
			}
			else
			{
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                if (equipZone != null)
                {
                    if (equipZone.IsOutdoor == false)
                    {
                        m_bChangeIndoor = false;
                        string szID = equipZone.ZoneName;
                        mView2.ShowEquipmentZone(equipZone, bShow);
                    }
                }
			}
			return;
		}

		private bool m_bChangeIndoor = false;

		public void HideZoneVolume()
		{
			//if (m_bChangeIndoor == false && mCurrentIndoorVolume != null)
			//	mCurrentIndoorVolume.SetVisible(false);
			if (mCurrentOutdoorVolume != null)
				mCurrentOutdoorVolume.SetVisible(false);

            // Shelter 볼륨의 상태를 저장한다.
            ArrayList arList = new ArrayList();

            foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
            {
                Dictionary<int, Spatial.Shelter> dicShelters = ZoneManager.Instance.GetShelters(type);

                if (dicShelters == null)
                    continue;

                foreach (KeyValuePair<int, Spatial.Shelter> pair in dicShelters)
                //foreach (KeyValuePair<int, Spatial.Shelter> pair in ZoneManager.Instance.DicSafeZones)
                {
                    Spatial.Shelter zone = pair.Value;
                    if (zone != null)
                    {
                        string szID = "safe" + zone.ShelterName;
                        try
                        {
                            ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);

                            if (volume == null)
                                continue;

                            arList.Add(volume.GetVisible());
                        }
                        catch (System.AccessViolationException ex)
                        {
                            Debug.WriteLine(ex.Message + " " + szID);
                        }
                    }
                }
            }

			mVolmumeManagerOut.SetVisibleAll(false);
			//mVolmumeManagerIn.SetVisibleAll(false);


            // Shelter 볼륨의 상태를 복구한다.
            int nCount = 0;

            foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
            {
                Dictionary<int, Spatial.Shelter> dicShelters = ZoneManager.Instance.GetShelters(type);

                if (dicShelters == null)
                    continue;

                foreach (KeyValuePair<int, Spatial.Shelter> pair in dicShelters)
                //foreach (KeyValuePair<int, Spatial.Shelter> pair in ZoneManager.Instance.DicSafeZones)
                {
                    Spatial.Shelter zone = pair.Value;
                    if (zone != null)
                    {
                        string szID = "safe" + zone.ShelterName;
                        try
                        {
                            ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);

                            if (volume == null)
                                continue;

                            bool bShow = (bool)arList[nCount++];
                            volume.SetVisible(bShow);
                        }
                        catch (System.AccessViolationException ex)
                        {
                            Debug.WriteLine(ex.Message + " " + szID);
                        }
                    }
                }
            }

			m_bChangeIndoor = false;
		}

        private global::Core.ZoneVolume mCurrentOutdoorVolume = null;
        private global::Core.ZoneVolume mCurrentIndoorVolume = null;

        public void HidePollutioinView()
        {
            //3D 광교 오염물질 전용
        }
        public void AddSafeZoneVolume()
        {
            ArrayList arBuildings = new ArrayList();

            foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
            {
                Dictionary<int, Spatial.Shelter> dicShelters = ZoneManager.Instance.GetShelters(type);

                if (dicShelters == null)
                    continue;

                foreach (KeyValuePair<int, Spatial.Shelter> pair in dicShelters)
                //foreach (KeyValuePair<int, Spatial.Shelter> pair in ZoneManager.Instance.DicSafeZones)
                {
                    Spatial.Shelter zone = pair.Value;

                    if (zone != null)
                    {
                        int nBoundaryCount = zone.Boundaries.Count;

                        for (int j = 0; j < nBoundaryCount; j++)
                        {
                            Polygon boundary = zone.Boundaries[j];

                            string szID = "safe" + zone.ShelterName + string.Format("{0:000}", j);

                            if (arBuildings.Contains(szID))
                                continue;
                            arBuildings.Add(szID);
                            float fHeight1 = 0.1f;
                            float fHeight2 = 40.0f;
                            if (boundary == null)
                                continue;

                            global::Core.ZonePolygon area = new global::Core.ZonePolygon(mView1);
                            int count = boundary.GetVertexCount();

                            if (count == 0)
                                continue;

                            for (int i = 0; i < count; i++)
                            {
                                UnE.Geometry.Vertex2D pos = boundary.GetVertex(i);
                                float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
                                float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
                                pos3DX /= 1000;
                                pos3DZ /= 1000;
                                area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
                            }
                            area.Height = fHeight1;
                            area.CreatePolygon();
                            try
                            {
                                global::Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID, false, Color.Blue);
                                volume.SetVisible(false);

                            }
                            catch (System.AccessViolationException ex)
                            {
                                Debug.WriteLine(ex.Message + " " + szID);
                            }
                        }
                    }
                }
            }
        }

		public void AddZoneVolume()
		{
			ArrayList arBuildings = new ArrayList();
			foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicZones)
			{
				Zone zone = pair.Value;

				if (zone != null)
				{
                    if (zone.Building != null && zone.Building.BuildingID != "yhNONE")
					{
						string szID = zone.Building.BuildingID;
						if (arBuildings.Contains(szID))
							continue;
						arBuildings.Add(szID);

                        string szTempID = szID.Replace("_1", "");
                        global::Core.Scene scene = mSceneManager.FindSceneNode(szTempID);
						if (scene != null)
						{
							float fHeight1 = scene.GetMinimum().Y - 0.1f;
							float fHeight2 = scene.GetMaximum().Y + 0.1f;
                            
                            if (szID.StartsWith("user14"))
                            {
                                fHeight1 = 0.1f;
                               // fHeight2 = 21.0f;
                            }

                            global::Core.ZonePolygon area = new global::Core.ZonePolygon(mView1);
                            if (zone.Polygon == null)
                                continue;

							int count = zone.Polygon.GetVertexCount();
							for (int i = 0; i < count; i++)
							{
								UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
								float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
								float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);

                                pos3DX /= 1000;
                                pos3DZ /= 1000;
								area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
							}
							area.Height = fHeight1;
							area.CreatePolygon();
                            global::Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID);
							volume.SetVisible(false);
						}                       
					}
					else
					{
						string szID = zone.ZoneName;
						if (arBuildings.Contains(szID))
							continue;
						arBuildings.Add(szID);
						float fHeight1 = 0.1f;
						float fHeight2 = 40.0f;

                        if (zone.Polygon == null)
                            continue;

                        global::Core.ZonePolygon area = new global::Core.ZonePolygon(mView1);
						int count = zone.Polygon.GetVertexCount();

                        if (count == 0)
                            continue;

						for (int i = 0; i < count; i++)
						{
							UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
							float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
                            pos3DX /= 1000;
                            pos3DZ /= 1000;
							area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
						}
						area.Height = fHeight1;
						area.CreatePolygon();
						try
						{
                            global::Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID);
							volume.SetVisible(false);
						}
						catch (System.AccessViolationException ex)
						{
							Debug.WriteLine(ex.Message + " " + szID);
						}
					}
				}
			}
		}

		public void AddBuildingName()
		{
			if (mView1 != null)
			{
				Dictionary<int, Building> m_dicBuildings = ZoneManager.Instance.DicBuildings;
				foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
				{
					Building obj = kv.Value;

                    if (obj.DisplayText.Trim().Length == 0)
                        continue;

					try
					{
                        mView1.SetTextLODDist(100.0f);
                        mView1.SetTextColor(128 / 255.0f, 255 / 255.0f, 128 / 255.0f);
                        mView1.SetTextHeight(15.0f);
                        //1호기 1525135.1305542,323881.536591215
                        //2호기 1605421.10046387,323397.886170073
                        //3호기 1714242.4936676,318803.211456938
                        //4호기 1798639.47862244,317594.137299223
                        //5호기 219042.076202393,366377.324692412
                        //6호기 123506.272125244,365183.117386503
                        if( obj.BuildingID == "yhz1")
                        {
                            int nID = mView1.AddGroupName(obj.DisplayText, 1525.135f, 39.0f, -323.881f);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
                        else if(obj.BuildingID == "yhz1_1")
                        {
                            int nID = mView1.AddGroupName(obj.DisplayText, 1605.421f, 39.0f, -323.397f);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
                        else if (obj.BuildingID == "yhz2")
                        {
                            int nID = mView1.AddGroupName(obj.DisplayText, 1714.242f, 39.0f, -318.803f);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
                        else if (obj.BuildingID == "yhz2_1")
                        {
                            int nID = mView1.AddGroupName(obj.DisplayText, 1798.639f, 39.0f, -317.594f);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
                        else if (obj.BuildingID == "yhz3")
                        {
                            int nID = mView1.AddGroupName(obj.DisplayText, 219.042f, 39.0f, -366.377f);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
                        else if (obj.BuildingID == "yhz3_1")
                        {
                            int nID = mView1.AddGroupName(obj.DisplayText, 123.506f, 39.0f, -365.183f);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
                        else
                        {
                            int nID = mView1.AddAliasName(obj.BuildingID, obj.DisplayText);
                            m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                        }
					}
					catch (System.Exception)
					{
					}
				}
			}
		}

        public void Add3DText()
        {
            if (mView1 != null)
            {
                foreach (_3DText text in ZoneManager.Instance._3DTextList)
                {
                    if (text.DisplayText.Trim().Length == 0)
                        continue;

                    try
                    {
                        if (text.TextColor == null)
                            mView1.SetTextColor(128 / 255.0f, 255 / 255.0f, 128 / 255.0f);
                        else
                        {
                            Color textColor = text.TextColor.Data;
                            mView1.SetTextColor(textColor.R / 255.0f, textColor.G / 255.0f, textColor.B / 255.0f);
                        }

                        if (text.TextFontHeight == null)
                            mView1.SetTextHeight(15.0f);
                        else
                            mView1.SetTextHeight(text.TextFontHeight.Data);

                        float pos3DX = (text.TextCenterX - ZoneManager.Instance.Dx) / 1000.0f;
                        float pos3DZ = (ZoneManager.Instance.Dy - text.TextCenterY) / 1000.0f;

                        int nID = mView1.AddGroupName(text.DisplayText, pos3DX, 100.0f, pos3DZ);
                        m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
        }

		public void RedrawWindow()
		{
			if (mView1 != null && mView1.Visible == true)
			{
				mView1.RedrawScene();
			}
			if (mView2 != null && mView2.Visible == true)
			{
				mView2.Refresh();
			}
		}

		public void Invalidate3DView(bool bErBack)
		{
            
			if (mView1 != null && mView1.Visible == true)
			{
                mView1.RedrawScene();
			}
			if (mView2 != null && mView2.Visible == true)
			{
				mView2.Invalidate(bErBack);
			}
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
						mView2.ClearAllData();
						m_bLoadInsideMode = false;
					}
				}
				catch (System.Exception)
				{
				}

				bool bExist = File.Exists(szInsideFullPath);
				if (bExist == false || bExtractInside == false)
				{
					mView2.Refresh();
					szPrevFileName = "";
					return;
				}

				m_nCurrentFloor = floor.FloorIndex;
				m_bChangeIndoor = true;
               
                mView2.SetImage(szInsideFullPath, m_currentIndoorZone);
                // 테스트
                //AddZoneName(m_currentIndoorZone);

                mView2.FitView();

                m_bLoadInsideMode = true;

                // 테스트
                //ShowZoneVolume(m_currentIndoorZone.ID, false, true);

                mView2.Refresh();

                GC.Collect();

			}
		}

		public void OpenModel()
		{
			if (szInsideFullPath != null)
			{           
                mView2.SetImage(szInsideFullPath, m_currentIndoorZone);  
				// 테스트
				//AddZoneName(m_currentIndoorZone);

                mView2.FitView();

				m_bLoadInsideMode = true;

				// 테스트
				//ShowZoneVolume(m_currentIndoorZone.ID, false, true);

                mView2.Refresh();

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

			mView1.ShowLayer(id, bShow);
			//mView2.ShowLayer(id, bShow);

			mView1.UpdatePOI();
			//mView2.UpdatePOI();

			RedrawWindow();

			return false;
		}

        public void AttachView(System.Windows.Forms.Control view, bool isOutdoor)
		{
			if (isOutdoor)
				mView1 = (BaseViewEx2)view;
			else
				mView2 = (ImageViewCtrl)view;

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
				view = mView2;
			}

            mMainToolStripContainer.ContentPanel.Controls.Remove(view);

			return view;
		}

		public void ClearPOISelection()
		{
			if (m_nLayout == 1)
			{
				mView1.ClearPOISelection();
			}
			else if (m_nLayout == 2)
			{
				mView1.ClearPOISelection();
				mView2.ClearPOISelection();
			}
			else if (m_nLayout == 3)
			{
				mView2.ClearPOISelection();
			}
		}

		public void HideAllPOIPopup()
		{
			if (m_nLayout == 1)
			{
				mView1.HideAllPOIPopup();
			}
			else if (m_nLayout == 2)
			{
				mView1.HideAllPOIPopup();
				mView2.HideAllPOIPopup();
			}
			else if (m_nLayout == 3)
			{
				mView2.HideAllPOIPopup();
			}
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

                     EquipmentZone equipZone = (EquipmentZone)arEquipzone[0];
                     UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                     owner.ShowEquipZoneCCTVs(equipZone.ID);
                 }

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

			BaseView view1 = (BaseView)mView1;
			view1.SetCheckPoistion(mCheckPosition);
            			
            mView2.SetCheckPoistion(mCheckPosition);

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
				BaseView view2 = (BaseView)mView1;
				//BaseView view1 = (BaseView)mView2;
                mView2.SetCheckPoistion(mCheckPosition);
				view2.SetCheckPoistion(mCheckPosition);
				if (m_nLayout == 3)
				{
					if (bResult == true)
					{
						// view2.AddPOI(szIconPath);
						if (LastPos != null)
						{
							int nID = view2.AddPOI(szIconPath, LastPos.X, LastPos.Y, LastPos.Z);
							LastPos.IconID = nID;
						}
						else
						{
							view2.AddPOI(szIconPath);
							//LastPos.IconID = nID;
						}
						view2.UpdateWindow();
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
			BaseView view1 = (BaseView)mView1;
			if (view1 != null)
			{
				if (LastPos != null)
                {
                //    float dx = 120894.0548f + 1008.531f;
                //    float dy = 157659.0963f - 506.251f;
                    float ox = LastPos.X;
                    float oz = - LastPos.Z;
					int nID = LastPos.IconID;
					if (nID != -1)
					{
						view1.RemovePOI(nID);
					}
					else
					{
						view1.RemovePOI(ox, LastPos.Y, oz);
					}
					view1.UpdateWindow();
				}
			}

            if (mView2 != null)
			{
				if (LastPos != null)
				{
					int nID = LastPos.IconID;
					if (nID != -1)
					{
						view1.RemovePOI(nID);
					}
					else
					{
                        //mView2.RemovePOI(LastPos.X, LastPos.Y);
					}
                    mView2.Refresh();
				}
			}
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

                if (mView2 != null && zone != null && zone.Polygon != null)
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
				BaseView view1 = (BaseView)mView1;
				if (view1 != null)
				{
					//float dx = 120894.0548f + 1008.531f;
					//float dy = 157659.0963f - 506.251f;
                    float ox = x;
					float oz = - z;
					int nID = view1.AddPOI(path, ox, y, oz);
					LastPos.IconID = nID;
					view1.UpdateWindow();
				}
				RedrawWindow();
			}
		}

		public UnE.SOP.HistoryDisasterPosition GetLastDisasterPosition()
		{
			return mLastPos;
		}

		public void SetDisasterPos_Click(object sender, EventArgs e)
		{
			if (m_nLayout == 3)
			{
				if (mCurrent != null && mFormPosition != null)
				{
					mCurrent.ClearSelect();
					string szSelectedName = mCurrent.OnSelect();

					if (szSelectedName == null || szSelectedName.Equals(""))
					{
						return;
					}

					Position3D pos3D = mCurrent.OnPosition();

					mCurrent.ClearSelect();

					szSelectedName = m_buildingCurrent.DisplayText;
                    string szBrName = m_buildingCurrent.BroadcastName;

					int nIdx = szSelectedName.IndexOf('*');
					if (nIdx != -1)
					{
						szSelectedName = szSelectedName.Substring(0, nIdx);
					}

                    nIdx = szBrName.IndexOf('*');
					if (nIdx != -1)
					{
						szBrName = szBrName.Substring(0, nIdx);
					}

					string szResult = null;
                    string szBroadcastName = "";
					if (m_nCurrentFloor < 0)
					{
						szResult = string.Format("{0} B{1}층", szSelectedName, System.Math.Abs(m_nCurrentFloor));
                        szBroadcastName = string.Format("{0} B{1}층", szBrName, System.Math.Abs(m_nCurrentFloor));
					}
					else
					{
						szResult = string.Format("{0} {1}층", szSelectedName, m_nCurrentFloor);
                        szBroadcastName = string.Format("{0} B{1}층", szBrName, System.Math.Abs(m_nCurrentFloor));
					}

					if (mFormPosition != null && mFormPosition.IsHandleCreated())
					{
						mLastPos = new HistoryDisasterPosition();
						mLastPos.PoistionName = szResult;

						mLastPos.X = pos3D.X;
						mLastPos.Y = pos3D.Y;
						mLastPos.Z = pos3D.Z;
						mLastPos.FloorIndex = m_nCurrentFloor;
                        mLastPos.BroadcastName = szBroadcastName;

						Form form = mFormPosition.GetInvokeForm();
						form.Invoke((MethodInvoker)delegate
						{
							mFormPosition.PositionName = szResult;
						});

						form.Invoke((MethodInvoker)delegate
						{
							mLastPos.DisasterName = mFormPosition.DisasterName;
						});
						mLastPos.BuildingID = m_buildingCurrent.BuildingID;
						form.Invoke((MethodInvoker)delegate
						{
							mFormPosition.AddLastHistoryDisasterPoistion(mLastPos);
						});
					}
				}
			}
			else
			{
				if (mCurrent != null && mFormPosition != null)
				{
					float ox = 1008.531f;
					float oy = 506.251f;
					// 120894.0548, Y:157659.0963
					float dx = 120894.0548f + ox;
					float dy = 157659.0963f - oy;

					//- X -> -1008.531 , Y-> 506.251 이동입니다.[14:12:07]
					// X : 118366.4117, Y:158297.2820
					// dx = 119374.9427   , dy = 157791.031;

					mCurrent.ClearSelect();
					string szSelectedName = mCurrent.OnSelect();
					Position3D pos3D = mCurrent.OnPosition();

					mCurrent.ClearSelect();

                    string szBroadcastName = "";

					bool isBuildingName = false;
					Building curBuilding = null;
					if (szSelectedName != null && szSelectedName != "")
					{
						curBuilding = ZoneManager.Instance.GetBuilding(szSelectedName);
						if (curBuilding != null)
						{
							szSelectedName = curBuilding.DisplayText;
                            szBroadcastName = curBuilding.BroadcastName;
							isBuildingName = true;
						}
					}

					if (isBuildingName == false)
					{
                        szBroadcastName = ZoneManager.Instance.CheckZoneBroadcastName(pos3D.X, pos3D.Z);
						string szName = ZoneManager.Instance.CheckZoneName(pos3D.X, pos3D.Z);
						if (szName != "")
						{
							szSelectedName = szName;                            
						}
						else
						{
							szSelectedName = "";
							return;
						}
					}

					pos3D.X = (pos3D.X + dx);
					pos3D.Z = dy - pos3D.Z;

					if (mFormPosition != null && mFormPosition.IsHandleCreated())
					{
						mLastPos = new HistoryDisasterPosition();
						mLastPos.PoistionName = szSelectedName;

						mLastPos.X = pos3D.X;
						mLastPos.Y = pos3D.Y;
						mLastPos.Z = pos3D.Z;
						mLastPos.FloorIndex = -999;
                        mLastPos.BroadcastName = szBroadcastName;

						if (isBuildingName == true)
							mLastPos.BuildingID = curBuilding.BuildingID;
						else
							mLastPos.BuildingID = "ZONE";

						Form form = mFormPosition.GetInvokeForm();
						form.Invoke((MethodInvoker)delegate
						{
							mFormPosition.PositionName = szSelectedName;
						});

						form.Invoke((MethodInvoker)delegate
						{
							mLastPos.DisasterName = mFormPosition.DisasterName;
						});

						form.Invoke((MethodInvoker)delegate
						{
							mFormPosition.AddLastHistoryDisasterPoistion(mLastPos);
						});
					}
				}
			}
		}		

        public void ToggleBuildingTextLayer()
        {
            m_bLODText = !m_bLODText;
            mView1.SetTextLOD(m_bLODText);
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

        private void mMainToolStripContainer_ContentPanel_Load(object sender, EventArgs e)
        {

        }
	}
}