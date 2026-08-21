using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class AccountLevel
    {
        public enum Fields { ID, LevelName };

        private int m_nID = -1;
        private string m_strLevelName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string LevelName
        {
            get { return m_strLevelName; }
            set { m_strLevelName = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "AccountLevel"; }
        }
    }
}
