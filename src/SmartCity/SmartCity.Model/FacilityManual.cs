using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class FacilityManual
    {
        public enum Fields { ID, FacilityType, ManualType, ManualTitle, ManualMembers, Number, Manual };

        private int m_nID = -1;
        private int m_nFacilityType = -1;
        private string m_strManualType = "";
        private string m_strManualTitle = "";
        private string m_strManualMembers = "";
        private int m_nNumber = -1;
        private string m_strManual = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }

        public string ManualType
        {
            get { return m_strManualType; }
            set { m_strManualType = value; }
        }

        public string ManualTitle
        {
            get { return m_strManualTitle; }
            set { m_strManualTitle = value; }
        }

        public string ManualMembers
        {
            get { return m_strManualMembers; }
            set { m_strManualMembers = value; }
        }

        public int Number
        {
            get { return m_nNumber; }
            set { m_nNumber = value; }
        }

        public string Manual
        {
            get { return m_strManual; }
            set { m_strManual = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Manual)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "FacilityManual"; }
        }
    }
}
