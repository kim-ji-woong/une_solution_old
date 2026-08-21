using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Drawing
{
    public abstract class PointShape : DXFViewer.Shape
    {
        protected IShapeAttrib m_attr = null;
        protected bool m_isFirstElement = false;
        protected bool m_isLastElement = false;
        protected int m_nID = -1;
        protected libShapeFile.ShapeInfo m_shapeInfo = null;

        public IShapeAttrib Attrib
        {
            get { return m_attr; }
        }

        public PointShape()
        {
            Selectable = true;
        }

        public bool FirstElement
        {
            get { return m_isFirstElement; }
            set { m_isFirstElement = value; }
        }

        public bool LastElement
        {
            get { return m_isLastElement; }
            set { m_isLastElement = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public libShapeFile.ShapeInfo ShapeInfo
        {
            get { return m_shapeInfo; }
            set { m_shapeInfo = value; }
        }

        public void SetAttrib(IShapeAttrib attr)
        {
            m_attr = attr;
        }

        public abstract void PostDraw(System.Drawing.Graphics g);
    }

    public abstract class BoundingShape : PointShape
    {
        protected double m_dMinX = 0.0;
        protected double m_dMaxX = 0.0;
        protected double m_dMinY = 0.0;
        protected double m_dMaxY = 0.0;

        public double MinX
        {
            get { return m_dMinX; }
        }

        public double MaxX
        {
            get { return m_dMaxX; }
        }

        public double MinY
        {
            get { return m_dMinY; }
        }

        public double MaxY
        {
            get { return m_dMaxY; }
        }
        
        public void SetBounding(double dMinX, double dMaxX, double dMinY, double dMaxY)
        {
            m_dMinX = dMinX;
            m_dMaxX = dMaxX;
            m_dMinY = dMinY;
            m_dMaxY = dMaxY;
        }

        public static UnE.Geometry.Vertex2D ScaleTransfer(double x, double y, double dScale, UnE.Geometry.Vertex2D vCenter)
        {
            double dx = x - vCenter.x;
            double dy = y - vCenter.y;

            UnE.Geometry.Vertex2D vScreenCenter = FormMain.Instance.ScreenCenter;
            return new UnE.Geometry.Vertex2D(vScreenCenter.x + dx * dScale, vScreenCenter.y + dy * dScale);
            //return new UnE.Geometry.Vertex2D(vCenter.x + dx * dScale, vCenter.y + dy * dScale);
        }

        //public static UnE.Geometry.Vertex2F ScaleTransfer(float x, float y, double dScale, UnE.Geometry.Vertex2F vCenter)
        //{
        //   // float dx = x - vCenter.x;
        //   // float dy = y - vCenter.y;

        //   // UnE.Geometry.Vertex2D vScreenCenter = FormMain.Instance.ScreenCenter;
        //   // return new UnE.Geometry.Vertex2F((float)(vScreenCenter.x + dx * dScale), (float)(vScreenCenter.y + dy * dScale));
        //}

        // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
        public static void SetPenWidth(System.Drawing.Pen pen, System.Drawing.Graphics g, int nLineWidth)
        {
            float fScaleX = g.Transform.Elements[0];
            float fScaleY = g.Transform.Elements[3];

            float fLineWidth = 1.0f / fScaleX * nLineWidth;
            pen.Width = fLineWidth;
        }
    }
}
