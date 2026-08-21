using System;
using System.Collections.Generic;
using System.Text;

namespace dnsDBUtil
{
    public class WebDBTransactionStateException : Exception
    {
        public WebDBTransactionStateException(string szMsg)
            : base(szMsg)
        {
        }
    }
}
