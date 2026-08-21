using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Door : Shape
    {
        //  미닫이문, 외여닫이문, 양쪽 외여닫이문, 쌍여닫이문, 양쪽 쌍여닫이문
        public enum DoorType { Sliding = 0, Hinged, Hinged2, DualHinged, DualHinged2 };

        private int m_nID = 0;
        private string m_strXMLID = "";
        // 회전각도(Degree)
        //private double m_dDirection = 0;
        private Vertex2D m_vHinge1 = null;
        private Vertex2D m_vHinge2 = null;
        private Vertex2D m_vPos = null;
        private float m_fWidth = 0.0f;
        private float m_fHeight = 0.0f;
        private float m_fElevation = 0.0f;
        private float m_fThick = 50.0f;
        private DoorType m_doorType = DoorType.Sliding;
        private Wall m_wall = null;

        private List<Property> m_properties = new List<Property>();

        //// 문의 두께 부분
        //private GraphicsPath m_path1 = null;
        //// 문의 힌지 부분
        //private GraphicsPath m_path2 = null;
        //// 쌍여닫이문의 힌지 부분
        //private GraphicsPath m_path3 = null;

        // 문에 의하여 벽체가 뚫리게 되는 영역
        // Line Type일 경우
        private Vertex2D m_vEmptyLineBegin = null;
        private Vertex2D m_vEmptyLineEnd = null;
        // Arc 또는 EArc Type일 경우
        private double m_dEmptyBeginAngle = 0.0;
        private double m_dEmptyEndAngle = 0.0;

        private List<Polygon> m_boundaryPolygons = new List<Polygon>();

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

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public float Width
        {
            get { return m_fWidth; }
            set { m_fWidth = value; }
        }

        public float Height
        {
            get { return m_fHeight; }
            set { m_fHeight = value; }
        }

        public float Elevation
        {
            get { return m_fElevation; }
            set { m_fElevation = value; }
        }

        public float Thick
        {
            get { return m_fThick; }
            set { m_fThick = value; }
        }

        public Wall Wall
        {
            get { return m_wall; }
            set { m_wall = value; }
        }

        public Vertex2D Hinge1
        {
            get { return m_vHinge1; }
            set { m_vHinge1 = value; }
        }

        public Vertex2D Hinge2
        {
            get { return m_vHinge2; }
            set { m_vHinge2 = value; }
        }

        public DoorType GetDoorType()
        {
            return m_doorType;
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public void SetDoorType(int nDoorType)
        {
            foreach (DoorType type in Enum.GetValues(typeof(DoorType)))
            {
                if (nDoorType == (int)type)
                {
                    m_doorType = type;
                    break;
                }
            }
        }

        public string GetDoorTypeName()
        {
            if (m_doorType == DoorType.Sliding)
                return "미닫이문";
            else if (m_doorType == DoorType.Hinged)
                return "외여닫이문";
            else if (m_doorType == DoorType.Hinged2)
                return "양쪽 외여닫이문";
            else if (m_doorType == DoorType.DualHinged)
                return "쌍여닫이문";
            else if (m_doorType == DoorType.DualHinged2)
                return "양쪽 쌍여닫이문";

            return "";
        }
    }
}
