using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.Model.Sop.Config
{
    public class LinkedSop
    {
        public enum Fields { ID, FacilityTypeID, DisasterCategoryID, SubDisasterCategoryID, DisasterName, LinkedBuildingID, LinkedZoneID, Description }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nFacilityTypeID = -1;
        public int FacilityTypeID
        {
            get { return m_nFacilityTypeID; }
            set { m_nFacilityTypeID = value; }
        }

        private int m_nDisasterCategoryID = -1;
        public int DisasterCategoryID
        {
            get { return m_nDisasterCategoryID; }
            set { m_nDisasterCategoryID = value; }
        }

        private int m_nSubDisasterCategoryID = -1;
        public int SubDisasterCategoryID
        {
            get { return m_nSubDisasterCategoryID; }
            set { m_nSubDisasterCategoryID = value; }
        }

        private string m_strDisasterName = "";
        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }

        private int? m_nLinkedBuildingID = -1;
        public int? LinkedBuildingID
        {
            get { return m_nLinkedBuildingID; }
            set { m_nLinkedBuildingID = value; }
        }

        private int? m_nLinkedZoneID = -1;
        public int? LinkedZoneID
        {
            get { return m_nLinkedZoneID; }
            set { m_nLinkedZoneID = value; }
        }

        private string m_strDescription = "";
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopConfigLinkedSop"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.LinkedBuildingID || field == Fields.LinkedZoneID || field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
