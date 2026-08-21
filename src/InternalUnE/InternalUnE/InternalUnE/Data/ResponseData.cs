using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternalUnE.Data
{
    public class ResponseData
    {
        private bool m_success = false;
        private string m_strMessage = null;

        public bool Success
        {
            get { return m_success; }
            set { m_success = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
    }

    public class ResponseURL : ResponseData
    {
        private string m_strURL = "";

        public string Url
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public ResponseURL()
        {
        }

        public ResponseURL(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }

    public class ResponseLinks : ResponseData
    {
        private List<LinkData> m_linkDatas = new List<LinkData>();

        public List<LinkData> LinkDatas
        {
            get { return m_linkDatas; }
            set { m_linkDatas = value; }
        }

        public ResponseLinks()
        {
        }

        public ResponseLinks(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }
}
