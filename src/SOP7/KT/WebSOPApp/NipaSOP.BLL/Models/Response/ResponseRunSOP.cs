using System;
using System.Collections.Generic;
using System.Text;

namespace NipaSOP.BLL.Models.Response
{
    public class ResponseRunSOP : MessageResult
    {
        private int? m_actionStepHistoryID = null;
        private string m_strAccessMode = null;
        private string m_strAccessToken = null;
        private string m_strServiceType = null;
        private string m_strSiteID = null;

        public int? ActionStepHistoryID
        {
            get { return m_actionStepHistoryID; }
            set { m_actionStepHistoryID = value; }
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

        public string SiteID
        {
            get { return m_strSiteID; }
            set { m_strSiteID = value; }
        }

        public ResponseRunSOP()
        {
        }

        public ResponseRunSOP(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
