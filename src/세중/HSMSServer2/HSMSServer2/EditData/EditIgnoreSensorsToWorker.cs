using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;

namespace HSMSServer2
{
    public class EditIgnoreSensorsToWorker : EditData
    {
        // Return 값 : arrDatas가 변경되었는가 여부
        public static bool ProcessChangeDataList(ArrayList arrDatas)
        {
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            int nDataCount = arrDatas.Count;

            for (int i=1;i<nDataCount;i++)
            {
                try
                {
                    int nSqlType = (int)arrDatas[i];

                    if (nSqlType == (int)EditData.DELETE)
                        i = ProcessDelete(arrDatas, i + 1, dbMgr, connection);
                    else if (nSqlType == (int)EditData.INSERT)
                        i = ProcessInsert(arrDatas, i + 1, dbMgr, connection);
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
        private static int ProcessDelete(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr, SqlConnection connection)
        {
            int nWorkerID = (int)arrDatas[nIndex];
            int nIgnoreObjectID = (int)arrDatas[nIndex + 1];
            int nObjectType = (int)arrDatas[nIndex + 2];
            int nSiteID = (int)arrDatas[nIndex + 3];

            string strSQL = string.Format("Delete from IgnoreSensorsToWorker where WorkerID = {0} and ObjectID = {1} and ObjectType = {2} and SiteID = {3}",
                nWorkerID, nIgnoreObjectID, nObjectType, nSiteID);

            dbMgr.ExecuteSQL(strSQL, connection);

            DataManager dataMgr = NetworkServer.Instance.DataManager;

            foreach (HSMS.DetectIgnoreWorker data in dataMgr.DetectIgnoreWorkers)
            {
                if (data.WorkerID == nWorkerID && data.IgnoreObjectID == nIgnoreObjectID &&
                    data.IgnoreObjectType == nObjectType && data.SiteID == nSiteID)
                {
                    dataMgr.DetectIgnoreWorkers.Remove(data);
                    break;
                }
            }

            return nIndex + 3;
        }

        // Return 값 : Last Index
        private static int ProcessInsert(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr, SqlConnection connection)
        {
            int nWorkerID = (int)arrDatas[nIndex++];
            int nIgnoreObjectID = (int)arrDatas[nIndex++];
            int nObjectType = (int)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex];

            DataManager dataMgr = NetworkServer.Instance.DataManager;

            if (dataMgr.FindIgnoreWorker(nWorkerID, nIgnoreObjectID, nObjectType, nSiteID) != null)
                return nIndex;

            string strSQL = string.Format("Insert into IgnoreSensorsToWorker (WorkerID, ObjectID, ObjectType, SiteID, Description) values " +
                "({0}, {1}, {2}, {3}, NULL)",
                nWorkerID, nIgnoreObjectID, nObjectType, nSiteID);

            dbMgr.ExecuteSQL(strSQL, connection);

            HSMS.DataWorker worker = dataMgr.GetWorkerFromID(nWorkerID);

            if (worker != null)
            {
                HSMS.DetectIgnoreWorker data = new HSMS.DetectIgnoreWorker();

                data.Worker = worker;
                data.WorkerID = nWorkerID;
                data.IgnoreObjectID = nIgnoreObjectID;
                data.IgnoreObjectType = nObjectType;
                data.SiteID = nSiteID;

                dataMgr.DetectIgnoreWorkers.Add(data);
            }
            
            return nIndex;
        }
    }
}
