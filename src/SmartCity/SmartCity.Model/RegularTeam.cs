using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class RegularTeam
    {
        public enum Fields { ID, TeamName, ParentTeamID };

        private int m_nID = -1;
        private string m_strTeamName = "";
        private int? m_nParentTeamID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public int? ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ParentTeamID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "RegularTeam"; }
        }
    }
}
