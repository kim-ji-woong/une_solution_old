using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Data.SqlClient;
using HSMS;


namespace HSMSServer2
{
    public class EditCar : EditData
    {
        public static byte[] ProcessChangeCar(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            // Update
            if (nChangeType == EditData.UPDATE)
            {
            }
            // Delete Car
            else if (nChangeType == EditData.DELETE)
            {
                int nTargetCarID = (int)arrDatas[2];
                string szCode = (string)arrDatas[3];
                DataCar car = dataMgr.FindCar(szCode);
                if (car != null)
                {
                    if (DBCarHelper.DeleteCar(dbMgr, car))
                    {
                        car.ID = -1;
                        car.SensorDetect = true;
                        dataMgr.RemoveCar(car);

                        return bytes;
                    }
                }
               
            }

            else if (nChangeType == EditData.INSERT)
            {
                string szCarNum = (string)arrDatas[2];
                int nSiteID = (int)arrDatas[3];
                bool bIgnore = (bool)arrDatas[4];

                Dictionary<string, DataCar> dicCars = ERPManager.Instance.DicCompanyCars;
                if (dicCars.ContainsKey(szCarNum))
                {
                    DataCar car = dicCars[szCarNum];
                    if (car != null)
                    {
                        car.SiteID = nSiteID;
                        car.SensorDetect = bIgnore;

                        if (DBCarHelper.AddCar(dbMgr, car))
                        {
                            dataMgr.AddCar(car);

                            ArrayList arData = new ArrayList();
                            arData.Add((int)ChangeDataType.CAR);
                            arData.Add(nChangeType);
                            arData.Add(car.ID);
                            arData.Add(car.Code);
                            arData.Add(car.SiteID);
                            arData.Add(car.SensorDetect);

                            return ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arData);
                        }
                    }
                }
            }
            return null;
        }

        // Return 값 : arrDatas가 변경되었는가 여부
        public static bool ProcessChangeDataList(ArrayList arrDatas)
        {
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i++)
            {
                try
                {
                    int nSqlType = (int)arrDatas[i];

                    if (nSqlType == (int)EditData.UPDATE)
                        i = ProcessUpdate(arrDatas, i + 1, dbMgr, connection);
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }
            }

            connection.Close();
            return false;
        }

        // Return 값 : Last Index
        private static int ProcessUpdate(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr, SqlConnection connection)
        {
            int nCarID = (int)arrDatas[nIndex++];
            string strCarNumber = (string)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];
            bool isDetect = (bool)arrDatas[nIndex];

            string strSQL = string.Format("Update Car set CarNumber = '{0}', SiteID = {1}, SensorDetect = {2} where ID = {3}",
                strCarNumber, nSiteID, isDetect ? 1 : 0, nCarID);

            dbMgr.ExecuteSQL(strSQL, connection);

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DataCar car = dataMgr.GetCarFromID(nCarID);

            if (car != null)
            {
                car.Number = strCarNumber;
                car.SiteID = nSiteID;
                car.SensorDetect = isDetect;
            }

            return nIndex;
        }
    }
}
