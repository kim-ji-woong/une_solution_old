using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Drawing
{
    public class ShapeLayer : DXFViewer.Layer
    {
        protected System.Drawing.Drawing2D.GraphicsPath m_pathLine = null;
        protected System.Drawing.Drawing2D.GraphicsPath m_pathFill = null;
        // Draw() 함수에서 마지막으로 그리기를 시도한 객체
        protected PointShape m_lastDrawingShape = null;
        //protected Polygon m_selectedShape = null;
        protected System.Drawing.Color m_colorSelected = System.Drawing.Color.Red;

        private bool m_usable = true;
        
        public PointShape LastDrawingShape
        {
            get { return m_lastDrawingShape; }
            set { m_lastDrawingShape = value; }
        }


        List<Polygon> mSelectList = new List<Polygon>();
        public List<Polygon> SelectedList
        {
            get { return mSelectList; }
            set { mSelectList = value; }
        }

        //public Polygon SelectedShape
        //{
        //    get { return m_selectedShape; }
        //    set { m_selectedShape = value; }
        //}

        public System.Drawing.Color SelectedColor
        {
            get { return m_colorSelected; }
            set { m_colorSelected = value; }
        }

        public ShapeLayer(DXFViewer.IPainter owner)
            : base(owner)
        {
        }

        public ShapeLayer(DXFViewer.IPainter owner, DXFViewer.LineType lineType)
            : base(owner, lineType)
        {
        }

        public System.Drawing.Drawing2D.GraphicsPath PathLine
        {
            get { return m_pathLine; }
            set { m_pathLine = value; }
        }

        public System.Drawing.Drawing2D.GraphicsPath PathFill
        {
            get { return m_pathFill; }
            set { m_pathFill = value; }
        }

        public bool Usable
        {
            get { return m_usable; }
            set { m_usable = value; }
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            if (FormMain.Instance.NoRefresh)
                return true;

            if (!m_usable)
                return true;

            m_lastDrawingShape = null;
            bool result = base.Draw(g, bDrawText);

            // GraphicPath에 저장된 값들을 그린다.
            if (m_lastDrawingShape != null)
                m_lastDrawingShape.PostDraw(g);

            System.Drawing.Color color = FormMain.Instance.SelectionManager.SelectedColor;
            foreach (Polygon poly in mSelectList)
            {
                poly.DrawSelection(g, color);
            }
                           
            /*#region 영역체크
            foreach (DXFViewer.Shape shape in Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;

                    int nPolygonCount = polygonList.GetPolygonCount();

                    for (int i=0;i<nPolygonCount;i++)
                    {
                        Drawing.Polygon polygon = polygonList.GetPolygon(i);
                        if (polygon.Selected)
                            polygon.DrawSelection(g, m_colorSelected);
                    }
                }
            }
            #endregion*/

            return result;
        }
    }
}
