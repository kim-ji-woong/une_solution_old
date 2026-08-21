using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.Model.Sop.Config
{
    public class SpecialMessage
    {
        public enum Fields { ID, Category, Message, Description };

        private int m_nID = -1;
        private string m_strCategory = "";
        private string m_strMessage = "";
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Category
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopConfigSpecialMessage"; }
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
