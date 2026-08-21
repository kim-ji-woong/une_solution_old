using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;

namespace XMLWebServiceManager.Shapes
{
    public class Wire : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private int m_nBeginPOI = 0;
        private int m_nEndPOI = 0;
        private int m_nPOITypeID = 0;
        private string m_nLines = "";
        private int m_nLevelID = 0;
        private List<Vertex2D> m_positions = new List<Vertex2D>();
        //private Bitmap m_Icon = null;
        private bool m_bVisible = true;
        private POI m_POIIcon = null;

        private double m_dMoveX = 0.0, m_dMoveY = 0.0;
        /// <summary>
        /// key : m_positions index
        /// </summary>
        //private Dictionary<int, Rectangle> m_rectEditVertex = new Dictionary<int, Rectangle>();
        private bool m_bRectEditVertexVisible = false;

        public const string WireIDTag = "pw";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string XMLID
        {
            get { return m_strXMLID; }
            set { m_strXMLID = value; }
        }

        public int BeginPOI
        {
            get { return m_nBeginPOI; }
            set { m_nBeginPOI = value; }
        }

        public int EndPOI
        {
            get { return m_nEndPOI; }
            set { m_nEndPOI = value; }
        }

        public int POITypeID
        {
            get { return m_nPOITypeID; }
            set { m_nPOITypeID = value; }
        }

        public string Lines
        {
            get
            {

                string strReturn = "";
                for (int i = 0; i < m_positions.Count; i++)
                {
                    strReturn += string.Format("{0},{1}", m_positions[i].x, m_positions[i].y);
                    if (i < m_positions.Count - 1)
                        strReturn += ",";
                }

                return strReturn;
            }
            set { m_nLines = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public List<Vertex2D> Positions
        {
            get { return m_positions; }
            set { m_positions = value; }
        }

        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public POI POIIcon
        {
            get { return m_POIIcon; }
            set { m_POIIcon = value; }
        }
    }

    public class MakeWire
    {
        private POI m_targetPOI = null; // null이면 빈 영역
        private Vertex2D m_targetVertex2D = null;

        public POI TargetPOI
        {
            get { return m_targetPOI; }
            set { m_targetPOI = value; }
        }
        public Vertex2D targetVertex2D
        {
            get { return m_targetVertex2D; }
            set { m_targetVertex2D = value; }
        }
    }
}
