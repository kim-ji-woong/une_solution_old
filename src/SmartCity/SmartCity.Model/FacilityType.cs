using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class FacilityType
    {
        public enum Fields { ID, FacilityType, LinkedTableName, Description };

        private int m_nID = -1;
        private string m_strFacilityType = "";
        private string m_strLinkedTableName = "";
        private string m_strDescription = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string FacilityTypeName
        {
            get { return m_strFacilityType; }
            set { m_strFacilityType = value; }
        }

        public string LinkedTableName
        {
            get { return m_strLinkedTableName; }
            set { m_strLinkedTableName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "FacilityType"; }
        }
    }
}
