namespace SOPManager.Model.Sop.Category
{
    public class DisasterType
    {
        public enum Fields { ID, Name, SubDisasterID };

        private int m_nID = -1;
        private string m_strTypeName = "";
        private int m_nSubDisasterCategoryID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        public int SubDisasterID
        {
            get { return m_nSubDisasterCategoryID; }
            set { m_nSubDisasterCategoryID = value; }
        }

        public static string TableName
        {
            get { return "SopCategoryDisasterType"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
