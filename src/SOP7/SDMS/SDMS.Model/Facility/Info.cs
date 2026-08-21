namespace SDMS.Model.Facility
{
    public class Info : IIDObject
    {
        public enum Fields { ID, ModelName, FacilityName, ZoneID };

        private int m_nID = -1;
        private string m_strModelName = "";
        private string m_strFacilityName = "";
        private int m_nZoneID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }

        public string FacilityName
        {
            get { return m_strFacilityName; }
            set { m_strFacilityName = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public static string TableName
        {
            get { return "SdmsFacilityInfo"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
