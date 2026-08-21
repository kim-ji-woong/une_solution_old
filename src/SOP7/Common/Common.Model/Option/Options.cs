using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model.Option
{
    public class Options
    {
        public enum Fields { ID, PropertyName, PropertyValue, SiteID, Description };

        public enum OptionTarget 
        { 
            SDMS = 0, 
            SOPSimulator,
            GLTF,
            NOT_DEFINED = 999
        }

        private int m_nID = -1;
        private OptionTarget m_eTargetName = OptionTarget.NOT_DEFINED;
        private string m_strPropertyName = "";
        private string m_strPropertyValue = null;
        private int m_nSiteID = -1;
        private string m_strDescription = null;

        // OptionSOPSimulator, OptionSDMS와 같은 이름
        public OptionTarget TargetName
        {
            get { return m_eTargetName; }
            set { m_eTargetName = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string PropertyName
        {
            get { return m_strPropertyName; }
            set { m_strPropertyName = value; }
        }

        public string PropertyValue
        {
            get { return m_strPropertyValue; }
            set { m_strPropertyValue = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.PropertyValue ||
                field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName(OptionTarget target)
        {
            if (target == OptionTarget.GLTF)
                return "OptionGltf";
            else if (target == OptionTarget.SDMS)
                return "OptionSDMS";
            else if (target == OptionTarget.SOPSimulator)
                return "OptionSOPSimulator";

            return "";
        }
    }
}
