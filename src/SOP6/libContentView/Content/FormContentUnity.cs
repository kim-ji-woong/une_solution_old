using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using UnE.Geometry;
using UnE.SOP;
using UnE.SOP.Workstate;
using UnE.Util.Unity;
using UnE.Spatial;
using UnE.Sensor;
using UnE.View.Content;
using SDMS;
using DBUtility2;

namespace UnE.View.Content
{
    public partial class FormContentUnity : Form, IDisasterContainer, IFormContent
    {
        // 1(Outside), 2(Both), 3(Inside)
        private int m_nLayout = 1;

        public int NumLayout
        {
            get { return m_nLayout; }
            set { SetLayoutMode(value); }
        }

        private UnE.Util.Unity.LayerManager m_layerOutside = null;

        public ILayerManager Layers
        {
            get { return m_layerOutside; }
        }

        private UnE.Util.Unity.Panel4Unity mView1 = null;
        private UnE.Util.Unity.Panel4Unity mView2 = null;

       // private Core.Engine mEngine = new Core.Engine();

        private string m_strZipFileFolderPath = "";
        private string m_strOutsideDAE = "";
        private string m_strInsideDAE = "";
        private Building m_buildingCurrent = null;
        private Dictionary<string, string> m_dicInsideDAE = null;
        private string m_strOutDaeName = "";

        private ArrayList mViewList = new ArrayList();
        private UnE.Util.Unity.Panel4Unity mCurrent = null;
        private string szIconPath = "";
        private string szMediaPath = "";

        private bool bExtractInside = false;
        private bool m_bLoadInsideMode = false;

        private string m_strUnityExePath = "";
        private string m_strUnityWindowName = "";

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

        private bool m_bEditMode = false;

        public bool EditMode
        {
            get { return m_bEditMode; }
            set
            {
                m_bEditMode = value;
                mView1.EditMode = value;
                mView2.EditMode = value;
            }
        }

        public bool BlinkMode
        {
            set
            {
                mView1.BlinkMode = value;
                mView2.BlinkMode = value;
            }
        }

        private SplitContainer m_LayerContainer = null;

        private MouseWorkMode mCurrentMouseWorkMode;
        public MouseWorkMode CurrentMouseWorkMode
        {
            get { return mCurrentMouseWorkMode; }
            set
            {
                mCurrentMouseWorkMode = value;
                mView1.CurrentMouseWorkMode = value;
                mView2.CurrentMouseWorkMode = value;
            }
        }

        private SortedList<string, ToolStripMenuItem> m_MenuList = new SortedList<string, ToolStripMenuItem>();
        public ToolStripMenuItem GetMenu(string szName)
        {
            if (szName == "ManualReport")
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
            if (m_MenuList.ContainsKey(szName))
            {
                m_MenuList.Remove(szName);
            }
            m_MenuList.Add(szName, menu);
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

        private string m_strSimulationBroadcastResultFilePath = "";

        // 연습모드용 방송이 끝나기를 기다리는 Timer의 최대 대기시간(10분)
        private int m_nSimulationBroadcastTimerWaitTime = 600;

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

        public FormContentUnity(IBaseViewOwner owner, string strUnityExePath, string strUnityWindowName)
        {
            m_BaseViewOwner = owner;
            UnE.View.Content.ViewUtils.RegisterContentView(this);
            UnE.View.Content.IFormContentOwner owner2 = UnE.View.Content.ViewUtils.GetContentViewOwner();
            

            szMediaPath = owner2.ResourcePath + "Media\\";
            szIconPath = szMediaPath + "icons\\화재.ico";

            m_strUnityExePath = strUnityExePath;
            m_strUnityWindowName = strUnityWindowName;

            Create3DView();

            InitializeComponent();

            mView2.Visible = false;
            mView1.Dock = DockStyle.Fill;
            mView1.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            MouseWheel += new MouseEventHandler(OnMouseWheel);

            CurrentMouseWorkMode = MouseWorkMode.ORBIT;
            m_strSimulationBroadcastResultFilePath = Application.StartupPath + "\\FinishSimulationBroadcast.txt";
        }

        System.Windows.Forms.ToolStrip MainToolStrip = null;
        public void AddMainToolStrip(System.Windows.Forms.ToolStrip strip, ViewType vtype)
        {
            MainToolStrip = strip;
            if (vtype == ViewType.OUTSIDE)
                mView1.AddMainToolStrip(strip);
            else if(vtype == ViewType.INSIDE)
                mView2.AddMainToolStrip(strip);

        }

        private void Create3DView()
        {
            m_LayerContainer = new SplitContainer();
            m_LayerContainer.Dock = DockStyle.Fill;
            m_LayerContainer.Visible = false;

            //Controls.Add(m_LayerContainer);

            mView1 = new UnE.Util.Unity.Panel4Unity(this.m_BaseViewOwner, ProxySOP.Instance.SiteID);// new Core.BaseView();
            mView1.BackColor = System.Drawing.Color.Transparent;
            mView1.Dock = System.Windows.Forms.DockStyle.Fill;
            mView1.Location = new System.Drawing.Point(0, 0);
            mView1.Name = "m3DView1";
            mView1.Size = new System.Drawing.Size(1900, 1040);
            mView1.TabIndex = 0;
            mView1.Click += new System.EventHandler(this.View1Click);


            mView2 = new UnE.Util.Unity.Panel4Unity(m_BaseViewOwner, ProxySOP.Instance.SiteID);

            if( ProxySOP.Instance.SiteID == 1)
                mView2.UseIndoor = false;

            mView2.Indoor = true;
            //mView2 = new BaseViewEx(this, true);// new Core.BaseView();
            //mView2.BackColor = System.Drawing.Color.Transparent;
            mView2.Dock = System.Windows.Forms.DockStyle.Fill;
            mView2.Location = new System.Drawing.Point(0, 0);
            //mView2.Name = "panel2";
            mView2.Size = new System.Drawing.Size(1900, 1040);
            mView2.TabIndex = 0;
            mView2.Click += new System.EventHandler(this.View2Click);




            Controls.Add(mView1);
            Controls.Add(mView2);

            m_layerOutside = new UnE.Util.Unity.LayerManager(mView1);
            mView1.LayerManager = m_layerOutside;
            mView2.LayerManager = m_layerOutside;


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
            m_layerOutside.AddLayer(ID.ID_LAYER_PSM_SENSOR, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_NOTICE, false);

            //mSceneManager = new Core.SceneManager(mView1);

            //mVolmumeManagerOut = new Core.ZoneVolumeManager(mView1);
            //mVolmumeManagerIn = new Core.ZoneVolumeManager(mView2);

            //mView1.BeginUnity(OnReadyUnity);
            //mView1.LoadHomeView("Main");
        }

        private void FormContent_SizeChanged(object sender, EventArgs e)
        {
        }

        private void FormLayout_Resize(object sender, EventArgs e)
        {
        }

        private void FormContent_Shown(object sender, EventArgs e)
        {
            //if (mView1 != null)
            //    mView1.ProcessCCTVLOD();
        }
        
        public void OnReadyUnityOutside()
        {
            AddGroupName();

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.OnReadyDataLoad();
            //FormMain.Instance.OnReadyDataLoad();
        }

        public void OnReadyUnityInside()
        {
            //FormMain.Instance.OnReadyDataLoad();
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

            mViewList.Add(mView1);


            mView1.Size = new Size(1280, 1024);
            mView2.Size = new Size(1280, 1024);
            mCurrent = mView1;

            try
            {

                mView1.NamedPipeName = "UnityPipeOutside";

                if (m_strUnityExePath.Contains(":\\") == false && m_strUnityExePath.Contains(":/") == false)
                {
                    if (m_strUnityExePath.StartsWith("\\") == false && m_strUnityExePath.StartsWith("/") == false)
                        m_strUnityExePath = "\\" + m_strUnityExePath;
                }

                string szPath2 = Path.GetDirectoryName(Application.ExecutablePath);

                if (m_strUnityExePath.Contains(":\\") || m_strUnityExePath.Contains(":/"))
                    mView1.UnityExePath = m_strUnityExePath;
                else
                    mView1.UnityExePath = szPath2 + m_strUnityExePath;

                mView1.UnityWndName = m_strUnityWindowName;

                /*if(ProxySOP.Instance.SiteID == 3)
                {
                  
                    string szPath2 = Path.GetDirectoryName(Application.ExecutablePath);
                    mView1.UnityExePath = szPath2 + "\\EnergyOutside.exe";
                    mView1.UnityWndName = "EnergyOutside";
                }
                else if (ProxySOP.Instance.SiteID == 100)
                {
                    string szPath2 = Path.GetDirectoryName(Application.ExecutablePath);
                    mView1.UnityExePath = szPath2 + "\\SeoulUnv.exe";
                    mView1.UnityWndName = "SeoulUnv";
                }
                else if (ProxySOP.Instance.SiteID == 101)
                {
                    string szPath2 = Path.GetDirectoryName(Application.ExecutablePath);
                    mView1.UnityExePath = szPath2 + "\\BusanUnv.exe";
                    mView1.UnityWndName = "BusanUnv";
                }
                else
                {
                    string szPath2 = Path.GetDirectoryName(Application.ExecutablePath);
                    mView1.UnityExePath = szPath2 + "\\UnitySam.exe";
                    mView1.UnityWndName = "AA_Unity";
                }*/
                mView1.PopupMenu = contextMenuStripManualReport;
                mView1.BeginUnity(OnReadyUnityOutside);
            }
            catch (System.Exception ex1)
            {
                Debug.WriteLine(ex1.StackTrace);
            }

            mViewList.Add(mView2);
            try
            {

                mView2.NamedPipeName = "UnityPipeInside";
                string szPath2 = Path.GetDirectoryName(Application.ExecutablePath);

                mView2.UnityExePath = szPath2 + "\\UnitySamInside.exe";
                mView2.UnityWndName = "UnitySamInside";

                if (ProxySOP.Instance.SiteID != 3)
                {
                    mView2.PopupMenu = contextMenuStripManualReport;
                    mView2.BeginUnity(OnReadyUnityInside);
                }
            }
            catch (System.Exception ex2)
            {
                Debug.WriteLine(ex2.StackTrace);
            }

            //mView1.CreateCompass(0.0f);
            //mView2.CreateCompass(0.0f);

            bool bSimMode = UnE.SOP.ProxySOP.Instance.SimulationMode;

            //m_strOutDaeName = m_strZipFileFolderPath + "outside\\ND_0326l.DAE";
            //m_strOutDaeName = m_strZipFileFolderPath + "\\yh20150424.scene";

            //if (!File.Exists(m_strOutDaeName) || (bSimMode || ModelManager.Instance.ExtractOutside == true))
            //{
            //    try
            //    {
            //        if (File.Exists(m_strOutsideDAE))
            //        {
            //            ExtractToTrg(m_strOutsideDAE, m_strZipFileFolderPath + "\\");
            //            //mView1.ExtractFile(m_strOutsideDAE, m_strZipFileFolderPath);
            //        }
            //    }
            //    catch (System.Exception ex)
            //    {
            //        Debug.WriteLine(ex.StackTrace);
            //    }
            //}

            //try
            //{
            //    mView1.OpenMesh(m_strOutDaeName, false);
            //    //mView1.OnViewTop();
            //    mView1.OnViewFix("Main");
            //}
            //catch (System.Exception)
            //{
            //}

            //// open floor mesh
            //string szFloorFile = Application.StartupPath + "\\DXF\\#1-2 BOILER\\1r403-1-886-ea152-205-f-001.png";
            //if (!File.Exists(szFloorFile) || (bSimMode || ModelManager.Instance.ExtractInside == true))
            //{
            //    try
            //    {
            //        //ExtractToTrg(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");
            //        //mView2.ExtractFile(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");
            //    }
            //    catch (System.Exception ex)
            //    {
            //        Debug.WriteLine(ex.StackTrace);
            //    }
            //}
            //bExtractInside = true;

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
            //mView1.SetIconPOISize(32.0f, 32.0f);

            LayoutOutside();

            
            //AddBuildingName();
            
            //Add3DText();

            //LoadPOIs();
            //mView1.AddComponent(0, 0, 0);
            

            //AddZoneVolume();
            //AddSafeZoneVolume();


            SetBuildingTextColor();
            mView1.Update3D();
           
            if( ProxySOP.Instance.SiteID != 3)
                mView2.Update3D();


            //Button b = new Button();
            //b.Size = new Size(1, 1);
            //mView1.Controls.Add(b);
            //b.Show();


        }

        private void SetBuildingTextColor()
        {
            string strBuildingTextColorTag = "3DBuildingTextColor", strBuildingGroupTextColorTag = "3DBuildingGroupTextColor";

            string strSQL = "select PropertyName, PropertyValue from OptionSDMS where PropertyName = '" + strBuildingTextColorTag + "' or PropertyName = '" + strBuildingGroupTextColorTag + "'";

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager dbMgr = owner.DBManager;

            if (dbMgr == null)
                return;

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            Color color = Color.Black;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strName = WebDBManager.GetStringField(arrResult[i]);
                string strValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strName == null || strValue == null)
                    continue;

                string[] tokens = strValue.Split(',');

                if (tokens.Length < 3)
                    continue;

                if (GetRGB(strValue, ref color))
                {
                    if (string.Compare(strName, strBuildingTextColorTag, true) == 0)
                        mView1.SetAliasTextColor(color);
                    else if (string.Compare(strName, strBuildingGroupTextColorTag, true) == 0)
                        mView1.SetTextColor(color);
                }
            }
        }

        private bool GetRGB(string strColor, ref Color color)
        {
            string[] tokens = strColor.Split(',');

            if (tokens.Length < 3)
                return false;

            int[] arr = new int[3];

            for (int i=0;i<3;i++)
            {
                if (int.TryParse(tokens[i].Trim(), out arr[i]) == false)
                    return false;
            }

            color = Color.FromArgb(arr[0], arr[1], arr[2]);
            return true;
        }

        private void FormLayout_Load(object sender, EventArgs e)
        {
            //Init3DView();
        }

        //private bool ExtractToTrg(string strSrcFile, string strTrgPath)
        //{
        //    try
        //    {
        //        if (Directory.Exists(strTrgPath))
        //            BackupManager.DeleteFolder(strTrgPath);

        //        if (!Directory.Exists(strTrgPath))
        //            Directory.CreateDirectory(strTrgPath);

        //        System.IO.Compression.ZipFile.ExtractToDirectory(strSrcFile, strTrgPath);
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Trace.WriteLine(e.Message);
        //        System.Diagnostics.Trace.WriteLine(e.StackTrace);
        //        return false;
        //    }

        //    return true;
        //}

        public void LoadPOIs()
        {
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.LoadPOI(mView1, false);

            //if (mView2.IsHandleCreated == true && mView2.Visible == true)
            //FormMain.Instance.DataManager.LoadPOI(mView2, true);

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
                mCurrent.Focus();
            }
        }

        public void View2Click(object sender, EventArgs e)
        {
            if (mView2 != null)
            {
                mCurrent = mView2;
                mView2.Focus();
            }           
        }

        public void TopView()
        {
            if (mCurrent != null)
            {
                if (mCurrent == mView1)
                    mView1.SetTopView();
                else
                {
                    mView2.SetFrontView();
                    mView2.Refresh();
                }
            }
            else
            {
                mView2.SetFrontView();
                mView2.Refresh();
            }
        }

        public void FrontViw()
        {
            if (mCurrent != null)
                mCurrent.SetFrontView();

        }

        public void LeftView()
        {
            if (mCurrent != null)
                mCurrent.SetLeftView();
        }

        public void RightView()
        {
            if (mCurrent != null)
                mCurrent.SetRightView();
        }

        public void RearView()
        {
            if (mCurrent != null)
                mCurrent.SetRearView();
        }

        public void HomeView(string szName)
        {
            if (mCurrent != null)
            {
                if (mCurrent == mView1)
                    mView1.LoadHomeView(szName);
                else
                {
                    mView2.SetFrontView();
                    mView2.Refresh();
                }
            }
            else
            {
                mView2.SetFrontView();
                mView2.Refresh();
            }
        }

        public void FitView()
        {
            if (mCurrent != null)
            {
                if (mCurrent == mView1)
                    mCurrent.SetFrontView();
                else
                {
                    mView2.SetFrontView();
                    mView2.Refresh();
                }
            }
            else
            {
               // mView2.FitView();
                mView2.Refresh();
            }
        }

        public void HideAllShelter()
        {
            //if (mView1 != null)
           //     mView1.HideAllShelter();
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
                            //ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                            //if (volume != null)
                            //    volume.SetVisible(false);
                        }
                        catch (System.AccessViolationException ex)
                        {
                            Debug.WriteLine(ex.Message + " " + szID);
                        }
                    }
                }
                }
        }

        public void HidePoll(int nPollID)
        {
            mView1.HideEmPoll(nPollID);
        }
        public void ShowEmPoll(int nPollID)
        {
            mView1.ShowEmPoll(nPollID);
        }

        public void ShowPollutionView(int windDirection, int windSpeed)
        {
            mView1.ShowPollution(windDirection, windSpeed);
        }


        public void HidePollutioinView()
        {
            mView1.HidePollution();
        }

        // nType : ShelterPath의 Type
        //         CoreAPI의 UBaseView::ShowPath(int nType)의 인자로 사용된다.
        // nShelterType : UnE.Spatial.Shelter.ShelterTypes(화재, 누출, 지진...)
        //                재난종류별 대피소를 각각 지정할 수 있도록 한다.
        public void ShowShelter(int nType, int nShelterType)
        {
            if (nType == 3)
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
                                //ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                                //volume.SetVisible(true);
                                //mView1.ShowShelterPath(nType);
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
                                //ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                                //volume.SetVisible(true);
                                //mView1.ShowShelterPath(nType);
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
                //mCurrent.OnMouseWheel(e.X, e.Y, e.Delta);
            }
            else
            {

                //mView2.OnMouseWheel(sender, e);

            }
        }

      

        public void ZoomIn()
        {
            if (mCurrent != null)
            {
                mCurrent.OnMouseWheel(0, 0, 24);
                //mCurrent.UpdatePOI();
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
                mCurrent.OnMouseWheel(0, 0, -24);
                //mCurrent.UpdatePOI();
            }
            else
            {
                //mView2.ZoomOut();
            }
        }

        public void ZoomBuilding(string szCode)
        {
            if (ProxySOP.Instance.SiteID == 1 || ProxySOP.Instance.SiteID == 100)
            {
                if (Char.IsDigit(szCode[0]))
                {
                    szCode = "z" + szCode;
                }
                mView1.SetZoomObject(szCode);
            }
            else
            {
                mView1.SetZoomObject(szCode);
            }
            /*if(ProxySOP.Instance.SiteID == 101)
            {                
                mView1.SetZoomObject(szCode);
            } 
            else
            {
                if (Char.IsDigit(szCode[0]))
                {
                    szCode = "z" + szCode;
                }
                mView1.SetZoomObject(szCode);
            }*/
        }

        public void ZoomTarget(float x, float y, float z, bool isIndoor)
        {
            y = 0.0f;
            if (isIndoor)
            {
                //if (mView2.MeshOpened)
                {
                    //mView2.SetTopView();

                    int nSiteID = ProxySOP.Instance.SiteID;
                    if (nSiteID != 100)
                    {
                        mView2.SetZoomObjectDistance(20.0f);
                    }
                   
                    mView2.ZoomTarget(x, y, z);
                    mView2.Update3D();
                }
            }
            else
            {
                //if (mView1 != null)
                //mView1.SetTopView();
                int nSiteID = ProxySOP.Instance.SiteID;
                if( nSiteID != 100)
                {                   
                    mView1.SetZoomObjectDistance(400.0f);
                }
                else if( nSiteID == 3)
                {
                    mView1.SetZoomObjectDistance(1.0f);
                }
                else
                {
                    mView1.SetZoomObjectDistance(15.0f);
                }

                
                mView1.ZoomTarget(x, y, z);
                mView1.Update3D();
                //mView1.Refresh();
            }
        }

        public void SelectPOI(POI poi, bool isIndoor)
        {
            if (isIndoor)
            {
                //if (mView2.MeshOpened)
                //mView2.SelectPOI(poi.ID);
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
                SetCurrentBuilding(poi.Zone.Building, poi.Zone);
                if (mView2.MeshOpened)
                    mView2.SelectPOI(poi.ID, poi.Facility.IconPath);
            }
            else
            {
                mView1.SelectPOI(poi.ID, poi.Facility.IconPath);
            }
        }

        private static int m_nImageNum = 1;
        public string SaveToTempImage()
        {

            string szPath1 = System.IO.Path.GetTempPath() + "view1" + m_nImageNum + ".png";
            string szPath2 = System.IO.Path.GetTempPath() + "view2" + m_nImageNum + ".png";

            try
            {
                if( File.Exists(szPath1))
                {
                    File.Delete(szPath1);
                }                
            }
            catch(Exception)
            {
            }

            try
            {
                if (File.Exists(szPath2))
                {
                    File.Delete(szPath2);
                }
            }
            catch (Exception)
            {
            }

            mView1.SaveScreen(szPath1);

            if( UnE.SOP.ProxySOP.Instance.Use2D == true)
                mView2.SaveScreen(szPath2);
            m_nImageNum++;

            if (m_nImageNum == 1000)
                m_nImageNum = 1;
            return szPath1;
        }

        public void SaveToImage()
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PNG Files (*.png)|*.png";
            string defaultName = "Untitled";
            dlg.FileName = defaultName;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string szPath = dlg.FileName;
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

            if (!Controls.Contains(m_LayerContainer))
                Controls.Add(m_LayerContainer);
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
                Controls.Remove(mView1);
                m_LayerContainer.Panel1.Controls.Add(mView1);
            }

            if (!m_LayerContainer.Panel2.Controls.Contains(mView2))
            {
                Controls.Remove(mView2);
                m_LayerContainer.Panel2.Controls.Add(mView2);
            }

            mView2.Dock = DockStyle.Fill;
            mView2.Visible = true;
            mView2.BringToFront();

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
                Controls.Add(mView1);
            }

            if (m_LayerContainer.Panel2.Controls.Contains(mView2))
            {
                m_LayerContainer.Panel2.Controls.Remove(mView2);
                Controls.Add(mView2);
            }
            if (Controls.Contains(m_LayerContainer))
                Controls.Remove(m_LayerContainer);


            mView2.AddMainToolStrip(MainToolStrip);

            mView1.Visible = false;

            mView2.Dock = DockStyle.Fill;
            mView2.Visible = true;
            mView2.BringToFront();

            mView2.Invalidate(true);
            mCurrent = mView2;
            m_buildingCurrent = null;

            mView2.Update3D();
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
                Controls.Add(mView1);
            }

            if (m_LayerContainer.Panel2.Controls.Contains(mView2))
            {
                m_LayerContainer.Panel2.Controls.Remove(mView2);
                Controls.Add(mView2);
            }

            if (Controls.Contains(m_LayerContainer))
                Controls.Remove(m_LayerContainer);
            
            mView1.AddMainToolStrip(MainToolStrip);
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

           // state.Zone = mView2.CurrentZone;
            //state.ImagePath = mView2.ImagePath;

            return state;
        }
        
        public void SaveCurrentTabLayout()
        {
            // do nothing
        }
        public void LoadTabLayout(int tabNumber)
        {
            // do nothing
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
            if (mLayooutStack.Count > 0)
            {
                ViewState state = mLayooutStack.Pop();
                nLayout = state.Layout;

                mView1.LoadViewState(szPushViewStat);

                //if (state.Zone != null && state.ImagePath != "")
                //{
                    //mView2.SetImage(state.ImagePath, state.Zone);
                //}
                if(m_bChangedTab == false)
                {
                    ContentOwnerTab tab = state.TabNumber;

                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    owner.ChangeTab(tab);
                }
                m_bChangedTab = false;
            }

            //if (nLayout > 0)
            //{
            //    // 1(Outside), 2(Both), 3(Inside)

            //    int nID = ID.ID_VIEW_OUTSIDE;
            //    switch (nLayout)
            //    {
            //        case 1: // outside
            //            nID = ID.ID_VIEW_OUTSIDE;
            //            break;
            //        case 2: // Both
            //            nID = ID.ID_VIEW_BOTHSIDE;
            //            break;
            //        case 3:
            //            nID = ID.ID_VIEW_INSIDE;
            //            break;
            //    }


            //    SetLayoutMode(nLayout);
            //    PageBackstageHome.Instance.Check3DViewMode(nID);
            //}
        }


        //public void AddOutZoneName()
        //{
        //    if (mView1 != null)
        //    {
        //        Dictionary<int, Zone> m_dicBuildingGroup = ZoneManager.Instance.DicOutdoorZones;
        //        foreach (KeyValuePair<int, Zone> kv in m_dicBuildingGroup)
        //        {
        //            Zone zone = kv.Value;
        //            try
        //            {
        //                Core.ZonePolygon area = new Core.ZonePolygon(mView1);
        //                int count = zone.Polygon.GetVertexCount();
        //                for (int i = 0; i < count; i++)
        //                {
        //                    UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
        //                    float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx) / 1000.0f;
        //                    float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y) / 1000.0f;


        //                    area.AddVertex(new Position3D(pos3DX, 0, pos3DZ));
        //                }
        //                area.Height = 0;
        //                area.CreatePolygon();
        //                Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, 20, zone.Building.BroadcastName);
        //                if (volume != null)
        //                    volume.SetVisible(false);
        //                Vertex2D pos2 = zone.Polygon.CalcWeightCenter();
        //                float pos3DX2 = ((float)pos2.x - ZoneManager.Instance.Dx);
        //                float pos3DZ2 = ZoneManager.Instance.Dy - (float)pos2.y;
        //                pos3DX2 /= 1000.0f;
        //                pos3DZ2 /= 1000.0f;
        //                string szName = string.Format("{0} [{1}]", zone.ZoneName, zone.DXFFileName);
        //                int nID = mView1.AddZoneName(szName, pos3DX2, 20.0f, pos3DZ2);
        //            }
        //            catch (System.Exception)
        //            {
        //            }
        //        }
        //    }
        //}

        //public void AddZoneName(Zone zone)
        //{
        //    if (mView2 != null)
        //    {
        //        try
        //        {
        //            string szName = string.Format("{0} [{1}]", zone.ZoneName, m_szFileName);
        //            mView2.AddZoneName(szName);
        //            //m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            ConnectionLogEx.Instance.WriteLine(ex.StackTrace);
        //        }
        //    }
        //}

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
                        if ((ProxySOP.Instance.SiteID == 100) || (ProxySOP.Instance.SiteID == 101))
                        {
                            float pos3DX = obj.TextCenterX;
                            float pos3DZ = obj.TextCenterY;

                            int nID = mView1.AddGroupName(obj.DisplayName, pos3DX, 3.5f, pos3DZ);
                         
                        }
                        else if(ProxySOP.Instance.SiteID == 3)
                        {
                            float pos3DX = obj.TextCenterX;
                            float pos3DZ = obj.TextCenterY;
                            int nID = mView1.AddGroupName(obj.DisplayName, pos3DX, 1.5f, pos3DZ);
                        }
                        else
                        {
                            float pos3DX = (obj.TextCenterX - ZoneManager.Instance.Dx);
                            float pos3DZ = (ZoneManager.Instance.Dy - obj.TextCenterY);

                            //float pos3DX = obj.TextCenterX;
                            //float pos3DZ = -obj.TextCenterY;

                            int nID = mView1.AddGroupName(obj.DisplayName, -pos3DX, 100.0f, pos3DZ);
                            //int nID = mView1.AddGroupName(obj.BuildingGroupName, pos3DX, 100.0f, pos3DZ);
                            //m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
                        }
                      
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
                //Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                //mCurrentOutdoorVolume = volume;
                //volume.SetVisible(bShow);
                //return volume;
            }
            else
            {
                if (zone.IsOutdoor == false)
                {
                    m_bChangeIndoor = false;
                    string szID = zone.ZoneName;

                    //mView2.ShowZonePolygon(zone, bShow);

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
                    } 
                }

                string szID = "";
                if (zone.IsOutdoor == false && zone.Building.BuildingID != "yhNONE")
                {
                    szID = zone.Building.BuildingID;
                    if (ProxySOP.Instance.SiteID != 101)
                    {
                        if (Char.IsDigit(szID[0]))
                        {
                            szID = "z" + szID;
                        }
                    }
                }
                else
                {
                    szID = zone.ZoneName;
                }
                
                mView1.SelectObject(szID);
                //Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
                //if (volume != null)
                //{
                //    mCurrentOutdoorVolume = volume;
                //    volume.SetVisible(bShow);
                //}
                //return volume;
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

                        if (bShow == true)
                            mView2.HideAllEquipmentZone();
                        mView2.ShowEquipmentZone(equipZone, bShow);
                        //mView2.ShowEquipmentZone(equipZone, bShow);
                    }
                }
            }
            //return null;
        }

        private bool m_bChangeIndoor = false;

        public void HideZoneVolume()
        {
            //if (m_bChangeIndoor == false && mCurrentIndoorVolume != null)
            //	mCurrentIndoorVolume.SetVisible(false);
            //if (mCurrentOutdoorVolume != null)
            //    mCurrentOutdoorVolume.SetVisible(false);



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
                            //ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);

                            //i//f (volume == null)
                            //    continue;

                            //arList.Add(volume.GetVisible());
                        }
                        catch (System.AccessViolationException ex)
                        {
                            Debug.WriteLine(ex.Message + " " + szID);
                        }
                    }
                }
            }

            mView1.ClearAllSelect();
            mView1.HideAllEquipmentZone();

            mView2.ClearAllSelect();
            mView2.HideAllEquipmentZone();

            //mVolmumeManagerOut.SetVisibleAll(false);
            //mVolmumeManagerIn.SetVisibleAll(false);


            // Shelter 볼륨의 상태를 복구한다.
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
                            //ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);

                            //if (volume == null)
                            //    continue;

                            //bool bShow = (bool)arList[nCount++];
                            //volume.SetVisible(bShow);
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

        //private Core.ZoneVolume mCurrentOutdoorVolume = null;
        //private Core.ZoneVolume mCurrentIndoorVolume = null;


        public void AddSafeZoneVolume()
        {
            //ArrayList arBuildings = new ArrayList();
            //foreach (KeyValuePair<int, Shelter> pair in ZoneManager.Instance.DicSafeZones)
            //{
            //    Shelter zone = pair.Value;

            //    if (zone != null)
            //    {

            //        string szID = "safe" + zone.ShelterName;
            //        if (arBuildings.Contains(szID))
            //            continue;
            //        arBuildings.Add(szID);
            //        float fHeight1 = 0.1f;
            //        float fHeight2 = 40.0f;
            //        if (zone.Polygon == null)
            //            continue;

            //        Core.ZonePolygon area = new Core.ZonePolygon(mView1);
            //        int count = zone.Polygon.GetVertexCount();

            //        if (count == 0)
            //            continue;

            //        for (int i = 0; i < count; i++)
            //        {
            //            UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
            //            float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
            //            float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
            //            pos3DX /= 1000;
            //            pos3DZ /= 1000;
            //            area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
            //        }
            //        area.Height = fHeight1;
            //        area.CreatePolygon();
            //        try
            //        {
            //            Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID, false, Color.Blue);
            //            volume.SetVisible(false);

            //        }
            //        catch (System.AccessViolationException ex)
            //        {
            //            Debug.WriteLine(ex.Message + " " + szID);
            //        }
            //    }
            //}
        }

        public void AddZoneVolume()
        {
            //ArrayList arBuildings = new ArrayList();
            //foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicZones)
            //{
            //    Zone zone = pair.Value;

            //    if (zone != null)
            //    {
            //        if (zone.Building != null && zone.Building.BuildingID != "yhNONE")
            //        {
            //            string szID = zone.Building.BuildingID;
            //            if (arBuildings.Contains(szID))
            //                continue;
            //            arBuildings.Add(szID);

            //            string szTempID = szID.Replace("_1", "");
            //            Core.Scene scene = mSceneManager.FindSceneNode(szTempID);
            //            if (scene != null)
            //            {
            //                float fHeight1 = scene.GetMinimum().Y - 0.1f;
            //                float fHeight2 = scene.GetMaximum().Y + 0.1f;

            //                Core.ZonePolygon area = new Core.ZonePolygon(mView1);
            //                if (zone.Polygon == null)
            //                    continue;

            //                int count = zone.Polygon.GetVertexCount();
            //                for (int i = 0; i < count; i++)
            //                {
            //                    UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
            //                    float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
            //                    float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);

            //                    pos3DX /= 1000;
            //                    pos3DZ /= 1000;
            //                    area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
            //                }
            //                area.Height = fHeight1;
            //                area.CreatePolygon();
            //                Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID);
            //                volume.SetVisible(false);
            //            }
            //        }
            //        else
            //        {
            //            string szID = zone.ZoneName;
            //            if (arBuildings.Contains(szID))
            //                continue;
            //            arBuildings.Add(szID);
            //            float fHeight1 = 0.1f;
            //            float fHeight2 = 40.0f;
            //            if (zone.Polygon == null)
            //                continue;

            //            Core.ZonePolygon area = new Core.ZonePolygon(mView1);
            //            int count = zone.Polygon.GetVertexCount();

            //            if (count == 0)
            //                continue;

            //            for (int i = 0; i < count; i++)
            //            {
            //                UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
            //                float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
            //                float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
            //                pos3DX /= 1000;
            //                pos3DZ /= 1000;
            //                area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
            //            }
            //            area.Height = fHeight1;
            //            area.CreatePolygon();
            //            try
            //            {
            //                Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID);
            //                volume.SetVisible(false);
            //            }
            //            catch (System.AccessViolationException ex)
            //            {
            //                Debug.WriteLine(ex.Message + " " + szID);
            //            }
            //        }
            //    }
            //}
        }

        public void HideEvacCircle()
        {
            if( mView1 != null)
            {
                mView1.ShowEvacCircle(0);
            }
        }

        public void SetEvacDistance(int nSensorID)
        {
            if(SensorManager.Instance.DicAllSenor.ContainsKey(nSensorID))
            {
                ISensor iSensor = SensorManager.Instance.DicAllSenor[nSensorID];
                if (iSensor != null && iSensor.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();

                    UnE.PSM.PSMSensor pSensor = owner.GetPSMSensor(iSensor.OrgSensorID);
                    if( pSensor != null)
                    {
                        UnE.PSM.PSMMaterial mat = owner.GetPSMMaterial(pSensor.MaterialType);
                        if( mat != null)
                        {
                            bool bIsDayLight = IsDayLight(DateTime.Now);

                            if( bIsDayLight == true)
                            {
                                mView1.SetEvacClircleDistance((int)mat.InitEvacDistance, (int)mat.DayEvacDistance);
                            }
                            else
                            {
                                mView1.SetEvacClircleDistance((int)mat.InitEvacDistance, (int)mat.NightEvacDistance);
                            }
                        }
                    }
                  
                }
                
            }
        }


        private bool GetWorkingHours(ref int nBeginHour, ref int nBeginMinute, ref int nEndHour, ref int nEndMinute)
        {
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager dbMgr = owner.DBManager;

            if (!GetWorkingHours(dbMgr, global::SOP.SOPSimulatorConfig.GetPropertyName(global::SOP.SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), ref nBeginHour, ref nBeginMinute))
                return false;

            if (!GetWorkingHours(dbMgr, global::SOP.SOPSimulatorConfig.GetPropertyName(global::SOP.SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), ref nEndHour, ref nEndMinute))
                return false;

            return true;
        }

        private bool GetWorkingHours(WebDBManager dbMgr, string strPropertyName, ref int nHour, ref int nMinute)
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strResult = WebDBManager.GetStringField(arrResult[0], "");

            if (!GetWorkingHours(strResult, ref nHour, ref nMinute))
                return false;

            return true;
        }

        private bool GetWorkingHours(string strWorkingHours, ref int nHour, ref int nMinute)
        {
            int nIndex = strWorkingHours.IndexOf(':');

            if (nIndex < 0)
                return false;

            string strHour = strWorkingHours.Substring(0, nIndex);
            string strMinute = strWorkingHours.Substring(nIndex + 1);

            if (!int.TryParse(strHour, out nHour))
                return false;

            if (!int.TryParse(strMinute, out nMinute))
                return false;

            if (nHour < 0 || nHour > 23)
                return false;

            if (nMinute < 0 || nMinute > 59)
                return false;

            return true;
        }

        public bool IsDayLight(DateTime time)
        {
            bool bResult = false;         
            
            int nBeginHour = 0;
            int nBeginMinute = 0;
            int nEndHour = 0;
            int nEndMinute = 0;

            if(GetWorkingHours(ref nBeginHour, ref nBeginMinute, ref nEndHour, ref nEndMinute))
            {
                if (time.Hour > nBeginHour)
                {
                    if (time.Hour < nEndHour)
                        bResult = true;
                    else if (time.Hour == nEndHour)
                        bResult = time.Minute <= nEndMinute;
                }
                else if (time.Hour == nBeginHour)
                {
                    if (time.Minute >= nBeginMinute)
                    {
                        if (time.Hour < nEndHour)
                            bResult = true;
                        else if (time.Hour == nEndHour)
                            bResult = time.Minute <= nEndMinute;
                    }
                }
            }           
            return bResult;
        }

        public void SetEvacCenter(EquipmentZone zone)
        {
            if (mView1 != null)
            {
                UnE.Geometry.Vertex2D vert = zone.Polygon.CalcWeightCenter();
                float pos3DX = ((float)vert.x - ZoneManager.Instance.Dx);
                float pos3DZ = (ZoneManager.Instance.Dy - (float)vert.y);

                mView1.SetEvacCircleCenter(-pos3DX, 0, pos3DZ);
            }
        }

        public void ShowEvacCircle(int nLevel)
        {
            if (mView1 != null)
            {
                mView1.ShowEvacCircle(nLevel);
            }
        }

        public void AddBuildingName()
        {
            //if (mView1 != null)
            //{
            //    Dictionary<int, Building> m_dicBuildings = ZoneManager.Instance.DicBuildings;
            //    foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
            //    {
            //        Building obj = kv.Value;

            //        if (obj.DisplayText.Trim().Length == 0)
            //            continue;

            //        try
            //        {
            //            mView1.SetTextLODDist(100.0f);
            //            mView1.SetTextColor(128 / 255.0f, 255 / 255.0f, 128 / 255.0f);
            //            mView1.SetTextHeight(15.0f);
            //            //1호기 1525135.1305542,323881.536591215
            //            //2호기 1605421.10046387,323397.886170073
            //            //3호기 1714242.4936676,318803.211456938
            //            //4호기 1798639.47862244,317594.137299223
            //            //5호기 219042.076202393,366377.324692412
            //            //6호기 123506.272125244,365183.117386503
            //            if (obj.BuildingID == "yhz1")
            //            {
            //                int nID = mView1.AddGroupName(obj.DisplayText, 1525.135f, 39.0f, -323.881f);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //            else if (obj.BuildingID == "yhz1_1")
            //            {
            //                int nID = mView1.AddGroupName(obj.DisplayText, 1605.421f, 39.0f, -323.397f);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //            else if (obj.BuildingID == "yhz2")
            //            {
            //                int nID = mView1.AddGroupName(obj.DisplayText, 1714.242f, 39.0f, -318.803f);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //            else if (obj.BuildingID == "yhz2_1")
            //            {
            //                int nID = mView1.AddGroupName(obj.DisplayText, 1798.639f, 39.0f, -317.594f);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //            else if (obj.BuildingID == "yhz3")
            //            {
            //                int nID = mView1.AddGroupName(obj.DisplayText, 219.042f, 39.0f, -366.377f);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //            else if (obj.BuildingID == "yhz3_1")
            //            {
            //                int nID = mView1.AddGroupName(obj.DisplayText, 123.506f, 39.0f, -365.183f);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //            else
            //            {
            //                int nID = mView1.AddAliasName(obj.BuildingCode, obj.DisplayText);
            //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
            //            }
            //        }
            //        catch (System.Exception)
            //        {
            //        }
            //    }
            //}
        }

        //public void Add3DText()
        //{
        //    if (mView1 != null)
        //    {
        //        foreach (_3DText text in ZoneManager.Instance._3DTextList)
        //        {
        //            if (text.DisplayText.Trim().Length == 0)
        //                continue;

        //            try
        //            {
                       
        //                if (text.TextColor == null)
        //                    mView1.SetTextColor(Color.FromArgb(128, 255 , 128));
        //                else
        //                {
        //                    Color textColor = text.TextColor.Data;
        //                    mView1.SetTextColor(textColor);
        //                }

        //                //if (text.TextFontHeight == null)
        //                //    mView1.SetTextHeight(15.0f);
        //               // else
        //                //    mView1.SetTextHeight(text.TextFontHeight.Data);

        //                float pos3DX = (text.TextCenterX - ZoneManager.Instance.Dx) / 1000.0f;
        //                float pos3DZ = (ZoneManager.Instance.Dy - text.TextCenterY) / 1000.0f;

        //                int nID = mView1.AddTextPOI(text.DisplayText, pos3DX, 100.0f, pos3DZ);
        //                m_layerOutside.GetLayer(ID.ID_LAYER_BUILDING_TEXT).Add(nID);
        //            }
        //            catch (System.Exception)
        //            {
        //            }
        //        }
        //    }
        //}

        public void RedrawWindow()
        {
            if (mView1 != null && mView1.Visible == true)
            {
                mView1.Update3D();
            }
            if (mView2 != null && mView2.Visible == true)
            {
                mView2.Update3D();
            }
        }

        public void Invalidate3DView(bool bErBack)
        {

            if (mView1 != null && mView1.Visible == true)
            {
                //mView1.Invalidate(bErBack);
                mView1.Update3D();

            }
            if (mView2 != null && mView2.Visible == true)
            {
                //mView2.Invalidate(bErBack);
                mView2.Update3D();
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


                if (m_currentIndoorZone == zone)
                    return;

                Floor floor = zone.Floor;
                m_currentIndoorZone = zone;
                string szCode = m_buildingCurrent.BuildingID;
                if (szCode == "")
                    return;
                
                if( UnE.SOP.ProxySOP.Instance.SiteID == 1)
                {
                    string szIndoorName = "";
                    // 층은 층인덱스 + 1임
                    float nFloor = floor.FloorIndex + 1;
                    // 지하층은 인덱스와 같은 값임
                    float nBaseFloor = floor.FloorIndex;

                    // 숫자로 시작하는 BuildingCode는 z를 붙여준다.
                    if (Char.IsDigit(szCode[0]))
                    {
                        szCode = "z" + szCode;
                    }

                    // floor가 0보다 작으면 지하층
                    if (floor.FloorIndex < 0.0f)
                        szIndoorName = string.Format("{0}_B{1:f1}", szCode, -nBaseFloor);
                    else
                        szIndoorName = string.Format("{0}_{1:f1}", szCode, nFloor);

                    // .0 으로 끝나는 부분은 삭제한다.
                    if (szIndoorName.EndsWith(".0"))
                        szIndoorName = szIndoorName.Substring(0, szIndoorName.Length - 2);

                    // .2, .5 와 같이 끝나는 경우 M을 붙인다.
                    if (szIndoorName[szIndoorName.Length - 2] == '.')
                    {
                        szIndoorName += "M";
                    }

                    mView2.OpenIndoor(szIndoorName, zone);
                    mView2.Update3D();
                }
                else if (UnE.SOP.ProxySOP.Instance.SiteID == 2)
                {
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
                        mView2.Refresh();
                        szPrevFileName = "";
                        return;
                    }
                    //mView2.SetImage(szInsideFullPath, m_currentIndoorZone);
                }                
                m_nCurrentFloor = floor.FloorIndex;
                m_bChangeIndoor = true;
                
                mView2.SetFrontView();
                m_bLoadInsideMode = true;
               
                mView2.Refresh();

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

                mView2.SetFrontView();

                m_bLoadInsideMode = true;

                // 테스트
                //ShowZoneVolume(m_currentIndoorZone.ID, false, true);

                mView2.Refresh();

                GC.Collect();

                szPrevFileName = szInsideFullPath;
                FormModelLoading.iForm.Close();
            }
        }

        public bool ShowLayer(int id, bool bShow)
        {
            if (bShow == true)
                Layers.ShowLayer(id);
            else
                Layers.HideLayer(id);

            //mView1.ShowLayer(id, bShow);
            //mView2.ShowLayer(id, bShow);

            //mView1.UpdatePOI();
            //mView2.UpdatePOI();

            RedrawWindow();

            return false;
        }

        public void AttachView(System.Windows.Forms.Control view, bool isOutdoor)
        {
            if (isOutdoor)
                mView1 = (UnE.Util.Unity.Panel4Unity)view;
            else
                mView2 = (UnE.Util.Unity.Panel4Unity)view;

            Controls.Add(view);

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

            Controls.Remove(view);

            return view;
        }

        public void ClearPOISelection()
        {
            if (m_nLayout == 1)
            {
                //mView1.ClearPOISelection();
            }
            else if (m_nLayout == 2)
            {
                //mView1.ClearPOISelection();
                //mView2.ClearPOISelection();
            }
            else if (m_nLayout == 3)
            {
                //mView2.ClearPOISelection();
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
                if (ProxySOP.Instance.SiteID == 100)
                {
                    Building bd = zone.Building;
                    if (bd != null)
                    {
                        string szFloor = zone.FloorIndex < 0 ? "B" + zone.FloorIndex : "" + (zone.FloorIndex + 1);
                        string szName = "z" + bd.BuildingID + "_" + szFloor + "F";
                        mView2.Open3dModel(szName);

                        LayoutInside();
                    }

                }
                else if (ProxySOP.Instance.SiteID == 101)           //부산대 indoor model 사용 여부에 따라 사용.
                {
                    Building bd = zone.Building;
                    if (bd != null)
                    {
                        string szFloor = zone.FloorIndex < 0 ? "B" + zone.FloorIndex : "" + (zone.FloorIndex + 1);
                        string szName =  bd.BuildingID + "_" + szFloor + "F";
                        mView2.Open3dModel(szName);

                        LayoutInside();
                    }

                }
                else
                {
                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    owner.SelectIndoorZone(zone);
                }
            }
        }

        private Zone m_ManualClickZone = null;

        public Zone ManualClickZone
        {
            get { return m_ManualClickZone; }
            set { m_ManualClickZone = value; }
        }

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

            UnE.Util.Unity.Panel4Unity view1 = (UnE.Util.Unity.Panel4Unity)mView1;

            Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
            formInvoke.Invoke((MethodInvoker)delegate
            {
                if(!contextMenuStripManualReport.Items.Contains(menuAddDisasterPos))
                {
                    contextMenuStripManualReport.Items.Insert(0, menuAddDisasterPos);                    
                }
                menuAddDisasterPos.Enabled = true;
            });
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
                if (contextMenuStripManualReport.Items.Contains(menuAddDisasterPos))
                {
                    contextMenuStripManualReport.Items.Remove(menuAddDisasterPos);
                }
                menuAddDisasterPos.Enabled = false;

                UnE.Util.Unity.Panel4Unity view2 = (UnE.Util.Unity.Panel4Unity)mView1;
                //BaseView view1 = (BaseView)mView2;
                //mView2.SetCheckPoistion(mCheckPosition);
                //view2.SetCheckPoistion(mCheckPosition);
                //if (m_nLayout == 3)
                //{
                //    if (bResult == true)
                //    {
                //        // view2.AddPOI(szIconPath);
                //        if (LastPos != null)
                //        {
                //            int nID = view2.AddPOI(szIconPath, LastPos.X, LastPos.Y, LastPos.Z);
                //            LastPos.IconID = nID;
                //        }
                //        else
                //        {
                //            view2.AddPOI(szIconPath);
                //            //LastPos.IconID = nID;
                //        }
                //        view2.UpdateWindow();
                //    }
                //}
                //else
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
            IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.ShowCCTVForm(bShow);
        }

        public void RemoveDisasterPos()
        {
            UnE.Util.Unity.Panel4Unity view1 = (UnE.Util.Unity.Panel4Unity)mView1;
            if (view1 != null)
            {
                if (LastPos != null)
                {
                    //    float dx = 120894.0548f + 1008.531f;
                    //    float dy = 157659.0963f - 506.251f;
                    float ox = LastPos.X;
                    float oz = -LastPos.Z;
                    int nID = LastPos.IconID;
                    if (nID != -1)
                    {
                        //view1.RemovePOI(nID);
                    }
                    else
                    {
                       // view1.RemovePOI(ox, LastPos.Y, oz);
                    }
                    view1.Update3D();
                }
            }

            if (mView2 != null)
            {
                if (LastPos != null)
                {
                    int nID = LastPos.IconID;
                    if (nID != -1)
                    {
                        //view1.RemovePOI(nID);
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
                    float oz = -z + (float)pos.y;
                    //int nID = mView2.AddPOI(path, x, y);
                    //LastPos.IconID = nID;
                    //mView2.Refresh();
                }
                RedrawWindow();
            }
            else
            {
                string path = szMediaPath + "icons\\" + disastertype + ".ico";
                UnE.Util.Unity.Panel4Unity view1 = (UnE.Util.Unity.Panel4Unity)mView1;
                if (view1 != null)
                {
                    //float dx = 120894.0548f + 1008.531f;
                    //float dy = 157659.0963f - 506.251f;
                    float ox = x;
                    float oz = -z;
                    //int nID = view1.AddPOI(path, ox, y, oz);
                    //LastPos.IconID = nID;
                   // view1.UpdateWindow();
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
                }
            }
            else
            {
                if (mCurrent != null && mFormPosition != null)
                {
                    float ox = 1008.531f;
                    float oy = 506.251f;

                    float dx = 120894.0548f + ox;
                    float dy = 157659.0963f - oy;

                    string szBroadcastName = "";
                    string szSelectedName = mCurrent.PopupObjName;
                    Building curBuilding = (Building)this.menuManualCCTV.Tag;
                    UnE.Util.Unity.Vector3 pos3D = (UnE.Util.Unity.Vector3)mCurrent.PopupMenu.Tag;
             
                    bool isBuildingName = false;
                    if (szSelectedName != null && szSelectedName != "")
                    {
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

        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                try
                {
                    if (process.ProcessName == strProcessName && process.HasExited == false)
                    {
                        process.Kill();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void ToggleBuildingTextLayer()
        {
            m_bLODText = !m_bLODText;
            //mView1.SetTextLOD(m_bLODText);
        }

        public void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {
            if (mView1 != null)
            {
                Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
                formInvoke.Invoke((MethodInvoker)delegate
                {
                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    owner.SetEarthquakeDetect(nIntensity, fMagnitude, strPosition, isRealMode);
                    mView1.ShowEarthquake(2, 3);
                });
            }
        }

        public bool EarthquakeEventIsFinished()
        {
            return true;
        }

        public void SelectBuilding(string strBuildingID)
        {
            if (mView1 != null)
            {
                mView1.SelectObject(strBuildingID);
            }
        }

        public void ShowBuildingCollapse(string szBuildingID, string szDisplayName)
        {
        }

        public void CloseBuilingCollapse(string szBuildingID)
        {
        }

        public void ChangeCampus()
        {
            
        }

        public void IsSameCampus(BuildingGroup group)
        {
            
        }

        public void SelectScene(string strSceneName)
        {
            if (mView1 != null)
            {
                mView1.SelectScene(strSceneName);
            }
        }

        public void ShowAlarmZone(string strZoneName, bool hideAllOthers)
        {
            if (mView1 != null)
            {
                mView1.ShowAlarmZone(strZoneName, hideAllOthers);
            }
        }

        public void HideAlarmZone(string strZoneName)
        {
            if (mView1 != null)
            {
                mView1.HideAlarmZone(strZoneName);
            }
        }

        public void HideAllAlarmZones()
        {
            if (mView1 != null)
            {
                mView1.HideAllAlarmZones();
            }
        }

        public void VisibleViewButton(string strBtnName, bool visible)
        {
            mView1.VisibleViewButton(strBtnName, visible);
        }

        public void AddWall()
        {
            mView1.AddWall();
        }

        public void AddDoor()
        {
            mView1.AddDoor();
        }

        public bool GetWalls(string strPath)
        {
            return mView1.GetWalls(strPath);
        }

        public bool LoadWalls(string strPath, string strSceneName)
        {
            return mView1.LoadWalls(strPath, strSceneName);
        }

        public void SetWallSnap(bool bUse)
        {
            mView1.SetWallSnap(bUse);
        }

        public void SetWallEditMode(bool bEdit)
        {
            mView1.SetWallEditMode(bEdit);
        }

        public void AddSpaceText(string strTxt)
        {
            mView1.AddSpaceText(strTxt);
        }

        public void ChangeColorSpaceText(string hexColor)
        {
            mView1.ChangeColorSpaceText(hexColor);
        }

        public void ChangeFontSpaceText(string name, float nSize, int nStyle)
        {
            mView1.ChangeFontSpaceText(name, nSize, nStyle);
        }

        public void LoadSpaceTexts(string strPath, string strScenName)
        {
            mView1.LoadSpaceTexts(strPath, strScenName);
        }

        public void GetSpaceTexts(string strPath)
        {
            mView1.GetSpaceTexts(strPath);
        }

        public void SetPoiLod(string strPOIType, bool useLOD)
        {
            mView1.SetPoiLod(strPOIType, useLOD);
        }

        public void AddPoiLodValue(float fMinZoomValue, float fMaxZoomValue, float fDistance)
        {
            mView1.AddPoiLodValue(fMinZoomValue, fMaxZoomValue, fDistance);
        }

        public void ClearPoiLodValue()
        {
            mView1.ClearPoiLodValue();
        }
    }
}