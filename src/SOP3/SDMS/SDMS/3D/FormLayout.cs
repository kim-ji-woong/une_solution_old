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

namespace SDMS
{
    /*public class BaseViewEx : BaseView
    {
        private bool m_isClicked = false;
        private Position3D m_ptTarget = new Position3D(0.0f, 0.0f, 0.0f);
        private Point m_ptOrigin = new Point();

        private Panel[] m_arrForms = null;
        private Point[] m_arrFormsOrigin = null;

        public BaseViewEx(FormLayout frmLayout)
        {
            mTarget.MouseDown += new MouseEventHandler(this.OnMouseDown);
            mTarget.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            mTarget.MouseUp += new MouseEventHandler(this.OnMouseUp);

            int nFormCount = 20;
            m_arrForms = new Panel[nFormCount];
            m_arrFormsOrigin = new Point[nFormCount];

            int nFormSize = 128, nSpace = 20;
            int nColumnCount = 5;

            for (int i = 0; i < nFormCount; i++)
            {
                int nRowIndex = i / nColumnCount;
                int nColumnIndex = i % nColumnCount;

                m_arrForms[i] = new Panel();
                m_arrForms[i].Size = new Size(nFormSize, nFormSize);
                m_arrForms[i].Location = new Point(nColumnIndex * (nFormSize + nSpace), nRowIndex * (nFormSize + nSpace));
                frmLayout.Controls.Add(m_arrForms[i]);
                m_arrForms[i].Show();
            }
        }

        public void OnMouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(sender, e);

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_isClicked = true;
                m_ptOrigin = (Point)this.Get2DPoint(m_ptTarget);

                int nFormCount = m_arrForms.Count();

                for (int i = 0; i < nFormCount; i++)
                {
                    m_arrFormsOrigin[i] = m_arrForms[i].Location;
                }
            }
        }

        public void OnMouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_isClicked = false;
                base.RedrawScene();
            }
        }

        public void OnMouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(sender, e);

            if (m_isClicked)
            {
                Point pt = (Point)this.Get2DPoint(m_ptTarget);
                int x = pt.X - m_ptOrigin.X;
                int y = pt.Y - m_ptOrigin.Y;

                int nFormCount = m_arrForms.Count();

                for (int i = 0; i < nFormCount; i++)
                {
                    m_arrForms[i].Location = new Point(m_arrFormsOrigin[i].X + x, m_arrFormsOrigin[i].Y + y);
                }
            }
        }
    }*/
    public partial class FormLayout : Form
    {
        private int m_nLayout = 1;
       
        private string m_strZipFileFolderPath = "";
        private string m_strOutsideDAE = "";
        private string m_strInsideDAE = "";
        private Data_Building m_buildingCurrent = null;
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


        private DataManager mDataMan = new DataManager();

        private bool bLoadComplete = false;
        public bool LoadComplete
        {
            get { return bLoadComplete; }
            set { bLoadComplete = value; }
        }
        private bool bFirstRedraw = true;

        private int m_nCurrentFloor = 0;

        private bool bExtractOutside = false;

        private string szPrevFileName = "";

        public FormLayout()
        {
            szMediaPath = Application.StartupPath + "\\Media\\";
            szIconPath = szMediaPath + "icons\\화재.ico";

            this.panel1 = new Core.BaseView();//new BaseViewEx(this);
            this.panel2 = new Core.BaseView();
            
            InitializeComponent();

            panel2.Visible = false;
            //panel1.Dock = DockStyle.Fill;
            panel1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            FormMain.Instance.SetFloorStatus(false, -1, -1);

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

        public void OnWorkflowEnd()
        {
        }

        bool bInit = false;
        public void Init3DView()
        {
            if (bInit == true)
                return;
            bInit = true;
            mDataMan.LoadBuildingData();
            mDataMan.LoadZones();
            mEngine.Init("SDMS");

            mViewList.Add(panel1);
            panel1.Size = new Size(1280, 1024);
            panel2.Size = new Size(1280, 1024);
            BaseView view1 = ((BaseView)panel1);
            mCurrent = view1;
            try
            {
                view1.Popup = null;//popupMenu;
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
                view2.Popup = null;// popupMenu;
                view2.InitBaseView();

            }
            catch (System.Exception ex2)
            {
                Debug.WriteLine(ex2.StackTrace);
            }

            m_strOutDaeName = m_strZipFileFolderPath + "ND_0326l.DAE";
            if (bExtractOutside == false || !File.Exists(m_strOutDaeName))
            {
                try
                {
                    view1.ExtractFile(m_strOutsideDAE, m_strZipFileFolderPath);
                    bExtractOutside = true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }

            try
            {
                view1.OpenMesh(m_strOutDaeName);
                view1.OnViewHome();
            }
            catch (System.Exception)
            {
            }
            // open floor mesh
            if (bExtractInside == false)
            {
                try
                {
                    view2.ExtractFile(m_strInsideDAE, m_strZipFileFolderPath + "inside\\");
                    bExtractInside = true;
                }
                catch (System.Exception)
                {
                }
            }
            Layout1();
            AddBuildingName();
            view1.UpdateWindow();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Init3DView();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            mEngine.EngineDispose();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            mCurrent = ((BaseView)panel1);
            mCurrent.Focus();
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            mCurrent = ((BaseView)panel2);
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
            view1.OnViewHome();
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
            //panel1.Dock = DockStyle.Fill;
            panel1.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            FormMain.Instance.SetFloorStatus(false, -1, -1);
            
            //panel1.Size = new Size(this.Size.Width, this.Size.Height);

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
                    Zone obj = kv.Value;
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
                    BuildingGroup obj = kv.Value;
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
            bFirstRedraw = false;
            BaseView view1 = ((BaseView)panel1);

            if (view1 != null)
            {
                Dictionary<int, Building> m_dicBuildings = mDataMan.DicBuildings;
                foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
                {
                    Building obj = kv.Value;
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
        
        // 실내뷰
        public void Layout3(Data_Building building)
        {
            Size sz = FormMain.Instance.PageHome.Get3DPane().Size;
            panel1.Size = new Size(400, 300);
            panel1.Dock = DockStyle.None;
            panel1.Location = new Point(0, 34);

            panel2.Size = new Size(sz.Width, sz.Height);
            panel2.Visible = true;

            m_buildingCurrent = building;

            if (building == null)
                FormMain.Instance.SetFloorStatus(false, -1, -1);
            else
                FormMain.Instance.SetFloorStatus(true, building.MinFloor, building.MaxFloor);
        }
        
        public void SetFilePath(string strCMOFolderPath, string strOutsideFilePath, string strInsideFilePath, Dictionary<string, string> dicInsideCMO)
        {            
            m_strZipFileFolderPath = strCMOFolderPath;
            m_strOutsideDAE = strOutsideFilePath;
            m_strInsideDAE = dicInsideCMO["Inside"];
            m_dicInsideDAE = dicInsideCMO;
        }

        private void SetCurrentBuilding(Building building, int showFloor)
        {
            Data_Building building2 = new Data_Building();
            building2.BroadCastingText = building.BroadcastName;
            building2.BuildingCode = building.BuildingCode;

            building2.BuildingName = building.BuildingName;
            building2.BuildingID = building.BuildingID;
            building2.MaxFloor = building.MaxFloorIndex;
            building2.MinFloor = building.MinFloorIndex;

            m_buildingCurrent = building2;
            FormMain.Instance.SetFloorStatus(true, m_buildingCurrent.MinFloor, m_buildingCurrent.MaxFloor);
            ShowIndoor(showFloor);
        }

        public void SetCurrentBuilding(Data_Building building)
        {
            if (m_buildingCurrent == building)
                return;

            m_buildingCurrent = building;

            if (m_nLayout == 3 || m_nLayout == 4)
            {
                // Change 실내층
                FormMain.Instance.SetFloorStatus(true, building.MinFloor, building.MaxFloor);
                ShowIndoor(1);
            }
            this.Focus();
        }
        private int nCurrentFloor = -999;
        public void ShowIndoor(int nFloorIndex)
        {
            if (m_buildingCurrent != null)
            {
                string szCode = m_buildingCurrent.BuildingID;
                if( szCode == "")
                    return;

                int nFloor = nFloorIndex - 1;
                if (nFloor >= m_buildingCurrent.MinFloor && nFloor <= m_buildingCurrent.MaxFloor)
                {
                    if (szCode[0] >= '0' && szCode[0] <= '9')
                    {
                        szCode = "z" + szCode;
                    }
                    nCurrentFloor = nFloor;
                    string szFileName = null;
                    if (nFloorIndex < 0)
                    {
                        szFileName = string.Format("{0}_B{1}.dae", szCode, Math.Abs(nFloorIndex));
                    }
                    else
                    {
                        szFileName = string.Format("{0}_{1}.dae", szCode, nFloorIndex);
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

                    try
                    {
                        if (bLoadInsideMode == true)
                        {
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
        }
        public void AddFireEquipment()
        {
            string szBuildingID = m_buildingCurrent.BuildingID;           
            Zone zone = mDataMan.GetZone(szBuildingID, nCurrentFloor);
            if (zone == null)
                return;

            ArrayList arResult = mDataMan.LoadFireEquipment( szBuildingID, nCurrentFloor );
            if (arResult == null || arResult.Count == 0)
                return;

            if (zone.Polygon == null)
                return;
            int vCount = zone.Polygon.GetVertexCount();
            if (vCount == 0)
                return;
                        
            float dx = 0;
            float dy = 0;
            for( int i = 0 ; i < vCount; i++)
            {
                dx += (float)zone.Polygon.GetVertex(i).x;
                dy += (float)zone.Polygon.GetVertex(i).y;
            }
            dx = dx / vCount * 0.001f;
            dy = dy / vCount * 0.001f;

            //BaseView view2 = ((BaseView)panel2);
            //foreach (FireEquipment equip in arResult)
            //{               
            //    view2.AddFire(equip.ID, dx - equip.X , 2.0f, dy - equip.Y, equip.EquipID);
            //}

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
