using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Overlay
{
    public abstract class OverlayShape
    {
        protected OverlayPainter m_painter = null;
        protected bool m_isSelected = false;

        public bool Selected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; }
        }

        public OverlayShape(OverlayPainter painter)
        {
            m_painter = painter;
            m_isSelected = true;
        }

        public abstract bool Draw(System.Drawing.Graphics g);
        public abstract void SetTempPoint(float x, float y);
        public abstract bool IsValid();
        public abstract void GetArea(ref double dGeneralArea, ref double dFieldArea, ref double dRiceFieldArea, ref double dMountainArea, DXFViewer.Layer layer);
        public abstract List<UnE.Geometry.Vertex2F> GetBoundaryPolygon(ref float minX, ref float minY, ref float maxX, ref float maxY);
        public abstract bool HitTest(float x, float y);
        public abstract void Move(float x, float y);
    }
}
