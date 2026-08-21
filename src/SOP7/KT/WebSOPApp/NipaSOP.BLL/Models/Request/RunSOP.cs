using System;
using System.Collections.Generic;
using System.Text;

namespace NipaSOP.BLL.Models.Request
{
    public class RunSOP
    {
        private int m_nBeginCode = -1;

        public int BeginCode
        {
            get { return m_nBeginCode; }
            set { m_nBeginCode = value; }
        }
    }
}
