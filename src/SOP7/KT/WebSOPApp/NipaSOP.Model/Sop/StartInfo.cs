using System;

namespace NipaSOP.Model.Sop
{
    public class StartInfo
    {
        public enum Fields { ID, TimeStamp, AccessMode, AccessToken, ServiceType, FacilityID };

        private int m_nID = -1;
        private DateTime m_dtTimeStamp = new DateTime();
        private string m_strAccessMode = "";
        private string m_strAccessToken = "";
        private string m_strServiceType = "";
        private int m_nFacilityID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_dtTimeStamp; }
            set { m_dtTimeStamp = value; }
        }

        public string AccessMode
        {
            get { return m_strAccessMode; }
            set { m_strAccessMode = value; }
        }

        public string AccessToken
        {
            get { return m_strAccessToken; }
            set { m_strAccessToken = value; }
        }

        public string ServiceType
        {
            get { return m_strServiceType; }
            set { m_strServiceType = value; }
        }

        public int FacilityID
        {
            get { return m_nFacilityID; }
            set { m_nFacilityID = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string TableName
        {
            get { return "SopStartInfo"; }
        }
    }
}
