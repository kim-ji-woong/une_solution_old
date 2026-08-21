using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Core;
using System.Threading;
using UnE.Geometry;



namespace HSMS
{
    public enum LayerType
    {
        Icon = 1,
        Text = 2,
        Worker = 3,
        Vehicle = 4,
        Equipment = 5,
        AP = 6,
        GasSensor = 7
    }

    public partial class FormContent : Form
    {
        private class CCTVTooltip : IFormVirtualCCTVOwner
        {
            private CCTVViewer.CCTV m_cctv = null;
            public CCTVViewer.CCTV CCTV
            {
                get { return m_cctv; }
                set { m_cctv = value; }
            }

            private int m_nPrevFormLocationX = -1;
            private int m_nPrevFormLocationY = -1;

            private string m_strCCTVName = "";
            public string CCTVName
            {
                get { return m_strCCTVName; }
                set { m_strCCTVName = value; }
            }

            private FormVirtualCCTV m_frmCCTV = null;
            public FormVirtualCCTV FormCCTV
            {
                get { return m_frmCCTV; }
            }

            private int m_nPOIID = 0;
            public int POIID
            {
                get { return m_nPOIID; }
                set { m_nPOIID = value; }
            }

            public void SetLocation(int x, int y)
            {
                m_nPrevFormLocationX = x;
                m_nPrevFormLocationY = y;
            }

            public void Show()
            {
                m_frmCCTV = new FormVirtualCCTV(m_cctv);

                if (m_nPrevFormLocationX >= 0 || m_nPrevFormLocationY >= 0)
                    m_frmCCTV.Location = new Point(m_nPrevFormLocationX, m_nPrevFormLocationY);

                m_frmCCTV.Show(FormMain.Instance);

            }

            public void Hide()
            {
                if (m_frmCCTV != null)
                {
                    m_frmCCTV.Close();
                    m_frmCCTV = null;
                }
            }
        }

        // 1(Outside), 2(Both), 3(Inside)
        private int m_nLayout = 1;

        public int NumLayout
        {
            get { return m_nLayout; }
            set { SetLayoutMode(value); }
        }

        Core.LayerManager m_layerOutside = null;
        public Core.LayerManager Layers
        {
            get { return m_layerOutside; }
        }
        
        private BaseViewEx mView1 = null;

        private Core.Engine mEngine = new Core.Engine();

        private string m_strZipFileFolderPath = "";
        private string m_strOutsideDAE = "";
        private string m_strInsideDAE = "";
       
        private Dictionary<string, string> m_dicInsideDAE = null;
        private string m_strOutDaeName = "";

        private PopupDialog.FormWorkerInfo m_frmWorkerInfo = new PopupDialog.FormWorkerInfo();
        private PopupDialog.FormVehicleInfo m_frmVehicleInfo = new PopupDialog.FormVehicleInfo();

        private ArrayList mViewList = new ArrayList();
        private BaseViewEx mCurrent = null;
        private string szIconPath = "";
        private string szMediaPath = "";
       
        private bool bLoadComplete = false;
        public bool LoadComplete
        {
            get { return bLoadComplete; }
            set { bLoadComplete = value; }
        }

        private bool bInit = false;

        private bool m_bEditMode = false;
        public bool EditMode
        {
            get { return m_bEditMode; }
            set
            {
                m_bEditMode = value;
                ((BaseViewEx)mView1).EditMode = value;
            }
        }

        private Dictionary<int, DataWorker> m_dicWorker = new Dictionary<int, DataWorker>();
        private Dictionary<int, DataCar> m_dicCars = new Dictionary<int, DataCar>();
        
        private Dictionary<int, HSMS.APData> m_dicAP = new Dictionary<int, HSMS.APData>();

        // POI ID, CCTVTooltip
        private Dictionary<int, CCTVTooltip> m_dicCCTVTooltip = new Dictionary<int, CCTVTooltip>();

        public BaseViewEx.MouseWorkMode CurrentMouseWorkMode
        {
            get { return ((BaseViewEx)mView1).CurrentMouseWorkMode; }
            set
            {
                ((BaseViewEx)mView1).CurrentMouseWorkMode = value;
            }
        }

        private Core.SceneManager mSceneManager = null;
        public Core.SceneManager SceneManager
        {
            get { return mSceneManager; }
            set { mSceneManager = value; }
        }

        private Core.ZoneVolumeManager mVolmumeManagerOut = null;
        public Core.ZoneVolumeManager VolmumeManager
        {
            get { return mVolmumeManagerOut; }
            set { mVolmumeManagerOut = value; }
        }

        public BaseViewEx OutdoorView
        {
            get { return mView1; }
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        public override void Refresh()
        {
            RedrawWindow();
        }
        
        public FormContent()
        {
            szMediaPath = EnginPath() + "Media\\";
            szIconPath = szMediaPath + "icons\\화재.ico";

            Create3DView();

            InitializeComponent();
            mView1.Dock = DockStyle.Fill;
            mView1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            MouseWheel += new MouseEventHandler(OnMouseWheel);
            CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.ORBIT;
        }

        private void Create3DView()
        {
            mView1 = new BaseViewEx(this);// new Core.BaseView();
            mView1.BackColor = System.Drawing.Color.Transparent;
            mView1.Dock = System.Windows.Forms.DockStyle.Fill;
            mView1.Location = new System.Drawing.Point(0, 0);
            mView1.Name = "m3DView1";
            mView1.Size = new System.Drawing.Size(1920, 1080);
            mView1.TabIndex = 0;
            mView1.Click += new System.EventHandler(this.View1_Click);

            mView1.EnableGradient = true;
            mView1.BackUpperColor = Color.FromArgb(150, 150, 150);
            mView1.BackBottomColor = Color.FromArgb(30, 30, 30);

            Controls.Add(mView1);

            m_layerOutside = new Core.LayerManager(mView1);

            AddLayer((int)LayerType.Icon, LayerType.Icon);
            AddLayer((int)LayerType.Text, LayerType.Text);
            AddLayer((int)LayerType.Worker, LayerType.Worker);
            AddLayer((int)LayerType.Vehicle, LayerType.Vehicle);
            AddLayer((int)LayerType.Equipment, LayerType.Equipment);
            AddLayer((int)LayerType.AP, LayerType.AP);
            AddLayer((int)LayerType.GasSensor, LayerType.GasSensor);

            mSceneManager = new Core.SceneManager(mView1);
            mVolmumeManagerOut = new Core.ZoneVolumeManager(mView1);

            mView1.ReadHomeView();
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


        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;
#if DOTNET45
            szWorkPath = szMainPath + "common12\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "..\\common12\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;
#endif
            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "..\\common\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;



            szWorkPath = szMainPath + "HSMS\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            return szMainPath;
        }

        public void CreateBeam()
        {            
            float[] fyt1 = { -21.0f, 38.0f, 78.0f, 99.5f };
            for (int j = 0; j < fyt1.Length; j++)
            {
                mView1.AddBeams( 0.0f, fyt1[j]);
            }
            
        }


        private void CreateCore()
        {
            float[] arrY =  {  
                0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f,
                24.0f, 28.0f, 39.5f, 43.5f, 47.5f,
                59.0f, 63.0f, 67.0f, 78.0f, 82.0f, 86.0f
            };

            float[] arrX =  {  
                0.0f, -14.0f           
            };

            for (int i = 0; i < arrY.Length; i++)
            {
                for (int j = 0; j < arrX.Length; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    mView1.AddCore(arrX[j], arrY[i]);
                }
            }

           
            for (int i = 0; i < 8; i++)
            {                
                mView1.AddCore(-26.0f, arrY[i]);
                mView1.AddCore(-38.0f, arrY[i]);         
            }

            float[] fyt1 = { 59.0f, 63.0f, 67.0f };
            float[] fxt1 = { -26.0f,-38.0f, -55.0f ,-67.0f};
            for (int i = 0; i < fyt1.Length; i++)
            {
                for (int j = 0; j < fxt1.Length; j++)
                {
                    mView1.AddCore(fxt1[j], fyt1[i]);
                }
            }

            float[] fyt2 = { -12.0f, -16.0f, -20.0f, 59.0f, 63.0f, 67.0f };
            float[] fxt2 = { -79.0f, -93.0f , -104.0f};//, -116.0f};
            for (int i = 0; i < fyt2.Length; i++)
            {
                for (int j = 0; j < fxt2.Length; j++)
                {
                    mView1.AddCore(fxt2[j], fyt2[i]);
                }
            }
            float[] fyt3 = { -12.0f, -16.0f, -20.0f, 39.5f, 43.5f, 47.5f, 59.0f, 63.0f, 67.0f, 78.0f, 82.0f, 86.0f };
            for (int i = 0; i < fyt3.Length; i++)
            {
                mView1.AddCore(-116.0f, fyt3[i]);
                
            }

            float[] fyt4 = { -12.0f, -16.0f, -20.0f};
            for (int i = 0; i < fyt4.Length; i++)
            {
                mView1.AddCore(-132.0f, fyt4[i]);

            }
           
            //mView1.UpdateWindow();
        }
        /// <summary>
        /// 
        /// </summary>
        /// 


        private bool m_bUseDB = false;
        public void Init3DView()
        {
            if (bInit == true)
                return;
            bInit = true;
            
            string szPath = EnginPath();

            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string szAppName = currentProcess.ProcessName;
            
            mEngine.Init(szPath, szAppName);

            mViewList.Add(mView1);
            mView1.Size = new Size(1280, 1024);

            mCurrent = mView1;
            try
            {
                mView1.InitBaseView();
            }
            catch (System.Exception ex1)
            {
                Debug.WriteLine(ex1.StackTrace);
            }




            m_bUseDB = ModelManager.Instance.UseDB;

            if (m_bUseDB == false)
            {
                m_strOutDaeName = Path.GetDirectoryName(Application.ExecutablePath) + "\\model\\A-1_1.DAE";

                try
                {
                    mView1.OpenMesh(m_strOutDaeName);
                    mView1.OnViewFit();
                }
                catch (System.Exception)
                {
                }
            }
            else
            {
                try
                {
                    float fWdith = ModelManager.Instance.Width;
                    float fHeight = ModelManager.Instance.Height;
                    float fElevation = ModelManager.Instance.Elevation;
                    mView1.CreateFloor(fWdith, fHeight, fElevation, Color.Green, Color.White, true);
                    mView1.OnViewFit();
                }
                catch(Exception)
                {
                }
            }

        
            mSceneManager.UpdateData();
            mView1.SetIconPOISize(64.0f, 64.0f);
            mView1.SetTextColor(0.1f, 0.1f, 0.05f);
            
            LayoutOutside();

            //////////////////////////////////////////////////////////////////////////
            mView1.CreateSceneNodes();

            if (m_bUseDB == false)
            {
                CreateCore();
                CreateBeam();
            }
            //////////////////////////////////////////////////////////////////////////
            EquipmentLayer layer = (EquipmentLayer)Layers.GetLayer((int)LayerType.Equipment);
            if (m_bUseDB == false)
            {
                CraneManager manager = CraneManager.Instance;
                Crane crane1 = manager.GetCrane(0);

               
                layer.Add(0, 0);
                Crane crane2 = manager.GetCrane(1);
                layer.Add(1, 0);

                crane1.SetLocation(30);
                crane1.SetHookLocation(9.0f);
                crane2.SetHookLocation(-9.0f);

                //////////////////////////////////////////////////////////////////////////
                MovingEquipmentManager equipMan = MovingEquipmentManager.Instance;
                MovingEquipment equipMent = equipMan.GetEquipment(0);
                layer.Add(0, 1);
                equipMent.SetLocation(4.0f);
            }      


            //////////////////////////////////////////////////////////////////////////
            CreateZoneVolume();
            
            SetPickMode();

            ShowLayer((int)LayerType.Worker, true);
            ShowLayer((int)LayerType.Vehicle, true);
            ShowLayer((int)LayerType.Equipment, true);

            mView1.UpdateWindow();
           
            Button b = new Button();
            b.Size = new Size(1, 1);
            mView1.Controls.Add(b);
            b.Show();

            Button b2 = new Button();
            b2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            b2.Size = new Size(1, 1);
            mView1.Controls.Add(b2);
            b2.Show();          
        }


        bool bFirstCall = true;
        public void SetAccidentText()
        {
            foreach(Worker worker in WorkerManager.Instance.Workers)
            {
                if( bFirstCall == true)
                {
                    bFirstCall = false;
                     worker.SetAccidentText("");
                }
               
                bool bShowText = worker.IsShowNameOnly();
                worker.ToggleText(!bShowText);
            }
        }

        private void FormLayout_Load(object sender, EventArgs e)
        {
        }

        private void CreateZoneVolume()
        {
            DataManager dataManger = FormMain.Instance.DataMgr;
            int nZoneGroupCount = dataManger.GetZoneGroupCount();

            for (int i = 0; i < nZoneGroupCount; i++)
            {
                ZoneGroup group = dataManger.GetZoneGroup(i);
                int nZoneCount = group.GetZoneCount();

                for (int j = 0; j < nZoneCount; j++)
                //ArrayList arZone = dataManger.DataZones;
                //foreach (DataZone zone in arZone)
                {
                    DataZone zone = group.GetZone(j);

                    if (zone.ZoneName == "PLAN")
                        continue;

                    if (zone != null)
                    {

                        string szID = zone.ZoneName;

                        float fHeight1 = 0.1f;
                        float fHeight2 = 4.0f;
                        Core.ZonePolygon area = new Core.ZonePolygon(mView1);
                        int count = zone.Boundary.GetVertexCount();
                        for (int k = 0; k < count; k++)
                        {
                            UnE.Geometry.Vertex2D pos = zone.Boundary.GetVertex(k);
                            float pos3DX = (float)(pos.x);
                            float pos3DZ = (float)(-pos.y);
                            area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
                        }
                        area.Height = fHeight1;
                        area.CreatePolygon();
                        try
                        {
                            Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID, zone.ZoneName);
                            if (volume != null)
                            {
                                mView1.ZoneVolumes[zone.ZoneName] = (volume);
                                volume.SetVisible(false);
                            }

                        }
                        catch (System.AccessViolationException ex)
                        {
                            Debug.WriteLine(ex.Message + " " + szID);
                        }
                    }
                }
            }
        }
        
        private void FormLayout_FormClosed(object sender, FormClosedEventArgs e)
        {
            mEngine.EngineDispose();
        }

        private void View1_Click(object sender, EventArgs e)
        {
            if (mView1 != null)
            {
                mCurrent = mView1;
                mCurrent.Focus();
            }
        }

        public void SaveHomeView()
        {
            if (mCurrent != null)
                mCurrent.SaveHomeView();
        }

        public void TopView()
        {
            if (mCurrent != null)
                mCurrent.OnViewTop();
        }

        public void FrontViw()
        {
            if (mCurrent != null)
                mCurrent.OnViewFront();
        }

        public void LeftView()
        {
            if (mCurrent != null)
                mCurrent.OnViewLeft();
        }

        public void RightView()
        {
            if (mCurrent != null)
                mCurrent.OnViewRight();
        }

        public void RearView()
        {
            if (mCurrent != null)
                mCurrent.OnViewRear();
        }

        public void HomeView()
        {
            if (mCurrent != null)
            {
                if (mCurrent == mView1)
                    mView1.OnViewFix();
                else
                    mCurrent.OnViewHome();
            }
        }

        public void FitView()
        {
            if (mCurrent != null)
                mCurrent.OnViewFit();
        }

        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (mCurrent != null)
            {
                mCurrent.OnMouseWheel(e.X, e.Y, e.Delta);
            }
        }

        public void ZoomIn()
        {
            if (mCurrent != null)
            {
                mCurrent.OnMouseWheel(0, 0, 240);
                //mCurrent.UpdatePOI();
            }
        }

        public void ZoomOut()
        {
            if (mCurrent != null)
            {
                mCurrent.OnMouseWheel(0, 0, -240);
                //mCurrent.UpdatePOI();
            }
        }

        public void ZoomTarget(float x, float y, float z, bool isIndoor)
        {
            y = 0.0f;           
            {               
                mView1.OnViewTop();
                mView1.ZoomTarget(new Position3D(x, y, z), 100.0f);
                mView1.Update();
            }
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
                switch (m_nLayout)
                {
                    case 1:
                        mView1.SaveScreen(szPath);
                        break;
                    case 2:
                        if (mCurrent != null)
                            mCurrent.SaveScreen(szPath);
                        break;
                    default:
                        break;
                };
            }
        }

        public void SetPickMode()
        {
            mView1.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.PICK;
        }
        public void SetOrbitMode()
        {
            mView1.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.ORBIT;
        }
        public void SetNoneMode()
        {
            mView1.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.NONE;
        }
        public void SetPanMode()
        {
            mView1.CurrentMouseWorkMode = BaseViewEx.MouseWorkMode.PANNING;
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
                default:
                    break;
            };
            m_nLayout = nLayout;
        }

        public void LayoutOutside()
        {
            if (m_nLayout == 1)
                return;
            m_nLayout = 1;

            mCurrent = mView1;

            mView1.Visible = true;
            mView1.Dock = DockStyle.Fill;
            mView1.Invalidate(true);
        }

        public void RedrawWindow()
        {
            if (mView1 != null && mView1.Visible == true)
            {

                mView1.RedrawScene();
                
            }
        }

        public void Invalidate3DView(bool bErBack)
        {
            if (mView1 != null && mView1.Visible == true)
            {
                mView1.Invalidate(bErBack);
            }           
        }

        public void SetFilePath(string strCMOFolderPath, string strOutsideFilePath, string strInsideFilePath, Dictionary<string, string> dicInsideCMO)
        {
            m_strZipFileFolderPath = strCMOFolderPath;
            m_strOutsideDAE = strOutsideFilePath;
            m_strInsideDAE = dicInsideCMO["Inside"];
            m_dicInsideDAE = dicInsideCMO;
        }
        
        public void SetLayerLOD(int nLevel, LayerType type)
        {
            Layer layer = Layers.GetLayer((int)type);
            if (layer != null)
            {
                layer.SetLOD(nLevel);
                mView1.Update();
            }           
        }
       
        public void AddLayer(int id, LayerType type)
        {  
            switch(type)
            {
                case LayerType.Icon:
                    m_layerOutside.AddLayer(id, false);
                    break;
                case LayerType.Text:
                    m_layerOutside.AddLayer(id, true);
                    break;
                case LayerType.Vehicle:
                    Core.VehicleLayer layerV = new Core.VehicleLayer(id);
                    m_layerOutside.AddLayer(layerV); 
                    break;
                case LayerType.Worker:
                    Core.WorkerLayer layer = new Core.WorkerLayer(id);
                    m_layerOutside.AddLayer(layer);           
                    break;
                case LayerType.Equipment:
                    Core.EquipmentLayer layerE = new Core.EquipmentLayer(id);
                    m_layerOutside.AddLayer(layerE); 
                    break;
                case LayerType.AP:
                    Core.APLayer layerAP = new Core.APLayer(id);
                    m_layerOutside.AddLayer(layerAP);
                    break;
                case LayerType.GasSensor:
                    Core.VehicleLayer layerGasSensor = new Core.VehicleLayer(id);
                    m_layerOutside.AddLayer(layerGasSensor);
                    break;
            }
        }
            
        public bool ShowLayer(int id, bool bShow)
        {
            if (bShow == true)
                Layers.ShowLayer(id);
            else
                Layers.HideLayer(id);

            mView1.ShowLayer(id, bShow);
            mView1.UpdatePOI();

            RedrawWindow();
            return false;
        }  
        
        public void ClearPOISelection()
        {
            mView1.ClearPOISelection();
        }

        public void HideAllPOIPopup()
        {
            mView1.HideAllPOIPopup();
        }

        public void RemoveWorker(DataWorker worker)
        {
            if (worker == null)
                return;
            if (worker.SensorWorker == null)
                return;
            SensorWorker sWorker = worker.SensorWorker;
            if (sWorker != null)
            {
                sWorker.OnVisible(false);
                Layers.GetLayer((int)LayerType.Worker).Remove(sWorker.WorkID);
                sWorker.Delete();

                m_dicWorker.Remove(sWorker.WorkID);
            }
            
        }

        public SensorWorker AddWorker(DataWorker worker)
        {
            SensorWorker sWorker = new SensorWorker(worker.Name);            
            string szIconPath = szMediaPath + "icons\\worker.ico";
            int nID = sWorker.CreateWorker(szIconPath);
            sWorker.SensorID = worker.Sensor;
            worker.SensorWorker = sWorker;
            sWorker.OnVisible(false);
            
            Layers.GetLayer((int)LayerType.Worker).Add(nID);
            m_dicWorker[sWorker.WorkID] = worker;
           
            return sWorker;
        }

        private VehicleType GetVehicleType(DataCar car)
        {
            return VehicleType.TRUCK;
            //string strCarName = car.Name.ToLower();

            //if (strCarName.IndexOf("fork") >= 0 || strCarName.IndexOf("지게차") >= 0)
            //    return VehicleType.FORKLIFT;
            //else if (strCarName.IndexOf("truck") >= 0 || strCarName.IndexOf("트럭") >= 0)
            //    return VehicleType.TRUCK;
            //
            //return VehicleType.OTHER;
        }

        private string GetVehicleIconName(DataCar car)
        {
            string strCarName = car.Name.ToLower();

            if (strCarName.IndexOf("fork") >= 0 || strCarName.IndexOf("지게차") >= 0)
                return "Vehicle.ico";
            else if (strCarName.IndexOf("truck") >= 0 || strCarName.IndexOf("트럭") >= 0)
                return "Vehicle3.ico";

            return  "Vehicle2.ico";
        }

        public SensorVehicle AddVehicle(DataCar car)
        {
            //VehicleType type = GetVehicleType(car);
            VehicleType type = GetVehicleType(car);
            SensorVehicle vehicle = new SensorVehicle(car.Name, type, car.Width / 1000.0f, car.Length / 1000.0f, car.Height / 1000.0f);

            string szIconPath = szMediaPath + GetVehicleIconName(car);
            int nID = vehicle.CreateVehicle(szIconPath);
            vehicle.SensorID = car.Sensor;
            car.SensorVehicle = vehicle;
            vehicle.OnVisible(false);
            Layers.GetLayer((int)LayerType.Vehicle).Add(nID);


            m_dicCars[vehicle.VehicleID] = car;

            return vehicle;
        }

        public SensorVehicle AddGasSensor(string strIconPath, GasSensor sensor)
        {
            VehicleType type = VehicleType.OTHER;
            SensorVehicle vehicle = new SensorVehicle(sensor.SensorName, type, 0.0f, 0.0f, 0.0f);

            int nID = vehicle.CreateVehicle(strIconPath);
            vehicle.SensorID = "GasSensor_" + sensor.GetHashCode().ToString();
            sensor.SensorVehicle = vehicle;

            vehicle.SetLocation(sensor.X, sensor.Y, -sensor.Z);

            if (sensor.SensorName.Length > 0)
                vehicle.SetLOD(2);

            vehicle.OnVisible(true);
            
            Layers.GetLayer((int)LayerType.GasSensor).Add(nID);

            return vehicle;
        }

        public void RemoveVehicle(DataCar car)
        {
            if (car == null)
                return;
            if (car.SensorVehicle == null)
                return;
            SensorVehicle vehicle = car.SensorVehicle;

            if( vehicle != null)
            {
                Layers.GetLayer((int)LayerType.Vehicle).Remove(vehicle.VehicleID);
                vehicle.OnVisible(false);
                vehicle.Delete();
                m_dicCars.Remove(vehicle.VehicleID);
            }            
        }

        public MovingEquipment AddMovingEquipment()
        {
            MovingEquipmentManager equipMan = MovingEquipmentManager.Instance;
            MovingEquipment equipMent = equipMan.GetEquipment(0);

            EquipmentLayer layer = (EquipmentLayer)Layers.GetLayer((int)LayerType.Equipment);
            layer.Add(0, 1);

            return equipMent;
        }

        public void Update3DView()
        {
            RedrawWindow();
        }

        //private bool GetCCTVInfo(string strSection, string strFileKey, string strPosKey, out string strFilePath, out string strCCTVName, out float x, out float y, out float z)
        //{
        //    strCCTVName = "";
        //    x = y = z = 0.0f;
        //    strFilePath = DBConn.GetInValue(strSection, strFileKey);

        //    if (strFilePath.Length == 0)
        //        return false;

        //    string strInfo = DBConn.GetInValue(strSection, strPosKey);

        //    if (strInfo.Length == 0)
        //        return false;

        //    int nIndex = strInfo.LastIndexOf(',');

        //    if (nIndex < 0)
        //        return false;

        //    string strZ = strInfo.Substring(nIndex + 1);
        //    strInfo = strInfo.Substring(0, nIndex);

        //    nIndex = strInfo.LastIndexOf(',');

        //    if (nIndex < 0)
        //        return false;

        //    string strY = strInfo.Substring(nIndex + 1);
        //    strInfo = strInfo.Substring(0, nIndex);

        //    nIndex = strInfo.LastIndexOf(',');

        //    if (nIndex < 0)
        //        return false;

        //    string strX = strInfo.Substring(nIndex + 1);
        //    strCCTVName = strInfo.Substring(0, nIndex);

        //    if (!float.TryParse(strX, out x))
        //        return false;

        //    if (!float.TryParse(strY, out y))
        //        return false;

        //    if (!float.TryParse(strZ, out z))
        //        return false;

        //    return true;
        //}

        private int SetCCTV(string strIconPath, CCTVViewer.CCTV cctv)
        {
            int poiID = -1;

            foreach (KeyValuePair<int, CCTVTooltip> item in m_dicCCTVTooltip)
            {
                if (item.Value.CCTV == cctv)
                {
                    poiID = item.Key;
                    break;
                }
            }

            if (poiID > -1)
            {
                OutdoorView.MovePOI(poiID, cctv.X, cctv.Y, cctv.Z);
            }
            else
            {
                poiID = OutdoorView.CreatePOI(strIconPath, cctv.X, cctv.Y, cctv.Z);

                if (poiID <= 0)
                    return poiID;

                CCTVTooltip tooltip = new CCTVTooltip();

                tooltip.CCTV = cctv;
                tooltip.POIID = poiID;

                m_dicCCTVTooltip[poiID] = tooltip;
            }

            return poiID;
        }

        public void CreateCCTVs()
        {
            string strIconPath = DBConn.GetInValue("CCTV", "icon_path");

            if (strIconPath.Length == 0)
                return;

            if (strIconPath.StartsWith("\\"))
                strIconPath = FormContent.EnginPath() + strIconPath;
            else
                strIconPath = FormContent.EnginPath() + "\\" + strIconPath;

            foreach (CCTVViewer.CCTV cctv in CCTVManager.Instance.DicCCTVList.Values)
            {
                SetCCTV(strIconPath, cctv);
            }


            List<int> liRemovePOI = new List<int>();
            foreach (KeyValuePair<int, CCTVTooltip> item in m_dicCCTVTooltip)
            {
                if (CCTVManager.Instance.DicCCTVList.Values.Contains(item.Value.CCTV) == false)
                {
                    liRemovePOI.Add(item.Key);
                }
            }

            foreach (int nPOI in liRemovePOI)
            {
                if (m_dicCCTVTooltip[nPOI].FormCCTV != null)
                    m_dicCCTVTooltip[nPOI].FormCCTV.Close();

                m_dicCCTVTooltip.Remove(nPOI);

                OutdoorView.RemovePOI(nPOI);
            }

        }

        private void SetAP(string strIconPath, APData ap)
        {
            Core.AP _ap = new Core.AP(ap.APName);

            int nID = _ap.CreateAP(strIconPath);
            _ap.SetLocation(ap.X, ap.Y, ap.Z);

            if (ap.APName.Length > 0)
                _ap.SetLOD(2);

            _ap.OnVisible(true);

            Layers.GetLayer((int)LayerType.AP).Add(nID);
            m_dicAP[_ap.WorkID] = ap;
        }

        public void CreateAPs()
        {
            if (APData.IconPath.Length == 0)
            {
                string strIconPath = DBConn.GetInValue("AP", "icon_path");

                if (strIconPath.Length == 0)
                    return;

                if (strIconPath.StartsWith("\\"))
                    strIconPath = FormContent.EnginPath() + strIconPath;
                else
                    strIconPath = FormContent.EnginPath() + "\\" + strIconPath;

                APData.IconPath = strIconPath;
            }

            foreach (APData ap in FormMain.Instance.DataMgr.DicAPs.Values)
            {
                SetAP(APData.IconPath, ap);
            }
        }

        public void CreateGasSensors()
        {
            if (GasSensor.IconPath.Length == 0)
            {
                string strIconPath = DBConn.GetInValue("GASSensor", "icon_path");

                if (strIconPath.Length == 0)
                    return;

                if (strIconPath.StartsWith("\\"))
                    strIconPath = FormContent.EnginPath() + strIconPath;
                else
                    strIconPath = FormContent.EnginPath() + "\\" + strIconPath;

                GasSensor.IconPath = strIconPath;
            }

            foreach (GasSensor sensor in FormMain.Instance.DataMgr.DicGasSensors.Values)
            {
                AddGasSensor(GasSensor.IconPath, sensor);
            }
        }

        public void OnSelectedPOI(int nPOIID)
        {
            if (m_dicCCTVTooltip.ContainsKey(nPOIID))
            {
                if (!m_frmWorkerInfo.IsDisposed)
                {
                    if (m_frmWorkerInfo.Visible)
                        m_frmWorkerInfo.Hide();
                }

                foreach (KeyValuePair<int, CCTVTooltip> item in m_dicCCTVTooltip)
                {
                    if (nPOIID == item.Key)
                    {
                        if (item.Value.FormCCTV == null || item.Value.FormCCTV.Visible == false)
                        {
                            item.Value.Show();
                        }
                        else
                        {
                            if (item.Value.FormCCTV.WindowState == FormWindowState.Minimized)
                                item.Value.FormCCTV.WindowState = FormWindowState.Normal;
                        }

                        item.Value.FormCCTV.BringToFront();

                    }
                    else
                    {
                        if (item.Value != null)
                        {
                            item.Value.Hide();
                        }
                    }
                }
            }
            else
            {

                int nCarPOI = nPOIID - 2;
                int nWorkPOI = nPOIID + 3;


                if (m_dicWorker.ContainsKey(nWorkPOI))
                {
                    if (!m_frmVehicleInfo.IsDisposed)
                    {
                        if (m_frmVehicleInfo.Visible)
                            m_frmVehicleInfo.Hide();
                    }

                    DataWorker worker = m_dicWorker[nWorkPOI];

                    if (m_frmWorkerInfo.IsDisposed)
                        m_frmWorkerInfo = new PopupDialog.FormWorkerInfo();

                    m_frmWorkerInfo.Worker = worker;

                    if (!m_frmWorkerInfo.Visible)
                        m_frmWorkerInfo.Show(this);
                }
                else if (m_dicCars.ContainsKey(nCarPOI))
                {

                    if (!m_frmWorkerInfo.IsDisposed)
                    {
                        if (m_frmWorkerInfo.Visible)
                            m_frmWorkerInfo.Hide();
                    }

                    DataCar car = m_dicCars[nCarPOI];

                    if (m_frmVehicleInfo.IsDisposed)
                        m_frmVehicleInfo = new PopupDialog.FormVehicleInfo();

                    m_frmVehicleInfo.Worker = car;

                    if (!m_frmVehicleInfo.Visible)
                        m_frmVehicleInfo.Show(this);
                }
                else
                {
                    if (!m_frmWorkerInfo.IsDisposed)
                    {
                        if (m_frmWorkerInfo.Visible)
                            m_frmWorkerInfo.Hide();
                    }
                    if (!m_frmVehicleInfo.IsDisposed)
                    {
                        if (m_frmVehicleInfo.Visible)
                            m_frmVehicleInfo.Hide();
                    }
                }
            }
        }

        public FormVirtualCCTV GetCurrentCCTV()
        {
            foreach (KeyValuePair<int, CCTVTooltip> pair in m_dicCCTVTooltip)
            {
                if (pair.Value.FormCCTV != null && !pair.Value.FormCCTV.IsDisposed && pair.Value.FormCCTV.Visible)
                    return pair.Value.FormCCTV;
            }

            return null;
        }
    }

    public class SensorWorker : Worker
    {
        private string m_strSensorID = "";

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        public SensorWorker()
            : base()
        {
        }

        public SensorWorker(string szName)
            : base(szName)
        {
        }
    }

    public class SensorVehicle : Vehicle
    {
        private string m_strSensorID = "";

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        public SensorVehicle()
            : base()
        {
        }

        // fWidth, fLength, fHeight : 단위(m)
        public SensorVehicle(string szName, VehicleType type, float fWidth, float fLength, float fHeight)
            : base(szName, type, fWidth, fLength, fHeight)
        {
        }
    }
}
