using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.BLL.Models.Vacation
{
    public class ProcessRequestResult : MessageResult
    {
        private int m_nRequestID = -1;
        private bool m_isPermit = false;

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public bool IsPermit
        {
            get { return m_isPermit; }
            set { m_isPermit = value; }
        }
    }
}
