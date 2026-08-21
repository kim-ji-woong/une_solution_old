using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.Model.Sop.Team
{
    public class Temporary
    {
        public enum Fields { ID, TeamName, ParentTeamID, IsNormal, SiteID };

        public int ID { get; set; }
        public string TeamName { get; set; }
        public int? ParentTeamID { get; set; }
        public bool IsNormal { get; set; }
        public int SiteID { get; set; }

        public static string GetTableName()
        {
            return "SopTeamTemporary";
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
