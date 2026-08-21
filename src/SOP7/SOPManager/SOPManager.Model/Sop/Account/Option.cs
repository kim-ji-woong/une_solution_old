namespace SOPManager.Model.Sop.Account
{
    /// <summary>
    /// 사용자 계정별로 저장할 옵션
    /// </summary>
    public class Option
    {
        public enum Fields { ID, UserID, Category, SubCategory, PropertyValue1, PropertyValue2, PropertyValue3, PropertyValue4 };

        private int m_nID = -1;
        private int m_nUserID = -1;
        private string m_strCategory = "";
        private string m_strSubCategory = "";
        private string m_strPropertyValue1 = "";
        private string m_strPropertyValue2 = "";
        private string m_strPropertyValue3 = "";
        private string m_strPropertyValue4 = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public string Category
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }

        public string SubCategory
        {
            get { return m_strSubCategory; }
            set { m_strSubCategory = value; }
        }

        public string PropertyValue1
        {
            get { return m_strPropertyValue1; }
            set { m_strPropertyValue1 = value; }
        }

        public string PropertyValue2
        {
            get { return m_strPropertyValue2; }
            set { m_strPropertyValue2 = value; }
        }

        public string PropertyValue3
        {
            get { return m_strPropertyValue3; }
            set { m_strPropertyValue3 = value; }
        }

        public string PropertyValue4
        {
            get { return m_strPropertyValue4; }
            set { m_strPropertyValue4 = value; }
        }

        public static string TableName
        {
            get { return "SopAccountOption"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.SubCategory || 
                field == Fields.PropertyValue1 || 
                field == Fields.PropertyValue2 ||
                field == Fields.PropertyValue3 ||
                field == Fields.PropertyValue4)
                isNullable = true;
            else
                isNullable = false;
            return field.ToString();
        }
    }
}
