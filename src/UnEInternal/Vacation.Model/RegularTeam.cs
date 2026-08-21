namespace Vacation.Model
{
    public class RegularTeam
    {
        public enum Fields { ID, Name, ParentID };

        private int m_nID = -1;
        private string m_strName = "";
        private int? m_nParentTeamID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public int? ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ParentID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "RegularTeam";
        }
    }
}
