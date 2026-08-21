using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Core;
using UnE.Geometry;
using UnE.SOP;
using UnE.SOP.Workstate;

namespace SDMS
{
	public partial class FormContent : Form, IDisasterContainer
	{
        public void HideAllShelter() { }

        public void ShowShelter(int nType) { }

		private class TimerEvent
		{
			public enum EventType { WAIT_FINISH_BROADCAST = 0, CHECK_SYNC, NONE };

			private EventType m_type = EventType.NONE;
			private int m_nData = 0;

			public EventType Type
			{
				get { return m_type; }
				set { m_type = value; }
			}

			public int Data
			{
				get { return m_nData; }
				set { m_nData = value; }
			}

			public TimerEvent()
			{
			}

			public TimerEvent(EventType type, int nData)
			{
				m_type = type;
				m_nData = 0;
			}
		}

		// 1(Outside), 2(Both), 3(Inside)
		private int m_nLayout = 1;

		public int NumLayout
		{
			get { return m_nLayout; }
			set { SetLayoutMode(value); }
		}

		private Core.LayerManager m_layerOutside = null;

		public Core.LayerManager Layers
		{
			get { return m_layerOutside; }
		}

        private BaseViewEx mView1 = null;
        private BaseViewEx mView2 = null;

		private Core.Engine mEngine = new Core.Engine();

		private string m_strZipFileFolderPath = "";
		private string m_strOutsideDAE = "";
		private string m_strInsideDAE = "";
		private Building m_buildingCurrent = null;
		private Dictionary<string, string> m_dicInsideDAE = null;
		private string m_strOutDaeName = "";

		private ArrayList mViewList = new ArrayList();
        private BaseViewEx mCurrent = null;
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

		private bool m_bEditMode = false;

		public bool EditMode
		{
			get { return m_bEditMode; }
			set
			{
				m_bEditMode = value;
                ((BaseViewEx)mView1).EditMode = value;
                ((BaseViewEx)mView2).EditMode = value;
			}
		}

		private SplitContainer m_LayerContainer = null;

        public MouseWorkMode CurrentMouseWorkMode
		{
            get { return ((BaseViewEx)mView1).CurrentMouseWorkMode; }
			set
			{
                ((BaseViewEx)mView1).CurrentMouseWorkMode = value;
                ((BaseViewEx)mView2).CurrentMouseWorkMode = value;
			}
		}

		private Core.SceneManager mSceneManager = null;

		public Core.SceneManager SceneManager
		{
			get { return mSceneManager; }
			set { mSceneManager = value; }
		}

		private Core.ZoneVolumeManager mVolmumeManagerOut = null;

		public Core.ZoneVolumeManager OutdoorVolmumeManager
		{
			get { return mVolmumeManagerOut; }
			set { mVolmumeManagerOut = value; }
		}

		private Core.ZoneVolumeManager mVolmumeManagerIn = null;

		public Core.ZoneVolumeManager IndoorVolmumeManager
		{
			get { return mVolmumeManagerIn; }
			set { mVolmumeManagerIn = value; }
		}

        public BaseViewEx OutdoorView
		{
			get { return mView1; }
		}

        public BaseViewEx IndoorView
		{
			get { return mView2; }
		}

		private bool m_bFirstBothSide = true;

		private string m_strSimulationBroadcastResultFilePath = "";

		// 연습모드용 방송이 끝나기를 기다리는 Timer의 최대 대기시간(10분)
		private int m_nSimulationBroadcastTimerWaitTime = 600;

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			int i = 0;
			i++;
		}

		public override void Refresh()
		{
			RedrawWindow();
		}
        
        public void ShowCCTVForm(bool bShow)
        {

        }

		public FormContent()
		{
			szMediaPath = FormMain.EnginPath() + "Media\\";
			szIconPath = szMediaPath + "icons\\화재.ico";

			Create3DView();

			InitializeComponent();

			mView2.Visible = false;
			mView1.Dock = DockStyle.Fill;
			mView1.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			MouseWheel += new MouseEventHandler(OnMouseWheel);

            CurrentMouseWorkMode = MouseWorkMode.ORBIT;
			m_strSimulationBroadcastResultFilePath = Application.StartupPath + "\\FinishSimulationBroadcast.txt";
		}

		private void Create3DView()
		{
			m_LayerContainer = new SplitContainer();
			m_LayerContainer.Dock = DockStyle.Fill;
			m_LayerContainer.Visible = false;

			//Controls.Add(m_LayerContainer);

            mView1 = new BaseViewEx(this);// new Core.BaseView();
			mView1.BackColor = System.Drawing.Color.Transparent;
			mView1.Dock = System.Windows.Forms.DockStyle.Fill;
			mView1.Location = new System.Drawing.Point(0, 0);
			mView1.Name = "m3DView1";
			mView1.Size = new System.Drawing.Size(1920, 1080);
			mView1.TabIndex = 0;
			mView1.Click += new System.EventHandler(this.View1_Click);

            mView2 = new BaseViewEx(this, true);// new Core.BaseView();
			mView2.BackColor = System.Drawing.Color.Transparent;
			mView2.Dock = System.Windows.Forms.DockStyle.Fill;
			mView2.Location = new System.Drawing.Point(0, 0);
			mView2.Name = "panel2";
			mView2.Size = new System.Drawing.Size(1920, 1080);
			mView2.TabIndex = 0;
			mView2.Click += new System.EventHandler(this.View2_Click);

			Controls.Add(mView1);
			Controls.Add(mView2);

			m_layerOutside = new Core.LayerManager(mView1);

			m_layerOutside.AddLayer(ID.ID_LAYER_DETECTOR, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_COOLER, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_PERSURE, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_CCTV, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_FIREEXT, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_FIREHYD, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_ALARMSTA, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_RECIVER, false);
			m_layerOutside.AddLayer(ID.ID_LAYER_TEXTPOI, true);
			m_layerOutside.AddLayer(ID.ID_LAYER_CCTVLOW, false);
            m_layerOutside.AddLayer(ID.ID_LAYER_CCTV_DISCONNECTED, false);

			mSceneManager = new Core.SceneManager(mView1);

			mVolmumeManagerOut = new Core.ZoneVolumeManager(mView1);
			mVolmumeManagerIn = new Core.ZoneVolumeManager(mView2);

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
			if (mView1 != null)
				mView1.ProcessCCTVLOD();
		}

		public void Init3DView()
		{
			if (bInit == true)
				return;
			bInit = true;
			string szPath = FormMain.EnginPath();

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
				mView2.InitBaseView();
			}
			catch (System.Exception ex2)
			{
				Debug.WriteLine(ex2.StackTrace);
			}

            bool bSimMode = UnE.SOP.ProxySOP.Instance.SimulationMode;

			m_strOutDaeName = m_strZipFileFolderPath + "outside\\ND_0326l.DAE";
            if (!File.Exists(m_strOutDaeName) || (bSimMode  || ModelManager.Instance.ExtractOutside == true))
			{
				try
				{
					ExtractToTrg(m_strOutsideDAE, m_strZipFileFolderPath + "outside\\");
					//mView1.ExtractFile(m_strOutsideDAE, m_strZipFileFolderPath);
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.StackTrace);
				}
			}

			try
			{
				mView1.OpenMesh(m_strOutDaeName);
				mView1.OnViewFix();
			}
			catch (System.Exception)
			{
			}

			// open floor mesh
			string szFloorFile = m_strZipFileFolderPath + "inside\\A-1_1.DAE";
			if (!File.Exists(szFloorFile) || (bSimMode  || ModelManager.Instance.ExtractInside == true))
			{
				try
				{
					ExtractToTrg(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");
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
			mView1.SetIconPOISize(64.0f, 64.0f);
			LayoutOutside();
			AddGroupName();
			AddBuildingName();
			LoadPOIs();
			AddZoneVolume();
			mView1.UpdateWindow();
			Button b = new Button();
			b.Size = new Size(1, 1);
			mView1.Controls.Add(b);
			b.Show();

			Button b2 = new Button();
			b2.Size = new Size(1, 1);
			mView2.Controls.Add(b2);
			b2.Show();

			//mView1.CreateCompass(0.0f);
		}

		private void FormLayout_Load(object sender, EventArgs e)
		{
			//Init3DView();
		}

		private bool ExtractToTrg(string strSrcFile, string strTrgPath)
		{
			try
			{
                if (Directory.Exists(strTrgPath))
				    BackupManager.DeleteFolder(strTrgPath);

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

		private void LoadPOIs()
		{
            FormMain.Instance.DataManager.LoadPOI((BaseViewEx)mView1, false);
            FormMain.Instance.DataManager.LoadPOI((BaseViewEx)mView2, true);

			CCTVManager.Instance.LoadEquipZoneCCTV();
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

		private void View2_Click(object sender, EventArgs e)
		{
			mCurrent = mView2;
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
			if (isIndoor)
			{
				if (mView2.MeshOpened)
				{
					mView2.OnViewTop();
					mView2.ZoomTarget(new Position3D(x, y, z), 20.0f);
					mView2.Update();
				}
			}
			else
			{
				//if (mView1 != null)
				mView1.OnViewTop();
				mView1.ZoomTarget(new Position3D(x, y, z), 100.0f);
				mView1.Update();
				//mView1.Refresh();
			}
		}

		public void SelectPOI(POI poi, bool isIndoor)
		{
			if (isIndoor)
			{
				if (mView2.MeshOpened)
					mView2.SelectPOI(poi.ID);
			}
			else
			{
				mView1.SelectPOI(poi.ID);
			}
		}

        public void SelectPOILoadZone(POI poi, bool isIndoor)
        {
            if (isIndoor)
            {
                //SetCurrentBuilding(poi.Zone.Building, poi.Zone);
                if (mView2.MeshOpened)
                    mView2.SelectPOI(poi.ID);
            }
            else
            {
                mView1.SelectPOI(poi.ID);
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

			mCurrent = mView2;
			mView2.Dock = DockStyle.Fill;
			mView2.Visible = true;

			mView1.Dock = DockStyle.Fill;
			mView1.Visible = true;
			mView1.Invalidate(true);
			mView2.Invalidate(true);

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

			mCurrent = mView2;

			mView1.Visible = false;

			mView2.Dock = DockStyle.Fill;
			mView2.Visible = true;
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
				Controls.Add(mView1);
			}

			if (m_LayerContainer.Panel2.Controls.Contains(mView2))
			{
				m_LayerContainer.Panel2.Controls.Remove(mView2);
				Controls.Add(mView2);
			}

			if (Controls.Contains(m_LayerContainer))
				Controls.Remove(m_LayerContainer);

			mCurrent = mView1;

			mView2.Visible = false;

			mView1.Visible = true;
			mView1.Dock = DockStyle.Fill;
			mView1.Invalidate(true);

			m_buildingCurrent = null;
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
						Core.ZonePolygon area = new Core.ZonePolygon(mView1);
						int count = zone.Polygon.GetVertexCount();
						for (int i = 0; i < count; i++)
						{
							UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
							float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
							area.AddVertex(new Position3D(pos3DX, 0, pos3DZ));
						}
						area.Height = 0;
						area.CreatePolygon();
						Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, 20, zone.Building.BroadcastName);
						if (volume != null)
							volume.SetVisible(true);
						Vertex2D pos2 = zone.Polygon.CalcWeightCenter();
						float pos3DX2 = ((float)pos2.x - ZoneManager.Instance.Dx);
						float pos3DZ2 = ZoneManager.Instance.Dy - (float)pos2.y;

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
					float pos3DX = 0.0f;
					float pos3DZ = 0.0f;

					string szName = string.Format("{0} [{1}]", zone.ZoneName, m_szFileName);
					int nID = mView2.AddZoneName(szName, pos3DX, 10.0f, pos3DZ);
					//m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
				}
				catch (System.Exception ex)
				{
					ConnectionLogEx.Instance.WriteLine(ex.StackTrace);
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
					try
					{
						float pos3DX = (obj.TextCenterX - ZoneManager.Instance.Dx);
						float pos3DZ = ZoneManager.Instance.Dy - obj.TextCenterY;

						int nID = mView1.AddGroupName(obj.BuildingGroupName, pos3DX, 100.0f, pos3DZ);
						//m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
					}
					catch (System.Exception)
					{
					}
				}
			}
		}

		public Core.ZoneVolume ShowZoneVolume(int zoneID, bool bOutDoorWnd, bool bShow)
		{
			Zone zone = ZoneManager.Instance.GetZone(zoneID);
			if (zone == null)
				return null;

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
				Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
				mCurrentOutdoorVolume = volume;
				volume.SetVisible(bShow);
				return volume;
			}
			else
			{
				if (zone.IsOutdoor == false)
				{
					m_bChangeIndoor = false;
					string szID = zone.ZoneName;
					mVolmumeManagerIn.ClearAll();

					mCurrentIndoorVolume = mVolmumeManagerIn.FindZoneVolume(szID);
					if (mCurrentIndoorVolume == null)
					{
						Core.ZonePolygon area = new Core.ZonePolygon(mView2);
						UnE.Geometry.Vertex2D posCenter = zone.Polygon.CalcWeightCenter();
						int count = zone.Polygon.GetVertexCount();
						for (int i = 0; i < count; i++)
						{
							//UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							float pos3DX = (float)(pos.x - posCenter.x);
							float pos3DZ = (float)(posCenter.y - pos.y);
							area.AddVertex(new Position3D(pos3DX, 0.0f, pos3DZ));
						}
						area.Height = 0.0f;
						area.CreatePolygon();
						mCurrentIndoorVolume = mVolmumeManagerIn.CreateZoneVolume(mView2, area, 3.0f, szID);
					}
					if (mCurrentIndoorVolume != null)
						mCurrentIndoorVolume.SetVisible(bShow);
					return mCurrentIndoorVolume;
				}
			}
			return null;
		}

		public Core.ZoneVolume ShowZoneVolume(int zoneID, int nEquipZoneID, bool bOutDoorWnd, bool bShow)
		{
			Zone zone = ZoneManager.Instance.GetZone(zoneID);
			if (zone == null)
				return null;

			EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
			if (equipZone == null)
				return null;

			if (bOutDoorWnd == true)
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
						Core.ZoneVolume lvolume = mVolmumeManagerOut.FindZoneVolume(szLinkID);
						lvolume.SetVisible(bShow);
					}
				}

				string szID = "";
				if (zone.IsOutdoor == false)
				{
					szID = zone.Building.BuildingID;
				}
				else
				{
					szID = zone.ZoneName;
				}
				Core.ZoneVolume volume = mVolmumeManagerOut.FindZoneVolume(szID);
				mCurrentOutdoorVolume = volume;
				volume.SetVisible(bShow);
				return volume;
			}
			else
			{
				if (equipZone.IsOutdoor == false)
				{
					m_bChangeIndoor = false;
					string szID = equipZone.ZoneName;
					mVolmumeManagerIn.ClearAll();

					mCurrentIndoorVolume = mVolmumeManagerIn.FindZoneVolume(szID);
					if (mCurrentIndoorVolume == null)
					{
						Core.ZonePolygon area = new Core.ZonePolygon(mView2);
						UnE.Geometry.Vertex2D posCenter = zone.Polygon.CalcWeightCenter();
						int count = equipZone.Polygon.GetVertexCount();
						for (int i = 0; i < count; i++)
						{
							//UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							UnE.Geometry.Vertex2D pos = equipZone.Polygon.GetVertex(i);
							float pos3DX = (float)(pos.x - posCenter.x);
							float pos3DZ = (float)(posCenter.y - pos.y);
							area.AddVertex(new Position3D(pos3DX, 0.0f, pos3DZ));
						}
						area.Height = 0.0f;
						area.CreatePolygon();
						mCurrentIndoorVolume = mVolmumeManagerIn.CreateZoneVolume(mView2, area, 3.0f, szID);
					}
					if (mCurrentIndoorVolume != null)
						mCurrentIndoorVolume.SetVisible(bShow);
					return mCurrentIndoorVolume;
				}
			}
			return null;
		}

		private bool m_bChangeIndoor = false;

		public void HideZoneVolume()
		{
			if (m_bChangeIndoor == false && mCurrentIndoorVolume != null)
				mCurrentIndoorVolume.SetVisible(false);
			if (mCurrentOutdoorVolume != null)
				mCurrentOutdoorVolume.SetVisible(false);

			mVolmumeManagerOut.SetVisibleAll(false);
			mVolmumeManagerIn.SetVisibleAll(false);

			m_bChangeIndoor = false;
		}

		private Core.ZoneVolume mCurrentOutdoorVolume = null;
		private Core.ZoneVolume mCurrentIndoorVolume = null;

		public void AddZoneVolume()
		{
			ArrayList arBuildings = new ArrayList();
			foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicZones)
			{
				Zone zone = pair.Value;

				if (zone != null)
				{
					if (zone.Building != null)
					{
						string szID = zone.Building.BuildingID;
						if (arBuildings.Contains(szID))
							continue;
						arBuildings.Add(szID);
						Core.Scene scene = mSceneManager.FindSceneNodeByAliasName(szID);
						if (scene != null)
						{
							float fHeight1 = scene.GetMinimum().Y - 0.1f;
							float fHeight2 = scene.GetMaximum().Y + 0.1f;

							Core.ZonePolygon area = new Core.ZonePolygon(mView1);
							int count = zone.Polygon.GetVertexCount();
							for (int i = 0; i < count; i++)
							{
								UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
								float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
								float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
								area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
							}
							area.Height = fHeight1;
							area.CreatePolygon();
							Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID);
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
						Core.ZonePolygon area = new Core.ZonePolygon(mView1);
						int count = zone.Polygon.GetVertexCount();
						for (int i = 0; i < count; i++)
						{
							UnE.Geometry.Vertex2D pos = zone.Polygon.GetVertex(i);
							float pos3DX = (float)(pos.x - ZoneManager.Instance.Dx);
							float pos3DZ = (float)(ZoneManager.Instance.Dy - pos.y);
							area.AddVertex(new Position3D(pos3DX, fHeight1, pos3DZ));
						}
						area.Height = fHeight1;
						area.CreatePolygon();
						try
						{
							Core.ZoneVolume volume = mVolmumeManagerOut.CreateZoneVolume(mView1, area, fHeight2, szID);
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
					try
					{
						int nID = mView1.AddAliasName(obj.BuildingID, obj.BroadcastName);
						m_layerOutside.GetLayer(ID.ID_LAYER_TEXTPOI).Add(nID);
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
				mView2.RedrawScene();
			}
		}

		public void Invalidate3DView(bool bErBack)
		{
			if (mView1 != null && mView1.Visible == true)
			{
				mView1.Invalidate(bErBack);
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

		private string m_szFileName = "";
		//private string m_szFullPath = "";

		public void ShowIndoor(Zone zone)
		{
			if (m_buildingCurrent != null)
			{
				//if (FormModelLoading.iForm.Visible == true)
				//{
				//    Thread.Sleep(500);
				//}

				Floor floor = zone.Floor;
				m_currentIndoorZone = zone;
				string szCode = m_buildingCurrent.BuildingID;
				if (szCode == "")
					return;

				float nFloor = floor.FloorIndex + 1;
				if (szCode[0] >= '0' && szCode[0] <= '9')
				{
					szCode = "z" + szCode;
				}
				nCurrentFloor = floor.FloorIndex;

				string szFileName = null;

				if (floor.FloorIndex < 0)
					szFileName = string.Format("{0}_B{1:f1}", szCode, -nCurrentFloor);
				else
					szFileName = string.Format("{0}_{1:f1}", szCode, nFloor);

				if (szFileName.EndsWith(".0"))
					szFileName = szFileName.Substring(0, szFileName.Length - 2);

				m_szFileName = szFileName;

				if (szFileName[szFileName.Length - 2] == '.')
				{
					szFileName += "M.dae";

					m_szFileName += "M";
				}
				else
				{
					szFileName += ".dae";
				}
				// find dae
                string szFullPath = m_strZipFileFolderPath + "inside\\" + szFileName;
                //MessageBox.Show(szFullPath);
				szInsideFullPath = szFullPath;
				if (szPrevFileName == szFullPath)
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
					mView2.UpdateWindow();
					szPrevFileName = "";
					return;
				}

				m_nCurrentFloor = floor.FloorIndex;
				m_bChangeIndoor = true;
				try
				{
					if (bExist == true)
					{
						if (FormModelLoading.iForm.Visible == false)
						{
							FormModelLoading.iForm.ThreadModal(this);
							FormModelLoading.iForm.ShowDialog(this);
						}
					}
				}
				catch (System.Exception)
				{
				}
			}
		}

		public void OpenModel()
		{
			if (szInsideFullPath != null)
			{
                ((BaseViewEx)mView2).OpenMesh(szInsideFullPath, m_currentIndoorZone);

				// 테스트
				//AddZoneName(m_currentIndoorZone);

				mView2.OnViewHome();

				m_bLoadInsideMode = true;

				// 테스트
				//ShowZoneVolume(m_currentIndoorZone.ID, false, true);

				mView2.UpdateWindow();

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

			mView1.ShowLayer(id, bShow);
			mView2.ShowLayer(id, bShow);

			mView1.UpdatePOI();
			mView2.UpdatePOI();

			RedrawWindow();

			return false;
		}

        public void AttachView(BaseViewEx view, bool isOutdoor)
		{
			if (isOutdoor)
				mView1 = view;
			else
				mView2 = view;

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

        public BaseViewEx DetachView(bool isOutdoor)
		{
            BaseViewEx view = null;

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
				FormMain.Instance.SelectIndoorZone(zone);
			}
		}

		private Zone m_ManualClickZone = null;

		public SDMS.Zone ManualClickZone
		{
			get { return m_ManualClickZone; }
			set { m_ManualClickZone = value; }
		}

		public void FireSensorReport_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			object obj = item.Tag;
			if (obj == null)
				return;

			string strFireZoneName = "";

			if (obj.GetType() == typeof(Building))
			{
				Building building = (Building)obj;
				strFireZoneName = building.BroadcastName;
			}
			else
			{
				Zone outerZone = (Zone)obj;

				ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(outerZone);

				if (arrEquipZones != null)
				{
					foreach (EquipmentZone equipZone in arrEquipZones)
					{
						if (SensorManager.Instance.DicSensorZone.ContainsKey(equipZone.ID))
						{
							EquipmentZoneObjectList list = SensorManager.Instance.DicSensorZone[equipZone.ID];

							if (list.SensorList != null && list.SensorList.Count > 0)
							{
                                if (GetSMSReportConfig())
								//if (GetSMSConfig())
								{
									// SOPServer를 이용하여 화재탐지 상황을 지정된 담당자들에게 문자로 보낸다.
									SendSimulationSMS(FormBroadcastConfig.SituationType.DETECT_FIRE, equipZone.ID);
								}

								ReactionLog log = new ReactionLog();
								log.ID = log.SensorHistoryID = log.GetHashCode();

								SensorZone sensor = (SensorZone)list.SensorList[0];
								ProcessIF process = ProcessManager.Instance.BeginProcess(sensor, log, ProcessType.FireAlarm);
								ProcessSimulationLog(process, sensor, log);

								if (process.TargetZone != null)
									strFireZoneName = process.TargetZone.DisplayText;
								break;
							}
						}
					}
				}
			}
		}

        private bool GetSMSReportConfig()
        {
            return FormSMSConfig.UseSMSOnReportFire;
        }
		/*private bool GetSMSConfig()
		{
			return FormSMSConfig.UseSMSOnDetectFire;
		}*/

		// strBeginTag와 strEndTag로 둘러쌓인 부분을 제거한 문자열을 리턴한다.
		// strFullMessage : strBeginTag와 strEndTag를 포함한 문자열
		public static string GetMessage(string strOriginMessage, string strBeginTag, string strEndTag, out string strFullMessage)
		{
			int nLen = strOriginMessage.Length;
			int nIndex = 0;

			string strMessage = "";
			strFullMessage = "";
			int nBeginTagLength = strBeginTag.Length;
			int nEndTagLength = strEndTag.Length;

			while (nIndex < nLen)
			{
				int nIndex1 = strOriginMessage.IndexOf(strBeginTag, nIndex);

				if (nIndex1 < 0)
				{
					strFullMessage += strOriginMessage.Substring(nIndex);
					strMessage += strOriginMessage.Substring(nIndex);
					break;
				}

				int len = nIndex1 - nIndex;

				if (len > 0)
				{
					strFullMessage += strOriginMessage.Substring(nIndex, len);
					strMessage += strOriginMessage.Substring(nIndex, len);
				}

				int nIndex2 = strOriginMessage.IndexOf(strEndTag, nIndex1 + nBeginTagLength);

				if (nIndex2 < 0)
				{
					strFullMessage += strOriginMessage.Substring(nIndex);
					strMessage += strOriginMessage.Substring(nIndex1);
					break;
				}

				len = nIndex2 - (nIndex1 + nBeginTagLength);

				if (len > 0)
					strFullMessage += strOriginMessage.Substring(nIndex1 + nBeginTagLength, len);

				nIndex = nIndex2 + nEndTagLength;
			}

			return strMessage;
		}

		public static string GetBroadcastMessage(string strOriginMessage, string strFireZoneName, int nRepeatCount)
		{
			string szBroadcastMessage;
			string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

			for (int j = 0; j < nRepeatCount; j++)
			{
				szBroadcastMessage += "...\n다시한번 알려드립니다...";
				szBroadcastMessage += strRepeatMessage;
			}

			szBroadcastMessage = szBroadcastMessage.Replace("●", strFireZoneName);
			return szBroadcastMessage;
		}

		private void ProcessSimulationLog(ProcessIF process, SensorZone sensor, ReactionLog log)
		{
			if (process == null)
				return;

			if (process.GetType() != typeof(FireDetectProcess))
				return;

			log.LogTime = DateTime.Now;
			log.Parameter1 = sensor.EquipZoneID.ToString();
			log.Parameter2 = sensor.ID.ToString();
			log.ReactionType = (int)ReactionType.BEGIN_STATUS;
			log.SensorHistoryID = 0;

			EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);

			if (equipZone != null)
			{
				log.Message = "[" + equipZone.DisplayText + "]에서 화재가 탐지 되었습니다.";
			}

			FireDetectProcess fProcess = (FireDetectProcess)process;
			fProcess.LastLog = log;

			ReactionLogManager.Instance.ProcessLog(log, true);
		}

		public void ManualReport_Click(object sender, EventArgs e)
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
				FormMain.Instance.EnableFireReportBtn(true, 2);
				// outdoor zone
			}
		}

		public void ManualCCTV_Click(object sender, EventArgs e)
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

				ArrayList arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(m_ManualClickZone);
				if (arEquipzone != null && arEquipzone.Count > 0)
				{
					EquipmentZone equipZone = (EquipmentZone)arEquipzone[0];
					PageBackstageHome.Instance.ShowBigCCTV(equipZone, false);
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

			BaseView view1 = (BaseView)mView1;
			view1.SetCheckPoistion(mCheckPosition);

			BaseView view2 = (BaseView)mView2;
			view2.SetCheckPoistion(mCheckPosition);
		}

		public void OnCheckEnd(bool bResult)
		{
			if (mFormPosition == null)
				return;
			mFormPosition.OnCheckPositionEnd -= OnCheckEnd;
			LastPos = mFormPosition.LastPosition;
			mCheckPosition = false;
			szIconPath = szMediaPath + "icons\\" + mFormPosition.DisasterName + ".ico";

			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
				BaseView view2 = (BaseView)mView1;
				BaseView view1 = (BaseView)mView2;
				view1.SetCheckPoistion(mCheckPosition);
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
					if (bResult == true)
					{
						if (LastPos != null)
						{
							float dx = 120894.0548f + 1008.531f;
							float dy = 157659.0963f - 506.251f;
							float ox = LastPos.X - dx;
							float oz = dy - LastPos.Z;
							int nID = view1.AddPOI(szIconPath, ox, LastPos.Y, oz);
							LastPos.IconID = nID;
						}
						else
							view1.AddPOI(szIconPath);
						view1.UpdateWindow();
					}
				}
			});
			mFormPosition = null;
		}

		public void RemoveDisasterPos()
		{
			BaseView view1 = (BaseView)mView1;
			if (view1 != null)
			{
				if (LastPos != null)
				{
					float dx = 120894.0548f + 1008.531f;
					float dy = 157659.0963f - 506.251f;
					float ox = LastPos.X - dx;
					float oz = dy - LastPos.Z;
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

			BaseView view2 = (BaseView)mView2;
			if (view2 != null)
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
						view2.RemovePOI(LastPos.X, LastPos.Y, LastPos.Z);
					}
					view2.UpdateWindow();
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
				BaseView view2 = (BaseView)mView2;
				if (view2 != null && zone != null)
				{
					UnE.Geometry.Vertex2D pos = zone.Polygon.CalcWeightCenter();
					float dx = 120894.0548f + 1008.531f;
					float dy = 157659.0963f - 506.251f;
					float ox = x - dx + (float)pos.x;
					float oz = dy - z + (float)pos.y;
					int nID = view2.AddPOI(path, x, y, z);
					LastPos.IconID = nID;
					view2.UpdateWindow();
				}
				RedrawWindow();
			}
			else
			{
				string path = szMediaPath + "icons\\" + disastertype + ".ico";
				BaseView view1 = (BaseView)mView1;
				if (view1 != null)
				{
					float dx = 120894.0548f + 1008.531f;
					float dy = 157659.0963f - 506.251f;
					float ox = x - dx;
					float oz = dy - z;
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
					szSelectedName = m_buildingCurrent.BroadcastName;
					int nIdx = szSelectedName.IndexOf('*');
					if (nIdx != -1)
					{
						szSelectedName = szSelectedName.Substring(0, nIdx);
					}

					string szResult = null;
					if (m_nCurrentFloor < 0)
					{
						szResult = string.Format("{0} B{1}층", szSelectedName, System.Math.Abs(m_nCurrentFloor));
					}
					else
					{
						szResult = string.Format("{0} {1}층", szSelectedName, m_nCurrentFloor);
					}

					if (mFormPosition != null && mFormPosition.IsHandleCreated())
					{
						mLastPos = new HistoryDisasterPosition();
						mLastPos.PoistionName = szResult;

						mLastPos.X = pos3D.X;
						mLastPos.Y = pos3D.Y;
						mLastPos.Z = pos3D.Z;
						mLastPos.FloorIndex = m_nCurrentFloor;

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

					bool isBuildingName = false;
					Building curBuilding = null;
					if (szSelectedName != null && szSelectedName != "")
					{
						curBuilding = ZoneManager.Instance.GetBuilding(szSelectedName);
						if (curBuilding != null)
						{
							szSelectedName = curBuilding.BroadcastName;
							isBuildingName = true;
						}
					}

					if (isBuildingName == false)
					{
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
				if (process.ProcessName == strProcessName)
				{
                    try
                    {
                        process.Kill();
                    }
                    catch(Exception ex)
                    {
                    }
					
					//break;
				}
			}
		}

		public void RunSimulationTimer(string strSiren, string strServerName, string strPort, string strMessage)
		{
			KillProcess("TTSSimulator");

			System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
			info.Arguments = strSiren + " 1 " + strServerName + " " + strPort + " " + strMessage + " \"" + m_strSimulationBroadcastResultFilePath + "\"";
			info.CreateNoWindow = true;
			info.FileName = Application.StartupPath + "\\TTSSimulator.exe";

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo = info;

			process.Start();

			// 연습모드용 방송이 끝나기를 기다리는 Timer 동작
			if (timerSimulation.Tag != null)
			{
				if (timerSimulation.Tag is string)
					timerSimulation.Tag = null;
				else
					return;
			}

			if (System.IO.File.Exists(m_strSimulationBroadcastResultFilePath))
				System.IO.File.Delete(m_strSimulationBroadcastResultFilePath);

			timerSimulation.Tag = new TimerEvent(TimerEvent.EventType.WAIT_FINISH_BROADCAST, 0);
			timerSimulation.Start();
		}

        private void timerSimulation_Tick(object sender, EventArgs e)
		{
			if (timerSimulation.Tag == null)
			{
				timerSimulation.Stop();
			}
			else
			{
				if ((timerSimulation.Tag is TimerEvent) == false)
				{
					timerSimulation.Stop();
					timerSimulation.Tag = null;
					return;
				}

				TimerEvent tEvent = (TimerEvent)timerSimulation.Tag;

				if (tEvent.Type == TimerEvent.EventType.WAIT_FINISH_BROADCAST)
				{
					if (System.IO.File.Exists(m_strSimulationBroadcastResultFilePath))
					{
						timerSimulation.Stop();
						timerSimulation.Tag = null;
						System.IO.File.Delete(m_strSimulationBroadcastResultFilePath);
						ResetSimulationAlarmSound();
					}
					else
					{
						tEvent.Data += 1;

						// 제한시간이 경과하면 강제로 타이머를 종료시킨다.
						if (tEvent.Data >= m_nSimulationBroadcastTimerWaitTime)
						{
							timerSimulation.Stop();
							timerSimulation.Tag = null;
							ResetSimulationAlarmSound();
						}
					}
				}
				else if (tEvent.Type == TimerEvent.EventType.CHECK_SYNC)
				{
					// 동기화 문제로 인하여 이미 종료된 Alarm 신호를 유지하고 있는지 확인한다.
					ProcessIF process = ProcessManager.Instance.GetProcess(tEvent.Data);

					if (process == null)
						FireDetectProcess.SoundPlayer.Stop();

					timerSimulation.Stop();
					timerSimulation.Tag = null;
				}
			}
		}

		private void ResetSimulationAlarmSound()
		{
			SeletCaseData data = DlgSelectCase.Instance.CurrentData;

			if (data == null || data.Sensor == null)
				return;

			ProcessIF process = ProcessManager.Instance.GetProcess(data.Sensor.ID);

			if (process != null && data.Sensor.SoundOn)
			{
				FireDetectProcess.PlaySound();

				timerSimulation.Tag = new TimerEvent(TimerEvent.EventType.CHECK_SYNC, data.Sensor.ID);
				timerSimulation.Start();
			}
		}

		// SOPServer를 이용하여 화재탐지 상황을 지정된 담당자들에게 문자로 보낸다.
		public void SendSimulationSMS(FormBroadcastConfig.SituationType type, int nEquipZoneID)
		{
			if (FormManager_Simulation.ManagerPhoneNumbers.Count == 0)
				return;

			ArrayList arrDatas = new ArrayList();

			arrDatas.Add(TrainingSimulatorCommandType.SEND_SDMS_SMS);
			arrDatas.Add((int)type);
			arrDatas.Add(nEquipZoneID);
			arrDatas.Add(FormManager_Simulation.ManagerPhoneNumbers.Count);
			arrDatas.Add("[연습모드]");

			foreach (KeyValuePair<string, string> pair in FormManager_Simulation.ManagerPhoneNumbers)
			{
				arrDatas.Add(pair.Value);
			}

			byte[] bytes = ClientProvider.MakeBytes(TCP_ID.TRAINING_SIMULATOR_COMMAND, arrDatas);
            SDMS.NetworkManager.Instance.Send(bytes);
		}
	}
}