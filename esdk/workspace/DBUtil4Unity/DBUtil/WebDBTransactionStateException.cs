using System;
using System.Collections.Generic;
using System.Text;

namespace DBUtility
{
    public class WebDBTransactionStateException : Exception
    {
        public WebDBTransactionStateException(string szMsg)
            : base(szMsg)
        {           
        }
    }
}
