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
    public class EditWorker : EditData
    {
        public static byte[] ProcessChangeWorker(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            // EnterLevel Update
            if (nChangeType == EditData.UPDATE)
            {
                int nTargetWorkerID = (int)arrDatas[2];
                int nChangeLevel = (int)arrDatas[3];
                DataWorker worker = dataMgr.GetWorkerFromID(nTargetWorkerID);
                if (worker != null)
                {
                    if (worker.DBEnterLevel != nChangeLevel)
                    {
                        if (DBWorkerHelper.UpdateWorker(dbMgr, worker,nChangeLevel))
                        {
                            worker.EnterLevel = nChangeLevel;
                            worker.DBEnterLevel = nChangeLevel;

                            return bytes;
                        }
                    }
                }
                
            }
            // Delete Worker
            else if (nChangeType == EditData.DELETE)
            {
                int nTargetWorkerID = (int)arrDatas[2];
                string szMemberID = (string)arrDatas[3];

                DataWorker worker = dataMgr.FindWorker(szMemberID);
                if (worker != null)
                {
                    if (DBWorkerHelper.DeleteWorker(dbMgr, worker))
                    {
                        worker.ID = -1;
                        worker.EnterLevel = -1;
                        worker.SensorDetect = true;
                        dataMgr.RemoveWorker(worker);

                        return bytes;
                    }
                }                
            }
            // Add Worker
            else if (nChangeType == EditData.INSERT)
            {
                string szMemberID = (string)arrDatas[2];
                int nEnterLevel = (int)arrDatas[3];
                int nSiteID = (int)arrDatas[4];
                bool bIgnore = (bool)arrDatas[5];
                Dictionary<string, DataWorker> dicWorkers = ERPManager.Instance.DicCompanyWorkers;
                if (dicWorkers.ContainsKey(szMemberID))
                {
                    DataWorker worker = dicWorkers[szMemberID];
                    if (worker != null)
                    {
                        worker.SiteID = nSiteID;
                        worker.SensorDetect = bIgnore;
                        worker.EnterLevel = nEnterLevel;

                        if (DBWorkerHelper.AddWorker(dbMgr, worker))
                        {
                            dataMgr.AddWorker(worker);

                            ArrayList arData = new ArrayList();

                            arData.Add((int)ChangeDataType.WORKER);
                            arData.Add(nChangeType);
                            arData.Add(worker.ID);
                            arData.Add(worker.MemberID);
                            arData.Add(worker.EnterLevel);
                            arData.Add(worker.SiteID);
                            arData.Add(worker.SensorDetect);

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
            int nWorkerID = (int)arrDatas[nIndex++];
            string strMemberID = (string)arrDatas[nIndex++];
            int nEnterLevel = (int)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];
            bool isDetect = (bool)arrDatas[nIndex];

            string strSQL = string.Format("Update Worker set MemberID = '{0}', WorkerLevel = {1}, SiteID = {2}, SensorDetect = {3} where ID = {4}",
                strMemberID, nEnterLevel, nSiteID, isDetect ? 1 : 0, nWorkerID);

            dbMgr.ExecuteSQL(strSQL, connection);

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DataWorker worker = dataMgr.GetWorkerFromID(nWorkerID);

            if (worker != null)
            {
                worker.MemberID = strMemberID;
                worker.EnterLevel = nEnterLevel;
                worker.DBEnterLevel = nEnterLevel;
                worker.SiteID = nSiteID;
                worker.SensorDetect = isDetect;
            }

            return nIndex;
        }
    }
}
