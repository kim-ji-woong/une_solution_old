using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DBUtility2;

namespace DBToXML.Data
{
    public class Project
    {
        public enum UnitOfLength { MM = 0, CM, Meter, KM };

        private int m_nID = 0;
        private UnitOfLength m_unit = UnitOfLength.MM;
        private string m_strName = "";
        private DateTime m_timeStamp = new DateTime();
        private string m_strAuthor = null;

        private Dictionary<int, Material> m_dicMaterials = new Dictionary<int, Material>();
        private Dictionary<int, POIType> m_dicPOITypes = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public UnitOfLength UnitType
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public string Author
        {
            get { return m_strAuthor; }
            set { m_strAuthor = value; }
        }

        public Dictionary<int, POIType> POITypes
        {
            get { return m_dicPOITypes; }
            set { m_dicPOITypes = value; }
        }

        public List<Material> Materials
        {
            get { return m_dicMaterials.Values.ToList(); }
        }

        public override string ToString()
        {
            return m_strName;
        }

        public string GetUnitString()
        {
            if (m_unit == UnitOfLength.MM)
                return "mm";
            else if (m_unit == UnitOfLength.CM)
                return "cm";
            else if (m_unit == UnitOfLength.Meter)
                return "meter";

            return "";
        }

        public List<Property> ReadProperty(WebDBManager dbMgr)
        {
            return Property.ReadDB(dbMgr, "ProjectProperties", "ProjectProperty", "ProjectID", ID);
        }

        public Material GetMaterial(int nMaterialID)
        {
            Material material;

            if (m_dicMaterials.TryGetValue(nMaterialID, out material))
                return material;

            return null;
        }

        public void SetMaterial(Material material, int nID)
        {
            m_dicMaterials[nID] = material;
        }
    }
}