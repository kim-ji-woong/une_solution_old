using Dashboard.Model;
using SOPManager.Model.Sop.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.BLL.Models.Response
{
    public class ResponseGetSelectDay : MessageResult
    {
        private Option m_selectDay = null;

        public Option SelectDay
        {
            get { return m_selectDay; }
            set { m_selectDay = value; }
        }
    }
}
