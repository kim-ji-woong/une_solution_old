using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;
using Core;
using UnE.Util.Unity;
using UnE.Sensor;
using UnE.Spatial;
using UnE.View.Content;

using System.Security.AccessControl;
using Microsoft.Win32;
using SDMS;
using System.IO;

namespace UnE.View.Content
{
    public partial class ImageViewCtrl : Panel, ISensorTooltipOwner, IBaseView
	{
        public void ShowNames(int nPOIID, bool bVisible)
        {
        }

        public void SetTextPOILOD(int nPOIID, bool bToggle, float fDist)
        {
        }

        public void ShowIconPOI(int nPOIID, bool bVisible)
        {
        }


        private ILayerManager mLayerManager = null;

        public ILayerManager LayerManager
        {
            get { return mLayerManager; }
            set { mLayerManager = value; }
        }

        public System.Drawing.Point GetPosition2D(int nPOIID, float x, float y, float z)
        {            
            return GlobalToScreen(x, y);
        }

        private Matrix mTransform = new Matrix();
        public Matrix Transform
        {
            get { return mTransform; }
        }

        private PointF mPtTranslation = new PointF(0.0f, 0.0f);
        public PointF PtTranslation
        {
            get { return mPtTranslation; }
        }

        private bool m_bBeginScaleSetting = false; // 시작Scale과 시작Point가 confing.ini에 세팅되었는지 여부
        private PointF m_ptBeginPT = new PointF(); // 세팅된 시작 Point
        private float m_nBeginScale = 1.0f; // 세팅된 시작 Scale

        private float m_nCurScale = 1.0f; // 현재 Scale        
        private float m_nScaleGap = 0.125f; // 스크롤 Gap
        private float m_nMinScale = 0.1403509f; // 최소 Scale        
        private float m_nMaxScale = 7.125f; // 최대 Scale
        // 여백 비율
        private int m_nDisasterEmptyPer = 20;
        private float[] mScaleList = 
        {
            7.125f, 7.0f, 6.875f, 6.75f, 6.625f, 6.5f, 6.375f, 6.25f, 6.125f, 6.0f, 5.875f, 5.75f, 5.625f, 
            5.5f, 5.375f, 5.25f, 5.125f, 5.0f, 4.875f, 4.75f, 4.625f, 4.5f, 4.375f, 4.25f, 4.125f, 4.0f, 3.875f, 3.75f, 3.625f, 
            3.5f, 3.375f, 3.25f, 3.125f, 3.0f, 2.875f, 2.75f, 2.625f, 2.5f, 2.375f, 2.25f, 2.125f, 2.0f, 1.875f, 1.75f, 1.625f,
            1.5f, 1.375f, 1.25f, 1.125f , 1.0f, 0.8888889f, 0.8f, 0.7272727f, 0.6666667f, 0.6153846f, 0.5714286f, 0.5333334f,
            0.5f, 0.4705882f, 0.4444444f, 0.4210526f, 0.4f, 0.3809524f, 0.3636364f, 0.3478261f, 0.3333333f, 0.32f, 0.3076923f, 0.2962963f, 
            0.2857143f, 0.2758621f, 0.2666667f, 0.2580645f, 0.25f, 0.2424242f, 0.2352941f, 0.2285714f, 0.2222222f, 0.2162162f, 0.2105263f, 
            0.2051282f, 0.2f, 0.1951219f, 0.1904762f, 0.1860465f, 0.1818182f, 0.1777778f, 0.173913f, 0.1702128f, 0.1666667f, 0.1632653f, 
            0.16f, 0.1568628f, 0.1538462f, 0.1509434f, 0.1481481f, 0.1454545f, 0.1428571f, 0.1403509f
        };

        private Image mBaseImage = null;
        public Image BaseImage
        {
            get { return mBaseImage; }          
        }

        private Size mSizeImage = new Size();
        public Size SizeImage
        {
            get { return mSizeImage; }
        }
        
        private ArrayList mBillBoardList = new ArrayList();

        // 마우스 드래그로 그려지는 사각형(Zoom)
        private Rectangle mRectDrawing = new Rectangle();

        // 이미지
        private Rectangle mRectImage = new Rectangle();

        // 이미지 중심점
        private Point mPtCenter = new Point();

        // 화면 중심점
        private PointF mPtGlobalCenter = new PointF();
        public PointF PtGlobalCenter
        {
            get { return mPtGlobalCenter; }
        }
                
        private bool mbDrag = false;
        
        private Point mPtPrev; 
        private Point mPtDragStart;
        private Point mPtDragCurrent;
        private Point mPtCurrent; 

        private bool m_bEditMode = false;
        public bool EditMode
        {
            get { return m_bEditMode; }
            set { m_bEditMode = value; }
        }


        private bool mbRotationMode = false;
        public bool RotationMode
        {
            get { return mbRotationMode; }
            set 
            {
                SetMode(value);
                mbRotationMode = value;
            }
        }

        private bool mbTranslateMode = false;
        private Timer timer1;
        
        public bool TranslateMode
        {
            get { return mbTranslateMode; }
            set 
            {
                SetMode(value);
                mbTranslateMode = value; 
            }
        }

        private bool bRectZoomMode = false;
        public bool RectZoomMode
        {
            get { return bRectZoomMode; }
            set 
            {
                SetMode(value);
                bRectZoomMode = value;
            }
        }

        private void SetMode(bool bFalse)
        {
            if( bFalse == true)
            {
                mbRotationMode = false;
                mbTranslateMode = false;
                bRectZoomMode = false;
            }
        }

        private int mBillboardWidth = 32;
        public int BillboardWidth
        {
            get { return mBillboardWidth; }
            set { mBillboardWidth = value; }
        }
        private int mBillboardHeight = 32;
        public int BillboardHeight
        {
            get { return mBillboardHeight; }
            set { mBillboardHeight = value; }
        }

        private bool m_bDrawBillBoard = true;
        public bool DrawBillBoard
        {
            get { return m_bDrawBillBoard; }
            set { m_bDrawBillBoard = value; }
        }

        private bool m_bDrawBuildingText = true;
        public bool DrawBuildingText
        {
            get { return m_bDrawBuildingText; }
            set { m_bDrawBuildingText = value; }
        }

        private IFormContent m_frmParent = null;
       
        private Zone m_currentIndoorZone = null;
        public Zone CurrentZone
        {
            get { return m_currentIndoorZone; }           
        }

        private string m_szImagePath = "";
        public string ImagePath
        {
            get { return m_szImagePath; }
        }

        /*private UnE.View.Content.ILayerManager mLayerManager;
        public UnE.View.Content.ILayerManager LayerManager
        {
            get { return mLayerManager; }
            set { mLayerManager = value; }
        }

        public void ShowNames(int nID, bool bVisible)
        { }
        public void SetTextPOILOD(int nID, bool bToggle,float fDist)
        { }
        public void ShowIconPOI(int nID, bool bShow)
        { }*/


        // key : POI id
        // value : POI 객체
        private Dictionary<int, POI> m_dicPOIs = new Dictionary<int, POI>();

        // Zone별 POI 리스트
        // Indoor View에서만 사용됨
        private Dictionary<Zone, ArrayList> m_dicZonePOIs = new Dictionary<Zone, ArrayList>();

        // Panning 또는 Orbit, Zoom In/Out 등의 동작을 위하여 임시로 숨겨놓은 POI Popup 창 리스트
        private ArrayList m_arrTemporaryHiddenPOIs = new ArrayList();

        private ArrayList m_arrLODShowingPOIs = new ArrayList();
        
        private Brush mBrushRect = null;
        private Pen mPenRect = null;
        private Pen mPenRedRect = null;
        private SolidBrush mBrushRedRect = null;
        private int m_nScaleIndex = 0;

        private int m_nShowTooltipX = 0;
        private int m_nShowTooltipY = 0;
        //private bool m_bShowTooltip = false;

        private Form m_formTooltip = null;
        private Timer m_TooltipTimer = new Timer();


        private IPopupFactory m_Factory = null;

        private string m_szToolKey = @"SDMS\Unity\Toolstrip2D";
        private string m_szPosSubKeyName = "MainToolStripPos";
        private string m_szToolStripName = "ToolboxStrip";
        private ToolStripContainer mMainContainer = new ToolStripContainer();

        private List<ImageOption> m_BaseImages = new List<ImageOption>();
        private Size m_nLod1ImageSize = new Size();

        private int m_nCurLodLevel = 1;
        private int m_nMinLodLevel = 1;
        private int m_nMaxLodLevel = 1;
        private int m_nLodUpdateVal = -1; 

        public void AddMainToolStrip(ToolStrip strip)
        {
            // read toolstrip position
            int nPos = ReadToolStripConfig();

            // Set StripName for using Key
            strip.Name = m_szToolStripName;

            // Add StripMenu
            SetToolStripMenu(strip, nPos);
        }              

        private void SetToolStripMenu(ToolStrip strip, int nPos)
        {
            if (nPos == 1)
                mMainContainer.RightToolStripPanel.Controls.Add(strip);
            else if (nPos == 2)
                mMainContainer.LeftToolStripPanel.Controls.Add(strip);
            else if (nPos == 3)
                mMainContainer.BottomToolStripPanel.Controls.Add(strip);
            else
                mMainContainer.TopToolStripPanel.Controls.Add(strip);

        }



        public void RemoveMainToolStrip(ToolStrip strip)
        {
            mMainContainer.RightToolStripPanel.Controls.Remove(strip);
            mMainContainer.LeftToolStripPanel.Controls.Remove(strip);
            mMainContainer.BottomToolStripPanel.Controls.Remove(strip);
            mMainContainer.TopToolStripPanel.Controls.Remove(strip);
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
            if (mMainContainer.TopToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 0;
            }
            else if (mMainContainer.RightToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 1;
            }
            else if (mMainContainer.LeftToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 2;
            }
            else if (mMainContainer.BottomToolStripPanel.Controls.ContainsKey(m_szToolStripName))
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

        private string regSubkey = "SDMS\\Unity\\HomveView\\";

        public void SaveHomeView(string szName)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(regSubkey + UnE.SOP.ProxySOP.Instance.SiteID + "\\" + szName, true);
            if (rk == null)
            {
                rk = Registry.CurrentUser.CreateSubKey(regSubkey + UnE.SOP.ProxySOP.Instance.SiteID + "\\" + szName);
            }

            float ele0 = mTransform.Elements[0];
            float ele1 = mTransform.Elements[1];
            float ele2 = mTransform.Elements[2];
            float ele3 = mTransform.Elements[3];
            float ele4 = mTransform.Elements[4];
            float ele5 = mTransform.Elements[5];
            
            rk.SetValue("Transform0", ele0);
            rk.SetValue("Transform1", ele1);
            rk.SetValue("Transform2", ele2);
            rk.SetValue("Transform3", ele3);
            rk.SetValue("Transform4", ele4);
            rk.SetValue("Transform5", ele5);
        }

        public void LoadHomeView(string szName)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(regSubkey + UnE.SOP.ProxySOP.Instance.SiteID + "\\" + szName, true);
            if (rk == null)
            {
                rk = Registry.CurrentUser.CreateSubKey(regSubkey + UnE.SOP.ProxySOP.Instance.SiteID + "\\" + szName);
            }
            object objEle0 = rk.GetValue("Transform0");
            object objEle1 = rk.GetValue("Transform1");
            object objEle2 = rk.GetValue("Transform2");
            object objEle3 = rk.GetValue("Transform3");
            object objEle4 = rk.GetValue("Transform4");
            object objEle5 = rk.GetValue("Transform5");

            if (objEle0 == null || objEle1 == null || objEle2 == null || objEle3 == null || objEle4 == null || objEle5 == null)
                return;

            float ele0 = 0;
            float.TryParse(objEle0.ToString(), out ele0);
            float ele1 = 0;
            float.TryParse(objEle1.ToString(), out ele1);
            float ele2 = 0;
            float.TryParse(objEle2.ToString(), out ele2);
            float ele3 = 0;
            float.TryParse(objEle3.ToString(), out ele3);
            float ele4 = 0;
            float.TryParse(objEle4.ToString(), out ele4);
            float ele5 = 0;
            float.TryParse(objEle5.ToString(), out ele5);

            mTransform = new Matrix(ele0, ele1, ele2, ele3, ele4, ele5);

            Invalidate();
        }

        private void ImageView_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ImageView_Resize(object sender, EventArgs e)
        {
            OnPanelResize();
        }

        private void ImageView_Paint(object sender, PaintEventArgs e)
        {
            OnPanelPaint(e);
        }

        private void ImageView_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e);
            Invalidate();
        }

        private void ImageView_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(sender, e);
            Invalidate();
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(sender, e);
            Refresh();
        }

        public void ImageView_FitView(object sender, EventArgs e)
        {
            ResetTransform();
            FitView();
            Refresh();
        }

        public ToolStripContainer ToolStripContainer
        {
            get { return mMainContainer; }
            set { mMainContainer = value; }
        }

        private IBaseViewOwner m_Owner = null;
        public IBaseViewOwner BaseViewOwner
        {
            get { return m_Owner; }
        }

        private System.Windows.Forms.OpenFileDialog mOpenFileDialog;
        
        public ImageViewCtrl(IFormContent formParent, IBaseViewOwner owner)
		{
            this.DoubleBuffered = true;
            m_Owner = owner;
            m_frmParent = formParent;

           // this.BackColor = Color.White;
             
            this.BackColor = Color.Yellow;
            mBrushRect = new HatchBrush(HatchStyle.Sphere, Color.Blue, Color.LightGreen);
            mPenRect = new Pen(mBrushRect, 0);
            mPenRedRect = new Pen(Color.Red, 1);
            mBrushRedRect = new SolidBrush(Color.FromArgb(150, 255, 0, 0));
             
            PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(OnPreviewKeyDown);
            m_TooltipTimer.Tick += new EventHandler(OnShowTooltip);
            MouseLeave += new EventHandler(OnMouseLeave);

            this.SizeChanged += new System.EventHandler(this.ImageView_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageView_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseUp);
            this.Resize += new System.EventHandler(this.ImageView_Resize);

            this.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.TopToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.LeftToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.RightToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.BottomToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);

            //mMainContainer.ContentPanel.BackColor = Color.Violet;
            this.Dock = DockStyle.Fill;
            mMainContainer.ContentPanel.Controls.Add(this);
            mMainContainer.Size = new System.Drawing.Size(1900, 1040);
            mMainContainer.Dock = DockStyle.Fill;
		}

        // LOD별 Scale 모음
        private Dictionary<int, float> m_DicLodScale = new Dictionary<int, float>();

        private void LoadSetting()
        { 
            DBUtility.Utility utill = new DBUtility.Utility();
            string scale = utill.getinivalue("SDMS", "2d_scale");
            string beginPT = utill.getinivalue("SDMS", "2d_beginPT");

            //scale과 beginPT가 모두 정의되어 있어야 첫화면에 적용시킴
            if (scale.Length > 0 && beginPT.Length > 0)
            {
                if (float.TryParse(scale, out m_nBeginScale))
                {
                    string[] beginPTs = beginPT.Split(',');
                    if (beginPTs.Length == 2)
                    {
                        float ptX = 0.0f;
                        float ptY = 0.0f;

                        if (float.TryParse(beginPTs[0], out ptX) && float.TryParse(beginPTs[1], out ptY))
                        {
                            m_ptBeginPT = new PointF(ptX, ptY);
                            m_bBeginScaleSetting = true;
                        }

                    }

                    m_nCurScale = m_nBeginScale;
                }
            }

            //config.ini
            //2d_scale=1.0
            //2d_beginPT=45,-75
            //2d_lod1_scale=1.0
            //2d_lod2_scale=1.5
            //2d_lod3_scale=2.1

            for (int i = 1; i <= m_nMaxLodLevel; i++)
            {
                string lodScale = utill.getinivalue("SDMS", "2d_lod" + i + "_scale");
                float nLodScale = 0.0f;

                if (float.TryParse(lodScale, out nLodScale))
                    m_DicLodScale.Add(i, nLodScale);
                else
                    m_DicLodScale.Add(i, i);
            }

            float fMinScale = m_nMinScale;
            if (float.TryParse(utill.getinivalue("SDMS", "2d_min_scale"), out fMinScale))
                this.m_nMinScale = fMinScale;

            float fMaxScale = m_nMaxScale;
            if (float.TryParse(utill.getinivalue("SDMS", "2d_max_scale"), out fMaxScale))
                this.m_nMaxScale = fMaxScale;

            float fScaleGap = m_nScaleGap;
            if (float.TryParse(utill.getinivalue("SDMS", "2d_scale_gap"), out fScaleGap))
                this.m_nScaleGap = fScaleGap;

            int nDisasterEmptyPer = m_nDisasterEmptyPer;
            if (int.TryParse(utill.getinivalue("SDMS", "2d_disaster_empty_per"), out nDisasterEmptyPer))
                this.m_nDisasterEmptyPer = nDisasterEmptyPer;
        }

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.EnableFireReportBtn(false);
        }

        public void SetImage(string szImagePath, Zone zone )
        {

            if (szImagePath.ToLower().EndsWith("blank.png"))
            {
                m_bFreeze = true;
            }
            else
            {
                m_bFreeze = false;
            }

            ArrayList arrPrevPOIs = null;
            ArrayList arrNextPOIs = null;

            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
                    arrPrevPOIs = arrPOIs;

                    foreach (POI poi in arrPOIs)
                    {
                        // 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
                        if (poi.Facility != null)
                        {
                            int nLayerID = poi.Facility.GetLayerID();
                            m_frmParent.Layers.GetLayer(nLayerID).Remove(poi.ID);
                        }

                        RemovePOI(poi.ID);

                        if (poi.Popup != null)
                        {
                            poi.Popup.Close();
                            poi.Popup = null;
                        }
                    }
                }
            }

            //ClearFireEquipments();

            ResetTransform();

            if (mBaseImage != null)
                mBaseImage.Dispose();

            if (System.IO.File.Exists(szImagePath))
            {
                mBaseImage = Bitmap.FromFile(szImagePath);
                mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
                mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
                mPtGlobalCenter = new Point(Width / 2, Height / 2);
                m_ptZoomCenter = mPtCenter;
                m_szImagePath = szImagePath;
            }
            else
            {
                mBaseImage = null;
            }
            

            m_currentIndoorZone = zone;

            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
                    arrNextPOIs = arrPOIs;

                    foreach (POI poi in arrPOIs)
                    {
                        if (poi.Facility == null)
                            continue;

                        string strIconPath = GetIconPath(poi.Facility.IconPath);

                        if (poi.Facility.Connected == false && poi.Type != IFacility.FacilityType.CCTV)
                        {
                            strIconPath = GetIconPath(poi.Facility.DisconnectIconPath);
                        }

                        int nID = AddPOI(strIconPath, poi.X, poi.Y);

                        poi.ID = nID;
                        m_dicPOIs[nID] = poi;

                        if (poi.Popup == null && poi.Facility != null)
                        {
                            if (m_Factory == null)
                            {
                                m_Factory = PopupFactoryHelper.GetFactory();
                            }
                            poi.Popup = poi.Facility.CreatePopup(this, m_Factory);
                        }

                        int nLayerID = poi.Facility.GetLayerID();
                        m_frmParent.Layers.GetLayer(nLayerID).Add(poi.ID);
                    }
                }

                //LoadFireEquipments();
                IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.ChangeZoneComboBox(m_currentIndoorZone);
            }

            ProcessCCTVLOD();
        }

        private bool m_bFreeze = false;

        public void SetImage(string szImagePath)
        {
            ResetTransform();
           
            if( System.IO.File.Exists(szImagePath))
            {
                if( szImagePath.ToLower().EndsWith("blank.png"))
                {
                    m_bFreeze = true;
                }
                else
                {
                    m_bFreeze = false;
                }
                mBaseImage = Bitmap.FromFile(szImagePath);
                mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
                mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
                mPtGlobalCenter = new Point(Width / 2, Height / 2);
            }
            else
            {
                mBaseImage = null;
            }          
        }

        public void SetImage(Image img, bool bBlank)
        {           
            ResetTransform();

            
            m_bFreeze = bBlank;
             
            mBaseImage = img;
            mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
            mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
            mPtGlobalCenter = new Point(Width / 2, Height / 2);
        }

        public void InitImage()
        { 
            string path = Application.StartupPath + "\\DXF\\2DImages\\";
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
                return;

            FileInfo[] files = di.GetFiles();
            if (files.Length == 0)
                return;

            foreach (FileInfo file in files)
            {
                string strExtension = file.Extension;
                if (strExtension.ToUpper() != ".PNG")
                    continue;

                string[] fileNames = file.Name.Replace(strExtension, "").Split('_');
                if (fileNames.Length != 3)
                    continue;

                // ex: 3_2_1 -> LOD LEVEL : 3, ROW : 2, COLUMN : 1

                int nLodLevel = CheckNumber(fileNames[0]);
                int nRow = CheckNumber(fileNames[1]);
                int nColumn = CheckNumber(fileNames[2]);
                if (nLodLevel < 0 || nRow < 0 || nColumn < 0)
                    continue;

                m_nMaxLodLevel = System.Math.Max(m_nMaxLodLevel, nLodLevel);

                Image img = Image.FromFile(file.FullName);
                 
                ImageOption imgOption = new ImageOption();
                imgOption.LODLevel = nLodLevel;
                imgOption.Image = img;
                imgOption.RowIndex = nRow;
                imgOption.ColumnIndex = nColumn;
                imgOption.Width = img.Width;
                imgOption.Height = img.Height; 
                m_BaseImages.Add(imgOption);

                if (nLodLevel == 1)
                {
                    m_nLod1ImageSize.Width += img.Width;
                    m_nLod1ImageSize.Height += img.Height;
                }
            }

            LoadSetting();

            //foreach (KeyValuePair<int, float> scale in m_DicLodScale)
            //{
                
            //}

            //int cnt = mScaleList.Where(p => p >= 1.0f).Count();
            ////int cnt = mScaleList.Length;
            //m_nLodUpdateVal = cnt / m_nMaxLodLevel;

            //int aa = 1;
            //int aa2 = 0;

            //// Max LOD 값만큼 Dic에 넣기
            //for (int i = m_nMaxLodLevel; i >= 1; i--)
            //{ 
            //    // LOD 단계별로 지정할 Scale 
            //    for (int j = aa2; j < mScaleList.Length; j++)
            //    {
            //        // 1미만인 Scale 값은 LOD 1단계에 넣는다
            //        if (i == 1 && mScaleList[j] <= 1.0f)
            //        {
            //            if (!abc.ContainsKey(i))
            //                abc.Add(i, new List<float>());
            //            abc[1].Add(mScaleList[j]);
            //        }
            //        else
            //        {
            //            // 1미만인 Scale 값은 LOD 1단계에 넣기 위해 LOD LEVEL 1은 제외한다
            //            if (i != 1 && aa > m_nLodUpdateVal)
            //            {
            //                aa = 1;
            //                aa2 = j; 
            //                break;                            
            //            }
            //            else
            //            {
            //                if (!abc.ContainsKey(i))
            //                    abc.Add(i, new List<float>());
            //                abc[i].Add(mScaleList[j]);
            //                aa++; 
            //            }
            //        }
            //    }
            //}

            ResetImageSize();
        }

        Dictionary<int, List<float>> abc = new Dictionary<int, List<float>>();

        private int CheckNumber(string temp)
        {
            int nNumber = -1;
            int.TryParse(temp, out nNumber);
            return nNumber;
        }

        public void ResetTransform()
        {
            mTransform.Reset();
            mTransform.Translate(mPtTranslation.X, mPtTranslation.Y);         
        }    
        
        private void ResetImageSize()
        {
            int width = 0;
            int height = 0;
            foreach (ImageOption imgOption in m_BaseImages)
            {
                if (m_nCurLodLevel != imgOption.LODLevel)
                    continue;

                if (imgOption.RowIndex == 1)
                    width += imgOption.Width;
                if (imgOption.ColumnIndex == 1)
                    height += imgOption.Height;
            }

            mSizeImage = new Size(width, height); 
        }

        public void FitView()
        {
            mPtTranslation.X = 0.0f;
            mPtTranslation.Y = 0.0f;

            ResetTransform();

            //Rectangle rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);             
            Rectangle rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);
            if (m_currentIndoorZone != null && (m_currentIndoorZone.Azimuth != 0.0f
               && m_currentIndoorZone.Azimuth != 180.0f))
            {
            //    rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Height, mSizeImage.Width);
            }
            //PointF ptZoomCenter = ScreenToGlobal(new PointF(Size.Width / 2.0f, Size.Height / 2.0f));
            PointF ptZoomCenter = ScreenToGlobal(new Point(rect.Location.X + (int)(rect.Width / 2.0f),
                   rect.Location.Y + (int)(rect.Height / 2.0f)));
            PointF ptImageOrgin = ScreenToGlobal(new Point(rect.Location.X,rect.Location.Y));
            PointF ptScrCenter = ScreenToGlobal(new PointF(Size.Width / 2.0f, Size.Height / 2.0f));
            //가로                  
            float fWidth = 0.0f;
            float fHeight = 0.0f;
            float fScale = 0.0f;


            if (m_bBeginScaleSetting)
            {
                fScale = m_nBeginScale;
            }
            else
            {
                //가로비
                fWidth = GetRatio(this.Size.Width, rect.Size.Width);
                //세로비
                fHeight = GetRatio(this.Size.Height, rect.Size.Height);

                if (m_currentIndoorZone != null && (m_currentIndoorZone.Azimuth != 0.0f
                   && m_currentIndoorZone.Azimuth != 180.0f))
                {
                    fWidth = GetRatio(this.Size.Width, rect.Size.Height);
                    //세로비
                    fHeight = GetRatio(this.Size.Height, rect.Size.Width);

                }
                // 작은 쪽을 기준으로 한다.
                fScale = (fWidth <= fHeight) ? fWidth : fHeight;

                // 시스템 스케일을 가져온다
                fScale = FindSystemScale(fScale);

                fScale = fScale * mTransform.Elements[3];  
            }

            if (fScale < m_nMinScale)
                fScale = m_nMinScale;

            if (fScale > m_nMaxScale)
                fScale = m_nMaxScale;
             
            float fRevScale = 1.0f / mTransform.Elements[3];
            mTransform.Translate(ptZoomCenter.X, ptZoomCenter.Y);
            mTransform.Scale(fRevScale, fRevScale);
            mTransform.Scale(fScale, fScale);
            mTransform.Translate(-ptZoomCenter.X,- ptZoomCenter.Y);

            ptZoomCenter = ScreenToGlobal(new Point(rect.Location.X + (int)(rect.Width / 2.0f),
                  rect.Location.Y + (int)(rect.Height / 2.0f)));
            
            ptImageOrgin = ScreenToGlobal(new Point(rect.Location.X, rect.Location.Y));
            ptScrCenter = ScreenToGlobal(new PointF(Size.Width / 2.0f, Size.Height / 2.0f));
            mTransform.Translate(ptScrCenter.X - ptZoomCenter.X, ptScrCenter.Y - ptZoomCenter.Y);

            //int nIndex = 0;
            //for (int i = 1; i < mScaleList.Length; i++)
            //{
            //    if ((fScale >= mScaleList[i] && fScale < mScaleList[i - 1]))
            //    {
            //        nIndex = i;
            //        break;
            //    } 
            //}

            m_nCurScale = fScale;
            SetLodLevel();
             
            Invalidate();
        }

        private void SetLodLevel(bool chgZoom = false)
        {
            for (int i = m_nMinLodLevel; i <= m_nMaxLodLevel; i++)
            {
                if (!m_DicLodScale.ContainsKey(i))
                    continue;

                if (m_nCurScale >= m_DicLodScale[i])
                {
                    if (m_DicLodScale.ContainsKey(i + 1))
                    {
                        if (m_nCurScale < m_DicLodScale[i + 1])
                        {
                            m_nCurLodLevel = i;
                            if (chgZoom)
                                m_bZoom = true;
                            break;
                        }
                    }
                    else
                    {
                        m_nCurLodLevel = i;
                        if (chgZoom)
                            m_bZoom = true;
                        break;
                    }
                }
                else
                {
                    m_nCurLodLevel = i;
                    if (chgZoom)
                        m_bZoom = true;
                    break;
                }
            }

            //m_DicLodScale[1] = 1.0;
            //m_DicLodScale[2] = 2.0;
            //m_DicLodScale[3] = 3.0;
            //m_DicLodScale[4] = 4.0;
            //m_nCurScale = 3.1

            //if (!m_DicLodScale.ContainsKey(m_nCurLodLevel))
            //    return;

            //if (m_nCurScale > m_DicLodScale[m_nCurLodLevel])
            //{
            //    if (m_nCurLodLevel == m_nMaxLodLevel)
            //        return;

            //    if (m_DicLodScale.ContainsKey(m_nCurLodLevel + 1))
            //    {
            //        if (m_nCurScale <= m_DicLodScale[m_nCurLodLevel + 1])
            //        {
            //            m_nCurLodLevel = m_nCurLodLevel + 1;
            //            if (chgZoom)
            //                m_bZoom = true;
            //        }
            //    }
            //}
            //else
            //{
            //    if (m_nCurLodLevel == 1)
            //        return;

            //    if (m_DicLodScale.ContainsKey(m_nCurLodLevel - 1))
            //    {
            //        if (m_DicLodScale[m_nCurLodLevel - 1] >= m_nCurScale)
            //        {
            //            m_nCurLodLevel = m_nCurLodLevel - 1;
            //            if (chgZoom)
            //                m_bZoom = true;
            //        }
            //    }
            //}

            //bool chgLoc = false;
            //foreach (KeyValuePair<int, List<float>> item in abc)
            //{
            //    foreach (float item2 in item.Value)
            //    {
            //        if (item2 == mScaleList[nIndex])
            //        {
            //            if (item.Key != m_nCurLodLevel)
            //            {
            //                m_nCurLodLevel = item.Key;                            
            //                chgLoc = true;

            //                if (chgZoom)
            //                    m_bZoom = true;

            //                break;
            //            }
            //        }
            //    }
            //    if (chgLoc)
            //        break;
            //}   
        }

        public float GetScale()
        {
            return mTransform.Elements[3];
        }

        public void OnPanelResize()
        {
            Point ptCenter = new Point(Width / 2, Height / 2);
           
            // TX 값을 구한다
            // TY 값도 구한다
            float tx = ptCenter.X - mPtGlobalCenter.X * mTransform.Elements[0];
            float ty = ptCenter.Y - mPtGlobalCenter.Y * mTransform.Elements[0];
            
            // TranForm시킨만큼 Center값도 옮김
            mPtGlobalCenter.X += tx;
            mPtGlobalCenter.Y += ty;

            mPtTranslation.X = mPtGlobalCenter.X - mRectImage.Width / 2;
            mPtTranslation.Y = mPtGlobalCenter.Y - mRectImage.Height / 2;

            if (m_bBeginScaleSetting && !m_bShowEquipmentZone)
            {
                // 구한 TX, TY값으로 TransForm
                mTransform.Translate(tx, ty);
                mTransform = new Matrix(m_nBeginScale, 0.0f, 0.0f, m_nBeginScale, m_ptBeginPT.X, m_ptBeginPT.Y);
            }
        }

        private Rectangle CalcRect(Point ptStart, Point ptEnd)
        {
            int mMinX = System.Math.Min(mPtDragStart.X, mPtDragCurrent.X);
            int mMaxX = System.Math.Max(mPtDragStart.X, mPtDragCurrent.X);

            int mMinY = System.Math.Min(mPtDragStart.Y, mPtDragCurrent.Y);
            int mMaxY = System.Math.Max(mPtDragStart.Y, mPtDragCurrent.Y);

            int nWidth = 0;
            int nHeight = 0;

            if (mMinX < 0)
                mMinX = 0;

            if (mMinY < 0)
                mMinY = 0;

            if (Width < mMaxX)
            {
                nWidth = Width - mMinX;
            }
            else
            {
                nWidth = mMaxX - mMinX;
            }

            if (Height < mMaxY)
            {
                nHeight = Height - mMinY;
            }
            else
            {
                nHeight = mMaxY - mMinY;
            }  
            return new Rectangle(mMinX, mMinY, nWidth, nHeight);
        }

        private ArrayList m_arrBuildingGroupText = null;
        public ArrayList ArrBuildingGroupText
        {
            get { return m_arrBuildingGroupText; }
            set { m_arrBuildingGroupText = value; }
        }

        private ArrayList m_arrBuildingText = null;
        public ArrayList ArrBuildingText
        {
            get { return m_arrBuildingText; }
            set { m_arrBuildingText = value; }
        }

        public void OnPanelPaint(PaintEventArgs e)
        {            
            Graphics g = e.Graphics;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                g.Transform = mTransform;                
            }
            catch (Exception)
            {
                return;
            }
                  
            if (m_bFreeze == false)
            {
                if (m_currentIndoorZone != null && m_currentIndoorZone.Azimuth != 0.0f)
                {
                    g.TranslateTransform((float)mSizeImage.Width / 2, (float)mSizeImage.Height / 2);
                    //now rotate the image
                    g.RotateTransform(m_currentIndoorZone.Azimuth);
                    g.TranslateTransform((float)-mSizeImage.Width / 2, (float)-mSizeImage.Height / 2);
                }            
            }

            //if (mBaseImage != null)
            //{
            //    Rectangle rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);
            //    g.DrawImage(mBaseImage, rect);
            //    mRectImage = rect;
            //    mPtCenter = new Point((int)(mRectImage.X + (mRectImage.Width * 0.5f)), (int)(mRectImage.Y + (mRectImage.Height * 0.5)));
            //}

            int MaxImgColumnCnt = 0;

            if (m_BaseImages != null && m_BaseImages.Count > 0)
            { 
                // 이미지 Draw 위치
                int drawX = mRectImage.X;
                int drawY = mRectImage.Y;

                // 이전 행 (행이 늘어나는 시점 체크용으로 사용)
                int prevRow = 0;

                foreach (ImageOption imgOption in m_BaseImages)
                {
                    if (m_nCurLodLevel != imgOption.LODLevel)
                        continue;
                         
                    if (prevRow > 0 && prevRow < imgOption.RowIndex)
                    {
                        drawX = mRectImage.X;
                        drawY += imgOption.Height - 1; // -1:이미지 경계 없애기 위해서
                    }
                    else if (prevRow > 0)
                    {
                        drawX += imgOption.Width - 1; // -1:이미지 경계 없애기 위해서
                    }

                    if (imgOption.RowIndex == 1)
                    {
                        MaxImgColumnCnt++;
                    }

                    Rectangle rect = new Rectangle(drawX, drawY, imgOption.Width, imgOption.Height);
                    g.DrawImage(imgOption.Image, rect);

                    prevRow = imgOption.RowIndex;
                } 

                Rectangle totalRect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);
                mRectImage = totalRect; 
            }

            mPtCenter = new Point((int)(mRectImage.X + (mRectImage.Width * 0.5f)), (int)(mRectImage.Y + (mRectImage.Height * 0.5)));

            if (m_bDrawBillBoard)
            {
                int IconWidth = (int)(32 * mTransform.Elements[0]);
                int IconHeight = (int)(32 * mTransform.Elements[3]); 

                g.ResetTransform();
                if (mSizeImage.Width > 0 && mSizeImage.Height > 0)
                {
                    foreach (BillBoard billBoard in mBillBoardList)
                    {
                        //float x = ConvertPoiX(billBoard.TX);
                        //float y = ConvertPoiY(billBoard.TY);
                        Point xx = ConvertPT(billBoard.TX, billBoard.TY);

                        Rectangle rect = new Rectangle((int)xx.X - IconWidth / 2, (int)xx.Y - IconHeight, IconWidth, IconHeight);
                        //Rectangle rect = new Rectangle((int)x, (int)y, IconWidth, IconHeight);
                        //if (billBoard.Enabled == true)
                        {
                            if (billBoard.Selected == true)
                            {
                                g.DrawImage(billBoard.SelectImage, rect);
                            }
                            else
                            {
                                g.DrawImage(billBoard.Image, rect);
                            }
                        }
                    }
                }
            }

            if (m_bShowEquipmentZone && m_showPolygonList != null && m_showPolygonList.Count > 0)
            {
                g.ResetTransform();
                List<Point> ddd = new List<Point>();

                for (int i = 0; i < m_showPolygonList.Count; i++)
                {
                    //float x = ConvertPoiX((float)m_showPolygonList[i].x / 1000);
                    //float y = ConvertPoiY((float)m_showPolygonList[i].y / 1000);
                    Point polygonPT = ConvertPT(m_showPolygonList[i].x, m_showPolygonList[i].y);

                    ddd.Add(polygonPT);
                }
                GraphicsPath path = new GraphicsPath();
                path.AddLines(ddd.ToArray());
                e.Graphics.FillPath(mBrushRedRect, path);
                e.Graphics.DrawPath(mPenRedRect, path);
            }

            if (m_bZoom)
                m_bZoom = false; 

            if (mbDrag == true && bRectZoomMode == true)
            {
                g.ResetTransform();                
                g.DrawRectangle(mPenRect, mRectDrawing);
            }  
            
            if( m_bDrawCompass == true && m_imgCompass != null)
            {
                g.ResetTransform();
                int nWidth = 180;
                float nHalf = nWidth / 2;
                g.TranslateTransform(nHalf, nHalf);
                //now rotate the image
                g.RotateTransform(m_fAzimuth);
                g.TranslateTransform(-nHalf, -nHalf);
                g.DrawImage(m_imgCompass, new Rectangle(0, 0, nWidth, nWidth));
            } 

            // 빌딩 Text  
            if (m_bDrawBuildingText)
            {
                Font font = new System.Drawing.Font(this.prgFont, 13F * mTransform.Elements[0], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                for (int i = 0; i < m_arrBuildingGroupText.Count; i += 5)
                {
                    string strBuildingName = m_arrBuildingGroupText[i].ToString();

                    Point minPt = ConvertPT(Convert.ToDouble(m_arrBuildingGroupText[i + 1]), Convert.ToDouble(m_arrBuildingGroupText[i + 2]));
                    Point maxPt = ConvertPT(Convert.ToDouble(m_arrBuildingGroupText[i + 3]), Convert.ToDouble(m_arrBuildingGroupText[i + 4]));

                    SizeF fontSize = g.MeasureString(strBuildingName, font);
                    Point beginPt = new Point(((maxPt.X + minPt.X) / 2) - (int)(fontSize.Width / 2), ((maxPt.Y + minPt.Y) / 2) - (int)(fontSize.Height / 2));

                    g.DrawString(strBuildingName, font, brushBlue, beginPt.X, beginPt.Y);
                }

                for (int i = 0; i < m_arrBuildingText.Count; i += 5)
                {
                    string strBuildingName = m_arrBuildingText[i].ToString();

                    Point minPt = ConvertPT(Convert.ToDouble(m_arrBuildingText[i + 1]), Convert.ToDouble(m_arrBuildingText[i + 2]));
                    Point maxPt = ConvertPT(Convert.ToDouble(m_arrBuildingText[i + 3]), Convert.ToDouble(m_arrBuildingText[i + 4]));

                    SizeF fontSize = g.MeasureString(strBuildingName, font);
                    Point beginPt = new Point(((maxPt.X + minPt.X) / 2) - (int)(fontSize.Width / 2), ((maxPt.Y + minPt.Y) / 2) - (int)(fontSize.Height / 2));

                    g.DrawString(strBuildingName, font, brushRed, beginPt.X, beginPt.Y);
                }
            }
        }
        private Brush brushRed = new SolidBrush(Color.Red);
        private Brush brushBlue = new SolidBrush(Color.Blue);

        public string prgFont = "굴림";

        /// <summary>
        /// 좌표 변환
        /// LOD 1단계일때의 좌표가 몇% 지점에 있는지 파악하여 이미지 SIZE가 변경된 후 해당 % 지점을 반환
        /// </summary>
        private Point ConvertPT(float x, float y)
        { 
            // LOD 1단계일때 지점(%) : a
            double xPer = x / m_nLod1ImageSize.Width * 100;
            double yPer = y / m_nLod1ImageSize.Height * 100;

            // 현재 이미지 사이즈에서 a 지점
            double curTargetX = mSizeImage.Width * xPer / 100;
            double curTargetY = mSizeImage.Height * yPer / 100;

            double tempX = (curTargetX * mTransform.Elements[0]) + mTransform.Elements[4];
            double tempY = (curTargetY * mTransform.Elements[3]) + mTransform.Elements[5];

            return new Point((int)tempX, (int)tempY);
        }
        private Point ConvertPT(double x, double y)
        { 
            // LOD 1단계일때 지점(%) : a
            double xPer = x / m_nLod1ImageSize.Width * 100;
            double yPer = y / m_nLod1ImageSize.Height * 100;

            // 현재 이미지 사이즈에서 a 지점
            double curTargetX = mSizeImage.Width * xPer / 100;
            double curTargetY = mSizeImage.Height * yPer / 100;

            double tempX = (curTargetX * mTransform.Elements[0]) + mTransform.Elements[4];
            double tempY = (curTargetY * mTransform.Elements[3]) + mTransform.Elements[5];

            return new Point((int)tempX, (int)tempY);
        }

        private void BaseMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                mPtPrev = e.Location;
                mPtDragStart = e.Location;
                mPtDragCurrent = e.Location;
                mbTranslateMode = false;

                mRectDrawing = new Rectangle();
                mbDrag = true;
                bRectZoomMode = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                mbDrag = true;
                mPtPrev = e.Location;
                mbTranslateMode = true;
                bRectZoomMode = false;
                mPtDragStart = e.Location;
                mPtDragCurrent = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                mbDrag = false;
                mPtPrev = e.Location;
                mbTranslateMode = false;
                bRectZoomMode = false;
                mPtDragStart = e.Location;
                mPtDragCurrent = e.Location;
            }
            else
            {
                mbDrag = false;
                bRectZoomMode = false;
                mbTranslateMode = false;

            }
        }
        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            DoMouseWork(sender, e, BaseMouseDown, MouseEvent.MOUSE_DOWN);

            IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.EnableFireReportBtn(false);           
        }

        public void ZoomIn()
        {
            if (m_bFreeze == true)
                return;

            float fCurScale = 1.0f / mTransform.Elements[3];

            if (!UpdateScale(true))
                return;
            
            mTransform.Translate(m_ptZoomCenter.X, m_ptZoomCenter.Y);
            mTransform.Scale(fCurScale, fCurScale);
            mTransform.Scale(m_nCurScale, m_nCurScale);
            mTransform.Translate(-m_ptZoomCenter.X, -m_ptZoomCenter.Y);
            Invalidate();
        }

        public void ZoomOut()
        {
            if (m_bFreeze == true)
                return;

            float fCurScale = 1.0f / mTransform.Elements[3];

            if (!UpdateScale(false))
                return;

            mTransform.Translate(m_ptZoomCenter.X, m_ptZoomCenter.Y);
            mTransform.Scale(fCurScale, fCurScale);
            mTransform.Scale(m_nCurScale, m_nCurScale); 
            mTransform.Translate(-m_ptZoomCenter.X, -m_ptZoomCenter.Y);

            Invalidate();
        }

        /// <summary>
        /// 스케일 update
        /// </summary>
        /// <param name="isZoomIn">ZOOM IN일 경우 true, OUT일 경우 false</param>
        private bool UpdateScale(bool isZoomIn)
        { 
            float fCurScale = m_nCurScale;
            if (isZoomIn)
            {
                if (fCurScale + m_nScaleGap > m_nMaxScale)
                    fCurScale = m_nMaxScale;
                else
                {
                    fCurScale = fCurScale + m_nScaleGap;
                    m_nCurScale = fCurScale;
                    return true;
                }
            }
            else
            {
                if (fCurScale - m_nScaleGap < m_nMinScale)
                    fCurScale = m_nMinScale;
                else
                {
                    fCurScale = fCurScale - m_nScaleGap;
                    m_nCurScale = fCurScale;
                    return true;
                }
            }

            return false;
        }

        private Point m_ptZoomCenter = new Point(); 
        private bool m_bZoom = false; 
        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_bFreeze == true)
                return;
             
            Form form = (Form)sender;
            Point ptOrg = form.PointToScreen(e.Location);
            Point ptNew = PointToClient(ptOrg);

            m_ptZoomCenter = ptNew;
            PointF pt = ScreenToGlobal(ptNew);

            float fCurScale = 1.0f / mTransform.Elements[3]; 
            if (e.Delta < 0)
            {
                if (!UpdateScale(false))
                    return;
            }
            else
            {
                if (!UpdateScale(true))
                    return;
            }
            
            int prevImgSizeW = (int)(mSizeImage.Width * mTransform.Elements[0]);
            int prevImgSizeH = (int)(mSizeImage.Height * mTransform.Elements[3]);
             
            float tWidth = ptNew.X - mTransform.Elements[4];
            float tHeight = ptNew.Y - mTransform.Elements[5];

            float widthPer = tWidth / prevImgSizeW * 100;
            float heightPer = tHeight / prevImgSizeH * 100;

            float prevEle4 = mTransform.Elements[4];
            float prevEle5 = mTransform.Elements[5];

            mTransform.Translate(pt.X, pt.Y);
            mTransform.Scale(fCurScale, fCurScale);
            mTransform.Scale(m_nCurScale, m_nCurScale); 
            mTransform.Translate(-pt.X, -pt.Y);
            
            // LOD LEVEL 지정   
            SetLodLevel(true);
             
            if (m_bZoom)
            {
                ResetImageSize();

                float ele0 = mTransform.Elements[0];
                float ele1 = mTransform.Elements[1];
                float ele2 = mTransform.Elements[2];
                float ele3 = mTransform.Elements[3];
                  
                float targetWidth = (((mSizeImage.Width * mTransform.Elements[0]) * widthPer) / 100);
                float targetHeight = (((mSizeImage.Height * mTransform.Elements[3]) * heightPer) / 100);

                float eleX = ptNew.X - targetWidth;
                float eleY = ptNew.Y - targetHeight; 

                mTransform = new Matrix(ele0, ele1, ele2, ele3, eleX, eleY); 
            }
             
            Invalidate();
        }

        public void BaseMouseMove(object sender, MouseEventArgs e)
        { 
            mPtCurrent = e.Location;

            if (mbDrag)
            {
                if (mbRotationMode == true)
                {
                    //float delta = 1.0f;
                    //Point ptNew = e.Location;
                    //int dx = mPtPrev.X - ptNew.X;
                    //if (dx < 0)
                    //{
                    //    delta = -1.0f;
                    //}
                    //if (delta == -1.0f)
                    //{
                    //    m_RotationAngle--;
                    //}
                    //else
                    //{
                    //    m_RotationAngle++;
                    //}

                    //if (mbRotationMode == true)
                    //{
                    //    PointF fptStart = ScreenToGlobal(mPtPrev);
                    //    PointF fptCurrent = ScreenToGlobal(mPtCurrent);
                    //    try
                    //    {
                    //        Point ptCenter = new Point();
                    //        ptCenter.X = mRectImage.X + mRectImage.Width / 2;
                    //        ptCenter.Y = mRectImage.Y + mRectImage.Height / 2;

                    //        float fValue = (float)GetAngle(mPtPrev, mPtCurrent, ptCenter);
                    //        System.Diagnostics.Trace.WriteLine("DDD : " + fValue);
                    //        mTransform.Translate(panel1.Width * 0.5f, panel1.Height * 0.5f);
                    //        mTransform.Rotate(fValue);
                    //        mTransform.Translate(-panel1.Width * 0.5f, -panel1.Height * 0.5f);
                    //    }
                    //    catch (Exception)
                    //    {
                    //    }
                    //}
                    //mPtPrev = e.Location;
                }
                else if (mbTranslateMode == true)
                {
                    PointF prevPt = ScreenToGlobal(mPtPrev);
                    PointF fpt = ScreenToGlobal(e.Location);

                    float dx = fpt.X - prevPt.X;
                    float dy = fpt.Y - prevPt.Y;

                    float curEle4 = mTransform.Elements[4];
                    float curEle5 = mTransform.Elements[5];
                    // 변경될 element
                    float chgEle4 = curEle4 + (dx * mTransform.Elements[0]);
                    float chgEle5 = curEle5 + (dy * mTransform.Elements[3]);
                    // 실제 Image Size
                    float imageWidth = mSizeImage.Width * mTransform.Elements[0];
                    float imageHeight = mSizeImage.Height * mTransform.Elements[3];
                                        
                    // 화면 밖으로 완전히 나가지 않도록 (50px까지 허용)
                    // zoom in/out 으로 인해 이미 화면 밖으로 나가있는 상태라면
                    bool isScOutside = false;
                    bool move = false;
                    if (curEle4 + imageWidth < 50 || curEle5 + imageHeight < 50)
                        isScOutside = true;
                    else if (curEle4 > this.Width - 50 || curEle5 > this.Height - 50)
                        isScOutside = true;

                    if (isScOutside)
                        move = true;
                    else
                    {
                        if (chgEle4 + imageWidth > 50 && chgEle5 + imageHeight > 50)
                        {
                            if (chgEle4 < this.Width - 50 && chgEle5 < this.Height - 50)
                            {
                                move = true;
                            }
                        }
                    }

                    if (move)
                        mTransform.Translate(dx, dy);

                    mPtPrev = e.Location;
                }
                else if (bRectZoomMode == true)
                {
                    mRectDrawing = CalcRect(mPtDragStart, e.Location);
                }
                mPtDragCurrent = e.Location;
            }
        }

        public void OnMouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            DoMouseWork(sender, e, BaseMouseUp, MouseEvent.MOUSE_UP);

            if (e.Button == MouseButtons.Left)
            {
                if (m_currentMode == MouseWorkMode.PICK)
                {
                    // IF NOT POI MOVE MODE
                    if (m_bDragPoi == false)
                        PickPOI(e.X, e.Y);
                    else
                    {
                        if (mPOIDragged != null)
                            OnPostMovePOI(mPOIDragged, e);

                        TurnOnTemporaryList();
                    }
                    mPOIDragged = null;
                    m_bDragPoi = false;
                }
                else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
                {
                    CreateFireSensor(e, null);
                }
                else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
                {
                    CreateSpringCooler(e, null);
                }
                else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
                {
                    CreatePumpPressure(e, null);
                }
                else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
                {
                    DeletePOI(e.X, e.Y);
                }
                else if (m_currentMode == MouseWorkMode.NEW_CCTV)
                {
                    CreateCCTVPOI(e, null);
                }
            }
            //Invalidate(true);
        }

        public void OnMouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            DoMouseWork(sender, e, BaseMouseMove, MouseEvent.MOUSE_MOVE);

#if DEBUG
			//Position3D pos = GetCameraPosition();
			//Quaternion3D ori = GetCameraOrientaion();
			//Position3D dir = GetCameraDirection();

			//if (pos != null)
			//{
			//    Debug.WriteLine("POSITION : " + pos.X + "," + pos.Y + "," + pos.Z);
			//    Debug.WriteLine("DIRECTION : " + dir.X + "," + dir.Y + "," + dir.Z);
			//    Debug.WriteLine("ORIENTATION : " + ori.X + "," + ori.Y + "," + ori.Z + ","+ ori.W );
			//}
#endif
        }

        private void OnPostMovePOI(POI poi, MouseEventArgs e)
        {
            if (poi.Type == IFacility.FacilityType.CCTV)
            {
                AddCCTVEditData(poi, e);
            }
        }

        private void AddCCTVEditData(POI poi, MouseEventArgs e)
        {
            CCTV cctv = (CCTV)poi.Facility;
            if (cctv == null)
                return;

            IChangedDataManager owner = UnE.View.Content.ViewUtils.GetContentViewOwner().IChangedDataManager;
            
            EditCCTV editCCTV = new EditCCTV(cctv);
            editCCTV.Position = new UnE.Geometry.Vertex3F(poi.X, poi.Y, poi.Z);
            editCCTV.Zone = GetPOIZone(e, poi.X, poi.Y, poi.Z);
            editCCTV.AddToManager(owner);

            poi.Zone = editCCTV.Zone;
        }

        private Zone GetPOIZone(MouseEventArgs e, float x, float y, float z)
        {
            if (m_bIndoor)
            {
                float nCurrentFloorIndex = -1.0f;
                Building building = m_frmParent.GetCurrentBuilding(ref nCurrentFloorIndex);

                if (building == null)
                    return null;

                return ZoneManager.Instance.GetZone(building.BuildingID, nCurrentFloorIndex);
            }
            else
            {
                MouseEventArgs arg = new MouseEventArgs(MouseButtons.Right, e.Clicks, e.X, e.Y, e.Delta);
                OnSavePt(arg);
                string strBuildingID = ZoneManager.Instance.GetBuildingName(x, y);
                if (strBuildingID == "")
                {
                    //ClearSelect();
                    return ZoneManager.Instance.GetOutsideZone(x, y);
                }
                else
                {
                    //ClearSelect();
                    Building building = ZoneManager.Instance.GetBuilding(strBuildingID);
                    if (building != null)
                    {
                        Zone zone = ZoneManager.Instance.GetZone(strBuildingID, building.MaxFloorIndex - 1);
                        if (zone == null)
                        {
                            return ZoneManager.Instance.GetOutsideZone(x, y);
                        }
                        return zone;
                    }
                }
                return ZoneManager.Instance.GetOutsideZone(x, z);
            }
        }


        public void BaseMouseUp(object sender, MouseEventArgs e)
        {
            Point pt = e.Location; 
            // Popup Menu
            if (e.Button == MouseButtons.Right)
            {
                if(mPopup != null)
                {
                    Point ptScreen = PointToScreen(pt);
                    mPopup.Show(ptScreen.X, ptScreen.Y);
                    mPopup.Tag = pt;
                }                
                return;
            }
            // Rect Zoom Mode인 경우 
            else if (e.Button == MouseButtons.Left)
            {
                if (bRectZoomMode == true)
                {
                    OnRectZoom(mPtDragStart, pt);
                }
            }       
          
            mRectDrawing = new Rectangle();
            mbDrag = false;
            bRectZoomMode = false;
            mbRotationMode = false;
            mbTranslateMode = false;

            mPtPrev = pt;
            mPtDragCurrent = pt;
        }

        // POI Drag Target
        private POI mPOIDragged = null;
        private bool m_bDragPoi = false;

        private void DoMouseWork(Object sender, MouseEventArgs e, MouseEventHandler baseHandler, MouseEvent mouseEvent)
        {
            if (m_bFreeze == true)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (m_currentMode == MouseWorkMode.PICK)
                {
                    if (this.Focused == false)
                        Focus();

                    if (mouseEvent == MouseEvent.MOUSE_DOWN)
                    {
                        if (m_bEditMode == true)
                        {
                            // SET POI MOVE MODE
                            mPtPrev.X = e.X;
                            mPtPrev.Y = e.Y;
                            int nPOIID = OnSelectPOI(e.X, e.Y);
                            if (nPOIID != -1)
                            {
                                if (m_dicPOIs.ContainsKey(nPOIID))
                                {
                                    mPOIDragged = m_dicPOIs[nPOIID];
                                    if (mPOIDragged != null)
                                    {                                        
                                        OnPostPick(null, m_arrTemporaryHiddenPOIs, false);
                                    }
                                }
                            }
                        }
                    }
                    else if (mouseEvent == MouseEvent.MOUSE_MOVE)
                    {
                        if (m_bEditMode == true)
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                int dx = e.X - mPtPrev.X;
                                int dy = e.Y - mPtPrev.Y;
                                // POI MOVE
                                if (mPOIDragged != null && dx != 0 && dy != 0)
                                {
                                    POI poi = mPOIDragged;

                                    BillBoard icon = GetBillboard(poi.ID);
                                    PointF ptOrg = new PointF(icon.TX, icon.TY);
                                    Point pt = GlobalToScreen(ptOrg);
                                    pt.X = pt.X + dx;
                                    pt.Y = pt.Y + dy;

                                    PointF pos = ScreenToGlobal(pt);
                                    mPtPrev.X = e.X;
                                    mPtPrev.Y = e.Y;

                                   // 
                                    if (MovePOI(poi.ID, pos.X, pos.Y))
                                    {
                                        PointF pt2 = GetReverseRotateTransformPoint(new PointF(pos.X, pos.Y));
                                        poi.X = pt2.X;
                                        poi.Y = pt2.Y;
                                        poi.Z = 0;

                                        Refresh();

                                        m_bDragPoi = true;
                                    }
                                }
                            }
                            else
                            {
                                m_bDragPoi = false;
                                mPOIDragged = null;
                            }
                        }
                    }
                }
                else if (m_currentMode == MouseWorkMode.PANNING)
                {
                    if (m_bFreeze == true)
                        return;
                
                    OnPrevPanning(mouseEvent);

                    MouseEventArgs arg = new MouseEventArgs(MouseButtons.Middle, e.Clicks, e.X, e.Y, e.Delta);
                    baseHandler(sender, arg);

                    OnPostPanning(mouseEvent);
                }
                else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_CCTV)
                { }
                else
                {
                    //OnPrevOrbit(mouseEvent);
                    baseHandler(sender, e);
                    //OnPostOrbit(mouseEvent);
                }
            }
            else
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    IFormContent formContent = UnE.View.Content.ViewUtils.GetContentView();
                    //IFormContent formContent = PageBackstageHome.Instance.ContentForm;
                    ToolStripMenuItem menuIndoor = formContent.GetMenu("Indoor");
                    ToolStripMenuItem menuManualReport = formContent.GetMenu("ManualReport");
                    ToolStripMenuItem menuManualCCTV = formContent.GetMenu("ManualCCTV");

                    if (mouseEvent == MouseEvent.MOUSE_UP)
                    {
                        Point pt = PointToScreen(new Point(e.X, e.Y));
                        if (this.Popup != null && this.Popup.Enabled == true)
                        {
                            this.Popup.Show(pt.X, pt.Y);
                        }
                    }
                    if (mouseEvent == MouseEvent.MOUSE_DOWN)
                    {
                        OnSavePt(e);

                        PointF pos = ScreenToGlobal(e.Location);

                        menuIndoor.Enabled = true;

                        ToolStripItemCollection c = menuIndoor.DropDownItems;
                        c.Clear();

                        ToolStripItemCollection r = menuManualReport.DropDownItems;
                        r.Clear();

                        ToolStripItemCollection v = menuManualCCTV.DropDownItems;
                        v.Clear();

                        Building building = null;
                        if (m_bIndoor)
                        {
                            if (m_currentIndoorZone != null)
                            {
                                building = m_currentIndoorZone.Building;
                                menuIndoor.Enabled = false;
                                menuManualReport.Tag = m_currentIndoorZone;
                                menuManualCCTV.Tag = m_currentIndoorZone;                                
                            }
                        }     
                        else
                        {
                            //Point d =  ConvertPT(pos.X, pos.Y);
                            string strBuildingName = ZoneManager.Instance.GetBuildingName(pos.X / m_nCurLodLevel, pos.Y / m_nCurLodLevel);
                            building = m_Owner.GetBuilding(strBuildingName);

                            if (building != null)
                            {
                                foreach (Zone zone in building.FloorList)
                                {
                                    ToolStripMenuItem item = new ToolStripMenuItem();
                                    item.Tag = zone;
                                    item.Click += m_Owner.MenuIndoorClicked;
                                    item.Text = zone.DisplayText;
                                    c.Add(item);

                                    ToolStripMenuItem item2 = new ToolStripMenuItem();
                                    item2.Tag = zone;
                                    item2.Click += m_Owner.MenualReportClicked;
                                    item2.Text = zone.DisplayText;
                                    r.Add(item2);

                                    ToolStripMenuItem item3 = new ToolStripMenuItem();
                                    item3.Tag = zone;
                                    item3.Click += m_Owner.ManualCCTVClicked;
                                    item3.Text = zone.DisplayText;
                                    v.Add(item3);
                                }

                                m_Owner.MenuManualReport.Tag = building;
                                m_Owner.MenuManualCCTV.Tag = building;

                                //Zone zone = ZoneManager.Instance.GetZone(d.X, d.Y);
                                //if (zone != null)
                                //{
                                //    m_currentIndoorZone = zone;

                                //    menuManualCCTV.Tag = m_currentIndoorZone;
                                //} 
                            }
                        }
                    }
                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                {
                    if (m_nMinScale == m_nMaxScale)
                        return;

                    OnPrevPanning(mouseEvent);
                }

                baseHandler(sender, e);

                if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                    OnPostPanning(mouseEvent);
            }

            if (e.Button == MouseButtons.None && mouseEvent == MouseEvent.MOUSE_MOVE)
            {
                ShowTooltip(e);
            }
            else
            {
                OnMouseLeave(this, new EventArgs());
            }
        } 

        private void OnMouseLeave(object sender, EventArgs e)
        {
            m_TooltipTimer.Stop();
            m_TooltipTimer.Enabled = false;

            if (m_formTooltip != null)
                m_formTooltip.Visible = false;

            m_formTooltip = null;
        }

        private void OnShowTooltip(object sender, EventArgs e)
        {
            //m_bShowTooltip = false;

            m_TooltipTimer.Stop();
            m_TooltipTimer.Enabled = false;

            int nPoiID = OnSelectPOI(m_nShowTooltipX, m_nShowTooltipY);
            if (nPoiID != -1)
            {
                POI poi = null;
                if (m_dicPOIs.TryGetValue(nPoiID, out poi))
                {
                    if (poi.Zone == null)
                        return;
                    if (poi == null || poi.Facility == null)
                        return;

                    if (poi.Facility.Type != IFacility.FacilityType.CCTV)
                        return;

                    if (m_bIndoor != poi.IsIndoor)
                        return;

                    //Point pt = GlobalToScreen(poi.X, poi.Y);

                    //float xPer = 0.0f;
                    //float yPer = 0.0f;
                    //foreach (BillBoard billBoard in mBillBoardList)
                    //{
                    //    if (billBoard.ID == poi.ID)
                    //    {
                    //        xPer = billBoard.XPer;
                    //        yPer = billBoard.YPer;
                    //        break;
                    //    }
                    //}

                    //float xPt = ((mSizeImage.Width * xPer / 100) * mTransform.Elements[0]) + mTransform.Elements[4];// -IconWidth / 2;
                    //float yPt = ((mSizeImage.Height * yPer / 100) * mTransform.Elements[3]) + mTransform.Elements[5]; // -IconHeight;

                    float xx = ConvertPoiX(poi.X);
                    float yy = ConvertPoiY(poi.Y);
                    Point pt = ConvertPT(xx, yy);

                    //Point pt = ConvertPT(poi.X, poi.Y);

                    //Point pt = new Point((int)xPt, (int)yPt);
                    CCTV cctv = (CCTV)poi.Facility;
                    m_formTooltip = new Form();

                    string szName = "CCTV : " + cctv.AccessKey;
                    string szZone = "위치 : " + poi.Zone.DisplayText;
                    Label lb = new Label();
                    lb.AutoSize = true;
                    lb.Text = szName;
                    lb.Location = new Point(10, 10);

                    int width1 = TextRenderer.MeasureText(lb.Text, new Font(lb.Font.FontFamily, lb.Font.Size, lb.Font.Style)).Width;

                    m_formTooltip.Controls.Add(lb);

                    Label lb2 = new Label();
                    lb2.AutoSize = true;
                    lb2.Text = szZone;
                    lb2.Location = new Point(10, 28);

                    int width2 = TextRenderer.MeasureText(lb2.Text, new Font(lb2.Font.FontFamily, lb2.Font.Size, lb2.Font.Style)).Width;

                    int maxWidth = width1 > width2 ? width1 : width2;
                    if (maxWidth < 130)
                    {
                        maxWidth = 130 + 20;
                    }
                    else
                    {
                        maxWidth = maxWidth + 20;
                    }
                    m_formTooltip.Controls.Add(lb2);

                    int nTooltipHeight = 50;
                    m_formTooltip.ShowInTaskbar = false;
                    m_formTooltip.Size = new Size(maxWidth, nTooltipHeight);
                    m_formTooltip.FormBorderStyle = FormBorderStyle.None;
                    m_formTooltip.StartPosition = FormStartPosition.Manual;
                    m_formTooltip.Opacity = 0.8f;
                    m_formTooltip.Location = PointToScreen(new Point(pt.X - (maxWidth / 2), pt.Y - nTooltipHeight - 50));
                    m_formTooltip.Show();

                    //m_bShowTooltip = true;
                    return;
                }
            }
        }

        private void ShowTooltip(MouseEventArgs e)
        {
            if (m_nShowTooltipX != e.X || m_nShowTooltipY != e.Y)
            {
                m_TooltipTimer.Stop();
                m_TooltipTimer.Enabled = false;

                //m_bShowTooltip = false;
                if (m_formTooltip != null)
                {
                    m_formTooltip.Visible = false;
                    m_formTooltip = null;
                }
            }

            if (m_formTooltip == null)
            {
                m_nShowTooltipX = e.X;
                m_nShowTooltipY = e.Y;
                m_TooltipTimer.Enabled = true;
                m_TooltipTimer.Interval = 800;
                m_TooltipTimer.Start();
                //Debug.WriteLine("X={0}, Y={1}", m_nShowTooltipX, m_nShowTooltipY);
                //Debug.WriteLine(e.ToString());
            }
        }

        protected Point mSavedPt = new Point();
        protected void OnSavePt(MouseEventArgs e)
        {            
            mSavedPt = e.Location;
        }

        private void OnPrevPanning(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPrevOrbit(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPostPanning(MouseEvent e)
        {
            if (e == MouseEvent.MOUSE_UP)
            {
                TurnOnTemporaryList();

                // LOD에 따라 CCTV POI들을 가시화한다.
                ProcessCCTVLOD();
            }
            else
                OnScreenMove();
        }


        private bool IsInCamera(float x, float y, float z)
        {
            // 화면좌표로 변환
            // 저장된 화면 클립바운드와 체크
            return false;
        }


        public void ProcessCCTVLOD()
        {
            Type type = typeof(CCTV);
            m_arrLODShowingPOIs.Clear();

            foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
            {
                POI poi = pair.Value;

                if (poi.Popup == null || poi.Facility == null || poi.Facility.GetType() != type)
                    continue;

                CCTV cctv = (CCTV)poi.Facility;

                if (cctv.LODType == CCTV.LOD.VERY_IMPORTANT)
                {
                    if (IsInCamera(poi.X, poi.Y, poi.Z))
                    {
                        if (!poi.Popup.IsVisible())
                        {
                            Point pt = GlobalToScreen(poi.X, poi.Y);
                            poi.Popup.Show(pt.X, pt.Y);
                        }

                        m_arrLODShowingPOIs.Add(poi);
                    }
                    else
                    {
                        IPOIPopup ctrl = poi.Popup;
                        ctrl.Hide();
                    }
                }
                else if (cctv.LODType == CCTV.LOD.IMPORTANT)
                {
                    if (IsInCamera(poi.X, poi.Y, poi.Z))
                    {
                        if (!poi.Popup.IsVisible())
                        {
                            Point pt = GlobalToScreen(poi.X, poi.Y);
                            poi.Popup.Show(pt.X, pt.Y);
                        }

                        m_arrLODShowingPOIs.Add(poi);
                    }
                    else
                    {
                        IPOIPopup ctrl = poi.Popup;
                        ctrl.Hide();
                    }
                }
            }
        }

        private void TurnOnTemporaryList()
        {
            foreach (POI poi in m_arrTemporaryHiddenPOIs)
            {
                Point pt = GlobalToScreen(new PointF(poi.X, poi.Y));
                poi.Popup.Show(pt.X, pt.Y);
            }

            m_arrTemporaryHiddenPOIs.Clear();
        }

        private void OnScreenMove()
        {
            bool refresh = false;

            if (m_bIndoor)
            {
                if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    foreach (POI poi in arrPOIs)
                    {
                        if (OnMovePOI(poi))
                            refresh = true;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
                {
                    if (OnMovePOI(pair.Value))
                        refresh = true;
                }
            }

            if (refresh)
            {
                Update();
            }
        }

        private bool OnMovePOI(POI poi)
        {
            IPOIPopup popup = poi.Popup;

            if (popup != null && popup.IsVisible())
            {
                Point pt = GlobalToScreen(new PointF(poi.X, poi.Y));
                popup.Show(pt.X, pt.Y);
                return true;
            }

            return false;
        }


        private bool m_bIndoor = true;
        public bool IsIndoor
        {
            get { return m_bIndoor; }
            set { m_bIndoor = value; }
        }

        private void OnPostPick(POI poi, ArrayList arrHidden = null, bool absolutely = false)
        {
            bool refresh = false;

            if (arrHidden != null)
                arrHidden.Clear();

            if (m_bIndoor)
            {
                if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    foreach (POI _poi in arrPOIs)
                    {
                        if (_poi == poi || _poi.Popup == null || !_poi.Popup.IsVisible())
                            continue;

                        if (arrHidden != null)// && IsLODShowingPOI(_poi))
                            arrHidden.Add(_poi);

                        _poi.Popup.Hide(absolutely);
                        refresh = true;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
                {
                    if (pair.Value == poi || pair.Value.Popup == null || !pair.Value.Popup.IsVisible())
                        continue;

                    if (arrHidden != null)// && IsLODShowingPOI(pair.Value))
                        arrHidden.Add(pair.Value);

                    pair.Value.Popup.Hide(absolutely);
                    refresh = true;
                }
            }

            if (poi != null)
            {
                IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.OnPostPickPOI(poi);
            }

            if (refresh)
            {
                Update();
            }
        }


        private void OnRectZoom(Point ptStart, Point ptEnd)
        {
            Rectangle rect = CalcRect(ptStart, ptEnd);

            PointF ptZoomCenter = ScreenToGlobal(new Point(rect.Location.X + (int)(rect.Width / 2.0f),
                   rect.Location.Y + (int)(rect.Height / 2.0f)));

            //가로                  
            float fWidth = 0.0f;
            float fHeight = 0.0f;
            float fScale = 0.0f;
            //가로비
            fWidth = GetRatio(rect.Size.Width, this.Size.Width);
            //세로비
            fHeight = GetRatio(rect.Size.Height, this.Size.Height);

            // 작은 쪽을 기준으로 한다.
            fScale = (fWidth <= fHeight) ? fWidth : fHeight;

            // 시스템 스케일을 가져온다
            fScale = FindSystemScale(fScale);

            //확대인지 축소인지?
            //축소면 뒤집음(
            if (ptEnd.X - ptStart.X < 0)
                fScale = 1.0f / fScale;

            fScale = fScale * mTransform.Elements[3];
            if (fScale < m_nMinScale)
                fScale = m_nMinScale;

            if (fScale > m_nMaxScale)
                fScale = m_nMaxScale;

            float fRevScale = 1.0f / mTransform.Elements[3];
            mTransform.Translate(ptZoomCenter.X, ptZoomCenter.Y);
            mTransform.Scale(fRevScale, fRevScale);
            mTransform.Scale(fScale, fScale);
            mTransform.Translate(-ptZoomCenter.X, -ptZoomCenter.Y);
        }

        public Point GlobalToScreen(PointF fpt)
        {
            Matrix mTemp = mTransform.Clone();
            PointF[] myArray =
            {
                fpt
            };
            mTemp.TransformPoints(myArray);
            int x = (int)myArray[0].X;
            int y = (int)myArray[0].Y;
            return new Point(x, y);
        }

        public Point GlobalToScreen(float xf, float yf)
        {          
            PointF fpt = new PointF(xf, yf);
            return GlobalToScreen(fpt);
        }

        public PointF ScreenToGlobal(Point pt)
        {
            PointF ff = new PointF(pt.X, pt.Y);
            return ScreenToGlobal(ff);
        }

        public PointF ScreenToGlobal(PointF fpt)
        {
            Matrix mTemp = mTransform.Clone();
            try
            {
                mTemp.Invert();
            }
            catch (Exception)
            {
            }
            PointF[] myArray =
            {
                fpt
            };
            mTemp.TransformPoints(myArray);
            return new PointF(myArray[0].X, myArray[0].Y);
        }        
 
        private float FindSystemScale(float fAspectScale)
        {
            float fResult = fAspectScale;
            for (int i = 0; i < mScaleList.Length; i++)
            {
                float fTest = mScaleList[i];
                if (fAspectScale - 0.002f < mScaleList[i] && fAspectScale + 0.002f >= mScaleList[i])
                {
                    fResult = mScaleList[i];
                    break;
                }
            }
            return fResult;
        }

        //현재 Scale값 구하기
        private int GetSacleIndex(bool bZoomIn)
        {
            float fScale = mTransform.Elements[0];
            int nIndex = -1;

            for (int i = 1; i < mScaleList.Length ; i++)
            {
                if (bZoomIn == false && (fScale >= mScaleList[i] && fScale < mScaleList[i - 1]))
                {                  
                    nIndex = i;
                    break;
                }
                if (bZoomIn == true &&  (fScale > mScaleList[i]))
                {
                    nIndex = i - 1;
                    break;
                }
            }
             
            if (fScale <= mScaleList[98])
            {
                nIndex = 98;
            }

            if(fScale >= mScaleList[0])
            {
                nIndex = 0;
            }

            if (nIndex == -1)
            {
                throw new ArithmeticException();
            }

            return nIndex;
        }

        private float GetAngle(Point ptStart, Point ptCurrent, Point ptCenter)
        {
            //int a =0;
            float Ax = (ptCurrent.X - ptCenter.X);
            float Bx = (ptStart.X - ptCenter.X);
            float Ay = (ptCurrent.Y - ptCenter.Y);
            float By = (ptStart.Y - ptCenter.Y);

            //내적
            float fInProduct = (Ax * Bx) + (Ay * By);
            float cross = (Ax * By ) - ( Ay * Bx);

            //lAl * lBl
            float fValueA = (float)System.Math.Sqrt(Ax * Ax + Ay * Ay);
            float fValueB = (float)System.Math.Sqrt(By * By + Bx * Bx);

            if( fValueA < 0.001f && fValueA > -0.000f)
            {
                fValueA = 1.0f;
                fValueB = 1.0f;
            }

            if (fValueB < 0.001f && fValueB > -0.000f)
            {
                fValueA = 1.0f;
                fValueB = 1.0f;
            }

            float fValue = fInProduct / (fValueA * fValueB);
            float fValue2 = cross / (fValueA * fValueB);
            //double dAngle = Math.Acos(fValue);
            if( fValue > 1.0f)            
            {
                fValue = 1.0f; 
            }

            if( fValue < - 1.0f)
            {
                fValue = -1.0f;
            }

            if (fValue2 > 1.0f)
            {
                fValue2 = 1.0f;
            }

            if (fValue2 < -1.0f)
            {
                fValue2 = -1.0f;
            }

            float fSeta = (float)System.Math.Acos(fValue);

            //float fSin = (float)Math.ASin(fSeta);

            float dAngle = (float)(fSeta * 180.0f / System.Math.PI);
            

            if(fValue2 > 0.0f)
            {
                dAngle *= -1.0f;
            }

            return dAngle;
        }

        //화면비 구하기
        private void GetAspectRatio(Size size, out int rWidth, out int rHeight)
        {
            //최대공약수
            int GreatestMeasure = 0;

            if (size.Width > size.Height)
            {
                GreatestMeasure = GetGreatestMeasure(size.Width, size.Height);
            }
            else
            {
                GreatestMeasure = GetGreatestMeasure(size.Height, size.Width);
            }


            rWidth = size.Width / GreatestMeasure;
            rHeight = size.Height / GreatestMeasure;
        }
		
        //최대공약수 구하기
        private int GetGreatestMeasure(int a, int b)
        {

            int temp = 0;
            while (a != 0)
            {
                if (a < b)
                {
                    temp = a;
                    a = b;
                    b = temp;
                    break;
                }
                a = a - b;
            }

            return b;
        }

        //m:n 구하기
        private float GetRatio(int m, int n)
        {
            if (n == 0)
                return 0.0f;

            float fResult = 0.0f;
            fResult = (float)m / (float)n;
            return fResult;
        }               
		
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Point ptCenter = new Point(Width / 2, Height / 2);
            PointF pt1 = ScreenToGlobal(ptCenter);

            if (!base.ProcessCmdKey(ref msg, keyData))
            {
                if (keyData.Equals(Keys.Up))
                {
                    PointF ptOffset = new PointF((float)ptCenter.X, ptCenter.Y - 10 * GetScale());
                    PointF pt2 = ScreenToGlobal(ptOffset);                   
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
                else if (keyData.Equals(Keys.Down))
                {
                    PointF ptOffset = new PointF((float)ptCenter.X, ptCenter.Y + 10 * GetScale());
                    PointF pt2 = ScreenToGlobal(ptOffset);
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
                else if (keyData.Equals(Keys.Left))
                {
                    PointF ptOffset = new PointF(ptCenter.X - 10 * GetScale(), (float)ptCenter.Y);
                    PointF pt2 = ScreenToGlobal(ptOffset);
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
                else if (keyData.Equals(Keys.Right))
                {
                    PointF ptOffset = new PointF(ptCenter.X + 10 * GetScale(), (float)ptCenter.Y);
                    PointF pt2 = ScreenToGlobal(ptOffset);
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
            }
            return false;
        }

        // 외부에서 Tooltip용 Control을 Add할때 사용 (ISensorTooltipOwner)
        public void AddToolTipControl(System.Windows.Forms.Control c)
        {
            if (this != null)
            {
                this.Controls.Add(c);
            }

        }

        public void EnablePOI(int nID, string szType, bool bEnable)
        {

        }

        private string GetIconPath(string szPath)
        {
            if (szPath.IndexOf("\\Media\\icons\\") != -1)
                return szPath;
            string szType = szPath.ToLower();
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            string resultPath = owner.ResourcePath + string.Format("\\Media\\icons\\{0}.ico", szType);
            return resultPath;
        }


        public int AddPOI(string szPath, float x, float y)
        {
            string szFilePath = GetIconPath(szPath);
            Image img = Image.FromFile(szFilePath);
            BillBoard billBoard = new BillBoard();
            billBoard.Image = img;
            m_nBillboardID++;
            billBoard.ID = m_nBillboardID;
            //PointF fpt2 = ScreenToGlobal();

            PointF rPt = GetRotateTransformPoint(new PointF(x, y));            
            billBoard.TX = rPt.X;
            billBoard.TY = rPt.Y;

            //float fConvertX = ConvertPoiX(rPt.X);
            //float fConvertY = ConvertPoiY(rPt.Y);
            //billBoard.TX = fConvertX;
            //billBoard.TY = fConvertY;  

            billBoard.Width = mBillboardWidth;
            billBoard.Height = mBillboardHeight;

            // test
            billBoard.XPer = rPt.X / mSizeImage.Width * 100;
            billBoard.YPer = rPt.Y / mSizeImage.Height * 100;

            mBillBoardList.Add(billBoard);
            return m_nBillboardID;
        }

        private float ConvertPoiX(float x)
        {
            x = x * 55 / 100;
            float tempX = x + 267;

            return tempX; 
        }

        private float ConvertPoiY(float y)
        {
            y = y * 60 / 100;
            float tempY = m_nLod1ImageSize.Height - y - 210;
            

            return tempY;
        }

        public int AddPOI(string szPath)
        {
            string szFilePath = GetIconPath(szPath);
            Image img = Image.FromFile(szFilePath);
            BillBoard billBoard = new BillBoard();
            billBoard.Image = img;
            m_nBillboardID++;
            billBoard.ID = m_nBillboardID;
            

            billBoard.TX = mPtCurrent.X;
            billBoard.TY = mPtCurrent.Y;

            billBoard.Width = mBillboardWidth;
            billBoard.Height = mBillboardHeight;
            mBillBoardList.Add(billBoard);
            return m_nBillboardID;
        }

        public void AddPOI(POI poi)
        { 
            poi.ViewType = 2;
            poi.ParentView = this;

            if (poi.Facility == null)
                return;
            
            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (!m_bIndoor || (m_bIndoor && poi.Zone == m_currentIndoorZone))
            {
                string strIconPath = poi.Facility.IconPath;
                string szFilePath = GetIconPath(strIconPath);
                int nID = AddPOI(szFilePath, poi.X, poi.Y);
                poi.ID = nID;
                m_dicPOIs[nID] = poi;
            }
            else if (poi.ID > 0)
                m_dicPOIs[poi.ID] = poi;

            if (m_bIndoor && poi.Zone != null)
            {
                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
                    arrPOIs.Add(poi);
                }
                else
                {
                    ArrayList arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                    arrPOIs.Add(poi);
                }
            }

            int nLayerID = poi.Facility.GetLayerID();
            m_frmParent.Layers.GetLayer(nLayerID).Add(poi.ID);
        }

        public void RemovePOI(int nID)
        {
            BillBoard board = GetBillboard(nID);
            if( board != null)
            {
                board.Visible = false;
                mBillBoardList.Remove(board);
            }
        }

        public void RemovePOI(float x, float y)
        {
            BillBoard delete = null;
            foreach (BillBoard board in mBillBoardList)
            {
                if (board.TX == x && board.TY == y)
                {
                    delete = board;
                    break;
                }
            }
            if (delete != null)
                mBillBoardList.Remove(delete);
        } 


        public bool IsTemporaryHiddenPOI(POI poi)
        {
            return false;
        }

        public  void EnablePOI(int nID, bool bEnable)
        {
            BillBoard billboard = GetBillboard(nID);
            if( billboard != null)
            {
                billboard.Enabled = bEnable;
            }
            Refresh();
        }

        private BillBoard GetBillboard(int nID)
        {
            BillBoard find = null;
            foreach (BillBoard board in mBillBoardList)
            {
                if (board.ID == nID)
                {
                    find = board;
                    break;
                }
            }
            return find;
        }

        public POI FindPOI(int nID)
        {
            if (m_dicPOIs.ContainsKey(nID))
                return m_dicPOIs[nID];

            return null;
        }

        public POI FindPOI(string szType)
        {
            return null;
        }

        public POI FindPOI(int nID, string szType)
        {
            if (m_dicPOIs.ContainsKey(nID))
                return m_dicPOIs[nID];

            return null;
        }

        public void UpdateIcon(int nID, string szPath)
        {
            BillBoard billBoard = GetBillboard(nID);
            if (billBoard == null) return;

            Image img = Image.FromFile(szPath);
            billBoard.Image = img;
        }

        public void SetCheckPoistion(bool bCheck)
        {

        }

        public void HideAllPOIPopup()
        {
            OnPostPick(null, null, true);
        }


        private void ClearAllSelectedPOI()
        {
            foreach (BillBoard board in mBillBoardList)
            {
                board.Selected = false;
            }
        }

        public void ClearPOISelection()
        {
            ClearAllSelectedPOI();
            m_arSelectedPoi.Clear();
        }

        public void ShowIconPOI(int nID, string szType, bool bShow)
        {
            BillBoard billboard = GetBillboard(nID);
            if (billboard != null)
            {
                billboard.Visible = bShow;
            }
            Refresh();
        }

        private void PickPOI(int x, int y)
        {
            int nPOIID = OnSelectPOI(x, y);
            bool bSelected = false;
            BillBoard board = GetBillboard(nPOIID);
            if( board != null)
            {
                // 이미 선택된 POI인지?
                bSelected = board.Selected;

                if ((ModifierKeys & Keys.Control) == Keys.Control)
                {
                    if (bSelected == false)
                    {
                        m_arSelectedPoi.Add(nPOIID);
                    }
                    else
                    {
                        m_arSelectedPoi.Remove(nPOIID);
                    }
                }
                else
                {
                    // Control키가 눌러지지 않는 경우 모두 클리어
                    ClearPOISelection();
                    bSelected = false;
                }

                // 현재 뷰에 포함된 POI인지 확인- add by skkim 2014-03-03
                if (nPOIID != -1 && m_dicPOIs.ContainsKey(nPOIID))
                {
                    POI poi = m_dicPOIs[nPOIID];
                    if (poi.IsIndoor == this.m_bIndoor)
                        SelectPOI(nPOIID, !bSelected);

                    if (!bSelected)
                    {
                        //if (m_dicPOIs.ContainsKey(nPOIID))
                        {
                            //POI poi = m_dicPOIs[nPOIID];
                            BaseViewOwner.SelectedPOI = poi;
                            bSelected = true;

                            if (poi.Popup != null)
                            {  
                                int IconHeight = (int)(32 * mTransform.Elements[3]);

                                //float xx = ConvertPoiX(poi.X);
                                //float yy = ConvertPoiY(poi.Y);
                                Point pt = ConvertPT(poi.X, poi.Y);

                                //Point pt = ConvertPT(poi.X, poi.Y);

                                poi.Popup.Show((int)pt.X, (int)pt.Y);

                                //Point pt = GlobalToScreen(poi.X, poi.Y);
                                //poi.Popup.Show(pt.X, pt.Y);
                            }
                        }
                    }
                }
            }
            else
            {
                ClearPOISelection();
                bSelected = false;

            }
         
            if (!bSelected)
                BaseViewOwner.SelectedPOI = null;

            OnPostPick(BaseViewOwner.SelectedPOI);
            
        }
        public void SelectPOI(int nID )
        {
            SelectPOI(nID, true);
        }

        public void SelectPOI(int nID, string szType)
        {
            SelectPOI(nID, true);
        }
        public void SelectPOI(int nID, bool bSelect )
        {
            ClearPOISelection();

            m_arSelectedPoi.Add(nID);

            BillBoard select = null;
            foreach (BillBoard board in mBillBoardList)
            {
                if (board.ID == nID)
                {
                    select = board;
                    break;
                }
            }
            if (select != null)
                select.Selected = bSelect;

            if (m_dicPOIs.ContainsKey(nID))
            {
                POI poi = m_dicPOIs[nID];
                BaseViewOwner.SelectedPOI = poi;
            }            
        }
        
        protected int OnSelectPOI(int x, int y)
        { 
            BillBoard select = null;
            ArrayList arTemp = new ArrayList(mBillBoardList);
            arTemp.Reverse();

            int IconWidth = (int)(32 * mTransform.Elements[0]);
            int IconHeight = (int)(32 * mTransform.Elements[3]); 

            foreach (BillBoard board in arTemp)
            {
                // 기존
                //Point pt1 = GlobalToScreen(board.X, board.Y);
                //Rectangle rect = new Rectangle(pt1.X, pt1.Y , board.Width, board.Height);

                //float rectX = ((mSizeImage.Width * board.XPer / 100) * mTransform.Elements[0]) + mTransform.Elements[4] -IconWidth / 2;
                //float rectY = ((mSizeImage.Height * board.YPer / 100) * mTransform.Elements[3]) + mTransform.Elements[5] -IconHeight;

                //float xx = ConvertPoiX(board.TX);
                //float yy = ConvertPoiY(board.TY);
                Point pt = ConvertPT(board.TX, board.TY);

                //PointF pt = ConvertPT(board.TX, board.TY);
                Rectangle rect = new Rectangle((int)pt.X - IconWidth / 2, (int)pt.Y - IconHeight, IconWidth, IconHeight);

                if (rect.Contains(x, y))
                {
                    select = board;
                    break;
                }
            }
            if (select != null)
                return select.ID;
            return -1;
        }

        private bool m_bDrawBitmap = false;
        public void SaveScreen(string szPath , bool bDrawCompass = true)
        {
            Bitmap bitmap = new Bitmap(this.Size.Width, this.Size.Height);


            bool bTemp = m_bDrawCompass;
            m_bDrawCompass = bDrawCompass;
            this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Size.Width, this.Size.Height));
            m_bDrawCompass = bTemp;

            try
            {
                bitmap.Save(szPath);
                
            }catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            
        }

        public void ClearAllData()
        {
            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    foreach (POI poi in arrPOIs)
                    {
                        // 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
                        RemovePOI(poi.ID);

                        if (poi.Popup != null)
                        {
                            poi.Popup.Close();
                            poi.Popup = null;
                        }
                    }
                }
            }
        }

        public void AddZoneName(string szName)
        {

        }

        public bool MovePOI(int nID, float x, float y)
        {
            BillBoard board = GetBillboard(nID);
            if (board == null)
                return false;

            board.TX = x;
            board.TY = y;
            
            return true;
        }

        private ContextMenuStrip mPopup = null;
        public ContextMenuStrip Popup
        {
            get { return mPopup; }
            set { mPopup = value; }
        }

        private MouseWorkMode m_currentMode = MouseWorkMode.NONE;
        public MouseWorkMode CurrentMouseWorkMode
        {
            get { return m_currentMode; }
            set { m_currentMode = value; }
        } 

        private bool m_bShowBuildingText = false;
        public bool bShowBuildingText = true;

        private bool m_bShowEquipmentZone = false;
        private List<Geometry.Vertex2D> m_showPolygonList = null;

        private Matrix rememberMatrix = null;

        public void ShowZone(Zone zone, bool bShow)
        {
            if (m_showPolygonList == null)
                m_showPolygonList = new List<Geometry.Vertex2D>();
            else
                m_showPolygonList.Clear();

            m_bShowEquipmentZone = bShow;

            List<EquipmentZone> equipZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
            foreach (EquipmentZone equipZone in equipZoneList)
            {
                UnE.Geometry.Polygon polygon = zone.Polygon;
                for (int i = 0; i < polygon.GetVertexCount(); i++)
                {
                    m_showPolygonList.Add(polygon.GetVertex(i));
                }
            }
            
            ShowPolygon();
        }

        public void ShowEquipmentZone(EquipmentZone zone, bool bShow)
        { 
            UnE.Geometry.Polygon polygon = zone.Polygon;
            m_showPolygonList = polygon.GetVertexList();
            m_bShowEquipmentZone = bShow;
            ShowPolygon();
        }

        private void ShowPolygon()
        {
            if (m_showPolygonList.Count > 0 && m_bShowEquipmentZone)
            {
                float ele0 = mTransform.Elements[0];
                float ele1 = mTransform.Elements[1];
                float ele2 = mTransform.Elements[2];
                float ele3 = mTransform.Elements[3];
                float ele4 = mTransform.Elements[4];
                float ele5 = mTransform.Elements[5];

                rememberMatrix = new Matrix(ele0, ele1, ele2, ele3, ele4, ele5);

                float minX = 0;
                float minY = 0;
                float maxX = 0;
                float maxY = 0;

                // 큰 사각형 구하기
                for (int i = 0; i < m_showPolygonList.Count; i++)
                {
                    //Point polygonPT = ConvertPT(m_showPolygonList[i].x, m_showPolygonList[i].y);

                    //float halfW = this.Width / 2;
                    //float halfImageW = mSizeImage.Width / 2;
                    //float w = halfW - polygonPT.X;
                    //
                    //float halfH = this.Height / 2;
                    //float halfImageH = mSizeImage.Height / 2;
                    //float h = halfH - polygonPT.Y;
                    //
                    //mTransform = new Matrix(ele0, ele1, ele2, ele3, ele4 + w, ele5 + h);

                    //break;

                    Point polygonPT = new Point((int)m_showPolygonList[i].x, (int)m_showPolygonList[i].y);

                    if (i == 0)
                    {
                        minX = polygonPT.X;
                        minY = polygonPT.Y;
                        maxX = polygonPT.X;
                        maxY = polygonPT.Y;
                    }
                    else
                    {
                        minX = System.Math.Min(minX, polygonPT.X);
                        minY = System.Math.Min(minY, polygonPT.Y);
                        maxX = System.Math.Max(maxX, polygonPT.X);
                        maxY = System.Math.Max(maxY, polygonPT.Y);
                    }
                }

                // 1배율 EquipZone의 Size, Location
                Point minPt = new Point((int)minX, (int)minY);
                Point maxPt = new Point((int)maxX, (int)maxY);

                Size imageSize = new System.Drawing.Size(maxPt.X - minPt.X, maxPt.Y - minPt.Y);

                // 스크린에 여백 비율 제외하고 늘어난 EquipZone Size, Location
                int beginX = this.Width * m_nDisasterEmptyPer / 100;
                int beginY = /*(this.Height - 87)*/ 877 * m_nDisasterEmptyPer / 100;

                int endX = this.Width * (100 - m_nDisasterEmptyPer) / 100;
                int endY = /*(this.Height - 87)*/ 877 * (100 - m_nDisasterEmptyPer) / 100;

                Point beginPt = new Point(beginX, beginY);
                Point endPt = new Point(endX, endY);

                Size disasterImageSize = new System.Drawing.Size(endPt.X - beginPt.X, endPt.Y - beginPt.Y);
                Point disasterImageCenterPt = new Point(this.Width / 2 - imageSize.Width / 2, /*(this.Height - 87)*/ 877 / 2 - imageSize.Height / 2);

                float rememberScale = -1.0f;
                int rememberLodLevel = -1;
                Point rememberEqPt = new Point();
                Size rememberEqSize = new Size();
                for (float i = m_nMinScale; i < m_nMaxScale; i += m_nScaleGap)
                {
                    int updateLodLevel = 0;
                    for (int j = m_nMinLodLevel; j <= m_nMaxLodLevel; j++)
                    {
                        if (!m_DicLodScale.ContainsKey(j))
                            continue;

                        if (i >= m_DicLodScale[j])
                        {
                            if (m_DicLodScale.ContainsKey(j + 1))
                            {
                                if (i < m_DicLodScale[j + 1])
                                {
                                    updateLodLevel = j;
                                    break;
                                }
                            }
                            else
                            {
                                updateLodLevel = j;
                                break;
                            }
                        }
                        else
                        {
                            updateLodLevel = j;
                            break;
                        }
                    }
                    int width = 0;
                    int height = 0;
                    foreach (ImageOption imgOption in m_BaseImages)
                    {
                        if (updateLodLevel != imgOption.LODLevel)
                            continue;

                        if (imgOption.RowIndex == 1)
                            width += imgOption.Width;
                        if (imgOption.ColumnIndex == 1)
                            height += imgOption.Height;
                    }

                    // 현재 LOD Level의 스크린 사이즈
                    Size tempScreenSize = new Size(width, height);
                    Point tempMinPt = hahaha(minX, minY, tempScreenSize, i);
                    Point tempMaxPt = hahaha(maxX, maxY, tempScreenSize, i);

                    Size tempSize = new Size(tempMaxPt.X - tempMinPt.X, tempMaxPt.Y - tempMinPt.Y);

                    // 스크린 빈공간으로 지정한 % (tPer) 보다 Equipzone 영역의 사이즈가 크거나 (가로,세로 둘중 하나라도)
                    // EquipZone 원래 크기의 2.5배 이상 커진다면
                    if (disasterImageSize.Width <= tempSize.Width || disasterImageSize.Height <= tempSize.Height || i >= 2.5)
                    {
                        if (rememberScale < 0)
                            rememberScale = i;
                        if (rememberLodLevel < 0)
                            rememberLodLevel = updateLodLevel;
                        if (rememberEqPt.X == 0 && rememberEqPt.Y == 0)
                            rememberEqPt = tempMinPt;
                        if (rememberEqSize.Width == 0 && rememberEqSize.Height == 0)
                            rememberEqSize = tempSize;

                        break;
                    }

                    rememberScale = i;
                    rememberLodLevel = updateLodLevel;
                    rememberEqPt = tempMinPt;
                    rememberEqSize = tempSize;
                    // 0. i Scale LodLevel 구하기
                    // 1. i와 i+= m_nScaleGap 사이에 있는 Size 인가
                    // 2. i 의 Scale의 LOD Level Setting
                }

                m_nCurScale = rememberScale;
                m_nCurLodLevel = rememberLodLevel;

                // LOD 단계 설정
                SetLodLevel(false);
                ResetImageSize();

                //float ele4temp = (rememberEqPt.X - (beginX * m_nCurScale)) * -1;
                //float ele5temp = (rememberEqPt.Y - (beginY * m_nCurScale)) * -1;


                float ele4temp = (rememberEqPt.X - beginX) * -1;
                float ele5temp = (rememberEqPt.Y - beginY) * -1;

                if (ele4temp > 0)
                    ele4temp = 0;
                if (ele5temp > 0)
                    ele5temp = 0;

                if (this.Width > (mSizeImage.Width * m_nCurScale) + ele4temp)
                {
                    float a = this.Width - ((mSizeImage.Width * m_nCurScale) + ele4temp);
                    ele4temp = ele4temp + a;
                }

                if (877 > (mSizeImage.Height * m_nCurScale) + ele5temp)
                {
                    float a = 877 - ((mSizeImage.Height * m_nCurScale) + ele5temp);
                    ele5temp = ele5temp + a;
                }

                mTransform = new Matrix(m_nCurScale, ele1, ele2, m_nCurScale, ele4temp, ele5temp);
            }

            Invalidate();
        }

        private Point hahaha(float x, float y, Size tempSize, float scale)
        {
            // LOD 1단계일때 지점(%) : a
            double xPer = x / m_nLod1ImageSize.Width * 100;
            double yPer = y / m_nLod1ImageSize.Height * 100;

            // 현재 이미지 사이즈에서 a 지점
            double curTargetX = tempSize.Width * xPer / 100;
            double curTargetY = tempSize.Height * yPer / 100;

            double tempX = (curTargetX * scale);
            double tempY = (curTargetY * scale);

            return new Point((int)tempX, (int)tempY);
        }

        private Point hahaha2(float x, float y, Size tempSize, float scale, float tPer)
        {
            float width = tempSize.Width * scale;
            float heigth = tempSize.Height * scale;

            float tempX = width * tPer / 100;
            float tempY = heigth * tPer / 100;

            return new Point((int)tempX, (int)tempY);
        }

        public void HideAllEquipmentZone()
        {
            m_bShowEquipmentZone = false;

            if (rememberMatrix != null)
            {
                float ele0 = rememberMatrix.Elements[0];
                float ele1 = rememberMatrix.Elements[1];
                float ele2 = rememberMatrix.Elements[2];
                float ele3 = rememberMatrix.Elements[3];
                float ele4 = rememberMatrix.Elements[4];
                float ele5 = rememberMatrix.Elements[5];

                mTransform = new Matrix(ele0, ele1, ele2, ele3, ele4, ele5);
                rememberMatrix = null;

                m_nCurLodLevel = m_nMinLodLevel;

                ResetImageSize();

                Invalidate();
            }            
        }
        //private void 

        private ArrayList m_arSelectedPoi = new ArrayList();
        public ArrayList SelectedPOIList
        {
            get { return m_arSelectedPoi; }
        }

        private int m_nBillboardID = 1;

        public POI CreateFireSensor(MouseEventArgs e, Zone zone)
        {
            PointF pos = ScreenToGlobal(e.Location);
           
            FireSensor sensor = new FireSensor();
            POI poi = new POI();
            
            PointF pt = GetReverseRotateTransformPoint(new PointF(pos.X, pos.Y));
            pos.X = pt.X;
            pos.Y = pt.Y;
            poi.Z = 0.0f;
            poi.Facility = sensor;
            //poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
            poi.IsIndoor = m_bIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            poi.Popup = sensor.CreatePopup(this, m_Factory);

            if (m_bIndoor)
            {
                poi.Zone = zone == null ? m_currentIndoorZone : zone;
            }
            else
            {
                poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, 0.0f) : zone;
            }

            EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, pos.X, pos.Y);
            if (equipZone == null)
                return null;
            sensor.EquipZoneID = equipZone.ID;

            string strPath = GetIconPath(sensor.IconPath);
            int nID = AddPOI(strPath, pos.X, pos.Y);
            poi.ID = nID;
            // set pick size;
            //base.SetPickSize(nID, 55, 55);

            m_dicPOIs[nID] = poi;
            BaseViewOwner.SelectedPOI = poi;
            m_frmParent.Layers.GetLayer(ID.ID_LAYER_DETECTOR).Add(nID);

            if (m_bIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            IChangedDataManager owner = UnE.View.Content.ViewUtils.GetContentViewOwner().IChangedDataManager;
            EditFireSensor editFireSensor = new EditFireSensor(sensor);
            editFireSensor.AddToManager(owner);

            return poi;
        }

        public POI CreateSpringCooler(MouseEventArgs e, Zone zone)
        {
            PointF pos = ScreenToGlobal(e.Location);

            SpringCooler sensor = new SpringCooler();
            POI poi = new POI();
            PointF pt = GetReverseRotateTransformPoint(new PointF(pos.X, pos.Y));
            pos.X = pt.X;
            pos.Y = pt.Y;
            poi.Z = 0.0f;
            poi.Facility = sensor;
            poi.IsIndoor = m_bIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            poi.Popup = sensor.CreatePopup(this, m_Factory);             
            poi.Zone =( (zone == null) ? m_currentIndoorZone : zone);
            
            EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, pos.X, pos.Y);
            string strPath = sensor.IconPath;
            int nID = AddPOI(strPath, pos.X, pos.Y);
            poi.ID = nID;
            // set pick size;
            //base.SetPickSize(nID, 55, 55);

            m_dicPOIs[nID] = poi;
            BaseViewOwner.SelectedPOI = poi;
            m_frmParent.Layers.GetLayer(ID.ID_LAYER_COOLER).Add(nID);

            if (m_bIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }
            IChangedDataManager owner = UnE.View.Content.ViewUtils.GetContentViewOwner().IChangedDataManager;
            EditSpringCooler editSpringCooler = new EditSpringCooler(sensor);
            editSpringCooler.AddToManager(owner);

            return poi;
        }

        public POI CreatePumpPressure(MouseEventArgs e, Zone zone)
        {
            PointF pos = ScreenToGlobal(e.Location);

            PumpPressureSensor sensor = new PumpPressureSensor();

            POI poi = new POI();
            PointF pt = GetReverseRotateTransformPoint(new PointF(pos.X, pos.Y));
            pos.X = pt.X;
            pos.Y = pt.Y;
            poi.Z = 0;
            poi.Facility = sensor;
            poi.IsIndoor = m_bIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            poi.Popup = sensor.CreatePopup(this, m_Factory);
            poi.Zone = ((zone == null) ? m_currentIndoorZone : zone);

            EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, pos.X, pos.Y);
            if (equipZone == null)
            {
                sensor.EquipZoneID = 0;
            }
            else
                sensor.EquipZoneID = equipZone.ID;
            string strPath = sensor.IconPath;
            int nID = AddPOI(strPath, pos.X, pos.Y);
            poi.ID = nID;
            // set pick size;


            //base.SetPickSize(nID, 55, 55);

            m_dicPOIs[nID] = poi;
            BaseViewOwner.SelectedPOI = poi;
            m_frmParent.Layers.GetLayer(ID.ID_LAYER_PERSURE).Add(nID);

            if (m_bIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            IChangedDataManager owner = UnE.View.Content.ViewUtils.GetContentViewOwner().IChangedDataManager;
            EditPumpPressuerSensor editPump = new EditPumpPressuerSensor(sensor);
            editPump.AddToManager(owner);

            return poi;
        }

        private PointF GetRotateTransformPoint(PointF pt)
        {
           
            if( IsIndoor == false)
            {
                return new PointF(pt.X, pt.Y);
            }
            if (m_currentIndoorZone == null && IsIndoor == true)
                return new PointF();

            
            Matrix t = new Matrix();
            PointF rCenter = new PointF(this.mSizeImage.Width / 2, mSizeImage.Height / 2);
            t.Translate(rCenter.X, rCenter.Y);
            t.Rotate(m_currentIndoorZone.Azimuth);
            t.Translate(-rCenter.X, -rCenter.Y);
            
            PointF[] myArray =
                {
                    pt
                };
            t.TransformPoints(myArray);
            return new PointF(myArray[0].X, myArray[0].Y);
        }

        private PointF GetReverseRotateTransformPoint(PointF pt)
        {

           
            if (IsIndoor == false)
            {
                return new PointF(pt.X, pt.Y);
            }

            if (m_currentIndoorZone == null)
                return new PointF();

            Matrix t = new Matrix();
            PointF rCenter = new PointF(this.mSizeImage.Width / 2, mSizeImage.Height / 2);
            t.Translate(rCenter.X, rCenter.Y);
            t.Rotate(-m_currentIndoorZone.Azimuth);
            t.Translate(-rCenter.X, -rCenter.Y);

            PointF[] myArray =
                {
                    pt
                };
            t.TransformPoints(myArray);
            return new PointF(myArray[0].X, myArray[0].Y);
        }

        private string m_szOverObjName = "";

        private Zone GetPOIZone(float x, float y, float z)
        {
            if (m_bIndoor)
            {
                return m_currentIndoorZone;
            }

            if (m_Owner == null)
                return null;


            string strBuildingID = m_szOverObjName;
            if (strBuildingID == "")
            {
                return m_Owner.GetOutsideZone(x, z);
            }
            else
            {
                Building building = m_Owner.GetBuilding(strBuildingID);
                if (building != null)
                {
                    Zone zone = m_Owner.GetZone(strBuildingID, building.MaxFloorIndex - 1);
                    if (zone == null)
                    {
                        return m_Owner.GetOutsideZone(x, z);
                    }
                    return zone;
                }
            }
            return m_Owner.GetOutsideZone(x, z);
        }


        public POI CreateCCTVPOI(MouseEventArgs e, Zone zone)
        {
            if (m_Owner == null)
                return null;

            PointF pos = ScreenToGlobal(e.Location);

            OnSavePt(e);         

            PointF pt = GetReverseRotateTransformPoint(new PointF(pos.X, pos.Y));
            pos.X = pt.X;
            pos.Y = pt.Y;         

            CCTV cctv = new CCTV();
            POI poi = new POI();
            poi.X = pos.X;
            poi.Y = pos.Y;
            poi.Z = 0.0f;
            poi.Facility = cctv;


            string strPath = cctv.IconPath;
            int nID = AddPOI(strPath, pos.X, pos.Y);
            poi.ID = nID;

            poi.IsIndoor = m_bIndoor;


            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (m_bIndoor)
            {
                poi.Zone = zone == null ? m_currentIndoorZone : zone;
            }
            else
            {
                poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, 0.0f) : zone;
            }
            //string szKey = GetKey(nID, strPath);
            //m_dicPOIs[szKey] = poi;
            m_dicPOIs[nID] = poi;
            m_Owner.SelectedPOI = poi;
            m_frmParent.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV).Add(nID);
                       
            if (m_bIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            string szName = m_szOverObjName;
            Building building = m_Owner.GetBuilding(szName);
            if (building == null)
                m_Owner.EditCCTV(cctv);
            else
                m_Owner.EditCCTV(cctv, building.DisplayText);
            return poi;
        }

        //public POI CreateCCTVPOI(MouseEventArgs e, Zone zone)
        //{
        //    PointF pos = ScreenToGlobal(e.Location);

        //    OnSavePt(e);
        //    string szName = m_currentIndoorZone.Building.BuildingID;
        //    Building building = ZoneManager.Instance.GetBuilding(szName);

        //    CCTV cctv = new CCTV();

        //    PointF pt = GetReverseRotateTransformPoint(new PointF(pos.X, pos.Y));
        //    pos.X = pt.X;
        //    pos.Y = pt.Y;

        //    string strPath = cctv.IconPath;// Application.StartupPath + "\\Media\\icons\\비산먼지.ico";
        //    int nID = AddPOI(strPath, pos.X, pos.Y);
        //    POI poi = new POI();
        //    poi.ID = nID;
        //    poi.X = pos.X;
        //    poi.Y = pos.Y;
        //    poi.Z = 0.0f;
        //    poi.Facility = cctv;
        //    poi.IsIndoor = m_bIndoor;

        //    if (m_Factory == null)
        //    {
        //        m_Factory = PopupFactoryHelper.GetFactory();
        //    }

        //    poi.Popup = cctv.CreatePopup(this, m_Factory);
        //    poi.Zone = ((zone == null) ? m_currentIndoorZone : zone);

        //    m_dicPOIs[nID] = poi;
        //    FormMain.Instance.PageHome.SelectedPOI = poi;

        //    m_frmParent.Layers.GetLayer(ID.ID_LAYER_CCTV).Add(nID);

        //    if (m_bIndoor && poi.Zone != null)
        //    {
        //        ArrayList arrPOIs = null;

        //        if (m_dicZonePOIs.ContainsKey(poi.Zone))
        //            arrPOIs = m_dicZonePOIs[poi.Zone];
        //        else
        //        {
        //            arrPOIs = new ArrayList();
        //            m_dicZonePOIs[poi.Zone] = arrPOIs;
        //        }

        //        if (!arrPOIs.Contains(poi))
        //            arrPOIs.Add(poi);
        //    }

        //    EditCCTV editCCTV = new EditCCTV(cctv);
        //    if (building != null)
        //    {
        //        editCCTV.Description = building.BroadcastName;
        //    }
        //    editCCTV.AddToManager(FormMain.Instance.PageHome);

        //    return poi;
        //}

        private void DeletePOI(int x, int y)
        {
            int nPOIID = OnSelectPOI(x, y);

            if (nPOIID > 0)
                DeletePOI(nPOIID);
        }

        public bool DeletePOI(int nID)
        {
            if (!m_dicPOIs.ContainsKey(nID))
                return false;

            POI poi = m_dicPOIs[nID];

            if (m_dicPOIs.Remove(nID))
            {
                POI poiSelected = this.BaseViewOwner.SelectedPOI;
                if (poiSelected != null && poiSelected.ID == nID)
                    BaseViewOwner.SelectedPOI = null;

                if (poi.Facility != null)
                    m_frmParent.Layers.GetLayer(poi.Facility.GetLayerID()).Remove(nID);
                RemovePOI(nID);

                if (poi.Zone != null)
                {
                    if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    {
                        ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
                        arrPOIs.Remove(poi);
                    }
                }

                OnPostDeletePOI(poi);
                return true;
            }

            return false;
        }

        private void OnPostDeletePOI(POI poi)
        {
            SDMS.IChangedDataManager mgr = ViewUtils.GetContentViewOwner().IChangedDataManager;
            switch (poi.Type)
            {
                case IFacility.FacilityType.CCTV:
                    EditCCTV cctv = new EditCCTV((CCTV)poi.Facility);
                    cctv.IsDeleting = true;
                    cctv.AddToManager(mgr);
                    break;

                case IFacility.FacilityType.FIRE_SENSOR:
                    EditFireSensor fireSensor = new EditFireSensor((FireSensor)poi.Facility);
                    fireSensor.IsDeleting = true;
                    fireSensor.AddToManager(mgr);
                    break;

                case IFacility.FacilityType.COOLER_SENSOR:
                    EditSpringCooler coolingSensor = new EditSpringCooler((SpringCooler)poi.Facility);
                    coolingSensor.IsDeleting = true;
                    coolingSensor.AddToManager(mgr);
                    break;

                case IFacility.FacilityType.PRESSURE_SENSOR:
                    EditPumpPressuerSensor pressureSensor = new EditPumpPressuerSensor((PumpPressureSensor)poi.Facility);
                    pressureSensor.IsDeleting = true;
                    pressureSensor.AddToManager(mgr);
                    break;
            }
        }

        private float m_fAzimuth = 0.0f;
        private bool m_bDrawCompass = false;
        private Image m_imgCompass = null;
        public void CreateCompass(float fAzimuth)
        {
            m_fAzimuth = fAzimuth;
            m_bDrawCompass = true;
            
            try
            {
                if( m_imgCompass == null)
                {
                     UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    string szMediaPath = owner.ResourcePath + "Media\\";
                    string szIconPath = szMediaPath + "models\\Compass.png";

                    m_imgCompass = Image.FromFile(szIconPath);
                }                
            }
            catch(Exception)
            {
                m_imgCompass = null;
            }            
        }

        internal void ShowLayer(int id, bool bShow)
        {
            if (id == ID.ID_LAYER_CCTV)
            {
                m_bDrawBillBoard = bShow;
                Invalidate();
            }
            else if (id == ID.ID_LAYER_BUILDING_TEXT)
            {
                m_bDrawBuildingText = bShow;
                Invalidate();
            }
        }

        internal void UpdatePOI()
        {
            //throw new NotImplementedException();
        }

        public void UpdateWindow()
        { }

        public void RemovePOI(float ox, float p, float oz)
        { }

        
        public int AddPOI(string szIconPath, float p1, float p2, float p3)
        { return -1; }



        internal void ZoomTarget(Position3D position3D, float p)
        {
            //ResetTransform();
            FitView();

            PointF ptTemp = ScreenToGlobal(new PointF(position3D.X, position3D.Y));
            PointF ptScrCenter = ScreenToGlobal(new PointF(Size.Width / 2.0f, Size.Height / 2.0f));
            PointF ptTrans = new PointF();
            ptTrans.X = this.mPtTranslation.X - position3D.X;
            ptTrans.Y = mPtTranslation.Y - position3D.Y;

            mTransform.Translate(ptTrans.X + ptScrCenter.X, ptTrans.Y + ptScrCenter.Y);

           // m_ptZoomCenter.X = (int)ptTrans.X;
           // m_ptZoomCenter.Y = (int)ptTrans.Y;

           // for (int i = 0; i < 5; i++ )
          //      ZoomIn();

        }

        internal void ZoomObject(string szCode)
        {
           // throw new NotImplementedException();
        }

        public void SelectObject(string name)
        {

        }

    }


    public class BillBoard
    {
        public BillBoard()
        {
        }

        private string m_szType = "";
        public string Type
        {
            get { return m_szType; }
            set { m_szType = value; }
        }


        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        
        private Image img = null;
        public Image Image
        {
            get { return img; }
            set
            { 
                img = value;
                mSelectImage = (Image)ChangeColor((Bitmap)img);
            }
        }

        private Image mSelectImage = null;
        public Image SelectImage
        {
            get { return mSelectImage; }
            set { mSelectImage = value; }
        }

        private float orgx = 0;
        public float X
        {
            get { return orgx; }
            set
            {
                orgx = value;
            }
        }

        private float orgY = 0;
        public float Y
        {
            get { return orgY; }
            set
            {
                orgY = value;
            }
        }

        private float ntx = 0;
        public float TX
        {
            get { return ntx; }
            set { ntx = value;
            rect.X = (int)value;
            }
        }

        private float nty = 0;
        public float TY
        {
            get { return nty; }
            set { nty = value;
            rect.Y = (int)value;

            }
        }

        private Rectangle rect = new Rectangle();
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }


        private int nWidth = 0;
        public int Width
        {
            get { return nWidth; }
            set 
            { 
                nWidth = value;
                rect.Width = value;
            }
        }

        private int nHeight = 0;
        public int Height
        {
            get { return nHeight; }
            set { nHeight = value;
            rect.Height = value;
            }
        }

        private bool m_bEnabled = false;
        public bool Enabled
        {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }

        private bool m_bSelected = false;

        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        private bool m_bVisible = false;
        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public static Bitmap ChangeColor(Bitmap scrBitmap)
        {
            //You can change your new color here. Red,Green,LawnGreen any..
            Color newColor = Color.MediumSeaGreen;
            Color actulaColor;
            //make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < scrBitmap.Width; i++)
            {
                for (int j = 0; j < scrBitmap.Height; j++)
                {
                    //get the pixel from the scrBitmap image
                    actulaColor = scrBitmap.GetPixel(i, j);
                    // > 150 because.. Images edges can be of low pixel colr. if we set all pixel color to new then there will be no smoothness left.
                    if (actulaColor.A > 150)
                        newBitmap.SetPixel(i, j, newColor);
                    else
                        newBitmap.SetPixel(i, j, actulaColor);
                }
            }
            return newBitmap;
        }

        private float m_nXPer = 0.0f;
        public float XPer
        {
            get { return m_nXPer; }
            set { m_nXPer = value; }
        }

        private float m_nYPer = 0.0f;
        public float YPer
        {
            get { return m_nYPer; }
            set { m_nYPer = value; }
        }
    }

    public class ImageOption
    {
        private int m_nLODLevel = -1;
        public int LODLevel
        {
            get { return m_nLODLevel; }
            set { m_nLODLevel = value; }
        }

        private int m_nRowIndex = -1;
        public int RowIndex
        {
            get { return m_nRowIndex; }
            set { m_nRowIndex = value; }
        }

        private int m_nColumnIndex = -1;
        public int ColumnIndex
        {
            get { return m_nColumnIndex; }
            set { m_nColumnIndex = value; }
        }

        private int m_nWidth = -1;
        public int Width
        {
            get { return m_nWidth; }
            set { m_nWidth = value; }
        }

        private int m_nHeight = -1;
        public int Height
        {
            get { return m_nHeight; }
            set { m_nHeight = value; }
        }

        private Image m_image = null;
        public Image Image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        private Rectangle m_Rect = new Rectangle();
        public Rectangle Rect
        {
            get { return m_Rect; }
            set { m_Rect = value; }
        } 
    }
}
