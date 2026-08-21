using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.BLL.Models.Account
{
    public class ExternalLoginResult : MessageResult
    {
        private string m_strName = "";
        private string m_strUserID = "";
        private string m_strTeamName = "";
        private string m_strLoginKey = "";

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string LoginKey
        {
            get { return m_strLoginKey; }
            set { m_strLoginKey = value; }
        }

        public ExternalLoginResult()
            : base()
        {
        }

        public ExternalLoginResult(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
