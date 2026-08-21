using System;
using System.Collections.Generic;
using System.Text;

namespace NipaSOP.BLL.Models.Request
{
    public class StartInfo
    {
        private string m_strAccessMode = "";
        private string m_strAccessToken = "";
        private string m_strServiceType = "";
        private int m_nFacilityID = 0;

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
    }
}
