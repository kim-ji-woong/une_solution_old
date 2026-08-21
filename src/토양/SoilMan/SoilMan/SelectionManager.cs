using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan
{
    public class SelectionManager
    {
        //private Drawing.Polygon m_selectedShape = null;

        private List<Drawing.Polygon> mSelectList = new List<Drawing.Polygon>();


        private System.Drawing.Color m_colorSelected = System.Drawing.Color.Red;
        private Popup.FormDetailAttrib m_frmDetailAttrib = new Popup.FormDetailAttrib();
        private DialogFormFrameEx m_dlgDetail = null;

        public DialogFormFrameEx DetailFrameDialog
        {
            get { return m_dlgDetail; }
            set { m_dlgDetail = value; }
        }

        public System.Drawing.Color SelectedColor
        {
            get { return m_colorSelected; }
            set
            {
                m_colorSelected = value;
            }
        }

        public List<Drawing.Polygon> SelectedShapeList
        {
            get { return mSelectList; }
        }

        public Popup.FormDetailAttrib DetailAttribForm
        {
            get { return m_frmDetailAttrib; }
        }

        public SelectionManager()
        {
            
            m_frmDetailAttrib.SelectionManager = this;

            m_dlgDetail = new DialogFormFrameEx(m_frmDetailAttrib);

            m_dlgDetail.TitleBarBackColor = System.Drawing.Color.FromArgb(50, 61, 96);
            m_dlgDetail.Text = "지목별 상세보기";
            m_dlgDetail.Sizable = true;
            m_dlgDetail.MinimumSize = new System.Drawing.Size(535, 425);
            m_dlgDetail.PictureBoxTitle.Visible = false;
    
            m_dlgDetail.TitleTextFont = new System.Drawing.Font("맑은 고딕", 12.0f);
            m_dlgDetail.TitlePosition = new System.Drawing.Point(10, 3);          
        }

        private Drawing.Polygon SelectPolygon(double x, double y, DXFViewer.Layer layer)
        {
            Drawing.PolygonList polygonList = null;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    polygonList = (Drawing.PolygonList)shape;
                    break;
                }
            }

            if (polygonList == null)
                return null;

            QuadNode node = FormMain.Instance.QuadTree.GetNode((float)x, (float)y);

            
            
            if (node != null)
            {
                List<Drawing.Polygon> searchList = new List<Drawing.Polygon>();
                foreach (int nIndex in node.Datas)
                {
                    Drawing.Polygon polygon = polygonList.GetPolygonFromID(nIndex);

                    if (polygon != null && polygon.HitTest(x, y))
                    {
                        //return polygon;
                        searchList.Add(polygon);
                    }                    
                }

                // 찾아진 폴리곤중 가장 작은 폴리곤을 리턴한다.
                if (searchList.Count == 0)
                    return null;

                Drawing.Polygon minPolygon = null;
                float minValue = float.MaxValue;
                foreach(Drawing.Polygon polygon in searchList)
                {
                    float dV = polygon.GetArea();
                    if (dV < minValue)
                    {
                        minValue = dV;
                        minPolygon = polygon;
                    }
                }
                return minPolygon;
            }

            return null;
        }

        // Return 값 : Refresh가 필요한가?
        //             true이면 Refresh가 필요
        public bool SelectShape(double x, double y, DXFViewer.Layer layer)
        {
            if (layer == null)
                return false;

            Drawing.Polygon selectedShape = null;

            // layer 내의 모든 객체를 검사하는 방식에서 QuadTree를 참조하는 방식으로 변경
            DXFViewer.Shape shape = (DXFViewer.Shape)SelectPolygon(x, y, layer);

            
            if (mSelectList.Contains(shape))
                return false;

            if (shape is Drawing.PolygonList)
            {
                shape = ((Drawing.PolygonList)shape).SelectedPolygon;
            }

            if (shape != null && shape is Drawing.Polygon)
            {
                selectedShape = (Drawing.Polygon)shape;

                m_frmDetailAttrib.Unselect();

                m_frmDetailAttrib.Select(selectedShape);

                if (!m_dlgDetail.Visible)
                    m_dlgDetail.Show(FormMain.Instance);
            }
            else
            {
                m_frmDetailAttrib.Unselect();
                ClearAllSelection();
            }

            return true;
        }

        public void SelectShape(Drawing.Polygon shape)
        {
            if (mSelectList.Contains(shape))
                return;

            Drawing.ShapeLayer ownLayer = (Drawing.ShapeLayer)shape.GetLayer();
            ownLayer.SelectedList.Add(shape);
            ownLayer.SelectedColor = m_colorSelected;

            mSelectList.Add(shape);
        }

        public void UnselectShape(Drawing.Polygon shape)
        {
            Drawing.ShapeLayer ownLayer = (Drawing.ShapeLayer)shape.GetLayer();
            ownLayer.SelectedList.Remove(shape);

            mSelectList.Remove(shape);
        }

        public void ClearAllSelection()
        {
            foreach (Drawing.Polygon shape in mSelectList)
            {
                Drawing.ShapeLayer ownLayer = (Drawing.ShapeLayer)shape.GetLayer();
                ownLayer.SelectedList.Remove(shape);               
            }
            mSelectList.Clear();
        }

        public void SetShapeInfo(libShapeFile.ShapeInfo shapeInfo, Drawing.ShapeLayer layer)
        {
            m_frmDetailAttrib.SetShapeInfo(shapeInfo, layer);
        }
    }

    public class DialogFormFrameEx : SoilMan.DialogFormFrame
    {
        public DialogFormFrameEx(System.Windows.Forms.Form frmMain, bool bCloseDispose = true)
			: base(frmMain, bCloseDispose)
        {
        }

        protected override void CloseButtonClicked()
        {
            this.Hide();
        }
    }
}
