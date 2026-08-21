using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.Model.Sop.Team
{
    public class TemporaryMember
    {
        public enum Fields { ID, DisplaySOPName, TeamID, RegularID, RegularMemberID, IsNormal, Role };

        public int ID { get; set; }
        public string DisplaySOPName { get; set; }
        public int TeamID { get; set; }
        public int? RegularID { get; set; }
        public int? RegularMemberID { get; set; }
        public int IsNormal { get; set; }
        public int? Role { get; set; }

        public static string GetTableName()
        {
            return "SopTeamTemporaryMember";
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.DisplaySOPName ||
                field == Fields.RegularID ||
                field == Fields.RegularMemberID ||
                field == Fields.Role)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
