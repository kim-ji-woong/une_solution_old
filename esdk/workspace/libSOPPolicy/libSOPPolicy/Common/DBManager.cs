using DBUtility2;
using System.Collections;

namespace libSOPPolicy.Common
{
    static class DBManager
    {
        public static ArrayList GetResultData(string strSQL, object dbMgr)
        {
            if (dbMgr is WebDBManager)
                return ((WebDBManager)dbMgr).GetResultData(strSQL);
            else if (dbMgr is DirectDBManager)
                return ((DirectDBManager)dbMgr).GetResultData(strSQL);

            return null;
        }
    }
}
