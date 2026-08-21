using System;
using System.Collections.Generic;
using UnE.Geometry;

namespace Common.Model
{
    // 대피소는 Building, Zone, EquipZone 중 한 곳에 연결되어 있을수 있다.
    // 동시에 두 군데 이상에 연결되어서는 안된다.
    // 물론 그 어느곳에도 연결되어 있을수도 있는데, 그 경우에는 Boundary를 사용하게 되며,
    // 대피소가 실제 Building, Zone, EquipZone에 연결되어 있을 경우에는 해당 객체의 Boundary를 사용하게 된다.
    public class Shelter
    {
        public enum Fields { ID, ShelterName, ShelterType, ShelterIDType, ShelterID, Boundary, SiteID, Description };

        public enum ShelterTypes { None = 0, Fire = 1, PSM = 2, Earthquake = 4 };
        public enum ShelterIDTypes { None = 0, Building, Zone, EquipZone };

        private int m_nID = 0;
        private string m_strShelterName = "";
        // 피난처가 다수의 장소일 수 있음
        private List<Polygon> m_boundaries = null;
        // ShelterTypes의 Bit Flag 조합
        private int m_nShelterType = (int)ShelterTypes.None;
        private int m_nShelterIDType = (int)ShelterIDTypes.None;
        private int? m_nShelterID = null;
        private int m_nSiteID = -1;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ShelterName
        {
            get { return m_strShelterName; }
            set { m_strShelterName = value; }
        }

        public List<Polygon> Boundary
        {
            get { return m_boundaries; }
            set { m_boundaries = value; }
        }

        // ShelterTypes의 Bit Flag 조합
        public int ShelterType
        {
            get { return m_nShelterType; }
            set { m_nShelterType = value; }
        }

        public int ShelterIDType
        {
            get { return m_nShelterIDType; }
            set { m_nShelterIDType = value; }
        }

        public int? ShelterID
        {
            get { return m_nShelterID; }
            set { m_nShelterID = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "Shelter"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ShelterID ||
                field == Fields.Boundary ||
                field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static ShelterTypes ToShelterType(int nType)
        {
            foreach (ShelterTypes type in Enum.GetValues(typeof(ShelterTypes)))
            {
                if (nType == (int)type)
                    return type;
            }

            return ShelterTypes.None;
        }

        public static ShelterIDTypes ToShelterIDType(int nType)
        {
            foreach (ShelterIDTypes type in Enum.GetValues(typeof(ShelterIDTypes)))
            {
                if (nType == (int)type)
                    return type;
            }

            return ShelterIDTypes.None;
        }
    }
}
