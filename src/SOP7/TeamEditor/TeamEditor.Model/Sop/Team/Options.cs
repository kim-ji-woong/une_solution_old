using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.Model.Sop.Team
{
    public class Options
    {
        public enum Fields { ID, PropertyID, PropertyName, PropertyValue };

        public int ID { get; set; }
        public int PropertyID { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public static string TableName
        {
            get { return "SopTeamOptions"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.PropertyName ||
                field == Fields.PropertyValue)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
