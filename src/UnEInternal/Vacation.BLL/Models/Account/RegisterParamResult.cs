using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vacation.BLL.Models.Account
{
    public class RegisterParamResult : MessageResult
    {
        private string m_strName = "";
        private string m_strLevel = "";
        private string m_strUserID = "";

        // 사용자 이름
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        // 직급
        public string Level
        {
            get { return m_strLevel; }
            set { m_strLevel = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }
}
