using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtility2
{
    public class WebDBTransactionStateException : Exception
    {
        public WebDBTransactionStateException(string szMsg)
            : base(szMsg)
        {
        }
    }

    public class WebDBMultiQueryStateException : Exception
    {
        public WebDBMultiQueryStateException(string szMsg)
            : base(szMsg)
        {
        }
    }
}
