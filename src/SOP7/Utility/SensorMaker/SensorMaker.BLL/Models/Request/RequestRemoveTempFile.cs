using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Request
{
    public class RequestRemoveTempFile
    {
        private int m_nUserID = -1;
        private string m_strUserName = "";
        private string m_strFileName = "";

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public string FileName
        {
            get { return m_strFileName; }
            set { m_strFileName = value; }
        }
    }
}
