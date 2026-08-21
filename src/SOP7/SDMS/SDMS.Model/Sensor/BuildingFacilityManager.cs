using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Sensor
{
    public class BuildingFacilityManager : IIDObject
    {
        public enum Fields { ID, MemberID, MemberType, FacilityType, DetectType, BuildingID, Description, SiteID };

        private int m_nID = -1;
        private int m_nMemberID = -1;
        private int m_nMemberType = (int)FacilityManager.MemberTypes.Unknown;
        private int m_nFaclityType = -1;
        private int m_nDetectType = (int)FacilityManager.DetectTypes.Detect;
        private int m_nBuildingID = -1;
        // TeamEditor.BLL.TemporaryMemberData.MemberTYpe
        private string m_strDescription = null;
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public int FacilityType
        {
            get { return m_nFaclityType; }
            set { m_nFaclityType = value; }
        }

        public int DetectType
        {
            get { return m_nDetectType; }
            set { m_nDetectType = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SdmsSensorBuildingFacilityManager"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
