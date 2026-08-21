using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSMS;

namespace HSMSServer2
{
    public class DBMessageHelper
    {
        public static bool UpdateSMSConfig(DBConn conn, bool bValues)
        { 
            bool bResult = false;
            int nSiteID = NetworkServer.Instance.SiteID;

            int nItemValue = (bValues == true ? 1 : 0);

            string szSQL = string.Format("Update Options Set ItemValue = {0} where ItemName = 'SendSMS' And SiteID = {1}", nItemValue, nSiteID);
            bResult = DBHelper.ExecuteSQL(conn, szSQL);
            return bResult;
        }
    }
}
