using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using HSMS;

namespace HSMSServer2
{
    public class DBCarHelper
    {
        public static bool AddCar(DBConn conn, DataCar car)
        {
            if (car == null)
                return false;

            bool bResult = false;
            try
            {            
                int nSiteID = NetworkServer.Instance.SiteID;
                int nMaxID = -1;               
                string strSQL = "insert into Car(ID, CarNumber,SiteID,SensorDetect) Values(" + DBHelper.MaxID + ",'" + car.Code + "', " + nSiteID + ",1)";

                bResult = DBHelper.ExecuteSQL(conn, strSQL, "Car", ref nMaxID);
                if (bResult == true)
                {
                    car.ID = nMaxID;
                }
            }
            catch (System.Exception)
            {
            }            
            return bResult;
        }

        public static bool RemoveCar(DBConn conn, DataCar car)
        {
            return DeleteCar(conn, car);
        }

        public static bool DeleteCar(DBConn conn, DataCar car)
        {
            if (car == null)
                return false;            

            bool bResult = false;
            try
            {
                int nSiteID = NetworkServer.Instance.SiteID;
                string strDeleteSQL = string.Format("Delete from Car where ID = {0} and SiteID={1}", car.ID, nSiteID);
                bResult = DBHelper.ExecuteSQL(conn, strDeleteSQL);
                if(bResult == true)
                    car.ID = -1;
            }
            catch (System.Exception)
            {
            }           
            return bResult;
        }
    }
}
