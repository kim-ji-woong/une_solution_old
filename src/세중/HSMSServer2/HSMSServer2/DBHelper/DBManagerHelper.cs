using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using HSMS;

namespace HSMSServer2
{
    public class DBManagerHelper
    {
        public static bool DeleteManager(DBConn conn, Manager worker)
        {
            if (worker == null)
                return false;
           
            int nSiteID = NetworkServer.Instance.SiteID;

            bool bResult = false;

            string strSQL = string.Format("delete from Manager where MemberID = {0} and SiteID = {1} ", worker.MemberID, nSiteID);

            bResult = DBHelper.ExecuteSQL(conn, strSQL);
            if (bResult == true)
            {
                worker.ID = -1;

            }
            return bResult;
        }

        public static bool AddManager(DBConn conn, Manager worker)
        {
            if (worker == null)
                return false;

            int nSiteID = NetworkServer.Instance.SiteID;

            int nMaxID = -1;
            
            string strSQL = "insert into Manager (ID,MemberID,SiteID) Values(" + DBHelper.MaxID + ",'" + worker.MemberID + "'," + nSiteID + ")";
            
            bool bResult = DBHelper.ExecuteSQL(conn, strSQL, "Manager", ref nMaxID);

            if (bResult == true)
            {
                worker.ID = nMaxID;
            }
            return bResult;
        }
    }
}
