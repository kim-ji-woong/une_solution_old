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

namespace SOPDisasterSystem
{
    public partial class FormLayout : Form
    {
        private FormMain m_frmMain = null;
        private int m_nLayout = 1;
       
        private string m_strZipFileFolderPath = "";
        private string m_strOutsideDAE = "";
        private string m_strInsideDAE = "";
        private SOPMonitoringSystem.Data_Building m_buildingCurrent = null;
        private Dictionary<string, string> m_dicInsideDAE = null;
        private string m_strOutDaeName = "";



        private Core.Engine mEngine = new Core.Engine();
        private ArrayList mViewList = new ArrayList();
        private Core.BaseView mCurrent = null;
        private string szIconPath = "";
        private string szMediaPath = "";

        private bool bExtractInside = false;
        private bool bLoadInsideMode = false;
        private string szInsideFullPath = null;

        private bool mCheckPosition = false;
        private SOPMonitoringSystem.HistoryDiasterPosition mLastPos = null;


		private DataManager mDataMan = null;

        private bool bLoadComplete = false;
        public bool LoadComplete
        {
            get { return bLoadComplete; }
            set { bLoadComplete = value; }
        }
        public SOPMonitoringSystem.HistoryDiasterPosition LastPos
        {
            get { return mLastPos; }
            set { mLastPos = value; }
        }

        private float m_nCurrentFloor = 0;

        private SOPMonitoringSystem.Popup.PopupStartEvent mFormPosition = null;

        private string szPrevFileName = "";

        public FormLayout(FormMain main)
        {
            m_frmMain = main;
            szMediaPath = EnginPath() + "Media\\";
            //szMediaPath = Application.StartupPath + "\\Media\\";
            szIconPath = szMediaPath + "icons\\화재.ico";
                      
            this.panel1 = new Core.BaseView();
            this.panel2 = new Core.BaseView();        
            
            InitializeComponent();
            
            panel2.Visible = false;
            panel1.Dock = DockStyle.Fill;
            panel1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            m_frmMain.SetFloorStatus(false, null);

            this.MouseWheel += new MouseEventHandler(OnMouseWheel);
			
        }

        private void FormLayout_Resize(object sender, EventArgs e)
        {
            BaseView view1 = (BaseView)panel1;
            BaseView view2 = (BaseView)panel2;
            switch (m_nLayout)
            {
                case 1:
                    view1.UpdateWindow();
                    break;
                case 2:
                    //Layout2();
                    break;
                case 3:
                    view1.UpdateWindow();
                    view2.UpdateWindow();
                    break;
                case 4:
                    //Layout4();
                    break;
            }
        }

        public void SetCheckPoistion(SOPMonitoringSystem.Popup.PopupStartEvent form, bool bCheck)
        {
            mLastPos = null;
            mFormPosition = form;
            
            form.OnCheckPositionEnd +=OnCheckEnd;
            
            mCheckPosition = true;
            
            BaseView view1 = (BaseView)panel1;
            view1.SetCheckPoistion(mCheckPosition);

            BaseView view2 = (BaseView)panel2;
            view2.SetCheckPoistion(mCheckPosition);  

        }

        public void OnCheckEnd(bool bResult)
        {
            if (mFormPosition == null)
                return;
            mFormPosition.OnCheckPositionEnd -= OnCheckEnd;
            LastPos = mFormPosition.LastPoistion;
            mCheckPosition = false;
            szIconPath = szMediaPath + "icons\\" + mFormPosition.DisasterName + ".ico";
			
            m_frmMain.LayoutForm.Invoke((MethodInvoker)delegate
            {				

                if (m_nLayout == 3)
                {
                    BaseView view2 = (BaseView)panel2;
                    view2.SetCheckPoistion(mCheckPosition);
                    if (bResult == true)
                    {
                       // view2.AddPOI(szIconPath);
                        if (LastPos != null)
                            view2.AddPOI(szIconPath, LastPos.X, LastPos.Y, LastPos.Z);
                        else
                            view2.AddPOI(szIconPath);
                        view2.UpdateWindow();
                    }
                }
                else
                {
                    BaseView view1 = (BaseView)panel1;
                    view1.SetCheckPoistion(mCheckPosition);
                    if (bResult == true)
                    {
                        if (LastPos != null)
                        {
                            float dx = 120894.0548f + 1008.531f;
                            float dy = 157659.0963f - 506.251f;
                            float ox = LastPos.X - dx;
                            float oz = dy - LastPos.Z;
                            view1.AddPOI(szIconPath, ox, LastPos.Y, oz);
                        }
                        else
                            view1.AddPOI(szIconPath);
                        view1.UpdateWindow();
                    }
                }                
            });
            mFormPosition = null;
        }

        public void OnWorkflowEnd()
        {
        }

        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            return szMainPath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			mDataMan = DataManager.Instance;
            mDataMan.LoadBuildingData();
            mDataMan.LoadZones();
            mDataMan.LoadEquipZones();

            mEngine.Init(EnginPath(), "SOPMonitoringSystem");         
            
            mViewList.Add(panel1);
            panel1.Size = new Size(1280, 1024);
            panel2.Size = new Size(1280, 1024);
            BaseView view1 = ((BaseView)panel1);
            mCurrent = view1;
            try
            {
                view1.Popup = popupMenu;
                view1.InitBaseView();                
            }
            catch (System.Exception ex1)
            {
                Debug.WriteLine(ex1.StackTrace);
            }

            mViewList.Add(panel2);
            BaseView view2 = ((BaseView)panel2);
            try
            {
                view2.Popup = popupMenu;
                view2.InitBaseView();     

            }
            catch (System.Exception ex2)
            {
                Debug.WriteLine(ex2.StackTrace);
            }

			m_strOutDaeName = m_strZipFileFolderPath + "ND_0326l.DAE";
			if (!File.Exists(m_strOutDaeName) || SOPMonitoringSystem.ModelManager.Instance.ExtractOutside == true)
			{
				try
				{
                    ExtractToTrg(m_strOutsideDAE, m_strZipFileFolderPath);
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.StackTrace);
				}
			}          
     
            try
            {
                view1.OpenMesh(m_strOutDaeName);
                view1.OnViewFix();
            }
            catch (System.Exception)
            {            
            }

			string szFloorFile = m_strZipFileFolderPath + "inside\\A-1_1.DAE";
			if (!File.Exists(szFloorFile) || SOPMonitoringSystem.ModelManager.Instance.ExtractInside == true)
			{
				try
				{
                    ExtractToTrg(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");

				}
				catch (System.Exception)
				{
				}
			}

			bExtractInside = true;
            
            Layout1();
            AddBuildingName();
            view1.UpdateWindow();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            mEngine.EngineDispose();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            mCurrent = ((BaseView)panel1);
            mCurrent.Focus();
			mCurrent.Update();
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            mCurrent = ((BaseView)panel2);
			mCurrent.Focus();
			mCurrent.Update();
        }

        public void btnTop_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewTop();
        }

        public void btnFront_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewFront();
        }

        public void btnLeft_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewLeft();
        }

        public void btnRight_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewRight();
        }

        public void btnHome_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewRear();
        }
        public void btnHome_Click_1(object sender, EventArgs e)
        {
            BaseView view1 = (BaseView)panel1;
            view1.OnViewFix();
            if (m_nLayout == 3 || m_nLayout == 4)
            {
                BaseView view2 = (BaseView)panel2;
                view2.OnViewHome();
            }            
        }

        public void btnZoomIn_Click(object sender, EventArgs e)
        {
        }

        public void btnFit_Click(object sender, EventArgs e)
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
            }
        }

        public void ZoomOut()
        {
            if (mCurrent != null)
            {
                mCurrent.OnMouseWheel(0, 0, -240);
            }
        }

        public void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string szName = mCurrent.OnSelect();
            Debug.WriteLine(szName);
        }

        public void  RemoveDisasterPos()
        {
            BaseView view1 = (BaseView)panel1;
            if (view1 != null)
            {
                if (LastPos != null)
                {
                    float dx = 120894.0548f + 1008.531f;
                    float dy = 157659.0963f - 506.251f;
                    float ox = LastPos.X - dx;
                    float oz = dy - LastPos.Z;
                    view1.RemovePOI(ox, LastPos.Y, oz);
                    view1.UpdateWindow();
                }
            }

            BaseView view2 = (BaseView)panel2;
            if (view2 != null)
            {
                if (LastPos != null)
                {
                    view2.RemovePOI(LastPos.X, LastPos.Y, LastPos.Z);
                    view2.UpdateWindow();
                }
            }
        }

        private bool ExtractToTrg(string strSrcFile, string strTrgPath)
        {
            try
            {
                if (!Directory.Exists(strTrgPath))
                    Directory.CreateDirectory(strTrgPath);

                System.IO.FileStream fs = new System.IO.FileStream(strSrcFile,
                                                     System.IO.FileMode.Open,
                                             System.IO.FileAccess.Read, System.IO.FileShare.Read);

                ICSharpCode.SharpZipLib.Zip.ZipInputStream zis =
                                        new ICSharpCode.SharpZipLib.Zip.ZipInputStream(fs);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ze;

                while ((ze = zis.GetNextEntry()) != null)
                {
                    if (!ze.IsDirectory)
                    {
                        string fileName = System.IO.Path.GetFileName(ze.Name);

                        string destDir = System.IO.Path.Combine(strTrgPath,
                                         System.IO.Path.GetDirectoryName(ze.Name));

                        if (false == Directory.Exists(destDir))
                        {
                            System.IO.Directory.CreateDirectory(destDir);
                        }

                        string destPath = System.IO.Path.Combine(destDir, fileName);

                        System.IO.FileStream writer = new System.IO.FileStream(
                                        destPath, System.IO.FileMode.Create,
                                                System.IO.FileAccess.Write,
                                                    System.IO.FileShare.Write);

                        byte[] buffer = new byte[2048];
                        int len;
                        while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, len);
                        }

                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
            //return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
        }

        public void AddDisasterPos(string disastertype, float x, float y, float z)
        {
            if (LastPos == null)
                return;

            if (LastPos.FloorIndex != -999)
            {
                if (mDataMan == null)
                    return;

                SOPDisasterSystem.Building curBuilding = null;

                curBuilding = mDataMan.GetBuilding(LastPos.BuildingID);

                if (curBuilding == null)
                    return;

                if (m_nLayout == 1)
                {
                    m_nLayout = 3;
                    m_frmMain.NumLayout = 3;
                    Size sz = m_frmMain.GetPaneVirtool().Size;
                    panel1.Size = new Size(400, 300);
                    panel1.Dock = DockStyle.None;
                    panel1.Location = new Point(0, 34);

                    panel2.Size = new Size(sz.Width, sz.Height);
                    panel2.Visible = true;
                }

                SetCurrentBuilding(curBuilding, LastPos.FloorIndex);
				Zone zone = DataManager.Instance.GetZone(curBuilding.BuildingID, LastPos.FloorIndex);

                string path = szMediaPath + "icons\\" + disastertype + ".ico";
                BaseView view2 = (BaseView)panel2;
				if (view2 != null && zone != null)
                {
					UnE.Geometry.Vertex2D pos = zone.Polygon.CalcWeightCenter();
					float dx = 120894.0548f + 1008.531f;
					float dy = 157659.0963f - 506.251f;
					float ox = x - dx + (float)pos.x;
					float oz = dy - z + (float)pos.y;
                    view2.AddPOI(path, x, y, z);
                    view2.UpdateWindow();
                }
                RedrawWindow();
            }
            else
            {

                if (m_nLayout == 3)
                {
                    m_nLayout = 1;
                    m_frmMain.NumLayout = 1;
                    Layout1();
                }

                string path = szMediaPath + "icons\\" + disastertype + ".ico";
                BaseView view1 = (BaseView)panel1;
                if (view1 != null)
                {
                    float dx = 120894.0548f + 1008.531f;
                    float dy = 157659.0963f - 506.251f;
                    float ox = x - dx;
                    float oz = dy - z;
                    view1.AddPOI(path, ox, y, oz);
                    view1.UpdateWindow();
                }
                RedrawWindow();
            }
            
        }


        

        public void toolStripMenuItem1_Click(object sender, EventArgs e)
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
                    szSelectedName = m_buildingCurrent.BroadCastingText;
                    int nIdx = szSelectedName.IndexOf('*');
                    if (nIdx != -1)
                    {
                        szSelectedName = szSelectedName.Substring(0, nIdx);
                    }

                    string szResult = null;
                    if (m_nCurrentFloor < 0)
                    {
                        szResult = string.Format("{0} B{1}층", szSelectedName, Math.Abs(m_nCurrentFloor));
                    }
                    else
                    {
                        szResult = string.Format("{0} {1}층", szSelectedName, m_nCurrentFloor);
                    }
                   
                    if (mFormPosition != null && mFormPosition.IsHandleCreated)
					{
						mLastPos = new SOPMonitoringSystem.HistoryDiasterPosition();
						mLastPos.PoistionName = szResult;

						mLastPos.X = pos3D.X;
						mLastPos.Y = pos3D.Y;
						mLastPos.Z = pos3D.Z;
						mLastPos.FloorIndex = m_nCurrentFloor;

						mFormPosition.Invoke((MethodInvoker)delegate
                        {
                            mFormPosition.PositionName = szResult;
                        });

                        mFormPosition.Invoke((MethodInvoker)delegate
                        {
                            mLastPos.DisasterName = mFormPosition.DisasterName;
                        });
                        mLastPos.BuildingID = m_buildingCurrent.BuildingID;
                        mFormPosition.Invoke((MethodInvoker)delegate
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
                    SOPDisasterSystem.Building curBuilding = null;
                    if (szSelectedName != null && szSelectedName != "")
                    {
                        curBuilding = mDataMan.GetBuilding(szSelectedName);
                        if (curBuilding != null)
                        {
                            szSelectedName = curBuilding.BroadcastName;
                            isBuildingName = true;
                        }
                       
                    }

                    if (isBuildingName == false)
                    {
                        string szName = mDataMan.CheckZoneName(pos3D.X, pos3D.Z);
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

					if (mFormPosition != null && mFormPosition.IsHandleCreated)
					{
						mLastPos = new SOPMonitoringSystem.HistoryDiasterPosition();
						mLastPos.PoistionName = szSelectedName;

						mLastPos.X = pos3D.X;
						mLastPos.Y = pos3D.Y;
						mLastPos.Z = pos3D.Z;
						mLastPos.FloorIndex = -999;
						if (isBuildingName == true)
							mLastPos.BuildingID = curBuilding.BuildingID;
						else
							mLastPos.BuildingID = "ZONE";

						mFormPosition.Invoke((MethodInvoker)delegate
						{
							mFormPosition.PositionName = szSelectedName;
						});

						mFormPosition.Invoke((MethodInvoker)delegate
						{
							mLastPos.DisasterName = mFormPosition.DisasterName;
						});

						mFormPosition.Invoke((MethodInvoker)delegate
						{
							mFormPosition.AddLastHistoryDisasterPoistion(mLastPos);
						});						      
					}                                  
                }
            }            
        }

        public void removePOIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
            {
                mCurrent.RemovePOI();
            }
        }
        public void clearSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.ClearSelect();

        }

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string szSelect = cmbPoiType.SelectedItem.ToString();
            if( szSelect != null && szSelect != "")
            {
                szIconPath = szMediaPath + "icons\\"+szSelect+".ico";
            }
        }

        public void btnAddPOI_Click(object sender, EventArgs e)
        {

        }
        public void SetLayoutMode(int nLayout)
        {
            m_nLayout = nLayout;
        }

        public void Layout1()
        {
            panel2.Visible = false;
            panel1.Dock = DockStyle.Fill;
            panel1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            
            m_frmMain.SetFloorStatus(false, null);
            
            panel1.Size = new Size(this.Size.Width, this.Size.Height);

            m_buildingCurrent = null;
        }       
 
        public void AddOutZoneName()
        {            
            BaseView view1 = ((BaseView)panel1);
            if (view1 != null)
            {
                Dictionary<int, Zone> m_dicBuildingGroup = mDataMan.DicOutdoorZones;
                foreach (KeyValuePair<int, Zone> kv in m_dicBuildingGroup)
                {
                    SOPDisasterSystem.Zone obj = (SOPDisasterSystem.Zone)(kv.Value);
                    try
                    {
                        
                    }
                    catch (System.Exception)
                    {             
                    }                   
                }                
            } 
        }

        public void AddGroupName()
        {
            BaseView view1 = ((BaseView)panel1);
            if (view1 != null)
            {
                Dictionary<int, BuildingGroup> m_dicBuildingGroup = mDataMan.DicBuildingGroup;
                foreach (KeyValuePair<int, BuildingGroup> kv in m_dicBuildingGroup)
                {
                    SOPDisasterSystem.BuildingGroup obj = (SOPDisasterSystem.BuildingGroup)(kv.Value);
                    try
                    {
                        
                    }
                    catch (System.Exception)
                    {             
                    }                   
                }                
            } 
        }
       
        public void AddBuildingName()
        {
            BaseView view1 = ((BaseView)panel1);
            if (view1 != null)
            {
                Dictionary<int, Building> m_dicBuildings = mDataMan.DicBuildings;
                foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
                {
                    SOPDisasterSystem.Building obj = (SOPDisasterSystem.Building)(kv.Value);
                    try
                    {
                        view1.AddAliasName(obj.BuildingID, obj.BroadcastName);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }       
        }

        public void RedrawWindow()
        {   
            BaseView view1 = (BaseView)panel1;
            view1.UpdateWindow();
            BaseView view2 = (BaseView)panel2;
            view2.UpdateWindow();
        }
        
        public void Layout3()
        {
            Size sz = m_frmMain.GetPaneVirtool().Size;
            panel1.Size = new Size(400, 300);
            panel1.Dock = DockStyle.None;
            panel1.Location = new Point(0, 34);

            panel2.Size = new Size(sz.Width, sz.Height);
            panel2.Visible = true;

            SOPMonitoringSystem.Data_Building buildingSelected = m_frmMain.GetSpace().GetSelectedBuilding();
            m_buildingCurrent = buildingSelected;

			if (buildingSelected == null)
				m_frmMain.SetFloorStatus(false, null);
			else
			{
				Building buildingInfo = DataManager.Instance.GetBuilding(m_buildingCurrent.BuildingID);
				if (buildingInfo != null)
				{
					ArrayList arFloors = (ArrayList)buildingInfo.FloorList.Clone();
					m_frmMain.SetFloorStatus(true, arFloors);					
				}   
			}
               
        }
        
        public void SetFilePath(string strCMOFolderPath, string strOutsideFilePath, string strInsideFilePath, Dictionary<string, string> dicInsideCMO)
        {            
            m_strZipFileFolderPath = strCMOFolderPath;
            m_strOutsideDAE = strOutsideFilePath;
            m_strInsideDAE = dicInsideCMO["Inside"];
            m_dicInsideDAE = dicInsideCMO;
        }

        private void SetCurrentBuilding(SOPDisasterSystem.Building building, float showFloor)
        {
            SOPMonitoringSystem.Data_Building building2 = new SOPMonitoringSystem.Data_Building();
            building2.BroadCastingText = building.BroadcastName;
            building2.BuildingCode = building.BuildingCode;

            building2.BuildingName = building.BuildingName;
            building2.BuildingID = building.BuildingID;
            building2.MaxFloor = building.MaxFloorIndex;
            building2.MinFloor = building.MinFloorIndex;

            m_buildingCurrent = building2;

			Building buildingInfo = DataManager.Instance.GetBuilding(m_buildingCurrent.BuildingID);
			if (buildingInfo != null)
			{
				if (m_nLayout == 3)
				{
					ArrayList arFloors = (ArrayList)buildingInfo.FloorList.Clone();
					m_frmMain.SetFloorStatus(true, arFloors);
					ShowIndoor(1.0f, (Floor)((Zone)arFloors[0]).Floor);
				}
			}            
        }

        public void SetCurrentBuilding(SOPMonitoringSystem.Data_Building building)
        {
            if (m_buildingCurrent == building)
                return;

            m_buildingCurrent = building;

			Building buildInfo = DataManager.Instance.GetBuilding(building.BuildingID);
			if (buildInfo != null)
			{
				if (m_nLayout == 3 )
				{
					// Change 실내층
					ArrayList arFloors = (ArrayList)buildInfo.FloorList.Clone();
					m_frmMain.SetFloorStatus(true, arFloors);
					ShowIndoor(1.0f, (Floor)((Zone)arFloors[0]).Floor);
				}
				this.Focus();
			}            
        }


        private float nCurrentFloor = -999.0f;
        public void ShowIndoor(float nFloorIndex, Floor floor)
        {
            if (m_buildingCurrent != null)
            {
                string szCode = m_buildingCurrent.BuildingID;
                if( szCode == "")
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
					szFileName = string.Format("{0}_{1:f1}", szCode,  nFloor);

				if (szFileName.EndsWith(".0"))
					szFileName = szFileName.Substring(0, szFileName.Length - 2);

				if (szFileName[szFileName.Length - 2] == '.')
				{
					szFileName += "M.dae";
				}
				else
				{
					szFileName += ".dae";
				}
				

                // find dae
                string szFullPath = m_strZipFileFolderPath + "inside\\" + szFileName;
                szInsideFullPath = szFullPath;
                if (szPrevFileName == szFullPath)
                {
                    return;
                }

                // clear current view
                BaseView view2 = ((BaseView)panel2);
				view2.Focus();
				view2.Update();

                try
                {
                    if (bLoadInsideMode == true)
                    {
                        RemoveDisasterPos();
                        view2.ClearAllData();
                        bLoadInsideMode = false;
                    }
                }
                catch (System.Exception)
                {                   
                }
                    
                bool bExist = File.Exists(szInsideFullPath);
                if (bExist == false || bExtractInside == false)
                {
                    view2.UpdateWindow();
                    szPrevFileName = "";
                    return;
                }

                m_nCurrentFloor = nFloorIndex;

                try
                {                        
                    if (bExist == true)
                    {
                        FormModelLoading.iForm.ThreadModal(this);  
                        FormModelLoading.iForm.ShowDialog(this);
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
                BaseView view2 = ((BaseView)panel2);
                view2.OpenMesh(szInsideFullPath);
                view2.OnViewTop();
                bLoadInsideMode = true;

                //AddFireEquipment();


                view2.UpdateWindow();
                szPrevFileName = szInsideFullPath;
                FormModelLoading.iForm.Close();
            }           
        }
    }
}
