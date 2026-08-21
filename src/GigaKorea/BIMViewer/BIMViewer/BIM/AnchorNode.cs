using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace BIMViewer.BIM
{
    public class AnchorNode
    {
        private Global m_global = null;
        private Local m_local = null;
        private List<Property> m_properties = new List<Property>();

        public Global Global
        {
            get { return m_global; }
            set { m_global = value; }
        }

        public Local Local
        {
            get { return m_local; }
            set { m_local = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }
    }

    public class Global
    {
        public enum UnitOfLength { MM = 0, CM, M, KM, DEGREE };

        private UnitOfLength m_unit = UnitOfLength.DEGREE;
        private Vertex2D m_vPos = null;

        public UnitOfLength Unit
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public static UnitOfLength GetUnit(string strUnit)
        {
            if (strUnit == "mm")
                return UnitOfLength.MM;
            else if (strUnit == "cm")
                return UnitOfLength.CM;
            else if (strUnit == "meter")
                return UnitOfLength.M;
            else if (strUnit == "km")
                return UnitOfLength.KM;
            else if (strUnit == "degree")
                return UnitOfLength.DEGREE;


            return UnitOfLength.MM;
        }

        public string GetUnitString()
        {
            if (m_unit == UnitOfLength.MM)
                return "mm";
            else if (m_unit == UnitOfLength.CM)
                return "cm";
            else if (m_unit == UnitOfLength.M)
                return "meter";
            else if (m_unit == UnitOfLength.KM)
                return "km";
            else if (m_unit == UnitOfLength.DEGREE)
                return "degree";

            return "";
        }
    }

    public class Local
    {
        private Vertex2D m_vPos = null;
        private Double m_dAngle = 0.0;

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public double Angle
        {
            get { return m_dAngle; }
            set { m_dAngle = value; }
        }
    }
}
