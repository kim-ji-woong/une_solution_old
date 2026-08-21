using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HSMS;
namespace HSMSServer2
{
    public interface IChangedDataChecker
    {
        void AddChangedData(IChangedData data);
    }
    
    public class ProxyHSMS
    {
        private static IChangedDataChecker m_checker = null;
        public static IChangedDataChecker Checker
        {
            get { return m_checker; }
            set { m_checker = value; }
        }

        private static DateTime m_LastDBAccess;
        public static System.DateTime LastDBAccess
        {
            get { return m_LastDBAccess; }
            set { m_LastDBAccess = value; }
        }
    }
}
