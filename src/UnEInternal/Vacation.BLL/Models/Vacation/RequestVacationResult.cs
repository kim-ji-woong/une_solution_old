using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.BLL.Models.Vacation
{
    public class RequestVacationResult : MessageResult
    {
        private VacationDetail m_detail = null;

        public VacationDetail VacationDetail
        {
            get { return m_detail; }
            set { m_detail = value; }
        }
    }

    public class RequestSpecialVacationResult : MessageResult
    {
        private List<Account.ApplicationUser> m_users = new List<Account.ApplicationUser>();
        private float m_fDays = 0;
        private Model.Response.ResponseType m_result = Model.Response.ResponseType.None;

        public List<Account.ApplicationUser> Users
        {
            get { return m_users; }
        }

        public float Days
        {
            get { return m_fDays; }
            set { m_fDays = value; }
        }

        public int ResponseType
        {
            get { return (int)m_result; }
            set { m_result = (Model.Response.ResponseType)value; }
        }
    }
}
