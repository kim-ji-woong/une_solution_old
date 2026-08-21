using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.Model
{
    public class VacationOption
    {
        public enum Fields { ID, PropertyName, PropertyValue, Description };

        private int m_nID = -1;
        private string m_strPropertyName = "";
        private string m_strPropertyValue = "";
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string PropertyName
        {
            get { return m_strPropertyName; }
            set { m_strPropertyName = value; }
        }

        public string PropertyValue
        {
            get { return m_strPropertyValue; }
            set { m_strPropertyValue = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "VacationOption";
        }
    }
}
