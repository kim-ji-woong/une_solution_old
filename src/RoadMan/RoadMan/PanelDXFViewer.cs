using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFViewer;
using UnE.Utility.Print;
using System.Drawing;

namespace RoadMan
{

    public partial class PanelDXFViewer : Panel, IExcelGridManager
	{
		public enum ActivityType { NONE = 0, SELECT_AREA, EDIT_SECTION, SETTING_STREET };
		public enum LoadingResult { NOT_LOADING = 0, FAIL, SUCCESS, GOING_ON };

		private DataManager m_dataManager = new DataManager();

		private DXFControl m_ctrl = null;
		private FormPrintPageSetup m_formSetup = null;
		
		
		//private FormPrintFrame m_frameSetup = null;
		private DialogFormFrame m_frameSetup = null;

		private ActivityType m_activityType = ActivityType.NONE;
		//private bool m_isLClicked = false;
		private int m_nSnapDistance = 5;
		private Shape m_selectedShape = null;
		//private Shape m_fixedSelection = null;
        private List<Shape> m_listFixedSelection = new List<Shape>();

		private FormProcessSchedule m_frmProcessSchedule = null;
		private FormProcessResult m_frmProcessResult = null;
		private FormLayer m_frmLayer = null;

        // DXF 경로가 상대경로인가?
        private bool m_relativePath = false;
        private string m_strRelativePath = "";

		private string m_strDXFFilePath = "";
		private Viewport m_viewport = null;
		private LoadingResult m_loadingStatus = LoadingResult.NOT_LOADING;

        private ISettingStreet m_settingStreet;
        private FormScheduleDetail m_frmScheduleDetail = null;

        private IExcelGridLinker m_excelGridLinker = null;
        // 도면에 해당하는 지역 이름
        private string m_strRegionName = "";

        private int m_nDockingMode = (int)FormMain.DockingType.LAYER;

		public DataManager DataManager
		{
			get { return m_dataManager; }
		}

		public DXFControl DXFControl
		{
			get { return m_ctrl; }
		}

		public FormPrintPageSetup PrintPageSetup
		{
			get { return m_formSetup; }
		}

		public DialogFormFrame PrintFrame
		{
			get { return m_frameSetup; }
		}

		public ActivityType Activity
		{
			get { return m_activityType; }
			set { m_activityType = value; }
		}

		public FormLayer LayerForm
		{
			get { return m_frmLayer; }
		}

		public FormProcessSchedule ProcessScheduleForm
		{
			get { return m_frmProcessSchedule; }
		}

		public FormProcessResult ProcessResultForm
		{
			get { return m_frmProcessResult; }
		}

		public List<ProcessSchedule> ProcessSchedules
		{
			get { return m_frmProcessSchedule.ProcessSchedules; }
		}

        public List<ProcessResult> ProcessResults
        {
            get { return m_frmProcessResult.ProcessResults; }
        }

        // DXF 경로가 상대경로인가?
        public bool IsRelativePath
        {
            get { return m_relativePath; }
            set { m_relativePath = value; }
        }

        public string RelativePath
        {
            get { return m_strRelativePath; }
            set { m_strRelativePath = value; }
        }

		public string DXFFilePath
		{
			get { return m_strDXFFilePath; }
			set { m_strDXFFilePath = value; }
		}

		public Viewport Viewport
		{
			get { return m_viewport; }
			set { m_viewport = value; }
		}

		public LoadingResult LoadingStatus
		{
			get { return m_loadingStatus; }
		}

        public ISettingStreet SettingStreet
        {
            get { return m_settingStreet; }
            set
            {
                m_settingStreet = value;

                if (value == null)
                {
                    ClearFixedSelection();
                    DXFControl.Refresh();
                }
            }
        }

        public FormScheduleDetail ScheduleDetailForm
        {
            get { return m_frmScheduleDetail; }
            set { m_frmScheduleDetail = value; }
        }

        public IExcelGridLinker ExcelGridLinker
        {
            get { return m_excelGridLinker; }
            set { m_excelGridLinker = value; }
        }

        // 도면에 해당하는 지역 이름
        public string RegionName
        {
            get { return m_strRegionName; }
            set { m_strRegionName = value; }
        }

        public int DockingMode
        {
            get { return m_nDockingMode; }
            set { m_nDockingMode = value; }
        }

		public PanelDXFViewer()
		{
			InitializeComponent();

			Init();
		}

		private bool m_bSelectMode = false;
		public bool SelectMode
		{
			get { return m_bSelectMode; }
			set
			{
				m_bSelectMode = value;

				if (m_bSelectMode == true)
					m_bScreenSelectMode = false;
			}
		}

		private bool m_bScreenSelectMode = false;
		public bool ScreenSelectMode
		{
			get { return m_bScreenSelectMode; }
			set
			{
				
				m_bScreenSelectMode = value;
				if( m_bScreenSelectMode == true)
					m_bSelectMode = false;

				//mScreenRectPainter.Clear(); 
			}
		}


		private bool m_bMemoMode = false;
		public bool MemoMode
		{
			get { return m_bMemoMode; }
			set 
			{
				m_bMemoMode = value; 
				if( value == true)
				{
					SelectMode = false;
					SetMemoMode();
				}
				else
				{
					if(mOverlayPanel != null)
					{
						mOverlayPanel.ClearTempMemo();
					}

					if(mFormMemo != null)
					{
						m_MemoLineColor = mFormMemo.LineColor;
						m_MemoTextColor = mFormMemo.TextColor;
						mFormMemo.Visible = false;
					}
				}
			}
		}

		private Color m_MemoLineColor = Color.Red;
		private Color m_MemoTextColor = Color.Yellow;
		private UnE.Overlay.OverlayPanel mOverlayPanel = null;
		public UnE.Overlay.OverlayPanel OverlayPanel
		{
			get { return mOverlayPanel; }

		}

		private UnE.Overlay.OverlaySelectScreenRectPainter mScreenRectPainter = null;
		public UnE.Overlay.OverlaySelectScreenRectPainter ScreenRectPainter
		{
			get { return mScreenRectPainter; }
		}


		private UnE.Overlay.OverlaySelectRectPainter mSelectRectPainter = null;
		public UnE.Overlay.OverlaySelectRectPainter SelectRectPainter
		{
			get { return mSelectRectPainter; }
		}

		private UnE.Underlay.UnderlayImagePainter mUnderImagePainter = null;
		public UnE.Underlay.UnderlayImagePainter UnderImagePainter
		{
			get { return mUnderImagePainter; }
		}


		private CaptureTool mScreenCaptuer = null;
		public CaptureTool ScreenCaptuer
		{
			get { return mScreenCaptuer; }
		}

		
		public PointF GetLBCornerPos()
		{
			UnE.Geometry.Vertex2D pt1 = m_ctrl.ObjectTL;
			UnE.Geometry.Vertex2D pt2 = m_ctrl.ObjectBR;

			UnE.Geometry.Vertex2D vMove = m_ctrl.MovedVertex;
			return new PointF((float)(pt1.x + vMove.x), (float)(pt2.y + vMove.y));

			//Point pt = new Point(0, this.Height);
			//UnE.Geometry.Vertex2D vert = m_ctrl.ScreenToGlobal(pt.X, pt.Y);
			//return new PointF((float)pt1.x, (float)pt2.y);
		}

		public SizeF GetDrawSize()
		{
			UnE.Geometry.Vertex2D pt1 = m_ctrl.ObjectTL;
			UnE.Geometry.Vertex2D pt2 = m_ctrl.ObjectBR;

			float width = (float)Math.Abs(pt2.x - pt1.x);
			float height = (float)Math.Abs(pt2.y - pt1.y);
			return new SizeF(width, height);
		}



		private void Init()
		{
			m_ctrl = new DXFControl();
			this.Controls.Add(m_ctrl);

		   // m_ctrl.Renderer = DXFViewer.IPainter.RendererType.OPEN_GL;
			m_ctrl.AntiAliasing = true;
			m_ctrl.BackColor = System.Drawing.Color.Black;
			m_ctrl.Dock = System.Windows.Forms.DockStyle.Fill;
			m_ctrl.DrawHatchFirst = false;

			m_ctrl.SetExternalWheelEvent = true;

			DXFExternPainter exPainter = new DXFExternPainter(m_ctrl);
			m_ctrl.ExternalPainter = exPainter;
			m_ctrl.GroupItemDistance = 30;
			m_ctrl.GroupItemMinCount = 3;
			m_ctrl.Location = new System.Drawing.Point(0, 0);
			m_ctrl.Name = "dxfControl_" + this.GetHashCode().ToString();
			m_ctrl.Panning = false;
			m_ctrl.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
			m_ctrl.PrintDocument = new DXFViewer.UPrintDocument();
			m_ctrl.Size = new System.Drawing.Size(680, 620);
			m_ctrl.TabIndex = 0;
			m_ctrl.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
			m_ctrl.UseGroupItem = false;
			m_ctrl.UseLastViewport = false;
			m_ctrl.UseMouseWheel = true;
            m_ctrl.BackColor = Options.Instance.BackColor;

            // 배경이미지가 도로 기반이므로 선택되는 도로의 Polygon 내부는 Orange 색으로 그린다.
            Color colorSelectedPolyLineBack = Color.Orange;
            m_ctrl.SelectedBrightPen2.Color = Color.FromArgb(255 - colorSelectedPolyLineBack.R, 255 - colorSelectedPolyLineBack.G, 255 - colorSelectedPolyLineBack.B);
            m_ctrl.SelectedBrightPen1.Color = colorSelectedPolyLineBack;

			m_ctrl.MouseMove += new MouseEventHandler(OnMouseMove);
			m_ctrl.MouseUp += new MouseEventHandler(OnMouseUp);
			m_ctrl.MouseDown += new MouseEventHandler(OnMouseDown);
			m_ctrl.MouseDoubleClick += new MouseEventHandler(OnMouseDoubleClick);

			mOverlayPanel = new UnE.Overlay.OverlayPanel(m_ctrl);
			mOverlayPanel.InvalidateControl += m_ctrl.Refresh;
			exPainter.OverlayPostPainter += mOverlayPanel.DrawOverlay;


			mSelectRectPainter = new UnE.Overlay.OverlaySelectRectPainter(m_ctrl);
			mSelectRectPainter.InvalidateControl += m_ctrl.Refresh;
			exPainter.SelectRectPostPaint += mSelectRectPainter.DrawSelectRect;

			mUnderImagePainter = new UnE.Underlay.UnderlayImagePainter(m_ctrl);
			mUnderImagePainter.InvalidateControl += m_ctrl.Refresh;
			exPainter.UnderlayPrePainter += mUnderImagePainter.OnPaint;


			mScreenRectPainter = new UnE.Overlay.OverlaySelectScreenRectPainter(m_ctrl);
			mScreenRectPainter.InvalidateControl += m_ctrl.Refresh;
			exPainter.SelectRectPostPaint += mScreenRectPainter.DrawSelectRect;
			
			mScreenCaptuer = new CaptureTool(this);

			CreateFormPrintPageSetup();
			InitDockingForms();
		}

		private static FormMemo mFormMemo = null;
		
		public void SetMemoMode()
		{
			if (mFormMemo == null)
			{			
				mFormMemo = new FormMemo();
				mFormMemo.TopMost = false;
			}			
			
			mFormMemo.SetPanel(mOverlayPanel);	
			mFormMemo.StartPosition = FormStartPosition.Manual;
			mFormMemo.Location = PointToScreen(new Point(0, 0));
			mFormMemo.Show(FormMain.Instance);
		}

		private void InitDockingForms()
		{
			m_frmLayer = new FormLayer();
			m_frmLayer.TopLevel = false;
			m_frmLayer.Dock = DockStyle.Fill;

			m_frmProcessSchedule = new FormProcessSchedule(this);
			m_frmProcessSchedule.TopLevel = false;
			m_frmProcessSchedule.Dock = DockStyle.Fill;

			m_frmProcessResult = new FormProcessResult(this);
			m_frmProcessResult.TopLevel = false;
			m_frmProcessResult.Dock = DockStyle.Fill;
		}


		private void CreateFormPrintPageSetup()
		{
			m_formSetup = new FormPrintPageSetup();
			m_formSetup.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
			m_formSetup.PageSettings = m_ctrl.PrintDocument.DefaultPageSettings;
			m_ctrl.PrintDocument.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(3, 3, 3, 3);
			m_formSetup.EnableMetric = false;
			m_formSetup.Document = (DXFViewer.UPrintDocument)m_ctrl.PrintDocument;

			m_frameSetup = new DialogFormFrame(m_formSetup, false);
			m_frameSetup.Text = "인쇄 설정";
			m_frameSetup.ShowCloseButton = true;
			m_frameSetup.ShowMaxButton = false;
			m_frameSetup.ShowMinButton = false;
			//m_frameSetup.FrameMaximized = false;
			m_frameSetup.Sizable = false;
			m_frameSetup.Size = new Size(588, 397);
			m_frameSetup.StartPosition = FormStartPosition.CenterScreen;
			m_frameSetup.WindowState = FormWindowState.Normal;
		}

		private bool GetDXFCoord(int x, int y, out double _x, out double _y)
		{
			_x = _y = 0.0;
			UnE.Geometry.Vertex2D vertex = m_ctrl.ScreenToGlobal(x, y);

			if (vertex != null)
			{
				UnE.Geometry.Vertex2D vMove = m_ctrl.MovedVertex;
				float fFlag = 1.0f;
				_x = (vertex.x - vMove.x) * fFlag;
				_y = (vertex.y - vMove.y) * fFlag;

				return true;
			}

			return false;
		}


		private void OnMouseMoveOverlay(object sender, MouseEventArgs e)
		{
			if (m_bMemoMode == true)
			{
				if (mOverlayPanel != null)
				{
					mOverlayPanel.OnMouseMove(sender, e);
				}
				//return;
			}
			else
			{
				if (m_bSelectMode == true)
				{
					if (mSelectRectPainter != null)
					{
						mSelectRectPainter.OnMouseMove(sender, e);
					}
				}

				if (m_bScreenSelectMode == true)
				{
					if (mScreenRectPainter != null)
					{
						mScreenRectPainter.OnMouseMove(sender, e);
					}
				}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (m_ctrl.IsOpened)
			{
				OnMouseMoveOverlay(sender, e);

				double x, y;
				if (GetDXFCoord(e.X, e.Y, out x, out y))
				{
					FormMain.Instance.SetStatusText(FormMain.StatusType.COORD, string.Format("({0:f1}, {1:f1}), 단위(m)", x, y));
				}

				UnE.Geometry.Vertex2D vertex = m_ctrl.ScreenToGlobal(e.X, e.Y);

                SelectMouseOver(vertex);

				/*if (m_activityType == ActivityType.EDIT_SECTION)
				{
					Hatch nearestHatch;
					double distance;
					UnE.Geometry.Vertex2D vNear = GetNearestVertexNDistance(vertex, out distance, out nearestHatch);

					if (m_isLClicked)
					{
						if (FormEditSection.Instance.CurrentHatch != null)
						{
							FormEditSection.Instance.CurrentHatch.UpdateEditBoxVertex(FormEditSection.Instance.CurrentHatch.GetEditBoxVertexCount() - 1, vNear);
							//m_ctrl.Refresh();
							DXFRefresh();
						}
						else
							m_ctrl.Cursor = Cursors.Arrow;
					}
					else
					{
						if (CheckSnapDistance(e.X, e.Y, vNear) && (FormEditSection.Instance.CurrentHatch == null || (FormEditSection.Instance.CurrentHatch != null && FormEditSection.Instance.CurrentHatch.CanAdd(vNear))))
						{
							m_ctrl.Cursor = Cursors.Cross;
						}
						else
						{
							m_ctrl.Cursor = Cursors.Arrow;
							//SelectMouseOver(vertex);
						}
					}
				}
				else
				{
					if (m_activityType != ActivityType.EDIT_SECTION)
					{
						m_ctrl.Cursor = Cursors.Arrow;
						SelectMouseOver(vertex);
					}
				}*/
			}
		}

		private bool CheckSnapDistance(int x, int y, UnE.Geometry.Vertex2D vertex)
		{
			if (vertex == null)
				return false;

			Point pt = m_ctrl.GlobalToScreen(vertex);
			int nLen = (int)System.Math.Sqrt((x - pt.X) * (x - pt.X) + (y - pt.Y) * (y - pt.Y));

			return nLen <= m_nSnapDistance;
		}

        //private UnE.Geometry.Vertex2D GetNearestVertexNDistance(UnE.Geometry.Vertex2D vertex, out double distance, out Hatch nearestHatch)
        //{
        //    UnE.Geometry.Vertex2D vResult = null, vNear = null;
        //    distance = 0.0;
        //    nearestHatch = null;
        //    bool isFirst = true;

        //    if (m_fixedSelection == null)
        //        return null;

        //    Shape shape = m_fixedSelection;
        //    //foreach (Shape shape in m_listFixedSelection)
        //    {
        //        /*if (shape.GetShapeType() != Shape.ShapeType.HATCH)
        //            continue;*/

        //        Hatch hatch = (Hatch)shape;
        //        double dLen = hatch.Polygon.GetDistanceNVertex(vertex, out vNear);

        //        if (isFirst)
        //        {
        //            distance = dLen;
        //            vResult = vNear;
        //            nearestHatch = hatch;
        //            isFirst = false;
        //        }
        //        else if (System.Math.Abs(distance) > System.Math.Abs(dLen))
        //        {
        //            distance = dLen;
        //            vResult = vNear;
        //            nearestHatch = hatch;
        //        }
        //    }

        //    return vResult;
        //}

		private void SelectMouseOver(UnE.Geometry.Vertex2D vertex)
		{
			if (vertex == null)
				return;

			DXFViewer.Shape shape = m_ctrl.SelectObject(vertex.x, vertex.y);

			if (shape != null)
			{
				if (m_selectedShape == shape)
					return;
				else
				{
					if (m_selectedShape != null && !m_listFixedSelection.Contains(m_selectedShape))
					//if (m_selectedShape != null && m_fixedSelection != m_selectedShape)
					{
						m_selectedShape.Selected = false;
						m_selectedShape = null;
					}
				}

				shape.Selected = true;
				m_selectedShape = shape;
				//m_ctrl.Refresh();
				DXFRefresh();
			}
			else if (m_selectedShape != null && !m_listFixedSelection.Contains(m_selectedShape))
			//else if (m_selectedShape != null && m_fixedSelection != m_selectedShape)
			{
				m_selectedShape.Selected = false;
				m_selectedShape = null;
				//m_ctrl.Refresh();
				DXFRefresh();
			}
		}


		private void OnMouseUpOverlay(object sender, MouseEventArgs e)
		{
			if (m_bMemoMode == true)
			{
				if (mOverlayPanel != null)
				{
					mOverlayPanel.OnMouseUp(sender, e);
				}
				//return;
			}
			else
			{
				if (m_bSelectMode == true)
				{
					if (mSelectRectPainter != null)
					{
						mSelectRectPainter.OnMouseUp(sender, e);
					}
				}

				if (m_bScreenSelectMode == true)
				{
					if (mScreenRectPainter != null)
					{
						mScreenRectPainter.OnMouseUp(sender, e);
					}
				}
			}
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			OnMouseUpOverlay(sender, e);

			m_frmLayer.HideLayerPriority();

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
                FormMain.Instance.SetStatusText(FormMain.StatusType.STATUS, "현재 작업을 표시합니다.");

				if (m_activityType == ActivityType.SELECT_AREA)
				{
                    //UnE.Geometry.Vertex2D vertex = m_ctrl.ScreenToGlobal(e.X, e.Y);
                    //DXFViewer.Shape shape = m_ctrl.SelectObject(vertex.x, vertex.y);

                    //if (shape != null)
                    //{
                    //    /*if (m_listFixedSelection.Contains(shape))
                    //    {
                    //        shape.Selected = false;
                    //        m_listFixedSelection.Remove(shape);
                    //    }
                    //    else
                    //    {
                    //        shape.Selected = true;

                    //        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    //        {
                    //            m_listFixedSelection.Add(shape);
                    //        }
                    //        else
                    //        {
                    //            foreach (Shape shp in m_listFixedSelection)
                    //            {
                    //                shp.Selected = false;
                    //            }

                    //            m_listFixedSelection.Clear();
                    //            m_listFixedSelection.Add(shape);
                    //        }
                    //    }*/
                    //    if (m_fixedSelection != shape)
                    //    {
                    //        if (m_fixedSelection != null)
                    //            m_fixedSelection.Selected = false;

                    //        m_fixedSelection = shape;
                    //        m_fixedSelection.Selected = true;
                    //    }

                    //    FormEditSection.Instance.SelectArea = false;
                    //    //m_ctrl.Refresh();
                    //    DXFRefresh();
                    //}
				}
                else if (m_activityType == ActivityType.SETTING_STREET && m_settingStreet != null)
                {
                    UnE.Geometry.Vertex2D vertex = m_ctrl.ScreenToGlobal(e.X, e.Y);
                    DXFViewer.Shape shape = m_ctrl.SelectObject(vertex.x, vertex.y);

                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                        m_settingStreet.AddShape(shape);
                    else
                        m_settingStreet.SetShape(shape);
                }

				//m_isLClicked = false;
			}
		}

		public void DXFRefresh()
		{
			m_ctrl.Refresh();
			DateTime dtNow = DateTime.Now;

			System.Diagnostics.Trace.WriteLine("DXFRefresh : " + dtNow.Minute.ToString() + ":" + dtNow.Second.ToString() + ", " + dtNow.Millisecond.ToString());
		}

		private void OnMouseDownOverlay(object sender, MouseEventArgs e)
		{
			if (m_bMemoMode == true)
			{
				if( mOverlayPanel != null)
				{
					mOverlayPanel.OnMouseDown(sender, e);					
				}
				//return;
			}
			else
			{
				if (m_bSelectMode == true)
				{
					if (mSelectRectPainter != null)
					{
						mSelectRectPainter.OnMouseDown(sender, e);
					}
				}

				if (m_bScreenSelectMode == true)
				{
					if (mScreenRectPainter != null)
					{
						mScreenRectPainter.OnMouseDown(sender, e);
					}
				}
			}			
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			OnMouseDownOverlay(sender, e);

			m_frmLayer.HideLayerPriority();

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				//m_isLClicked = true;

                UnE.Geometry.Vertex2D vertex = m_ctrl.ScreenToGlobal(e.X, e.Y);
                DXFViewer.Shape shape = m_ctrl.SelectObject(vertex.x, vertex.y);
                string strStreetName = m_dataManager.GetStreetName(shape);

                if (shape == null)
                    FormMain.Instance.SetStatusText(FormMain.StatusType.STATUS, "");
                else
                {
                    string strStatus = "";
                    
                    if (strStreetName.Length == 0)
                        strStatus = string.Format("{0} : {1:X}, Layer : {2}", shape.GetShapeType().ToString(), shape.ID, shape.GetLayer().LayerName);
                    else
                        strStatus = string.Format("{0}, {1} : {2:X}, Layer : {3}", strStreetName, shape.GetShapeType().ToString(), shape.ID, shape.GetLayer().LayerName);

                    FormMain.Instance.SetStatusText(FormMain.StatusType.STATUS, strStatus);
                }

                //if (m_activityType == ActivityType.EDIT_SECTION && m_ctrl.Cursor == Cursors.Cross)
                //{
                //    DXFExternPainter painter = (DXFExternPainter)m_ctrl.ExternalPainter;
                //    Layer layer = painter.GetLayer(DXFExternPainter.LayerType.PROCESS_SCHEDULE);

                //    if (layer == null)
                //        return;

                //    Hatch nearestHatch;
                //    double distance;
                //    UnE.Geometry.Vertex2D vertex = m_ctrl.ScreenToGlobal(e.X, e.Y);
                //    UnE.Geometry.Vertex2D vNear = GetNearestVertexNDistance(vertex, out distance, out nearestHatch);

                //    if (vNear == null)
                //        return;

                //    if (CheckSnapDistance(e.X, e.Y, vNear))
                //    {
                //        if (FormEditSection.Instance.CurrentHatch == null && FormEditSection.Instance.CurrentProperty != null)
                //        {
                //            EditBoxHatch editHatch = new EditBoxHatch(nearestHatch.Polygon);
                //            layer.Add(editHatch);

                //            FormEditSection.Instance.CurrentHatch = editHatch;
                //            FormEditSection.Instance.CurrentProperty.Sectors.Add(new SchedulePropertySector(editHatch, nearestHatch));
                //            editHatch.LinkedScheduleProperty = FormEditSection.Instance.CurrentProperty;
                //        }

                //        FormEditSection.Instance.CurrentHatch.AddEditBoxVertex(vNear);

                //        if (FormEditSection.Instance.CurrentHatch.GetEditBoxVertexCount() >= EditBoxHatch.LIMIT_EDITBOX_COUNT)
                //        {
                //            FormEditSection.Instance.CurrentHatch.Selected = false;
                //            FormEditSection.Instance.CurrentHatch = null;
                //            m_ctrl.Cursor = Cursors.Arrow;
                //            m_activityType = ActivityType.NONE;
                //            //m_fixedSelection = null;
                //            m_listFixedSelection.Clear();
                //        }

                //        //m_ctrl.Refresh();
                //        DXFRefresh();
                //    }
                //    /*else
                //        m_currentHatch = null;*/
                //}
			}
		}

		public void SelectPanel()
		{
            FormMain frmMain = FormMain.Instance;
            PanelDXFViewer prevPanel = frmMain.SelectedPanel;

            if (prevPanel != null)
            {
                if (prevPanel.SettingStreet != null)
                    prevPanel.SettingStreet.Close();

                if (prevPanel.ScheduleDetailForm != null)
                    prevPanel.ScheduleDetailForm.Close();

                prevPanel.ProcessScheduleForm.CloseScheduleProperty();
                prevPanel.ProcessResultForm.ClosePropertyForm();

                if (frmMain.OptionForm != null)
                    frmMain.OptionForm.SetPanel(this);

                FormMain.Instance.EndLoadSearch();
            }

            frmMain.PanelLayer.Controls.Clear();
            frmMain.PanelProcessSchedule.Controls.Clear();
            frmMain.PanelProcessResult.Controls.Clear();

            frmMain.PanelLayer.Controls.Add(m_frmLayer);
            frmMain.PanelProcessSchedule.Controls.Add(m_frmProcessSchedule);
            frmMain.PanelProcessResult.Controls.Add(m_frmProcessResult);

            frmMain.SelectedPanel = this;

			m_frmLayer.Show();
			m_frmProcessSchedule.Show();
			m_frmProcessResult.Show();
		}

		public void ClearProcessSchedule()
		{
			m_frmProcessSchedule.ClearProcessSchedule();
		}

        public void ClearProcessResult()
        {
            m_frmProcessResult.ClearProcessResult();
        }

		public void AddProcessSchedule(ProcessSchedule schedule)
		{
			schedule.ParentPane = this;
			schedule.PanelName = this.GetHashCode().ToString();
			UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(schedule);

			m_frmProcessSchedule.AddProcessSchedule(schedule);
		}

        public void AddProcessResult(ProcessResult result)
        {
            m_frmProcessResult.AddProcessResult(result);
        }

		private void OnMouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (m_activityType == ActivityType.NONE)
			{
                m_dataManager.SelectShape(this, e.X, e.Y, true);
				//DXFExternPainter painter = (DXFExternPainter)m_ctrl.ExternalPainer;
				//painter.SelectShape(m_ctrl, e.X, e.Y, true);
			}
		}

		public static void SetTabPageText(TabPage page, string strDXFFilePath, LoadingResult result)
		{
			string strDXFFileName = FormMain.Instance.GetProjectName(strDXFFilePath);

			if (result == LoadingResult.FAIL)
			{
				page.Text = "로딩실패 - " + strDXFFileName;
				page.ToolTipText = strDXFFileName;
			}
			else if (result == LoadingResult.SUCCESS)
			{
				page.Text = strDXFFileName;
				page.ToolTipText = strDXFFileName;
			}
			else if (result == LoadingResult.GOING_ON)
			{
				page.Text = "로딩중...";
				page.ToolTipText = strDXFFileName;
			}

			if (page.Tag != null)
			{
				PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
				panel.m_loadingStatus = result;
			}
		}

		// 쓰레드를 사용하는 버전
		public bool PostOpenDXF(DXFDatas datas)
		{
            List<LayerData> arrLayers = datas == null ? null : datas.LayerDatas;
            Dictionary<string, List<int>> streetShapeIDs = datas == null ? null : datas.StreetShapes;
            Dictionary<string, StreetCenterLine> dicStreetCenterLines = datas == null ? null : datas.StreetCenterLines;

			TabPage page = (TabPage)this.Parent;
			SetTabPageText(page, this.DXFFilePath, LoadingResult.GOING_ON);

			List<LayerData> arrOriginLayers = this.DataManager.PostOpenDXF(this.DXFFilePath, this);

			if (arrOriginLayers == null)
			{
				SetTabPageText(page, this.DXFFilePath, LoadingResult.FAIL);
				string szMsg = "DXF 파일의 경로가 잘못되었거나 해당 파일을 사용할 수 없습니다.\r\n" + this.DXFFilePath;

				UnE.Utility.UMessageBox.Show(szMsg, "도면 파일 열기", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				int nLayerCount = arrOriginLayers.Count;

				if (nLayerCount == 0)
				{
					m_loadingStatus = LoadingResult.NOT_LOADING;
					return false;
				}

				this.Cursor = Cursors.WaitCursor;

				if (arrLayers != null)
				{
					DXFViewer.Layer[] layers = new DXFViewer.Layer[nLayerCount];

					foreach (LayerData layer in arrOriginLayers)
					{
						LayerData layer2 = GetLayer(layer.LayerName, arrLayers);

						if (layer2 != null)
						{
							layer.Enabled = layer2.Enabled;
							layer.Visible = layer2.Visible;
							layer.LinkedLayer.Hidden = !layer.Visible;

							layer.Color = layer2.Color;
							layer.Alpha = layer2.Alpha;
							layer.LinkedLayer.LineColor = Color.FromArgb(layer.Alpha, layer.Color);

							layer.LayerIndex = layer2.LayerIndex;
							layers[layer.LayerIndex - 1] = layer.LinkedLayer;
						}
					}

					m_ctrl.Layers.Clear();

					foreach (DXFViewer.Layer layer in layers)
					{
						if (layer != null)
							m_ctrl.Layers.Insert(0, layer);
					}
				}

				m_frmLayer.SetLayers(arrOriginLayers);
				this.DataManager.SetSelectableLayers();
				m_frmProcessSchedule.SetSectors(m_ctrl);

				m_ctrl.LoadViewport(Viewport, true);

				DataManager.SetLinkedLayers(m_frmLayer.GetLayerList());

				// Layer 설정 완료

                Dictionary<int, Shape> dicShapeIDs = m_dataManager.SetStreetShapes(m_ctrl.Layers, streetShapeIDs);
                m_dataManager.SetStreetCenterLines(dicShapeIDs, dicStreetCenterLines);

				this.Cursor = Cursors.Arrow;
				SetTabPageText(page, this.DXFFilePath, LoadingResult.SUCCESS);
			}

			return true;
		}

		// 쓰레드를 사용하지 않는 버전
		public bool OpenDXF(DXFDatas datas)
		{
            List<LayerData> arrLayers = datas.LayerDatas;
            Dictionary<string, List<int>> streetShapeIDs = datas.StreetShapes;

			TabPage page = (TabPage)this.Parent;
			SetTabPageText(page, this.DXFFilePath, LoadingResult.GOING_ON);
			
			//string strDXFFileName = FormMain.Instance.GetProjectName(this.DXFFilePath);

			List<LayerData> arrOriginLayers = this.DataManager.OpenDXF(this.DXFFilePath, this);

			if (arrOriginLayers == null)
			{
				SetTabPageText(page, this.DXFFilePath, LoadingResult.FAIL);

				string szMsg = "DXF 파일의 경로가 잘못되었거나 해당 파일을 사용할 수 없습니다.\r\n" + this.DXFFilePath;
                UnE.Utility.UMessageBox.Show(this, szMsg, "도면 파일 열기", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//m_dataMgr.Restore();
			}
			else
			{
				int nLayerCount = arrOriginLayers.Count;

				if (nLayerCount == 0)
				{
					m_loadingStatus = LoadingResult.NOT_LOADING;
					return false;
				}

				this.Cursor = Cursors.WaitCursor;

				DXFViewer.Layer[] layers = new DXFViewer.Layer[nLayerCount];

				foreach (LayerData layer in arrOriginLayers)
				{
					LayerData layer2 = GetLayer(layer.LayerName, arrLayers);

					if (layer2 != null)
					{
						layer.Visible = layer2.Visible;
						layer.LinkedLayer.Hidden = !layer.Visible;

						layer.Color = layer2.Color;
						layer.Alpha = layer2.Alpha;
						layer.LinkedLayer.LineColor = Color.FromArgb(layer.Alpha, layer.Color);

						layer.LayerIndex = layer2.LayerIndex;
						layers[layer.LayerIndex - 1] = layer.LinkedLayer;
					}
				}

				m_ctrl.Layers.Clear();

				foreach (DXFViewer.Layer layer in layers)
				{
					if (layer != null)
						m_ctrl.Layers.Insert(0, layer);
				}

				m_frmLayer.SetLayers(arrOriginLayers);
				this.DataManager.SetSelectableLayers();
				m_frmProcessSchedule.SetSectors(m_ctrl);
				
				m_ctrl.LoadViewport(Viewport, true);

				//m_dataMgr.Backup();
				DataManager.SetLinkedLayers(m_frmLayer.GetLayerList());

				// Layer 설정 완료

                m_dataManager.SetStreetShapes(m_ctrl.Layers, streetShapeIDs);

				this.Cursor = Cursors.Arrow;
				SetTabPageText(page, this.DXFFilePath, LoadingResult.SUCCESS);
			}

			return true;
		}

		private LayerData GetLayer(string strLayerName, List<LayerData> arrLayers)
		{
			foreach (LayerData layer in arrLayers)
			{
				if (layer.LayerName == strLayerName)
					return layer;
			}

			return null;
		}

        public void ClearFixedSelection()
        {
            foreach (Shape shape in m_listFixedSelection)
            {
                shape.Selected = false;
                shape.SelectedShowing = Shape.SelectedShowingType.BRIGHT_EFFECT;
            }

            m_listFixedSelection.Clear();
        }

        public void RemoveFixedSelection(Shape shape)
        {
            m_listFixedSelection.Remove(shape);
        }

        public void AddFixedSelection(Shape shape)
        {
            if (!m_listFixedSelection.Contains(shape))
                m_listFixedSelection.Add(shape);
        }

        public List<FixedSelectionData> GetFixedSelections()
        {
            if (m_listFixedSelection.Count == 0)
                return null;

            List<FixedSelectionData> datas = new List<FixedSelectionData>();

            foreach (Shape shape in m_listFixedSelection)
            {
                datas.Add(new FixedSelectionData(shape, shape.SelectedShowing));
            }

            return datas;
        }

        public ScheduleProperty FindScheduleProperty(string strStreetName)
        {
            foreach (ProcessSchedule schedule in m_frmProcessSchedule.ProcessSchedules)
            {
                foreach (ScheduleProperty prop in schedule.Properties)
                {
					if (string.Compare(prop.StreetName, strStreetName, false) == 0)
                        return prop;
                }
            }

            return null;
        }

		public void PanelDXFViewer_LocationChanged(object sender, EventArgs e)
		{
			Point pt = PointToScreen(new Point(0, 0));
			if( mFormMemo != null)
				mFormMemo.Location = pt;		
			
		}

		public void PanelDXFViewer_SizeChanged(object sender, EventArgs e)
		{
			if( mFormMemo != null)
			{
				int width = this.Width;
				int height = this.Height;

				if (width < mFormMemo.Width)
				{
					mFormMemo.Visible = false;
				}
				else
				{
					if (height > mFormMemo.Height)
					{
						if (m_bMemoMode == true && mFormMemo.Visible == false)
						{
							mFormMemo.Visible = true;
						}
					}					
				}
				
				if (height < mFormMemo.Height)
				{
					mFormMemo.Visible = false;
				}
				else
				{
					if (width > mFormMemo.Width)
					{
						if (m_bMemoMode == true && mFormMemo.Visible == false)
						{
							mFormMemo.Visible = true;
						}
					}					
				}
			}		

		}

        public bool SaveReportRawData(int nIndex, string strFolderPath, ref string strErrorMessage)
        {
            RawDataMaker maker = new RawDataMaker(this);
            
            if (!maker.SaveFile(nIndex, strFolderPath))
            {
                strErrorMessage = maker.ErrorMessage;
                return false;
            }

            return true;
        }
	}

    public interface ISettingStreet
    {
        void SetShape(Shape shape);
        void AddShape(Shape shape);
        void ClearShape();
        void Close();
    }

    public class FixedSelectionData
    {
        private Shape m_shape = null;
        private Shape.SelectedShowingType m_showingType = Shape.SelectedShowingType.NONE;

        public Shape Shape
        {
            get { return m_shape; }
            set { m_shape = value; }
        }

        public Shape.SelectedShowingType ShowingType
        {
            get { return m_showingType; }
            set { m_showingType = value; }
        }

        public FixedSelectionData()
        {
        }

        public FixedSelectionData(Shape shape, Shape.SelectedShowingType type)
        {
            m_shape = shape;
            m_showingType = type;
        }

        public static bool IsSame(List<FixedSelectionData> datas1, List<FixedSelectionData> datas2)
        {
            if (datas1 == datas2)
                return true;

            if (datas1 == null || datas2 == null)
                return false;

            int nCount1 = datas1.Count;
            int nCount2 = datas2.Count;

            if (nCount1 != nCount2)
                return false;

            foreach (FixedSelectionData data in datas1)
            {
                if (!FindData(data, datas2))
                    return false;
            }

            return true;
        }

        private static bool FindData(FixedSelectionData data, List<FixedSelectionData> datas)
        {
            foreach (FixedSelectionData data2 in datas)
            {
                if (data2.Shape == data.Shape && data2.ShowingType == data.ShowingType)
                    return true;
            }

            return false;
        }
    }
}
