using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model
{
    public class Site
    {
        public enum Fields { ID, SiteName, TeamID };

        public int ID { get; set; }
        public string SiteName { get; set; }
        public int? TeamID { get; set; }

        public static string TableName
        {
            get { return "Site"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.TeamID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
