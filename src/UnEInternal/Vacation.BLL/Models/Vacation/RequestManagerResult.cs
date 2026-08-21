using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vacation.BLL.Models.Vacation
{
    public class RequestManagerResult : MessageResult
    {
        private bool m_isOverRequest = false;
        private List<Account.ApplicationUser> m_managers = new List<Account.ApplicationUser>();

        // 부여된 휴가일수를 초과하였는가?
        public bool IsOverRequest
        {
            get { return m_isOverRequest; }
            set { m_isOverRequest = value; }
        }

        public List<Account.ApplicationUser> Managers
        {
            get { return m_managers; }
        }
    }
}
