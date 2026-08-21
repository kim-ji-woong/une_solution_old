using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class JobLevel
    {
        public enum Fields { ID, LevelName, LevelNo };

        private int m_nID = -1;
        private string m_strLevelName = "";
        private int? m_nLevelNo = null;

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

        public int? LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.LevelNo)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "JobLevel"; }
        }
    }
}
