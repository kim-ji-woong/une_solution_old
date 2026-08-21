using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;
    using SOPManager.Model.Sop.Account;

    public class ResponseCheckParamsCode : MessageResult
    {
        private int nID = -1;
        private string m_strUserName = "";
        private string m_strUserID = "";

        public int ID
        {
            get { return nID; }
            set { nID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }
}
