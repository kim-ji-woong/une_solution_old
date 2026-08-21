using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeFileReader.Drawing
{
    public class ShapeLayer : DXFViewer.Layer
    {
        protected System.Drawing.Drawing2D.GraphicsPath m_pathLine = null;
        protected System.Drawing.Drawing2D.GraphicsPath m_pathFill = null;

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
    }
}
