using DBUtility2;
using System.Collections;

namespace SH_Temp_Humidity_Server.Data
{
    public class DirectDBManagerEx
    {
        private DirectDBManager m_dbMgr = null;

        public int SiteID
        {
            get { return m_dbMgr.SiteID; }
        }

        public string ErrorMessage
        {
            get { return m_dbMgr.ErrorMessage; }
        }

        public DirectDBManagerEx(DirectDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public ArrayList GetResultData(string strSQL)
        {
            if (m_dbMgr.Connect() == false)
                return null;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            m_dbMgr.Close();
            return arrResult;
        }
    }
}
