using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnE.Geometry;

namespace XMLWebServiceManager.Shapes
{
    public abstract class Shape
    {
        protected Vertex2D m_vTL = null;
        protected Vertex2D m_vBR = null;

        //protected Color m_clrText = Color.Black;
        //protected Font m_font = null;

        //protected Layer m_layer = null;
        //protected bool m_selected = false;

        //protected Color m_selectedFillColor = Color.FromArgb(137, 203, 250);
        //protected Color m_selectedLineColor = Color.FromArgb(137, 203, 250);

        public Vertex2D TopLeft
        {
            get { return m_vTL; }
        }

        public Vertex2D BottomRight
        {
            get { return m_vBR; }
        }
    }
}
