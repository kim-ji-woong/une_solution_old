using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace BIMViewer
{
    public class Viewport
    {
        private float m_f11, m_f12, m_f21, m_f22, m_fdx, m_fdy;
        private Vertex2D m_vTL;
        private Vertex2D m_vBL;
        private Vertex2D m_vBR;
        private double m_dWeight;

        public float F11
        {
            get { return m_f11; }
            set { m_f11 = value; }
        }

        public float F12
        {
            get { return m_f12; }
            set { m_f12 = value; }
        }

        public float F22
        {
            get { return m_f22; }
            set { m_f22 = value; }
        }

        public float F21
        {
            get { return m_f21; }
            set { m_f21 = value; }
        }

        public float Dx
        {
            get { return m_fdx; }
            set { m_fdx = value; }
        }

        public float Dy
        {
            get { return m_fdy; }
            set { m_fdy = value; }
        }

        public Vertex2D TopLeft
        {
            get { return m_vTL; }
            set { m_vTL = value; }
        }

        public Vertex2D BottomLeft
        {
            get { return m_vBL; }
            set { m_vBL = value; }
        }

        public Vertex2D BottomRight
        {
            get { return m_vBR; }
            set { m_vBR = value; }
        }

        public double Weight
        {
            get { return m_dWeight; }
            set { m_dWeight = value; }
        }

        public Viewport()
        {
            m_f11 = m_f12 = m_f21 = m_f22 = m_fdx = m_fdy = 0.0f;
            m_vTL = m_vBL = m_vBR = null;
            m_dWeight = 0.0;
        }
    }
}
