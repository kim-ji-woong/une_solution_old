using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using HSMS;

namespace HSMSServer2
{
    public class DBZoneHelper
    {
        private static string GetPermitString(DataZone zone)
        {
            int nCount = zone.GetPermitLevelCount();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nCount; i++)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }
                int nPermitLevel = zone.GetPermitLevel(i);
                sb.Append(nPermitLevel);
            }
            return sb.ToString();
        }

        public static bool UpdateZoneLevel(DBConn conn, DataZone zone)
        {
            if (zone == null)
                return false;

            bool bResult = false;
            int nSiteID = NetworkServer.Instance.SiteID;
            try
            {
                string szPermitString = GetPermitString(zone);
                string szUpdateSql = string.Format("Update Zone Set PermitLevel = '{0}' where ID = {1} and SiteID = {2}", szPermitString, zone.ID, nSiteID);

                bResult = DBHelper.ExecuteSQL(conn, szUpdateSql);
         
            }
            catch (System.Exception)
            {               
            }
            return bResult;
        }  
    }
}
