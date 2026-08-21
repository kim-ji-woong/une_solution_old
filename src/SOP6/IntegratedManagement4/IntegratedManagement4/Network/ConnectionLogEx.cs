using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;

namespace SDMSServer
{
    public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get
            {
                return m_instance2;
            }
        }

        public static bool MakeInstance()
        {
            m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_instance2.m_isOpened = true;
            return m_instance2.m_isOpened;
        }

        public override bool Write(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.DebugFormat("{0}", str);

            return true;
        }

        public override bool WriteLine(object str, Exception e)
        {
            if (logger != null)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                logger.Debug("프로그램 오류 : " + str, e);
                logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
            }
            return true;
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.Debug(str);

            return true;
        }
    }
}
