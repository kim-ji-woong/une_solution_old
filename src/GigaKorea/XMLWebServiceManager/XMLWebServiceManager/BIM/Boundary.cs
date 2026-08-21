using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Boundary
    {
        private List<PathItem> m_boundary = new List<PathItem>();

        public void AddLine(Line2D line)
        {
            PathItem item = new PathItem();
            item.SetLine(line);
            m_boundary.Add(item);
        }

        public void AddArc(Arc2D arc)
        {
            PathItem item = new PathItem();
            item.SetArc(arc);
            m_boundary.Add(item);
        }

        public void AddEArc(EArc2D eArc)
        {
            PathItem item = new PathItem();
            item.SetEArc(eArc);
            m_boundary.Add(item);
        }

        public List<PathItem> GetBoundary()
        {
            return m_boundary;
        }

    }
}
