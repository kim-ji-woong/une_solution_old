namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorMaterial 물질 및 종류
    /// </summary>
    public class Material : IIDObject
    {
        public enum Fields { ID, MaterialName, UOM, SiteID, Description };

        private int m_nID = -1;
        // 물질이름
        private string m_strMaterialName = "";
        // 물질의 단위(%, ppm...)
        private string m_strUOM = "";
        private int m_nSiteID = -1;
        private string m_strDescription = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        /// <summary>
        /// 물질이름
        /// </summary>
        public string MaterialName
        {
            get { return m_strMaterialName; }
            set { m_strMaterialName = value; }
        }

        /// <summary>
        /// 물질의 단위(%, ppm...)
        /// </summary>
        public string UOM
        {
            get { return m_strUOM; }
            set { m_strUOM = value; }
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
            get { return "SdmsSensorMaterial"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.UOM ||
                field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
