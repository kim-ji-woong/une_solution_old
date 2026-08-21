using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.Model.Sop.Team
{
    public class Regular
    {
        public enum Fields { ID, TeamName, ParentTeamID };

        public int ID { get; set; }
        public string TeamName { get; set; }
        public int? ParentTeamID { get; set; }

        public static string GetTableName()
        {
            return "SopTeamRegular";
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ParentTeamID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
