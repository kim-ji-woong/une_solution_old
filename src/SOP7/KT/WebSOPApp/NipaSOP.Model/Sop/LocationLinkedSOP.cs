namespace NipaSOP.Model.Sop
{
    public class LocationLinkedSOP
    {
        public enum Fields { FacilityID, FacilityTypeID, DisasterCategoryID, SubDisasterCategoryID, DisasterName };

        private int m_nFacilityID = -1;
        private int m_nFacilityTypeID = -1;
        private int m_nDisasterCategoryID = -1;
        private int m_nSubDisasterCategoryID = -1;
        private string m_strDisasterName = "";

        public int FacilityID
        {
            get { return m_nFacilityID; }
            set { m_nFacilityID = value; }
        }

        public int FacilityTypeID
        {
            get { return m_nFacilityTypeID; }
            set { m_nFacilityTypeID = value; }
        }

        public int DisasterCategoryID
        {
            get { return m_nDisasterCategoryID; }
            set { m_nDisasterCategoryID = value; }
        }

        public int SubDisasterCategoryID
        {
            get { return m_nSubDisasterCategoryID; }
            set { m_nSubDisasterCategoryID = value; }
        }

        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string TableName
        {
            get { return "SopLocationLinkedSOP"; }
        }
    }
}
