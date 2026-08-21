using Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.BLL.Models.Response
{
    public class ResponseCurrentWorkPermit : MessageResult
    {
        private List<CurrentWorkPermit> m_currentWorkPermits = null;

        public List<CurrentWorkPermit> CurrentWorkPermits
        {
            get { return m_currentWorkPermits; }
            set { m_currentWorkPermits = value; }
        }
    }
}
