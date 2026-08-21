using System;
using System.Collections.Generic;
using System.Text;

namespace NipaSOP.BLL.Models.Response
{
    public class ResponseStartInfo : MessageResult
    {
        private int? m_nBeginCode = null;

        public int? BeginCode
        {
            get { return m_nBeginCode; }
            set { m_nBeginCode = value; }
        }
    }
}
