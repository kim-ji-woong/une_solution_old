using System;
using System.Collections.Generic;
using System.Text;

namespace NipaSOP.Model.Sop
{
    public class Facility
    {
        public enum Fields { ID, FacilityName, SiteName, DisplayName, SiteID };

        private int m_nID = -1;
        private string m_strFacilityName = "";
        private string m_strSiteName = "";
        private string m_strDisplayName = "";
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string FacilityName
        {
            get { return m_strFacilityName; }
            set { m_strFacilityName = value; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
            set { m_strSiteName = value; }
        }

        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string TableName
        {
            get { return "SopFacility"; }
        }
    }
}
