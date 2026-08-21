using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using HSMS;

namespace HSMSServer2
{
    public class DBOptionHelper
    {       

        public static bool UpdateOption(DBConn conn, string strItemName, string strItemValue)
        {
            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch(Exception)
            {
                return false;
            }
            
            int nSiteID = NetworkServer.Instance.SiteID;

            string strSQL = string.Format("Select id from Options where ItemName = '{0}' and SiteID = {1}",
                strItemName, nSiteID);

            int nOptionID = -1;
            try
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        nOptionID = (int)reader[0];
                    }
                    reader.Close();
                }            
                connection.Close();
            }
            catch(Exception)
            {
                return false;
            }

            
            if (nOptionID <= 0)
            {                
                strSQL = string.Format("Insert into Options (ID, ItemName, ItemValue, SiteID, Description) values " +
                    "({0}, '{1}', '{2}', {3}, '')",
                    DBHelper.MaxID, strItemName, strItemValue, nSiteID);
                return DBHelper.ExecuteSQL(conn, strSQL, "Options", ref nOptionID);
            }
            else
            {
                strSQL = string.Format("Update Options set ItemValue = '{0}' where ID = {1} and SiteID = {2}",
                    strItemValue, nOptionID, nSiteID);

                return DBHelper.ExecuteSQL(conn, strSQL);
            }
        }
    }
}
