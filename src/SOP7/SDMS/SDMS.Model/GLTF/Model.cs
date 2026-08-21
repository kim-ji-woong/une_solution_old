namespace SDMS.Model.GLTF
{
    public class Model : IIDObject
    {
        public enum Fields { ID, ParentID, ModelName, SiteID };

        private int m_nID = -1;
        private int? m_nParentID = null;
        private string m_strModelName = "";
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int? ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SdmsGltfModel"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ParentID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
