namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorFacilityType 센서의 타입을 나타낸다.
    /// </summary>
    public class FacilityType : IIDObject
    {
        public enum Fields { ID, TypeName, LinkedTableName, SiteID, Description, DisasterCategoryID, SubDisasterCategoryID };

        private int m_nID = -1;
        private string m_strTypeName = "";
        // 연결된 DB Table 이름
        private string m_strLinkedTableName = null;
        private int m_nSiteID = -1;
        private string m_strDescription = null;
        private int? m_nDisasterCategoryID = null;
        private int? m_nSubDisasterCategoryID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TypeName
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        /// <summary>
        /// 연결된 DB Table 이름
        /// </summary>
        public string LinkedTableName
        {
            get { return m_strLinkedTableName; }
            set { m_strLinkedTableName = value; }
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

        public int? DisasterCategoryID
        {
            get { return m_nDisasterCategoryID; }
            set { m_nDisasterCategoryID = value; }
        }
        public int? SubDisasterCategoryID
        {
            get { return m_nSubDisasterCategoryID; }
            set { m_nSubDisasterCategoryID = value; }
        }

        public static string TableName
        {
            get { return "SdmsSensorFacilityType"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.LinkedTableName ||
                field == Fields.Description ||
                field == Fields.DisasterCategoryID || 
                field == Fields.SubDisasterCategoryID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
