using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaData.Models
{
    public class Region
    {
        private int m_nID = -1;
        private string m_strName = "";
        private List<Vertex2F> m_boundary = new List<Vertex2F>();
        private string m_strDescription = "";
        private string m_strBoundary = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Boundary
        {
            get { return BoundaryString(); }
            set { ParseBoundaryString(value); }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public void AddVertex(Vertex2F vertex)
        {
            m_boundary.Add(vertex);
            m_strBoundary = null;
        }

        public void AddVertex(float x, float y)
        {
            m_boundary.Add(new Vertex2F(x, y));
            m_strBoundary = null;
        }

        public int GetVertexCount()
        {
            return m_boundary.Count();
        }

        public Vertex2F GetVertex(int nIndex)
        {
            if (nIndex >= GetVertexCount())
                return null;

            return m_boundary[nIndex];
        }

        public Vertex2F RemoveVertex(int nIndex)
        {
            if (nIndex >= GetVertexCount())
                return null;

            m_strBoundary = null;

            Vertex2F vertex = m_boundary[nIndex];
            m_boundary.RemoveAt(nIndex);
            return vertex;
        }

        public void Clear()
        {
            m_boundary.Clear();
            m_strBoundary = "";
        }

        private string BoundaryString()
        {
            if (m_strBoundary != null)
                return m_strBoundary;

            m_strBoundary = "";

            foreach (Vertex2F vertex in m_boundary)
            {
                m_strBoundary += string.Format("{0:F2},{1:F2}", vertex.X, vertex.Y);
            }

            return m_strBoundary;
        }

        private void ParseBoundaryString(string strBoundary)
        {
            m_boundary.Clear();
            m_strBoundary = "";

            string[] arrNumbers = strBoundary.Split(',');
            int nCount = arrNumbers.Count();

            if (nCount % 2 == 1)
                return;

            float x, y;

            for (int i = 0; i < nCount; i += 2)
            {
                string strX = arrNumbers[i].Trim();
                string strY = arrNumbers[i + 1].Trim();

                if (float.TryParse(strX, out x) && float.TryParse(strY, out y))
                {
                    Vertex2F vertex = new Vertex2F(x, y);
                    m_boundary.Add(vertex);
                }
                else
                {
                    m_boundary.Clear();
                    return;
                }
            }

            m_strBoundary = strBoundary;
        }

        public bool IsInclude(float x, float y, int nCoverage)
        {
            if (m_boundary.Count == 0)
                return false;

            float minX = m_boundary[0].X;
            float minY = m_boundary[0].Y;
            float maxX = minX;
            float maxY = minY;

            if (nCoverage < 0)
                nCoverage = 0;

            if (x >= minX - nCoverage && x <= maxX + nCoverage &&
                y >= minY - nCoverage && y <= maxY + nCoverage)
                return true;

            return false;
        }
    }
}
